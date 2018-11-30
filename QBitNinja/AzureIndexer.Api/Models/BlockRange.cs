namespace AzureIndexer.Api.Models
{
    public class BlockRange
    {
        public string Target { get; set; }

        public int From { get; set; }

        public int Count { get; set; }

        public bool Processed { get; set; }

        public override string ToString()
        {
            return $"{Target}- {From}-{Count}";
        }
    }
}
