namespace AzureIndexer.Api.Infrastructure
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using AutoMapper;
    using Models;
    using Models.Response;
    using NBitcoin;

    public class SmartContractSearchService : ISmartContractSearchService
    {
        private readonly ConcurrentChain chain;
        private readonly QBitNinjaConfiguration configuration;
        private readonly IMapper mapper;

        public SmartContractSearchService(ConcurrentChain chain, QBitNinjaConfiguration configuration, IMapper mapper)
        {
            this.chain = chain;
            this.configuration = configuration;
            this.mapper = mapper;
        }

        public async Task<SmartContractModel> FindSmartContract(uint256 txId)
        {
            var client = this.configuration.Indexer.CreateIndexerClient();
            var smartContract = await client.GetSmartContractAsync(txId);

            return (SmartContractModel) smartContract;
        }
    }
}
