# Glasswall ICAP Service - Minimum Viable Prototype
ICAP Service with ICAP Resource that interfaces with GW Cloud products

- [Getting Started](#getting-started)
- [Building ICAP Service](#building-icap-service)
    - [Build the Server](#build-the-server)
    - [Build the Modules](#build-the-modules)
- [Testing the Installation](#testing-the-installation)
- [Retrieving Statistics](#retrieving-the-statistics)
- [gw_rebuild cloud-proxy-app](#gw_rebuild-cloud-proxy-app)
    - [Setup the `cloud-proxy-app` Build Environment](#setup-the-cloud-proxy-app-build-environment)
    - [Building `cloud-proxy-app`](#building-cloud-proxy-app)
    - [Running `cloud-proxy-app`](#running-cloud-proxy-app)
    - [Getting Environment Configuration](#getting-environment-configuration)
- [Running ICAP Server in Docker Container](#running-icap-server-in-docker-container)
    - [Building the Docker Image](#building-the-docker-image)
    - [Running the Docker Image](#running-the-docker-image)

## Getting started
The original baseline code has been cloned from the open source project
https://sourceforge.net/projects/c-icap/

Demonstration ICAP Resources have been removed.

## Building ICAP Service

These instructions guide the user through the steps involved in installing the Glasswall ICAP PoC on a Linux host.

Running the follow commands will ensure the necessary packages are installed.
```
sudo apt-get update
sudo apt-get upgrade -y
sudo apt-get install git
sudo apt-get install gcc
sudo apt-get install -y doxygen
sudo apt-get install make
sudo apt-get install automake
sudo apt-get install automake1.11
sudo apt-get install libtool
```

### Build the Server
From where the repo was cloned to, navigate into the `mvp-icap-service/c-icap` folder and run the scripts to setup the Makefiles.
```
autoreconf -i
```
Run the configure script, specifying where the server should be installed, through the `prefix` argument.
```
./configure --prefix=/usr/local/c-icap
```
After running the configuration script, build and install the server.
```
make 
sudo make install
```
The option is available to generate the documentation, if required
```
make doc
```

### Build the Modules

Navigate to the modules folder (`mvp-icap-service/c-icap-modules`) and run the scripts to setup the Makefiles.
```
autoreconf -i
```
Run the configure script, specifing where the server was installed, in both the `with-c-icap` and `prefix` arguments.
```
./configure --with-c-icap=/usr/local/c-icap --prefix=/usr/local/c-icap
```
After running the configuration script, we can build and install
```
make 
sudo make install
```
> During the `make install` there will be some warnings about `libtools`, these can be ignored.

After installation, the configuration files for each module/service are available in the c-icap server configuration directory, `/usr/local/c-icap/etc/` using the location folder specified in the 'configure' commands above.  

For a module/service to be recognisd by the C-ICAP server its configuration file needs to be included into the main c-icap server configuration file. The following command adds the `gw_rebuild.conf` file
```
sudo sh -c 'echo "Include gw_rebuild.conf" >>  /usr/local/c-icap/etc/c-icap.conf'
```

## Testing the Installation

On the host server run the ICAP Server with the following command
```
sudo /usr/local/c-icap/bin/c-icap -N -D -d 10
```

From a separate command prompt, run the client utility to send an options request. The module specified in the `-s` argument must have been `Included` into the `gw_test.conf` file in the step above.
```
/usr/local/c-icap/bin/c-icap-client -s gw_rebuild
```

Run the client utility sending a file through the ICAP Server. This requires sufficient configuration to have been provided for the
```
/usr/local/c-icap/bin/c-icap-client -f <full path to source file>  -o <full path to output file> -s gw_test
/usr/local/c-icap/bin/c-icap-client -f <full path to source file>  -o <full path to output file> -s gw_rebuild
```

A full list of the command line options available to the client utility are available from the application's `help` option.
```
/usr/local/c-icap/bin/c-icap-client  --help
```

## Retrieving Statistics

The ICAP Server provides an `info` resource that can be used to query for processing statistics. The `c-icap-client` can be used to query for this information. The address of the ICAP Server host can be substituted for `localhost` in this example, if necessary.
```
sudo /usr/local/c-icap/bin/c-icap-client -s "info?view=text" -i localhost -req any

```
If `view=text` is not included, then the response is formatted as an HTML table.
The `-req` attribute is required in order to avoid an `options` request from being sent (the default).

Excerpt of typical statistics request
```
Service gw_rebuild Statistics
==================
Service gw_rebuild REQMODS : 0
Service gw_rebuild RESPMODS : 0
Service gw_rebuild OPTIONS : 1
Service gw_rebuild ALLOW 204 : 0
Service gw_rebuild REQUESTS SCANNED : 0
Service gw_rebuild REBUILD FAILURES : 0
Service gw_rebuild REBUILD ERRORS : 0
Service gw_rebuild SCAN REBUILT : 0
Service gw_rebuild UNPROCESSED : 0
Service gw_rebuild UNPROCESSABLE : 0
Service gw_rebuild BYTES IN : 0 Kbs 133 bytes
Service gw_rebuild BYTES OUT : 0 Kbs 255 bytes
Service gw_rebuild HTTP BYTES IN : 0 Kbs 0 bytes
Service gw_rebuild HTTP BYTES OUT : 0 Kbs 0 bytes
Service gw_rebuild BODY BYTES IN : 0 Kbs 0 bytes
Service gw_rebuild BODY BYTES OUT : 0 Kbs 0 bytes
Service gw_rebuild BODY BYTES SCANNED : 0 Kbs 0 bytes

```

## `gw_rebuild cloud-proxy-app` 
The `cloud-proxy-app` is used by the `gw_rebuild` ICAP Resource to process the received HTTP Content. It is a .NET Core 3.1 Console Application is is build externally to the ICAP Server and Resource.

### Setup the `cloud-proxy-app` Build Environment
The application should be built on the same host as the ICAP Server and associated components. The following steps assume an Ubuntu distro.
```
$ cat /etc/lsb-release
DISTRIB_ID=Ubuntu
DISTRIB_RELEASE=18.04
DISTRIB_CODENAME=bionic
DISTRIB_DESCRIPTION="Ubuntu 18.04.4 LTS"
```

Using the instructions provided by Microsoft [Install .NET Core SDK or .NET Core Runtime on Ubuntu](https://docs.microsoft.com/en-us/dotnet/core/install/linux-ubuntu#1804-)

Add the Microsoft package signing key to your list of trusted keys and add the package repository.
```
wget https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
```

Install the SDK
```
sudo apt-get update; \
  sudo apt-get install -y apt-transport-https && \
  sudo apt-get update && \
  sudo apt-get install -y dotnet-sdk-3.1
```

Since the SDK include the runtime, there is no need to add this seperately.

### Building `cloud-proxy-app` 
From where the repo was cloned to, navigate into the `c-icap/cloud-proxy-app` folder.
Restore the solution's packages
```
dotnet restore ./cloud-proxy-app.sln
``` 

Publish the solution
```
dotnet publish -c Release ./cloud-proxy-app.sln
```
The displayed logs will then report where the published application has been deployed to
```
Microsoft (R) Build Engine version 16.7.0-preview-20360-03+188921e2f for .NET
Copyright (C) Microsoft Corporation. All rights reserved.

  Determining projects to restore...
  All projects are up-to-date for restore.
  cloud-proxy-app -> /mnt/c/Users/paul/code_projects/mvp-icap-service/cloud-proxy-app/bin/Release/netcoreapp3.1/cloud-proxy-app.dll
  cloud-proxy-app -> /mnt/c/Users/paul/code_projects/mvp-icap-service/cloud-proxy-app/bin/Release/netcoreapp3.1/publish/
```

### Running `cloud-proxy-app` 
The app is run by the `gw_rebuild` ICAP Resource to pass content to the cloud-based Glasswall processing components.

The app receives configuration items through two mechanisms: command line and environment variables.

#### Command Line Arguments
These are configuration items that are new for each content item to be processed. These are provided by the `gw_rebuild` ICAP Resource at run-time.
- -i : filepath of file to be rebuilt.
- -o : filepath where rebuilt file should be written.

#### Environment Variables
These are static configuration that is used by the app to connect with other components.
- *FileProcessingStorageConnectionString* : connection string for Azure Storage Account being used for `original` and `rebuilt` stores.
- *FileProcessingStorageOriginalStoreName* : name of the container being used as the `original` store.
- *TransactionOutcomeQueueConnectionString* : connection string for Azure Service Bus Namespace in which the `Transaction Outcome` Queue is located.
- *TransactionOutcomeQueueName* : name give to the Azure Service Bus Queue being used as the `Transaction Outcome Queue`.

### Getting Environment Configuration
The `cloud-proxy-app` requires configuration to be provided in its Environment variables in order to communicate with the Cloud components. The following steps provide guidance on how to find the configuration required for each variable using the Azure command line tool. The following commands should be run in a Powershell console. For convienience variables are used for configuration items that are referenced repeatedly. For this reason the same console sessions should be used throughout.

**Select the correct Azure Subscription**

Ensure the correct Azure Subscription is current 'Default'
```
az account list --query '[].[name, isDefault]'

[
  [
    "Visual Studio Professional Subscription",
    false
  ],
  [
    "Required-Subscription",
    true
  ]
]
```
If the required subscription is not default, specify the required subscription using the "name" provided in the 'az account list' command
```
az account set -s 'Required-Subscription'
```

**Find the Resource Group**
```
az group list --query '[].name'
```
This provides a list of the current Resource Groups in the subscription
```
[
  "gw-gks-icap-squid-proxy",
  "gw-icap-rg-qa",
  "gw-icap-rg-stage",
  "NetworkWatcherRG",
  "gw-icap-rg-manual",
  "gw-icap-rg-arm"
]
```
Having found the target resource group, save that name into a variable
```
$resourceGroupName='gw-icap-rg-qa'
```

**Find the Storage Account Connection String and Container Name**

Having selected the resource group we need to find the Storage Account
```
az storage account list -g $resourceGroupName
```
The correct storage account must be selected from the provided list. In this case `gwicapqasa` is the Storage Account to use, the other autogenerated Storage account is required for Durable Function storage.
```
[
  "gwicapqasa",
  "slsuksqa49c2d5"
]
```
The connection string can then be retrieved using the name of the Storage Account
```
$storageAccountName='gwicapqasa'

$connectionstring=$(az storage account show-connection-string \
    --name $storageAccountName \
    --resource-group $resourceGroupName \
    --output tsv)
```

This connection string should be used in the `FileProcessingStorageConnectionString` environment variable.

Using the connection string we can list the available containers
```
az storage container list --connection-string $connectionstring --query '[].name'
```

The appropriate container name should be used in the `FileProcessingStorageOriginalStoreName` variable
```
[
  "original-store",
  "rebuild-store"
]
```

**Find the Service Bus Connection String and Queue Name**

We can use the Resource Group to find the Service Bus Namespace
```
az servicebus namespace list --resource-group $resourceGroupName --query '[].name'
```
```
[
  "gwicapqasb"
]
```
The name of the Namespace can then be used to acquire the Connection string
```
$servicebusnamespace='gwicapqasb'

$sbconnectionstring=az servicebus namespace authorization-rule keys list \
            --resource-group $resourceGroupName    \
            --namespace-name $servicebusnamespace  \
            --name RootManageSharedAccessKey       \
            --query primaryConnectionString
```
This connection string should be used in the `TransactionOutcomeQueueConnectionString` variable.

To get a list of the available queues
```
az servicebus queue list --resource-group $resourceGroupName \
            --namespace-name $servicebusnamespace \
            --query '[].name'
```
```
[
  "transaction-outcome"
]
```
The appropriate queue name should be used in the `TransactionOutcomeQueueName` variable


## Running ICAP Server in Docker Container

### Building the Docker Image
From where the repo was cloned to, navigate into the `c-icap` folder
```
docker build -t c-icap-server .
```

### Running the Docker Image 
The Proxy API App requires a number of environment variables to be configured to enable it to connect to the resources on which it is dependent. These are described in the sections above. They are provided on the `docker run` command-line when starting the Docker container.
```
docker run -d \
-e TransactionOutcomeQueueName='local-transaction-outcome' \
-e TransactionOutcomeQueueConnectionString='AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlm' \
-e FileProcessingStorageConnectionString='AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlm' \
-e FileProcessingStorageOriginalStoreName='local-original' \
 -p 1344:1344 \
 c-icap-server:latest
```
