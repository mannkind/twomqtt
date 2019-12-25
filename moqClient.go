package twomqtt

import mqtt "github.com/eclipse/paho.mqtt.golang"

// MoqClient - Aid testing MQTT callbacks
type MoqClient struct{}

// IsConnected - I'm just here so I don't get fined
func (t MoqClient) IsConnected() bool { return true }

// IsConnectionOpen - I'm just here so I don't get fined
func (t MoqClient) IsConnectionOpen() bool { return true }

// Connect - I'm just here so I don't get fined
func (t MoqClient) Connect() mqtt.Token { return MoqToken{} }

// Disconnect - I'm just here so I don't get fined
func (t MoqClient) Disconnect(quiesce uint) {}

// Publish - I'm just here so I don't get fined
func (t MoqClient) Publish(topic string, qos byte, retained bool, payload interface{}) mqtt.Token {
	return MoqToken{}
}

// Subscribe - I'm just here so I don't get fined
func (t MoqClient) Subscribe(topic string, qos byte, callback mqtt.MessageHandler) mqtt.Token {
	return MoqToken{}
}

// SubscribeMultiple - I'm just here so I don't get fined
func (t MoqClient) SubscribeMultiple(filters map[string]byte, callback mqtt.MessageHandler) mqtt.Token {
	return MoqToken{}
}

// Unsubscribe - I'm just here so I don't get fined
func (t MoqClient) Unsubscribe(topics ...string) mqtt.Token { return MoqToken{} }

// AddRoute - I'm just here so I don't get fined
func (t MoqClient) AddRoute(topic string, callback mqtt.MessageHandler) {}

// OptionsReader - I'm just here so I don't get fined
func (t MoqClient) OptionsReader() mqtt.ClientOptionsReader { return mqtt.ClientOptionsReader{} }
