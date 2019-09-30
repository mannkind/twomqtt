// +build mage

package main

import (
	"fmt"

	"github.com/magefile/mage/mg"
	"github.com/magefile/mage/sh"
)

const (
	BaseBinaryVersion = "latest"
	BinaryName        = "twomqtt"
	BaseDockerAccount = "mannkind"
)

// Everything below here is the same between projects
var DockerImage = fmt.Sprintf("%s/%s", BaseDockerAccount, BinaryName)

// Commands
var g0 = sh.RunCmd("go")
var git = sh.RunCmd("git")
var docker = sh.RunCmd("docker")

type Go mg.Namespace
type Git mg.Namespace
type Docker mg.Namespace

// Default target to run when none is specified
var Default = All

// go:wire, go:format, go:vet, go:build, and go:test in that order
func All() {
	mg.SerialDeps(Go.Wire)
	mg.SerialDeps(Go.Format)
	mg.SerialDeps(Go.Vet)
	mg.SerialDeps(Go.Build)
	mg.SerialDeps(Go.Test)
	mg.SerialDeps(Go.Tidy)
}

// Remove the binary and architecture specific Dockerfiles
func Clean() error {
	fmt.Println("Cleaning")
	if err := sh.Run("rm", "-f", BinaryName); err != nil {
		return err
	}

	return nil
}

// Compile the application with the proper ldflags
func (Go) Build() error {
	fmt.Println("Building")
	return g0("build", ".")
}

// Ensure the code is formatted properly
func (Go) Format() error {
	fmt.Println("Formatting")
	return g0("fmt", ".")
}

// Ensure the code passes vetting
func (Go) Vet() error {
	fmt.Println("Vetting")
	return g0("vet", ".")
}

// Get the compile-time DI tool
func (Go) GetWire() error {
	fmt.Println("Getting Wire")
	return g0("get", "github.com/google/wire/cmd/wire")
}

// Generate the dependencies at compile-time
func (Go) Wire() error {
	mg.SerialDeps(Go.GetWire)

	fmt.Println("Wiring")
	return sh.Run("wire", "gen")
}

// Run the application tests
func (Go) Test() error {
	mg.SerialDeps(Go.Build)

	fmt.Println("Testing")
	return g0("test", "--coverprofile", "/tmp/app.cover", "-v", ".")
}

// Run the tidy command
func (Go) Tidy() error {
	mg.SerialDeps(Go.Build)

	fmt.Println("Tidying")
	return g0("mod", "tidy")
}
