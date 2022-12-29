FROM mcr.microsoft.com/dotnet/sdk:7.0 as build
WORKDIR /build
COPY ProductProvisioner.csproj ProductProvisioner.csproj
RUN dotnet restore
COPY . .
RUN dotnet publish -c release -o /app

FROM mcr.microsoft.com/dotnet/runtime:7.0
WORKDIR /app

COPY --from=build /app .

ENTRYPOINT ["dotnet", "ProductProvisioner.dll"]
