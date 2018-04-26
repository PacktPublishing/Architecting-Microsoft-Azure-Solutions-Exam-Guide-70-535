using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace PacktPubUploadToTableStorage
{
    class Program
    {
        static void Main(string[] args)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            // Create the table client.

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference("packtpubContact");

            // Create the table if it doesn't exist.
            table.CreateIfNotExists();

            // Create a new contact entity.
            Contact contact1 = new Contact("Zaal", "Sjoukje");
            contact1.Email = "sjoukje@packtpub.com";

            // Create the TableOperation object that inserts the contact.
            TableOperation insertOperation = TableOperation.Insert(contact1);

            // Execute the insert operation.
            table.Execute(insertOperation);
        }
    }
}
