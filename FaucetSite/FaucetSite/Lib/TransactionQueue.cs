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
        private ActionBlock<string> queue;

        public TransactionQueue(IWalletUtils wallet)
        {
            this.wallet = wallet;
            this.queue = new ActionBlock<string>(SendCoinAsync, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 1 });
        }

        public void Enqueue(string address) => queue.Post(address);

        private async Task SendCoinAsync(string address)
        {
            try
            {
                await wallet.SendCoin(address);
            }
            catch
            {
            }
        }
    }
}
