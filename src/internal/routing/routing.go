package routing

import (
	"linkrouter/src/config"
	"log"
	"regexp"
	"strings"

	"github.com/gin-gonic/gin"
)

func Routes(router *gin.Engine, configuration *config.Config) {

	placeholderRegexp := regexp.MustCompile(`\{([a-zA-Z_][a-zA-Z0-9_]*)\}`)

	if configuration.RootRedirect != "" {
		router.GET("/", func(context *gin.Context) {
			err := Handle(context, configuration.RootRedirect, []string{})

			if err != nil {
				log.Fatalln(err)
				return
			}
		})
	}

	for _, route := range configuration.Routes {

		matches := placeholderRegexp.FindAllStringSubmatch(route.Route, -1)

		var placeholders []string

		for _, match := range matches {

			route.Route = strings.ReplaceAll(route.Route, match[0], ":"+match[1])

			placeholders = append(placeholders, match[1])

		}

		router.GET(route.Route, func(context *gin.Context) {
			err := Handle(context, route.RedirectUrl, placeholders)

			if err != nil {
				log.Fatalln(err)
				return
			}
		})
	}

	if configuration.NotFoundBehaviour.RedirectOn404 {
		router.NoRoute(func(context *gin.Context) {
			err := Handle(context, configuration.NotFoundBehaviour.RedirectUrl, []string{})

			if err != nil {
				log.Fatalln(err)
				return
			}
		})
	}

}
