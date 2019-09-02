package twomqtt

import (
	log "github.com/sirupsen/logrus"
)

// CommandPublisher - Determine commands and inform observers
type CommandPublisher struct {
	observers map[CommandObserver]struct{}
}

// Register - Register an observer
func (c *CommandPublisher) Register(l CommandObserver) {
	c.observers[l] = struct{}{}
}

// SendCommand - Send commands to observers
func (c *CommandPublisher) SendCommand(cmd Command, e Event) {
	log.WithFields(log.Fields{
		"Event": e,
	}).Debug("Sending command to observers")

	for o := range c.observers {
		o.ReceiveCommand(cmd, e)
	}

	log.Debug("Finished sending command to observers")
}
