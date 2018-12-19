using System.Threading.Tasks;
using NBitcoin;

namespace Stratis.Bitcoin.Features.AzureIndexer.IndexTasks
{
    public interface IIndexTask
    {
        void Index(BlockFetcher blockFetcher, TaskScheduler scheduler, Network network);

        bool SaveProgression
        {
            get;
            set;
        }

        bool EnsureIsSetup
        {
            get;
            set;
        }
    }
}