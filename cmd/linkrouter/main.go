package main

import (
	"linkrouter/config"
	"linkrouter/internal/routing"
	"log"

	"github.com/gin-gonic/gin"
)

func main() {
	configuration, err := config.Load("data/config.json")

	if err != nil {
		log.Fatal(err)
		return
	}

	log.Println("LinkRouter - mxritz.dev")

	router := gin.Default()

	routing.Routes(router, configuration)

	router.Run(":8080")
}
