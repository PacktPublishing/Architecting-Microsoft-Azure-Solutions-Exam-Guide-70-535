using Microsoft.WindowsAzure.Storage.Table;

namespace PacktPubUploadToTableStorage
{
    class Contact : TableEntity
    {
        public Contact(string lastName, string firstName)
        {
            this.PartitionKey = lastName;
            this.RowKey = firstName;
        }

        public Contact() { }

        public string Email { get; set; }
    }
}
