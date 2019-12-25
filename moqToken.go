package twomqtt

import "time"

// MoqToken - Aid MQTT testing
type MoqToken struct{}

// Wait - I'm just here so I don't get fined
func (t MoqToken) Wait() bool { return true }

// WaitTimeout - I'm just here so I don't get fined
func (t MoqToken) WaitTimeout(time.Duration) bool { return true }

// Error - I'm just here so I don't get fined
func (t MoqToken) Error() error { return nil }
