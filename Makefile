run:
	dotnet run hello-dotnet.csproj --configuration Development
	
start-redis:
	podman run --rm -d --name redis-stack-server -p 6379:6379 redis/redis-stack-server:latest