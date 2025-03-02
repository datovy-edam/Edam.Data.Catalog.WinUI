# Edam.Data.CatalogModel

Requires: Edam.Common

The Catalog Model define the C# classes used in the related CatalogDb EF based implementation of a database to manage Containers, (File) Items, and Item Data (Files), details follow:

- Containers are similar to a file-system drive.  They have a "root" item that is automatically created and will have its independent tree representation that can be built by submitting file paths as you will do in a File System.

- A container (File) Item can be a branch or leaf of a Tree and may have zero, one, or more data (Files) that are defined independently and stored in the Data Store as binary (blob) files.

- A Data (File) is a child of an Item and will hold the binary blob.
  
The Catalog Tree Builder can be used to prepare a Tree that could be viewed in Web or other related targets.

Try the component by running the provided Test Catalog various class / methods.




