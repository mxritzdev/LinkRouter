package config

import (
	"encoding/json"
	"os"
)

func Load(path string) (*Config, error) {

	file, err := os.OpenFile(path, os.O_CREATE|os.O_RDONLY, 0644)
	if err != nil {
		return nil, err
	}

	defer file.Close()

	data, err := os.ReadFile(path)

	if err != nil {
		return nil, err
	}

	var config Config

	dummyData(&config)

	if string(data) == "" {
		data = []byte("{}")
	}

	if err := json.Unmarshal(data, &config); err != nil {
		return nil, err
	}

	newJson, err := json.MarshalIndent(&config, "", "  ")

	if err := os.WriteFile(path, newJson, 0644); err != nil {
		return nil, err
	}

	return &config, nil

}

func dummyData(config *Config) {
	config.RootRedirect = "https://example.com"

	config.Routes = []Route{
		{Route: "/github", RedirectUrl: "https://github.com/yourgithub"},
		{Route: "/example", RedirectUrl: "https://example.com/article"},
	}
}
