package twomqtt

import (
	"testing"
)

func TestCommandObserverRegister(t *testing.T) {
	p := &CommandPublisher{}
	m := &mockCommandObserver{}

	// Shouldn't panic with nil map
	p.Register(m)
}

type mockCommandObserver struct {
	CommandObserver
}

func (c *mockCommandObserver) ReceiveCommand(cmd Command, e Event) {

}
