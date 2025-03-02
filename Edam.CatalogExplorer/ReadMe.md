# EDAM Data Catalog - Services Library

The Catalog services are similar to a file system to allow the specification of
paths, folder, and files and store data as binary files or other content-types.
The Library contains:

- Catalog Db (the Repository)
- Catalog Model (C# classes and resources)
- Catalog Service Client (implements a client for catalog web API)
- Catalog Explorer (WinUI 3 App for testing or adding data)

Details of each follow.

## Catalog Repository
This first implementation uses an EF based repository to manage Containers,
(File) Items, and Item Data (Files), details follow:

- Containers are similar to a file-system drive.  They have a "root" item that
is automatically created and will have its independent tree representation that
can be built by submitting file paths as you will do in a File System.

- A container (File) Item can be a branch or leaf of a Tree and may have zero,
one, or more data (Files) that are defined independently and stored in the
Data Store as binary (blob) files.

- A Data (File) is a child of an Item and will hold the binary blob.

## Catalog Model
The Catalog Model define the C# classes used in the related Catalog Repository
implementation of a database to manage Containers, (File) Items, and Item Data
(Files), details follow:

- Containers are similar to a file-system drive.  They have a "root" item that
is automatically created and will have its independent tree representation that
can be built by submitting file paths as you will do in a File System.

- A container (File) Item can be a branch or leaf of a Tree and may have zero,
one, or more data (Files) that are defined independently and stored in the Data
Store as binary (blob) files.

- A Data (File) is a child of an Item and will hold the binary blob.
  
The Catalog Tree Builder can be used to prepare a Tree that could be viewed in
Web or other related targets.

## Catalog Service Client

The Catalog Service Client implements the `ICatalogService` class and method to
access the related Web base API.  The `Edam.Data.CatalogService` project
implements the API and could participate in an .Net Aspire project.

## Catalog Explorer

This (WinUI 3) console application is provided to demonstrate how to use
the Edam.Data.Catalog (DB and Model) libraries.

The main objective is to have a Catalog of items such as folders and files that
will work both in Windows or a Browser (WebApplication) with the minimum
dependencies to other external (paid) services.  The (TO-BE) Catalog Services
API will include all the expected functionality to allow access to file and
folders for EDAM Projects but still these services should be independent and its
functionality applicable to any application.

## Catalog Service Interfaces
As of this version there are 2 available implentations to support the Catalog
Service instance including:

- local service - implemented by instancing the Catalog Service within the
host.
- web service - implemented by firing web-service that implements the
interface.

The selection of one or the other is done by detecting the container.  
Get an instance of the Catalog Service by calling:

```
var catalogService = CatalogServiceHelper.GetCatalog([connection-uri]);
```

or

```
var catalogService = await CatalogServiceHelper.GetCatalogAsync([connection-uri]);
```

Note that the optional connection URI can be a connection string if local or an
http URI if a web service is required.  If not provided it must be set using
the main project appsettings.json setting the `DefaultConnectionString` and/or
the `CatalogServiceBaseUri`.

## What is next?

- Support local file system as a container

