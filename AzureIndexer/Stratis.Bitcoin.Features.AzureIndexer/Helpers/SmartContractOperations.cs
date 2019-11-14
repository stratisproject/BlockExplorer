using CSharpFunctionalExtensions;
using NBitcoin;
using Stratis.SmartContracts.CLR;
using Stratis.SmartContracts.CLR.Compilation;
using Stratis.SmartContracts.CLR.Decompilation;
using Stratis.SmartContracts.CLR.Local;
using Stratis.SmartContracts.CLR.Serialization;
using Stratis.SmartContracts.Core;
using Stratis.SmartContracts.Core.Receipts;
using Stratis.SmartContracts.RuntimeObserver;
using System;
using System.Text;
using System.Linq;
using Stratis.SmartContracts.Core.State;
using Stratis.SmartContracts;
using Microsoft.Extensions.Logging;

namespace Stratis.Bitcoin.Features.AzureIndexer.Helpers
{

    /// <summary>
    /// Utility class meant to be singleton, that allow to perform operations against deployed smart contracts.
    /// </summary>
    public class SmartContractOperations
    {
        private readonly Network network;
        private readonly ILoggerFactory loggerFactory;
        private readonly IStateRepositoryRoot stateRepositoryRoot;
        private readonly CSharpContractDecompiler cSharpContractDecompiler;
        private readonly IReceiptRepository receiptRepository;
        private readonly ISerializer serializer;
        private readonly ICallDataSerializer smartContractSerializer;
        private readonly IMethodParameterStringSerializer methodParameterStringSerializer;
        private readonly ILocalExecutor localExecutor;
        private ILogger logger;

        public SmartContractOperations(
            Network network,
            ILoggerFactory loggerFactory,
            IStateRepositoryRoot stateRepositoryRoot,
            CSharpContractDecompiler cSharpContractDecompiler,
            IReceiptRepository receiptRepository,
            ISerializer serializer,
            ICallDataSerializer callDataSerializer,
            IMethodParameterStringSerializer methodParameterStringSerializer,
            ILocalExecutor localExecutor)
        {
            this.network = network;
            this.loggerFactory = loggerFactory;
            this.stateRepositoryRoot = stateRepositoryRoot;
            this.cSharpContractDecompiler = cSharpContractDecompiler;
            this.receiptRepository = receiptRepository;
            this.serializer = serializer;
            this.smartContractSerializer = callDataSerializer;
            this.methodParameterStringSerializer = methodParameterStringSerializer;
            this.localExecutor = localExecutor;

            this.logger = loggerFactory.CreateLogger(this.GetType().FullName);
        }

        public (bool hasSmartContractExecution, ContractTxData contractTxData, Receipt receipt, bool isSmartContractCreation)
            GetSmartContractExecution(Transaction transaction)
        {
            TxOut smartContractExecutionOutput = transaction.Outputs.FirstOrDefault(output => output.ScriptPubKey.IsSmartContractExec());
            if (smartContractExecutionOutput != null)
            {
                Receipt receipt = this.receiptRepository?.Retrieve(transaction.GetHash());
                Result<ContractTxData> contractTxDataResult = this.smartContractSerializer.Deserialize(smartContractExecutionOutput.ScriptPubKey.ToBytes());

                return (
                    true,
                    contractTxDataResult.Value,
                    receipt,
                    contractTxDataResult.Value?.IsCreateContract ?? smartContractExecutionOutput.ScriptPubKey.IsSmartContractCreate());
            }
            else
            {
                return (false, null, null, false);
            }
        }

        /// <summary>
        /// Gets the contract detail.
        /// </summary>
        /// <param name="contractAddress">The contract address.</param>
        /// <returns>Contract details.</returns>
        public (byte[] contractByteCode, string contractCode, bool isStandardToken) GetContractDetail(uint160 contractAddress)
        {
            if (contractAddress != null)
            {
                byte[] contractByteCode = this.stateRepositoryRoot.GetCode(contractAddress);
                if (contractByteCode?.Length > 0)
                {
                    Result<string> csharpDecompileResult = this.cSharpContractDecompiler.GetSource(contractByteCode);
                    if (csharpDecompileResult.IsSuccess)
                    {
                        string contractCode = csharpDecompileResult.Value;
                        Result<IContractModuleDefinition> contractModuleDefinition = ContractDecompiler.GetModuleDefinition(contractByteCode);

                        if (contractModuleDefinition.IsSuccess)
                        {
                            bool isStandardToken = contractModuleDefinition.Value.IsStandardToken();
                            return (contractByteCode, contractCode, isStandardToken);
                        }
                    }
                }
            }

            return (null, null, false);
        }

