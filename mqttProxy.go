package twomqtt

import (
	"encoding/json"
	"fmt"
	"sync"
	"time"

	mqtt "github.com/eclipse/paho.mqtt.golang"
	backoff "github.com/jpillora/backoff"
	log "github.com/sirupsen/logrus"
)

// MQTTProxy - A wrapper for the Paho MQTT
type MQTTProxy struct {
	MQTTProxyConfig
	Client        mqtt.Client
	lastPublished sync.Map
}

// NewMQTTProxy - Create a proxy for an MQTT Client
func NewMQTTProxy(mqttCfg MQTTProxyConfig) *MQTTProxy {
	c := MQTTProxy{
		MQTTProxyConfig: mqttCfg,
	}
	return &c
}

// Initialize - Initialize the MQTT Client with all the appropriate settings
func (c *MQTTProxy) Initialize(onConn mqtt.OnConnectHandler, onLost mqtt.ConnectionLostHandler) {
	opts := mqtt.NewClientOptions().
		AddBroker(c.Broker).
		SetClientID(c.ClientID).
		SetOnConnectHandler(onConn).
		SetConnectionLostHandler(onLost).
		SetUsername(c.Username).
		SetPassword(c.Password).
		SetWill(c.AvailabilityTopic(), "offline", 0, true)

	c.Client = mqtt.NewClient(opts)
}

// LogSettings - Logs the current settings
func (c *MQTTProxy) LogSettings() {
	redactedPassword := ""
	if len(c.Password) > 0 {
		redactedPassword = "<REDACTED>"
	}

	log.WithFields(log.Fields{
		"MQTT.ClientID":        c.ClientID,
		"MQTT.Broker":          c.Broker,
		"MQTT.Username":        c.Username,
		"MQTT.Password":        redactedPassword,
		"MQTT.Discovery":       c.Discovery,
		"MQTT.DiscoveryPrefix": c.DiscoveryPrefix,
		"MQTT.DiscoveryName":   c.DiscoveryName,
		"MQTT.TopicPrefix":     c.TopicPrefix,
	}).Info("MQTT Environmental Settings")
}

// PublishWithOpts - Publish a topic and payload with specific options
func (c *MQTTProxy) PublishWithOpts(topic string, payload string, opts MQTTProxyPublishOptions) {
	llog := log.WithFields(log.Fields{
		"topic":   topic,
		"payload": payload,
	})

	// Should we check for duplicates?
	if opts.DuplicateCheck {
		// Should we publish this again?
		// NOTE: We must allow the availability topic to publish duplicates
		if lastPayload, ok := c.lastPublished.Load(topic); topic != c.AvailabilityTopic() && ok && lastPayload == payload {
			llog.Debug("Duplicate avoided while publishing to MQTT")
			return
		}
	}

	llog.Info("Publishing to MQTT")

	if token := c.Client.Publish(topic, 0, opts.Retained, payload); token.Wait() && token.Error() != nil {
		log.Error("Error publishing to MQTT")
	}

	log.Info("Finished publishing to MQTT")
	c.lastPublished.Store(topic, payload)
}

// Publish - Publish a topic and payload with default options
func (c *MQTTProxy) Publish(topic string, payload string) {
	c.PublishWithOpts(topic, payload, MQTTProxyPublishOptions{
		Retained:       true,
		DuplicateCheck: true,
	})
}

// PublishDiscovery - Publish a MQTTDiscovery
func (c *MQTTProxy) PublishDiscovery(mqd *MQTTDiscovery) {
	topic := fmt.Sprintf("%s/%s/%s/%s/config", mqd.DiscoveryPrefix, mqd.Component, mqd.NodeID, mqd.ObjectID)
	payloadBytes, _ := json.Marshal(mqd)
	payload := string(payloadBytes)

	c.Publish(topic, payload)
}

// LastPublishedOnTopic - Determine what was last published on a topic
func (c *MQTTProxy) LastPublishedOnTopic(topic string) string {
	if result, ok := c.lastPublished.Load(topic); ok {
		return result.(string)
	}

	return ""
}

// Subscribe - Subscribe to a topic and payload via the MQTTClientWrapperWrapper
func (c *MQTTProxy) Subscribe(topic string, qos byte, callback mqtt.MessageHandler) mqtt.Token {
	log.WithFields(log.Fields{
		"topic": topic,
		"qos":   qos,
	}).Info("Subscribing to MQTT topic")

	result := c.Client.Subscribe(topic, qos, callback)

	log.Info("Finished subscribing to MQTT topic")
	return result
}

// Run - Connect to MQTT
func (c *MQTTProxy) Run() {
	log.Info("Connecting to MQTT")

	b := &backoff.Backoff{
		Max:    5 * time.Minute,
		Jitter: true,
	}

	for {
		if token := c.Client.Connect(); !token.Wait() || token.Error() != nil {
			log.WithFields(log.Fields{
				"error": token.Error(),
			}).Error("Error connecting to MQTT")

			delay := b.Duration()

			log.WithFields(log.Fields{
				"delay": delay,
			}).Debug("Delayed (re)connecting to MQTT")

			time.Sleep(delay)
		}

		b.Reset()
		break
	}
}

// NewMQTTDiscovery - Proxy a new MQTTDiscovery object
func (c *MQTTProxy) NewMQTTDiscovery(name string, sensor string, sensorType string) *MQTTDiscovery {
	mqd := MQTTDiscovery{
		DiscoveryPrefix:   c.DiscoveryPrefix,
		Component:         sensorType,
		NodeID:            c.DiscoveryName,
		ObjectID:          discoveryObjectID(name, sensor),
		AvailabilityTopic: c.AvailabilityTopic(),
		Name:              discoveryObjectName(c.DiscoveryName, name, sensor),
		StateTopic:        c.StateTopic(name, sensor),
		UniqueID:          discoveryObjectUniqueID(c.DiscoveryName, name, sensor),
		Device: MQTTDiscoveryDevice{
			Identifiers:  []string{c.AvailabilityTopic()},
			Manufacturer: "twomqtt",
			Name:         c.DiscoveryName,
		},
	}

	return &mqd
}

// AvailabilityTopic - Generate the availability topic
func (c *MQTTProxy) AvailabilityTopic() string {
	return fmt.Sprintf("%s/status", c.TopicPrefix)
}

// StateTopic - Generate the state topic for a named sensor
func (c *MQTTProxy) StateTopic(name string, sensor string) string {
	return fmt.Sprintf("%s/%s/state", c.TopicPrefix, discoveryTopicID(name, sensor))
}

// CommandTopic - Generate the command topic for a named sensor
func (c *MQTTProxy) CommandTopic(name string, sensor string) string {
	return fmt.Sprintf("%s/%s/command", c.TopicPrefix, discoveryTopicID(name, sensor))
}
