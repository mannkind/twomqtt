#!/bin/bash

if [[ -z $1 ]]; then
    echo "A project name is needed"
    exit 1
fi

TWOMQTT=$(echo "$1" | tr '[:upper:]' '[:lower:]')2mqtt
PROJECT=$1
PROJECTLOWER=$(echo "$1" | tr '[:upper:]' '[:lower:]')
PROJECTUPPER=$(echo "$1" | tr '[:lower:]' '[:upper:]')
PROJECTTEST=${PROJECT}Test

echo "Creating directory called $TWOMQTT"
cp -r pollingexample2mqtt $TWOMQTT

echo "Creating project $PROJECT"
cd $TWOMQTT
mv PollingExample $PROJECT
mv $PROJECT/PollingExample.csproj $PROJECT/$PROJECT.csproj

echo "Creating $PROJECTTEST"
mv PollingExampleTest $PROJECTTEST
mv $PROJECTTEST/PollingExampleTest.csproj $PROJECTTEST/$PROJECTTEST.csproj

echo "Creating template files"
find . -type f -exec \
    sed -i \
        -e "s/pollingexample/$PROJECTLOWER/g" \
        -e "s/PollingExample/$PROJECT/g" \
        -e "s/POLLINGEXAMPLE/$PROJECTUPPER/g" \
        {} \;

echo "Building $PROJECT"
dotnet build $PROJECT

echo "Building $PROJECTTEST"
dotnet test $PROJECTTEST