package twomqtt

import (
	log "github.com/sirupsen/logrus"
)

// StatePublisher - Determine states and inform observers
type StatePublisher struct {
	observers map[StateObserver]struct{}
}

// Register - Register an observer
func (c *StatePublisher) Register(l StateObserver) {
	c.observers[l] = struct{}{}
}

// SendState - Send states to observers
func (c *StatePublisher) SendState(e Event) {
	log.WithFields(log.Fields{
		"Event": e,
	}).Debug("Sending event to observers")

	for o := range c.observers {
		o.ReceiveState(e)
	}

	log.Debug("Finished sending event to observers")
}
