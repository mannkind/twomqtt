# Base Build Image
FROM golang:1.13-alpine3.10
WORKDIR /app
RUN apk add --no-cache --update build-base git && \
    git clone https://github.com/magefile/mage && \
    cd mage && go run bootstrap.go install && cd - && \
    rm -rf mage