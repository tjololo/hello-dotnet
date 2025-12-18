FROM mcr.microsoft.com/dotnet/sdk:10.0.101-alpine3.23 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0.1-alpine3.23 AS final
WORKDIR /app
COPY --from=build-env /app/out .
COPY --from=build-env /usr/share/zoneinfo/Europe/Oslo /usr/share/zoneinfo/Europe/Oslo
USER app
ENTRYPOINT ["dotnet", "hello-dotnet.dll"]