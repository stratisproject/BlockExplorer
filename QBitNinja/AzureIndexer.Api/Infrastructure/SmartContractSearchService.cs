namespace AzureIndexer.Api.Infrastructure
{
    using System.Threading.Tasks;
    using Models.Response;
    using NBitcoin;

    public class SmartContractSearchService : ISmartContractSearchService
    {
        private readonly QBitNinjaConfiguration configuration;

        public SmartContractSearchService(QBitNinjaConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<SmartContractModel> FindSmartContract(uint256 txId)
        {
            var client = this.configuration.Indexer.CreateIndexerClient();
            var smartContract = await client.GetSmartContractAsync(txId);
            if (smartContract == null)
            {
                return null;
            }

            var smartContractModel = new SmartContractModel
            {
                Hash = smartContract.Id,
                OpCode = smartContract.OpCode,
                MethodName = smartContract.MethodName,
                GasPrice = new MoneyModel { Satoshi = smartContract.GasPrice }
            };

            var smartContractDetails = await client.GetSmartContractDetailsAsync(smartContract.Id);
            if (smartContractDetails != null)
            {
                smartContractModel.Code = smartContractDetails.Code;
            }

            return smartContractModel;
        }
    }
}
