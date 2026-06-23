package config

type Config struct {
	RootRedirect      string            `json:"RootRedirect"`
	NotFoundBehaviour NotFoundBehaviour `json:"NotFoundBehaviour"`
	Routes            []Route           `json:"Routes"`
}

type NotFoundBehaviour struct {
	RedirectOn404 bool   `json:"RedirectOn404"`
	RedirectUrl   string `json:"RedirectUrl"`
}

type Route struct {
	Route       string `json:"Route"`
	RedirectUrl string `json:"RedirectUrl"`
}
