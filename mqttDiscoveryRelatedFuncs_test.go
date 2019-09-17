package twomqtt

import (
	"testing"
)

func TestDiscoveryCalculateSlug(t *testing.T) {
	var tests = []struct {
		Name     string
		Sensor   string
		Sep      string
		Expected string
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

	for _, v := range tests {
		actual := discoveryCalculateSlug(v.Name, v.Sensor, v.Sep)
		if actual != v.Expected {
			t.Errorf("Actual:%s\nExpected:%s", actual, v.Expected)
		}
	}
}

func TestDiscoveryTopicID(t *testing.T) {
	var tests = []struct {
		Name     string
		Sensor   string
		Expected string
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

	for _, v := range tests {
		actual := discoveryTopicID(v.Name, v.Sensor)
		if actual != v.Expected {
			t.Errorf("Actual:%s\nExpected:%s", actual, v.Expected)
		}
	}
}

func TestDiscoveryObjectID(t *testing.T) {
	var tests = []struct {
		Name     string
		Sensor   string
		Expected string
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

	for _, v := range tests {
		actual := discoveryObjectID(v.Name, v.Sensor)
		if actual != v.Expected {
			t.Errorf("Actual:%s\nExpected:%s", actual, v.Expected)
		}
	}
}

func TestDiscoveryObjectName(t *testing.T) {
	var tests = []struct {
		DiscoveryName string
		Name          string
		Sensor        string
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
		actual := discoveryObjectName(v.DiscoveryName, v.Name, v.Sensor)
		if actual != v.Expected {
			t.Errorf("Actual:%s\nExpected:%s", actual, v.Expected)
		}
	}
}

func TestDiscoveryObjectUniqueID(t *testing.T) {
	var tests = []struct {
		DiscoveryName string
		Name          string
		Sensor        string
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
		actual := discoveryObjectUniqueID(v.DiscoveryName, v.Name, v.Sensor)
		if actual != v.Expected {
			t.Errorf("Actual:%s\nExpected:%s", actual, v.Expected)
		}
	}
}
