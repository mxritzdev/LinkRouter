# This version is still in early development, use the old version instead [Install old version](https://github.com/mxritzdev/LinkRouter/tree/master#installation)

# Linkrouter
**LinkRouter** is a lightweight, configurable routing application that directs incoming requests to designated target URLs based on a user-defined configuration file.

[⬇️How to install⬇️](#installation)

## Features
-   **Path-based Redirection:** Reads a config file that maps paths to redirect URLs. When a request hits a registered path, the router issues an HTTP redirect to the corresponding target.
-   **Low Resource Usage:** Uses less than 50MB of RAM, making it ideal for constrained environments. 
-   **Docker-Deployable:** Comes with a minimal Dockerfile for easy containerized deployment.
-   **Placeholders:** Supports placeholders in redirect URLs, allowing dynamic URL generation based on the requested path. For example, a route defined as `/user/{username}` can redirect to `https://example.com/profile/{username}`, where `{username}` is replaced with the actual value from the request.
-   **Status Code:** You are able to configure if the redirect should redirect to an url or just return a custom status code of your choice. Example `"RedirectUrl": "-> 418"` will return the status code 418 (I'm a teapot :) )

## Configuration
Routes are managed via a configuration file, `/data/config.json`. You can define paths and their corresponding URLs in this file. The application automatically normalizes routes to handle both trailing and non-trailing slashes.
> Every route **must** start with a slash
### Example Config
```json
{
  "RootRedirect": "https://example.com", // leave empty for 404
  "NotFoundBehaviour": {
    "RedirectOn404": false,
    "RedirectUrl": ""
  },
  "Routes": [
    {
      "Route": "/github", // has to start with a /
      "RedirectUrl": "https://github.com/yourgithub"
    },
    {
      "Route": "/article/{id}", // {id} is a placeholder
      "RedirectUrl": "https://example.com/article/{id}" // {id} will be replaced in with the actual value from the request
    },
    {
      "Route": "/teapot",
      "RedirectUrl": "-> 418" // returns status code 418 (I'm a teapot :) )
    },
  ]
}
```
## Installation
> **Docker** is required to deploy this project. [Install docker](https://docs.docker.com/get-started/)

### Using Docker Compose - **NOT YET CONFIGURED**
1. Create a `docker-compose.yml` file with the following content:

```yaml
services:
  linkrouter:
    image: ghcr.io/mxritzdev/linkrouter:latest
    ports:
      - "80:8080"
    volumes:
      - ./data:/app/data
```
2. Run `docker compose up -d` to start the container
3. Configure your routes in `./data/config.json`
### Using `docker run`
1. Run this command: `docker run -p 80:8080 -v ./data:/app/data ghcr.io/mxritzdev/linkrouter:latest`
2. Configure your routes in `./data/config.json`

## Contributing

Contributions are welcome! Please submit a pull request or open an issue to discuss improvements or new features.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

##  Contact

For questions or support, please reach out via discord at **mxritzdev**
