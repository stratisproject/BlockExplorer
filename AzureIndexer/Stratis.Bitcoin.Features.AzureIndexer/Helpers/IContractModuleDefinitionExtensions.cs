namespace Stratis.Bitcoin.Features.AzureIndexer.Helpers
{
    using System.Linq;
    using Stratis.SmartContracts.CLR;

    public static class IContractModuleDefinitionExtensions
    {
        private static string smartContractFullName = typeof(SmartContracts.Standards.IStandardToken).FullName;

        /// <summary>
        /// Determines whether contractModuleDefinition is a standard token.
        /// </summary>
        /// <param name="contractModuleDefinition">Smart Contract module definition.</param>
        /// <returns>
        ///   <c>true</c> if <paramref name="contractType" /> is a Standard Token otherwise, <c>false</c>.
        /// </returns>
        /// TODO: Should be moved to smart contract project and should have a better handling of what a standard token is
        /// (should not stick with SmartContracts.Standards.IStandardToken in order to allow 3rd party to define their own
        /// standard token interface)
        public static bool IsStandardToken(this IContractModuleDefinition contractModuleDefinition)
        {
            return HasInterface(contractModuleDefinition.ContractType, smartContractFullName);
        }

        /// <summary>
        /// Determines whether the specified contract type implements the specified interface.
        /// </summary>
        /// <param name="contractType">Type of the smart contract.</param>
        /// <param name="interfaceFullName">Full name of the interface.</param>
        /// <returns>
        ///   <c>true</c> if the specified contract type has interface; otherwise, <c>false</c>.
        /// </returns>
        private static bool HasInterface(Mono.Cecil.TypeDefinition contractType, string interfaceFullName)
        {
            return contractType.Interfaces.Any(i => i.InterfaceType.FullName.Equals(interfaceFullName))
                || contractType.NestedTypes.Any(t => HasInterface(t, interfaceFullName));
        }
    }
}