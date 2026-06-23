package routing

import (
	"fmt"
	"net/http"
	"regexp"
	"strconv"
	"strings"

	"github.com/gin-gonic/gin"
)

func Handle(context *gin.Context, redirectUrl string, placeholders []string) error {

	statusCodeRegexp := regexp.MustCompile("^\\s*->\\s*(\\d{3})\\s*$")

	matches := statusCodeRegexp.FindStringSubmatch(redirectUrl)

	if matches != nil {
		statusCode, err := strconv.Atoi(matches[1])

		if err != nil || statusCode < 200 || statusCode > 599 {
			context.String(http.StatusInternalServerError, "Invalid status code, should be a number (200 <= x <= 599), please check the config")

			return nil
		}

		text := http.StatusText(statusCode)

		if text == "" {
			text = "Unknown Error"
		}

		context.String(statusCode, fmt.Sprintf("%d %s", statusCode, text))

		return nil
	}

	for _, placeholder := range placeholders {

		value := context.Param(placeholder)

		redirectUrl = strings.ReplaceAll(redirectUrl, "{"+placeholder+"}", value)
	}

	context.Redirect(http.StatusFound, redirectUrl)

	return nil
}
