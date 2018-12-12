package twomqtt

import (
	"reflect"
)

// Command - A type used to communicate commands between publisher and observer
type Command = int64

// Event - A type used to communicate state between publisher and observer
type Event struct {
	Type    reflect.Type
	Payload interface{}
}

// Observer -
type Observer interface {
	ReceiveState(Event)
	ReceiveCommand(Command, Event)
}

// Publisher -
type Publisher interface {
	Register(Observer)
}
