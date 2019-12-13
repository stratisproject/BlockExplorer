using Newtonsoft.Json;

namespace AzureIndexer.Api.Models.Response
{
    public class SmartContractActionModel
    {
        [JsonProperty("address", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string Address { get; set; }

        [JsonProperty("addressBase58", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string AddressBase58 { get; set; }

        [JsonProperty("txId", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string TxId { get; set; }

        [JsonProperty("opCode", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string OpCode { get; set; }

        [JsonProperty("gasPrice", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public long GasPrice { get; set; }

        [JsonProperty("methodName", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string MethodName { get; set; }

        [JsonProperty("isSuccessful", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public bool IsSuccessful { get; set; }

        [JsonProperty("errorMessage", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorMessage { get; set; }

        [JsonProperty("isStandardToken", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsStandardToken { get; set; }

        [JsonProperty("code", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string Code { get; set; }

        [JsonProperty("contractName", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string ContractName { get; set; }

        [JsonProperty("contractSymbol", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string ContractSymbol { get; set; }

        [JsonProperty("logs", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string Logs { get; internal set; }
    }
}