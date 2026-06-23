FROM golang:1.26.4-alpine AS builder

WORKDIR /build

COPY ../go.mod go.sum ./

RUN go mod download

COPY .. .

ENV CGO_ENABLED=0 \
    GOOS=linux \
    GOARCH=amd64

RUN go build -o app ./cmd/linkrouter

FROM alpine:3.20

WORKDIR /bin

COPY --from=builder /build/app .

WORKDIR /

EXPOSE 8080

ENTRYPOINT ["./bin/app"]