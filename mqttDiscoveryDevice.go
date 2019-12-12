package twomqtt

// MQTTDiscoveryDevice - Contains the Device properties for MQTT Discovery.
type MQTTDiscoveryDevice struct {
	Identifiers  []string `json:"identifiers,omitempty"`
	Connections  []string `json:"connections,omitempty"`
	Manufacturer string   `json:"manufacturer,omitempty"`
	Model        string   `json:"model,omitempty"`
	Name         string   `json:"name,omitempty"`
	SWVersion    string   `json:"sw_version,omitempty"`
}
