using NBitcoin;
using Stratis.SmartContracts.CLR;
using Stratis.SmartContracts.CLR.Serialization;
using Stratis.SmartContracts.Core.Receipts;
using System.Collections.Generic;

namespace Stratis.Bitcoin.Features.AzureIndexer.Helpers
{
    public interface ISmartContractOperations
    {
        ContractTxData BuildLocalCallTxData(uint160 contractAddress, ulong gasPrice, ulong gasLimit, string methodName, params string[] parameters);
        (byte[] contractByteCode, string contractCode, bool isStandardToken) GetContractDetail(uint160 contractAddress);
        TReturnValue GetPropertyValue<TReturnValue>(uint160 contractAddress, string propertyName, int height, ulong gasPrice = 1000, ulong gasLimit = 10000);
        (bool hasSmartContractExecution, ContractTxData contractTxData, Receipt receipt, bool isSmartContractCreation) GetSmartContractExecution(Transaction transaction);
        string GetStandardTokenName(uint160 contractAddress);
        string GetStandardTokenName(uint160 contractAddress, int height, ulong gasPrice = 1000, ulong gasLimit = 10000);
        string GetStandardTokenSymbol(uint160 contractAddress);
        string GetStandardTokenSymbol(uint160 contractAddress, int height, ulong gasPrice = 1000, ulong gasLimit = 10000);
        object GetStorageValue(uint160 contractAddress, string storageKey, MethodParameterDataType dataType);
        TValue GetStorageValue<TValue>(uint160 contractAddress, string storageKey);
        List<LogResponse> MapLogResponses(Receipt receipt);
    }
}