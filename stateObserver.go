package twomqtt

// StateObserver - Observe state changes and do something with them (e.g. publish over MQTT)
type StateObserver interface {
	ReceiveState(Event)
}
