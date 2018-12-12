package twomqtt

import (
	"fmt"
	"reflect"
	"strings"
)

// MQTTOverride -
func MQTTOverride(field reflect.StructField) (string, bool) {
	opt := field.Tag.Get("mqtt")
	return opt, strings.Contains(opt, ",ignore")
}

// MQTTDiscoveryOverride -
func MQTTDiscoveryOverride(field reflect.StructField) (string, bool) {
	opt := field.Tag.Get("mqttDiscoveryType")
	return opt, strings.Contains(opt, ",ignore")
}

func discoveryCalculateSlug(name string, sensor string, sep string) string {
	slug := sensor
	if name != "" {
		slug = fmt.Sprintf("%s%s%s", name, sep, sensor)
	}

	return strings.ToLower(slug)
}

func discoveryTopicID(name string, sensor string) string {
	return discoveryCalculateSlug(name, sensor, "/")
}

func discoveryObjectID(name string, sensor string) string {
	return discoveryCalculateSlug(name, sensor, "_")
}

func discoveryObjectName(discoveryName string, name string, sensor string) string {
	return fmt.Sprintf("%s %s", discoveryName, discoveryCalculateSlug(name, sensor, " "))
}

func discoveryObjectUniqueID(discoveryName string, name string, sensor string) string {
	return fmt.Sprintf("%s.%s", discoveryName, discoveryCalculateSlug(name, sensor, "."))
}
