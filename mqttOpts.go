package twomqtt

import (
	"fmt"
	"strings"
)

// MQTTOpts - Configuration applicable to the MQTTProxy
type MQTTOpts struct {
	ClientID        string `env:"MQTT_CLIENTID"`
	Broker          string `env:"MQTT_BROKER" envDefault:"tcp://test.mosquitto.org:1883"`
	Username        string `env:"MQTT_USERNAME"`
	Password        string `env:"MQTT_PASSWORD"`
	TopicPrefix     string `env:"MQTT_TOPICPREFIX"`
	Discovery       bool   `env:"MQTT_DISCOVERY" envDefault:"false"`
	DiscoveryPrefix string `env:"MQTT_DISCOVERYPREFIX" envDefault:"homeassistant"`
	DiscoveryName   string `env:"MQTT_DISCOVERYNAME"`
}

// AvailabilityTopic - Generate the availability topic
func (c *MQTTOpts) AvailabilityTopic() string {
	return fmt.Sprintf("%s/status", c.TopicPrefix)
}

// StateTopic - Generate the state topic for a named sensor
func (c *MQTTOpts) StateTopic(deviceName string, sensorName string) string {
	return fmt.Sprintf("%s/%s/state", c.TopicPrefix, c.discoveryTopicID(deviceName, sensorName))
}

// CommandTopic - Generate the command topic for a named sensor
func (c *MQTTOpts) CommandTopic(deviceName string, sensorName string) string {
	return fmt.Sprintf("%s/%s/command", c.TopicPrefix, c.discoveryTopicID(deviceName, sensorName))
}

func (c *MQTTOpts) discoveryTopic(component string, nodeID string, objectID string) string {
	return fmt.Sprintf("%s/%s/%s/%s/config", c.DiscoveryPrefix, component, nodeID, objectID)
}

func (c *MQTTOpts) discoveryCalculateSlug(deviceName string, sensorName string, delim string) string {
	slug := sensorName
	if deviceName != "" {
		slug = fmt.Sprintf("%s%s%s", deviceName, delim, sensorName)
	}

	return strings.ToLower(slug)
}

func (c *MQTTOpts) discoveryTopicID(deviceName string, sensorName string) string {
	return c.discoveryCalculateSlug(deviceName, sensorName, "/")
}

func (c *MQTTOpts) discoveryObjectID(deviceName string, sensorName string) string {
	return c.discoveryCalculateSlug(deviceName, sensorName, "_")
}

func (c *MQTTOpts) discoveryObjectName(deviceName string, sensorName string) string {
	return fmt.Sprintf("%s %s", c.DiscoveryName, c.discoveryCalculateSlug(deviceName, sensorName, " "))
}

func (c *MQTTOpts) discoveryObjectUniqueID(deviceName string, sensorName string) string {
	return fmt.Sprintf("%s.%s", c.DiscoveryName, c.discoveryCalculateSlug(deviceName, sensorName, "."))
}
