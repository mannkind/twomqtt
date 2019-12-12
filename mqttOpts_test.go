package twomqtt

import (
	"testing"
)

func TestDiscoveryCalculateSlug(t *testing.T) {
	var tests = []struct {
		DeviceName string
		SensorName string
		Delim      string
		Expected   string
	}{
		{
			"name",
			"sensor",
			".",
			"name.sensor",
		},
		{
			"",
			"sensor",
			".",
			"sensor",
		},
	}

	c := MQTTOpts{}
	for _, v := range tests {
		actual := c.discoveryCalculateSlug(v.DeviceName, v.SensorName, v.Delim)
		if actual != v.Expected {
			t.Errorf("Actual:%s\nExpected:%s", actual, v.Expected)
		}
	}
}

func TestDiscoveryTopicID(t *testing.T) {
	var tests = []struct {
		DeviceName string
		SensorName string
		Expected   string
	}{
		{
			"name",
			"sensor",
			"name/sensor",
		},
		{
			"",
			"sensor",
			"sensor",
		},
	}

	c := MQTTOpts{}
	for _, v := range tests {
		actual := c.discoveryTopicID(v.DeviceName, v.SensorName)
		if actual != v.Expected {
			t.Errorf("Actual:%s\nExpected:%s", actual, v.Expected)
		}
	}
}

func TestDiscoveryObjectID(t *testing.T) {
	var tests = []struct {
		DeviceName string
		SensorName string
		Expected   string
	}{
		{
			"name",
			"sensor",
			"name_sensor",
		},
		{
			"",
			"sensor",
			"sensor",
		},
	}

	c := MQTTOpts{}
	for _, v := range tests {
		actual := c.discoveryObjectID(v.DeviceName, v.SensorName)
		if actual != v.Expected {
			t.Errorf("Actual:%s\nExpected:%s", actual, v.Expected)
		}
	}
}

func TestDiscoveryObjectName(t *testing.T) {
	var tests = []struct {
		DiscoveryName string
		DeviceName    string
		SensorName    string
		Expected      string
	}{
		{
			"dname",
			"name",
			"sensor",
			"dname name sensor",
		},
		{
			"dname",
			"",
			"sensor",
			"dname sensor",
		},
	}

	for _, v := range tests {
		c := MQTTOpts{
			DiscoveryName: v.DiscoveryName,
		}
		actual := c.discoveryObjectName(v.DeviceName, v.SensorName)
		if actual != v.Expected {
			t.Errorf("Actual:%s\nExpected:%s", actual, v.Expected)
		}
	}
}

func TestDiscoveryObjectUniqueID(t *testing.T) {
	var tests = []struct {
		DiscoveryName string
		DeviceName    string
		SensorName    string
		Expected      string
	}{
		{
			"dname",
			"name",
			"sensor",
			"dname.name.sensor",
		},
		{
			"dname",
			"",
			"sensor",
			"dname.sensor",
		},
	}

	for _, v := range tests {
		c := MQTTOpts{
			DiscoveryName: v.DiscoveryName,
		}
		actual := c.discoveryObjectUniqueID(v.DeviceName, v.SensorName)
		if actual != v.Expected {
			t.Errorf("Actual:%s\nExpected:%s", actual, v.Expected)
		}
	}
}
