FROM mcr.microsoft.com/dotnet/sdk:7.0 as build
WORKDIR /src
COPY . .
RUN if [ ! -d output ]; then dotnet build -o output -c Release PollingExample; fi

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS runtime
COPY --from=build /src/output app
ENTRYPOINT ["dotnet", "./app/PollingExample.dll"]
