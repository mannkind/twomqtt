package twomqtt

// CommandObserver - Observe commands and take action
type CommandObserver interface {
	ReceiveCommand(Command, Event)
}
