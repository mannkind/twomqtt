package twomqtt

// MQTTProxyConfig - Configuration applicable to the MQTTProxy
type MQTTProxyConfig struct {
	ClientID        string `env:"MQTT_CLIENTID"`
	Broker          string `env:"MQTT_BROKER" envDefault:"tcp://test.mosquitto.org:1883"`
	Username        string `env:"MQTT_USERNAME"`
	Password        string `env:"MQTT_PASSWORD"`
	TopicPrefix     string `env:"MQTT_TOPICPREFIX"`
	Discovery       bool   `env:"MQTT_DISCOVERY" envDefault:"false"`
	DiscoveryPrefix string `env:"MQTT_DISCOVERYPREFIX" envDefault:"homeassistant"`
	DiscoveryName   string `env:"MQTT_DISCOVERYNAME"`
}
