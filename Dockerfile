FROM mcr.microsoft.com/dotnet/sdk:5.0.400-alpine3.13 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0.9-alpine3.13 AS final
WORKDIR /app
COPY --from=build-env /app/out .
RUN addgroup -g 3000 dotnet && adduser -u 1000 -G dotnet -D -s /bin/false dotnet
USER dotnet
ENTRYPOINT ["dotnet", "hello-dotnet.dll"]