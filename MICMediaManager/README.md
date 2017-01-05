# MIC Media Manager

.NET Core 1.1 MVC Web App used to manager slides for the media player to display. 

## Requirements

* .NET Core 1.1
* SQL Server or SQL Azure
* Azure Blob Storage

## Configuration Items

1. **DefaultConnection** - SQL Database connection string
2. **StorageAccountName** - Name of the Azure Blob Storage account where images will be saved.
3. **StorageAccountKey** - Access key for Azure Blob Storage
4. **BlobContainerName** - Name of the blob container you want the images stored in

Once these items are configured your application will run. 

