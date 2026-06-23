FROM golang:1.26.4-alpine AS builder

WORKDIR /build

COPY go.mod go.sum ./

RUN go mod download

COPY . .

ARG TARGETOS
ARG TARGETARCH

RUN CGO_ENABLED=0 GOOS=${TARGETOS:-linux} GOARCH=${TARGETARCH:-amd64} \
    go build -o app .

FROM alpine:3.20

WORKDIR /bin

COPY --from=builder /build/app .

WORKDIR /

ENTRYPOINT ["./bin/app"]