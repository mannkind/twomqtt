GOCMD=go
GOBUILD=$(GOCMD) build
GOCLEAN=$(GOCMD) clean
GOTEST=$(GOCMD) test
GOGET=$(GOCMD) get
GOFMT=$(GOCMD) fmt
GOVET=$(GOCMD) vet
WIRECMD=wire gen

all: clean wire build test format vet
test: 
		$(GOTEST) --coverprofile=/tmp/app.cover -v ./...
format:
	    $(GOFMT) .
clean: 
		$(GOCLEAN)
vet:
	    $(GOVET) .
get_wire:
		$(GOGET) github.com/google/wire/cmd/wire
wire: get_wire
		$(WIRECMD)
build: format
		$(GOBUILD) $(BINARY_VERSION_FLAGS) -o /tmp/a.out -v