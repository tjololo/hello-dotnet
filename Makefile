run:
	dotnet run hello-dotnet.csproj --configuration Development
	
start-redis:
	podman run --rm -d --name redis-stack-server -p 6379:6379 redis/redis-stack-server:latest
	
create-kind-cluster:
	kind create cluster --config contrib/k8s/kind-config.yaml
	
build-load-dev-image-archive: 
	podman build -t hello-dotnet:dev -f Dockerfile .
	podman save -o hello-dotnet-dev.tar hello-dotnet:dev
	kind load image-archive hello-dotnet-dev.tar

deploy-traefik: 
	helm repo add traefik https://traefik.github.io/charts      
	helm repo update
	helm install traefik traefik/traefik --values contrib/k8s/traefik-values.yaml

deploy-app:
	kubectl apply -f contrib/k8s/app.yaml
	
demo: create-kind-cluster build-load-dev-image-archive deploy-traefik deploy-app
	@echo "Dev environment is ready"
	
demo-curls:
	curl -s http://localhost:8080/hello -q | jq
	@echo  ""
	@echo  "-------------------------"
	@echo  ""
	curl -s http://localhost:8080/calldown -q | jq
	@echo  ""
	@echo  "-------------------------"
	@echo  ""
	curl -s http://localhost:8080/calldown | jq '.message | fromjson | .serverHeaders'
	
demo-curl-calldown:
	curl -s http://localhost:8080/calldown -q | jq
	
demo-curl-calldown-headers:
	curl -s http://localhost:8080/calldown | jq '.message | fromjson | .serverHeaders'