# stratfaucet

This is a basic faucet for Stratis Platform test tokens (TSTRAT). It is written in dotnet core/typescript/Angular. It uses Refit against the stratisX `Stratis.StratisD` and is bundled with Docker containers.

## Runing the faucet with Docker

* Edit appsettings.json.docker:

``` 
  "Faucet": {
    "FullNodeApiurl": "http://0.0.0.0:37220",
    "FullNodePassword": "<password>",
    "FullNodeAccountName": "account 0",
    "FullNodeWalletName": "<walletname>"
  }
```

* Build the dotnet core container 
``` 
docker build . 
```

* Build the `stratisd` Docker container 

```
cd Docker/Stratis.StratisD/
docker build . 
```

* Create a docker network 

`docker network create mynet`

* Start the `stratisD` container on the created network
```
docker run --name wallet --network mynet -p 37220:37220 -it <container id>
```

* Start stratfaucet container
```
docker run --network mynet -p 5000:5000 -it <stratfaucet container id>
```

# TODO items

* Styling
* Queue for messages 
