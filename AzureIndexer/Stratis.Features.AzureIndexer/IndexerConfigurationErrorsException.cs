namespace Stratis.Features.AzureIndexer
{
    using System;

    public class IndexerConfigurationErrorsException : Exception
    {
        public IndexerConfigurationErrorsException(string message) : base(message)
        {

        }
    }
}
