package twomqtt

import "strings"

// SimpleKVMapParser - Aids parsinng simple key:value,key:value,key:value mapping config items
func SimpleKVMapParser(kvSep string, itemSep string) func(v string) (interface{}, error) {
	if kvSep == "" {
		kvSep = ":"
	}

	if itemSep == "" {
		itemSep = ","
	}

	return func(v string) (interface{}, error) {
		result := map[string]string{}
		pieces := strings.Split(v, itemSep)
		for _, m := range pieces {
			parts := strings.Split(m, kvSep)

			// Default to an empty name
			if len(parts) == 1 {
				parts = append(parts, "")
			}

			result[parts[0]] = parts[1]
		}
		return result, nil
	}
}
