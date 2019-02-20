namespace Stratis.Bitcoin.Features.AzureIndexer
{
    using System;
    using System.Collections.Generic;
    using System.Text;

	public class IndexerConfigurationErrorsException : Exception
	{
		public IndexerConfigurationErrorsException(string message) : base(message)
		{

		}
	}
}
