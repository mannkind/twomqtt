package twomqtt

import (
	"encoding/json"
	"sync"
	"time"

	mqtt "github.com/eclipse/paho.mqtt.golang"
	backoff "github.com/jpillora/backoff"
	log "github.com/sirupsen/logrus"
)

// MQTT - A wrapper for the Paho MQTT
type MQTT struct {
	MQTTOpts
	connectHandler    func()
	disconnectHandler func()
	subscribeHandler  func()
	discoveryHandler  func() []MQTTDiscovery
	readHandler       func()
	client            mqtt.Client
	lastPublished     sync.Map
}

// NewMQTT - Create a wrapper for Paho MQTT
func NewMQTT(opts MQTTOpts) *MQTT {
	c := MQTT{
		MQTTOpts: opts,
	}

	return &c
}

// SetDiscoveryHandler - Sets the function to be called when it's time to publish MQTT discovery messages
func (c *MQTT) SetDiscoveryHandler(handler func() []MQTTDiscovery) *MQTT {
	c.discoveryHandler = handler

	return c
}

// SetSubscribeHandler - Sets the function to be called when it's time to subscribe to MQTT topics
func (c *MQTT) SetSubscribeHandler(handler func()) *MQTT {
	c.subscribeHandler = handler

	return c
}

// SetReadIncomingChannelHandler - Sets the function to be called when it's time to subscribe to MQTT topics
func (c *MQTT) SetReadIncomingChannelHandler(handler func()) *MQTT {
	c.readHandler = handler

	return c
}

// SetConnectHandler - Sets the function to be called at the end of a successful MQTT connection
func (c *MQTT) SetConnectHandler(handler func()) *MQTT {
	c.connectHandler = handler

	return c
}

// SetDisconnectHandler - Sets the function to be called at the end of a successful MQTT connection
func (c *MQTT) SetDisconnectHandler(handler func()) *MQTT {
	c.disconnectHandler = handler

	return c
}

// Initialize - Initialize the MQTT Client with all the appropriate settings
func (c *MQTT) Initialize() *MQTT {
	opts := mqtt.NewClientOptions().
		AddBroker(c.Broker).
		SetClientID(c.ClientID).
		SetOnConnectHandler(c.onConnect).
		SetConnectionLostHandler(c.onDisconnect).
		SetUsername(c.Username).
		SetPassword(c.Password).
		SetWill(c.AvailabilityTopic(), "offline", 0, true)

	c.client = mqtt.NewClient(opts)

	return c
}

// PublishWithOpts - Publish a topic and payload with specific options
func (c *MQTT) PublishWithOpts(topic string, payload string, opts MQTTPublishOpts) MQTTMessage {
	llog := log.WithFields(log.Fields{
		"topic":   topic,
		"payload": payload,
	})

	// Should we publish this again if it's a duplicate
	if opts.DuplicateCheck {
		if lastPayload, ok := c.lastPublished.Load(topic); ok && lastPayload == payload {
			llog.Debug("Duplicate avoided while publishing to MQTT")
			return MQTTMessage{}
		}
	}

	llog.Info("Publishing to MQTT")
	if token := c.client.Publish(topic, 0, opts.Retained, payload); token.Wait() && token.Error() != nil {
		log.Error("Error publishing to MQTT")
	}

	log.Info("Finished publishing to MQTT")
	c.lastPublished.Store(topic, payload)

	return MQTTMessage{
		Topic:   topic,
		Payload: payload,
	}
}

// Publish - Publish a topic and payload with default options
func (c *MQTT) Publish(topic string, payload string) MQTTMessage {
	return c.PublishWithOpts(topic, payload, MQTTPublishOpts{
		Retained:       true,
		DuplicateCheck: true,
	})
}

// Subscribe - Subscribe to a topic and payload via the MQTTClientWrapperWrapper
func (c *MQTT) Subscribe(topic string, qos byte, callback mqtt.MessageHandler) mqtt.Token {
	log.WithFields(log.Fields{
		"topic": topic,
		"qos":   qos,
	}).Info("Subscribing to MQTT topic")

	result := c.client.Subscribe(topic, qos, callback)

	log.Info("Finished subscribing to MQTT topic")
	return result
}

// Run - Connect to MQTT
func (c *MQTT) Run() {
	// Log the current settings
	c.logSettings()

	// (Optionall) Start reading from the incoming channel
	if c.readHandler != nil {
		go c.readHandler()
	}

	// Connect to MQTT broker, with backoff/jitter
	log.Info("Connecting to MQTT")
	b := &backoff.Backoff{
		Max:    5 * time.Minute,
		Jitter: true,
	}

	for {
		if token := c.client.Connect(); !token.Wait() || token.Error() != nil {
			log.WithFields(log.Fields{
				"error": token.Error(),
			}).Error("Error connecting to MQTT")

			delay := b.Duration()

			log.WithFields(log.Fields{
				"delay": delay,
			}).Debug("Delayed (re)connecting to MQTT")

			time.Sleep(delay)
			continue
		}

		b.Reset()
		break
	}
}

func (c *MQTT) logSettings() {
	log.Info("Logging MQTT Environment Settings")

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
	}).Info("MQTT Environment Settings")

	log.Info("Logged MQTT Environment Settings")
}

func (c *MQTT) publishDiscovery() {
	if c.discoveryHandler == nil {
		return
	}

	log.Info("Publishing MQTT Discovery")

	mqds := c.discoveryHandler()
	for _, mqd := range mqds {
		topic := c.discoveryTopic(mqd.Component, mqd.NodeID, mqd.ObjectID)
		payloadBytes, _ := json.Marshal(mqd)
		payload := string(payloadBytes)

		c.Publish(topic, payload)
	}

	log.Info("Published MQTT Discovery")
}

func (c *MQTT) onConnect(client mqtt.Client) {
	log.Info("Connected to MQTT")

	// Publish availabilty
	c.client.Publish(c.AvailabilityTopic(), 0, true, "online")

	// (Optional) Publish discovery
	c.publishDiscovery()

	// (Optional) Subscribe to relevant topics
	if c.subscribeHandler != nil {
		c.subscribeHandler()
	}

	// (Optional) Any additional things after connecting
	if c.connectHandler != nil {
		c.connectHandler()
	}
}

func (c *MQTT) onDisconnect(client mqtt.Client, err error) {
	log.WithFields(log.Fields{
		"error": err,
	}).Error("Disconnected from MQTT")

	// (Optional) Any additional things after disconnecting
	if c.disconnectHandler != nil {
		c.disconnectHandler()
	}
}
