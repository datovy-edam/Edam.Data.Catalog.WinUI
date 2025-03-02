# Edam.Data.CatalogDb

Requires: Edam.Common

The Catalog DB is an EF based implementation of a database to manage Containers, (File) Items, and Item Data (Files), details follow:

- Containers are similar to a file-system drive.  They have a "root" item that is automatically created and will have its independent tree representation that can be built by submitting file paths as you will do in a File System.

- A container (File) Item can be a branch or leaf of a Tree and may have zero, one, or more data (Files) that are defined independently and stored in the Data Store as binary (blob) files.

- A Data (File) is a child of an Item and will hold the binary blob.

The Catalog requires that a Connection String is made available to connect to the target database.  Currently only SQL Client has been tested.

Try the component by running the provided Test Catalog various class / methods.  The tests also shows how to get a Catalog Service instance, query files, store data, and build a Tree.

The (separete) "CatalogModel" component library contains a class to help in building a Catalog Tree (see CatalogTreeBuilder) that could be used in Blazor UI libraries to visualize the Tree.


