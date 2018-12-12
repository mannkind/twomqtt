package twomqtt

// MoqMessage - Aid testing MQTT callbacks
type MoqMessage struct {
	TopicSrc   string
	PayloadSrc string
}

// Duplicate - I'm just here so I don't get fined
func (m *MoqMessage) Duplicate() bool {
	return false
}

// Qos - I'm just here so I don't get fined
func (m *MoqMessage) Qos() byte {
	return 0
}

// Retained - I'm just here so I don't get fined
func (m *MoqMessage) Retained() bool {
	return true
}

// Topic - I'm just here so I don't get fined
func (m *MoqMessage) Topic() string {
	return m.TopicSrc
}

// MessageID - I'm just here so I don't get fined
func (m *MoqMessage) MessageID() uint16 {
	return 0
}

// Payload - I'm just here so I don't get fined
func (m *MoqMessage) Payload() []byte {
	return []byte(m.PayloadSrc)
}

// Ack - I'm just here so I don't get fined
func (m *MoqMessage) Ack() {
}
