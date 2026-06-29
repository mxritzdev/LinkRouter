package main

import (
	"linkrouter/src/config"
	"linkrouter/src/internal/routing"
	"log"

	"github.com/gin-gonic/gin"
)

func main() {
	configuration, err := config.Load("./data/config.json")

	if err != nil {
		log.Fatal(err)
		return
	}

	log.Println("LinkRouter - mxritz.dev ")

	router := gin.Default()

	routing.Routes(router, configuration)

	log.Println("Running on 0.0.0.0:8080")
	router.Run(":8080")
}
