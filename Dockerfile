FROM mcr.microsoft.com/dotnet/sdk:8.0.402-alpine3.19 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0.8-alpine3.19 AS final
WORKDIR /app
COPY --from=build-env /app/out .
COPY --from=build-env /usr/share/zoneinfo/Europe/Oslo /usr/share/zoneinfo/Europe/Oslo
USER app
ENTRYPOINT ["dotnet", "hello-dotnet.dll"]