#if !CLIENT
namespace AzureIndexer.Api.Models
#else
namespace QBitNinja.Client.Models
#endif
{
	public class ChainStatus
	{
		public ChainStatus()
		{
			
		}
		public BlockInformation LatestBlock
		{
			get;
			set;
		}
		
	}
}