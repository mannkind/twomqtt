package twomqtt

import (
	"reflect"
	"strings"
)

// MQTTDiscoveryOverride -
func MQTTDiscoveryOverride(field reflect.StructField) (string, bool) {
	opt := field.Tag.Get("mqttDiscoveryType")
	return opt, strings.Contains(opt, ",ignore")

}
