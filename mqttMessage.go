package twomqtt

// MQTTMessage - The result of a publish action
type MQTTMessage struct {
	Topic   string
	Payload string
}
