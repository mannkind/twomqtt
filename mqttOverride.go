package twomqtt

import (
	"reflect"
	"strings"
)

// MQTTOverride -
func MQTTOverride(field reflect.StructField) (string, bool) {
	opt := field.Tag.Get("mqtt")
	return opt, strings.Contains(opt, ",ignore")
}
