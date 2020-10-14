# hello-dotnet
Hello world asp.net application

## Documentation
This is going to be short and sweet
### Environment variables
| Environment variable | Description | Default |
|----------------------|-------------|---------|
| SHUTDOWN_TIMEOUT | Number of seconds the server will wait for requests to complete before shutting down | 5 |

### Rest API
#### Hello
Returns a simple hello message
* URL

  /hello
* URL params

  __Optional__
  
  sleep=[integer] 
  
  _Number of seconds the server should sleep befor returning an answere_
