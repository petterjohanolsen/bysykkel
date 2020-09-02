## Table of contents
* [General info](#general-info)
* [Technologies](#technologies)
* [Configuration](#configuration)
* [Setup](#setup)
* [Run](#run)

## General info
This project fetches data from Oslo Bysykkel's GBFS (General Bikeshare Feed Specification) realtime data.
	
## Technologies
Project is created with:
* .NET Core v3.1
	
## Configuration
The following settings may be changed in BysykkelMvc/appsettings.json:
- baseUrl
- clientIdentifier

## Setup
To run this project, download and install .NET Core SDK for your operating system from https://dotnet.microsoft.com/download, then run the following commands:

```
$ git clone https://github.com/petterjohanolsen/bysykkel.git
$ cd bysykkel/BysykkelMvc
$ dotnet run
```

## Run
To start, navigate to http://localhost:5000/ in a web browser.
