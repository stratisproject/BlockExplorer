using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NBitcoin;
using Stratis.Bitcoin.Features.IndexStore;
using Stratis.Bitcoin.Features.MemoryPool;
using Stratis.Bitcoin.Features.RPC.Models;
using System;

namespace Stratis.Bitcoin.Features.RPC.Controllers
{
    /// <summary>
    /// An interface for an index store controller.
    /// </summary>
    public interface IIndexStoreRPCController
    {
        /// <summary>
        /// Creates a new index.
        /// </summary>
        /// <param name="name">The name of the index.</param>
        /// <param name="multiValue">Indicates whether the index should be multi-value.</param>
        /// <param name="builder">The index builder.</param>
        /// <param name="dependencies">A list of index dependencies needed to compile the index.</param>
        /// <returns>An indication whether the index was successfully created.</returns>
        Task<bool> CreateIndexAsync(string name, bool multiValue, string builder, string[] dependencies = null);

        /// <summary>
        /// Describes a given index.
        /// </summary>
        /// <param name="name">The name of the index to describe.</param>
        /// <returns>The index description or null if not found.</returns>
        string[] DescribeIndex(string name);

        /// <summary>
        /// Drops the given index when found.
        /// </summary>
        /// <param name="name">The name of the index to drop.</param>
        /// <returns>An indication whether the index was successfully dropped.</returns>
        Task<bool> DropIndexAsync(string name);

        /// <summary>
        /// Gets the raw transaction based on the transaction id.
        /// </summary>
        /// <param name="txid">The transaction id.</param>
        /// <param name="verbose">Return a <see cref="TransactionVerboseModel"/> or a <see cref="TransactionBriefModel"/>.</param>
        /// <returns>The transaction model or null if not found.</returns>
        Task<TransactionModel> GetRawTransactionAsync(string txid, int verbose = 0);

        /// <summary>
        /// Lists the index names from the index store.
        /// </summary>
        /// <returns>An array of index names.</returns>
        string[] ListIndexNames();
    }

    public class IndexStoreRPCController : FeatureController, IIndexStoreRPCController
    {
        private readonly ILogger logger;
        protected IndexStoreManager IndexManager;
        public MempoolManager MempoolManager { get; private set; }

        public IndexStoreRPCController(
            ILoggerFactory loggerFactory,
            IndexStoreManager indexManager,
            MempoolManager mempoolManager = null)
            : base()
        {
            this.logger = loggerFactory.CreateLogger(this.GetType().FullName);
            this.IndexManager = indexManager;
        }

        /// <inheritdoc />
        [ActionName("createindex")]
        //[ActionDescription("Creates and index in the index store.")]
        public async Task<bool> CreateIndexAsync(string name, bool multiValue, string builder, string[] dependancies = null)
        {
            if (dependancies?.Length == 0)
                dependancies = null;

            return await this.IndexManager.IndexRepository.CreateIndexAsync(name, multiValue, builder, dependancies);
        }

        /// <inheritdoc />
        [ActionName("dropindex")]
        //[ActionDescription("Drops an index from the index store.")]
        public async Task<bool> DropIndexAsync(string name)
        {
            return await this.IndexManager.IndexRepository.DropIndexAsync(name);
        }

        /// <inheritdoc />
        [ActionName("listindexnames")]
        //[ActionDescription("Lists the names of the indexes in the index store.")]
        public string[] ListIndexNames()
        {
            return this.IndexManager.IndexRepository.Indexes.Keys.ToArray();
        }

        /// <inheritdoc />
        [ActionName("describeindex")]
        //[ActionDescription("Describes an index in the index store.")]
        public string[] DescribeIndex(string name)
        {
            if (!this.IndexManager.IndexRepository.Indexes.TryGetValue(name, out Index index))
                return null;

            return new string[] { index.ToString() };
        }

        /// <inheritdoc />
        [ActionName("getrawindexstoretransaction")]
        //[ActionDescription("Gets a raw transaction from the index store.")]
        public async Task<TransactionModel> GetRawTransactionAsync(string txid, int verbose = 0)
        {
            uint256 trxid;
            if (!uint256.TryParse(txid, out trxid))
                throw new ArgumentException(nameof(txid));

            Transaction trx = null;
            if (this.MempoolManager != null)
                trx = (await this.MempoolManager?.InfoAsync(trxid))?.Trx;

            if (trx == null)
                trx = await this.IndexManager?.BlockRepository?.GetTrxAsync(trxid);

            if (trx == null)
                return null;

            if (verbose != 0)
            {
                ChainedBlock block = await this.GetTransactionBlockAsync(trxid);
                return new TransactionVerboseModel(trx, this.Network, block, this.ChainState?.ConsensusTip);
            }
            else
                return new TransactionBriefModel(trx);
        }

        private async Task<ChainedBlock> GetTransactionBlockAsync(uint256 trxid)
        {
            ChainedBlock block = null;
            uint256 blockid = await this.IndexManager?.BlockRepository?.GetTrxBlockIdAsync(trxid);
            if (blockid != null)
                block = this.Chain?.GetBlock(blockid);
            return block;
        }
    }
}