        public ContractTxData BuildLocalCallTxData(uint160 contractAddress, ulong gasPrice, ulong gasLimit, string methodName, params string[] parameters)
        {
            if (parameters != null && parameters.Length > 0)
            {
                object[] methodParameters = this.methodParameterStringSerializer.Deserialize(parameters);

                return new ContractTxData(ReflectionVirtualMachine.VmVersion, (Gas)gasPrice, (Gas)gasLimit, contractAddress, methodName, methodParameters);
            }

            return new ContractTxData(ReflectionVirtualMachine.VmVersion, (Gas)gasPrice, (Gas)gasLimit, contractAddress, methodName);
        }

        public string GetStandardTokenSymbol(uint160 contractAddress, int height, ulong gasPrice = 1000, ulong gasLimit = 10000)
        {
            return this.GetPropertyValue<string>(contractAddress, "Symbol", height, gasPrice, gasLimit);
        }

        public string GetStandardTokenName(uint160 contractAddress, int height, ulong gasPrice = 1000, ulong gasLimit = 10000)
        {
            return this.GetPropertyValue<string>(contractAddress, "Name", height, gasPrice, gasLimit);
        }

        public string GetStandardTokenSymbol(uint160 contractAddress)
        {
            return this.GetStorageValue<string>(contractAddress, "Symbol");
        }

        public string GetStandardTokenName(uint160 contractAddress)
        {
            return this.GetStorageValue<string>(contractAddress, "Name");
        }

        public TReturnValue GetPropertyValue<TReturnValue>(uint160 contractAddress, string propertyName, int height, ulong gasPrice = 1000, ulong gasLimit = 10000)
        {
            ContractTxData callTxData = this.BuildLocalCallTxData(contractAddress, gasPrice, gasLimit, $"get_{propertyName}");

            ILocalExecutionResult result = this.localExecutor.Execute(
                    (ulong)height,
                    new uint160(),
                    0,
                    callTxData);

            return (TReturnValue)result.Return;
        }

        public TValue GetStorageValue<TValue>(uint160 contractAddress, string storageKey)
        {
            MethodParameterDataType dataType = 0;
            Type type = typeof(TValue);
            switch (type)
            {
                case Type _ when type == typeof(bool):
                    dataType = MethodParameterDataType.Bool;
                    break;
                case Type _ when type == typeof(byte):
                    dataType = MethodParameterDataType.Byte;
                    break;
                case Type _ when type == typeof(char):
                    dataType = MethodParameterDataType.Char;
                    break;
                case Type _ when type == typeof(string):
                    dataType = MethodParameterDataType.String;
                    break;
                case Type _ when type == typeof(uint):
                    dataType = MethodParameterDataType.UInt;
                    break;
                case Type _ when type == typeof(int):
                    dataType = MethodParameterDataType.Int;
                    break;
                case Type _ when type == typeof(ulong):
                    dataType = MethodParameterDataType.ULong;
                    break;
                case Type _ when type == typeof(long):
                    dataType = MethodParameterDataType.Long;
                    break;
                case Type _ when type == typeof(Address):
                    dataType = MethodParameterDataType.Address;
                    break;
                case Type _ when type == typeof(byte[]):
                    dataType = MethodParameterDataType.ByteArray;
                    break;
                default:
                    this.logger.LogError("Unknown Storage Type {0}. Returning default value.", type.Name);
                    return default(TValue);
            }

            object value = this.GetStorageValue(contractAddress, storageKey, dataType);
            return (TValue)value;
        }

        public object GetStorageValue(uint160 contractAddress, string storageKey, MethodParameterDataType dataType)
        {
            byte[] bytes = this.stateRepositoryRoot.GetStorageValue(contractAddress, Encoding.UTF8.GetBytes(storageKey));
            if (bytes != null)
            {
                switch (dataType)
                {
                    case MethodParameterDataType.Bool:
                        return this.serializer.ToBool(bytes);
                    case MethodParameterDataType.Byte:
                        return bytes[0];
                    case MethodParameterDataType.Char:
                        return this.serializer.ToChar(bytes);
                    case MethodParameterDataType.String:
                        return this.serializer.ToString(bytes);
                    case MethodParameterDataType.UInt:
                        return this.serializer.ToUInt32(bytes);
                    case MethodParameterDataType.Int:
                        return this.serializer.ToInt32(bytes);
                    case MethodParameterDataType.ULong:
                        return this.serializer.ToUInt64(bytes);
                    case MethodParameterDataType.Long:
                        return this.serializer.ToInt64(bytes);
                    case MethodParameterDataType.Address:
                        return this.serializer.ToAddress(bytes);
                    case MethodParameterDataType.ByteArray:
                        return bytes.ToHexString();
                }
            }

            return null;
        }
    }
}
