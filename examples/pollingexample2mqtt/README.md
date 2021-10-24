# pollingexample2mqtt

## Configuration

Configuration happens via environmental variables

```bash
POLLINGEXAMPLE__POLLINGINTERVAL                    - [OPTIONAL] The delay between travel time lookups lookups, defaults to "0.00:01:07"
POLLINGEXAMPLE__MQTT__TOPICPREFIX                  - [OPTIONAL] The MQTT topic on which to publish the collection lookup results, defaults to "home/pollingexample"
POLLINGEXAMPLE__MQTT__DISCOVERYENABLED             - [OPTIONAL] The MQTT discovery flag for Home Assistant, defaults to false
POLLINGEXAMPLE__MQTT__DISCOVERYPREFIX              - [OPTIONAL] The MQTT discovery prefix for Home Assistant, defaults to "homeassistant"
POLLINGEXAMPLE__MQTT__DISCOVERYNAME                - [OPTIONAL] The MQTT discovery name for Home Assistant, defaults to "pollingexample"
POLLINGEXAMPLE__MQTT__BROKER                       - [OPTIONAL] The MQTT broker, defaults to "test.mosquitto.org"
POLLINGEXAMPLE__MQTT__USERNAME                     - [OPTIONAL] The MQTT username, default to ""
POLLINGEXAMPLE__MQTT__PASSWORD                     - [OPTIONAL] The MQTT password, default to ""
```
