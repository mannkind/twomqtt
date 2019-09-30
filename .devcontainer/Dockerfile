FROM golang:1.13
ARG USERNAME=vscode
ARG USER_UID=1000
ARG USER_GID=$USER_UID

# Configure apt, install packages and tools
RUN apt-get update \
	&& apt-get install -y git vim 2>&1 \
	# Install gocode-gomod
	&& go get -x -d github.com/stamblerre/gocode 2>&1 \
	&& go build -o gocode-gomod github.com/stamblerre/gocode \
	&& mv gocode-gomod $GOPATH/bin/ \
	#
	# Install Go tools
	&& go get -u -v \
	github.com/mdempsky/gocode \
	github.com/uudashr/gopkgs/cmd/gopkgs \
	github.com/ramya-rao-a/go-outline \
	github.com/acroca/go-symbols \
	github.com/godoctor/godoctor \
	golang.org/x/tools/cmd/guru \
	golang.org/x/tools/cmd/gorename \
	github.com/rogpeppe/godef \
	github.com/zmb3/gogetdoc \
	github.com/haya14busa/goplay/cmd/goplay \
	github.com/sqs/goreturns \
	github.com/josharian/impl \
	github.com/davidrjenni/reftools/cmd/fillstruct \
	github.com/fatih/gomodifytags \
	github.com/cweill/gotests/... \
	golang.org/x/tools/cmd/goimports \
	golang.org/x/lint/golint \
	golang.org/x/tools/gopls \
	github.com/alecthomas/gometalinter \
	honnef.co/go/tools/... \
	github.com/golangci/golangci-lint/cmd/golangci-lint \
	github.com/mgechev/revive \
	github.com/derekparker/delve/cmd/dlv 2>&1 \
	# Install mage
	&& git clone https://github.com/magefile/mage \
	&& cd mage && go run bootstrap.go install && cd - \
	&& rm -rf mage \
	#
	# Create a non-root user to use if preferred - see https://aka.ms/vscode-remote/containers/non-root-user.
	&& groupadd --gid $USER_GID $USERNAME \
	&& useradd -s /bin/bash --uid $USER_UID --gid $USER_GID -m $USERNAME \
	# Clean up
	&& apt-get autoremove -y \
	&& apt-get clean -y \
	&& rm -rf /var/lib/apt/lists/*
