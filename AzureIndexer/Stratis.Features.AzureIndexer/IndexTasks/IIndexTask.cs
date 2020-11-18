namespace Stratis.Features.AzureIndexer.IndexTasks
{
    using System.Threading.Tasks;
    using NBitcoin;

    public interface IIndexTask
    {
        void Index(BlockFetcher blockFetcher, TaskScheduler scheduler, Network network);

        bool SaveProgression { get; set; }

        bool EnsureIsSetup { get; set; }
    }
}