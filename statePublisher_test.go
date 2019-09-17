package twomqtt

import (
	"testing"
)

func TestStateObserverRegister(t *testing.T) {
	p := &StatePublisher{}
	m := &mockStateObserver{}

	// Shouldn't panic with nil map
	p.Register(m)
}

type mockStateObserver struct {
	CommandObserver
}

func (c *mockStateObserver) ReceiveState(e Event) {

}
