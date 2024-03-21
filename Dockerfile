FROM mcr.microsoft.com/dotnet/sdk:8.0.203-alpine3.19 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0.3-alpine3.19 AS final
WORKDIR /app
COPY --from=build-env /app/out .
USER app
ENTRYPOINT ["dotnet", "hello-dotnet.dll"]