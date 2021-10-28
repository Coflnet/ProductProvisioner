FROM mcr.microsoft.com/dotnet/sdk:6.0 as build
WORKDIR /build
COPY ProductProvisioner.csproj ProductProvisioner.csproj
RUN dotnet restore
COPY . .
RUN dotnet publish -c release

FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /app

COPY --from=build /build/bin/release/net6.0/publish/ .

ENTRYPOINT ["dotnet", "ProductProvisioner.dll"]
