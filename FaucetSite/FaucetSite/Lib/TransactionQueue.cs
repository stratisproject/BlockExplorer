using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace FaucetSite.Lib
{
    public class TransactionQueue
    {
        private readonly IWalletUtils wallet;
        private ActionBlock<Item> queue;

        /// <summary>
        /// Number of items in queue waiting to be processed
        /// </summary>
        public int Count => queue.InputCount;

        public TransactionQueue(IWalletUtils wallet)
        {
            this.wallet = wallet;
            this.queue = new ActionBlock<Item>(SendCoinAsync, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 1 });
        }

        public async Task<Transaction> EnqueueAsync(string address)
        {
            var item = new Item(address);
            queue.Post(item);
            return await item.Completion;
        }

        private async Task SendCoinAsync(Item item)
        {
            try
            {
                var transaction = await wallet.SendCoin(item.Address);
                item.TaskCompletionSource.SetResult(transaction);
            }
            catch (Exception e)
            {
                item.TaskCompletionSource.SetException(e);
            }
        }

        class Item
        {
            public TaskCompletionSource<Transaction> TaskCompletionSource;
            public string Address { get; private set; }
            public Task<Transaction> Completion => TaskCompletionSource.Task;
            public Item(string address)
            {
                this.TaskCompletionSource = new TaskCompletionSource<Transaction>();
                this.Address = address;
            }
        }
    }
}
