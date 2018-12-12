package twomqtt

// GeneralConfig - Configuration applicable to the genereal application
type GeneralConfig struct {
	DebugLogLevel bool `env:"DEBUG" envDefault:"false"`
}
