# TNCovidBedApi

[![.NET](https://github.com/DevTamizhan/TNCovidBedApi/actions/workflows/dotnet.yml/badge.svg)](https://github.com/DevTamizhan/TNCovidBedApi/actions/workflows/dotnet.yml)

### Description 
Tamil Nadu Government has created a website http://tncovidbeds.tnega.org/ to track corona ward bed status over the entire state under single website.

The website has rich features but the main backbone of the website is the API it is using to fetch data. Though the API is powerful, the website doesn't make the full use of the API to provide service.

This library will enables you to download data from the API with various filters like filtering the beds based on availability, getting nearby hospitals to a particular location, etc. 

This library is built with .Net Core 3.1 and can be used with all supporting applications from .Net Core 3.1 and above.

The library supports

1. Asynchronous download of district details
2. Getting cached district details
3. Asynchronous download of corona ward details
4. Getting cached corona ward details
5. Getting corona ward details of nearby hospitals from a location with particular distance


### Using in your project
The API can be downloaded as nuget package from https://www.nuget.org/packages/TNCovidBedApi/

Or you can direcly install in your project using the command

    dotnet add package TNCovidBedApi --version 1.2.1

Make sure that your project is .Net Core 3.1 or higher

### Documentation
The complete documentation of using the library is at https://github.com/DevTamizhan/TNCovidBedApi/wiki
