namespace Stratis.Bitcoin.Features.AzureIndexer.IndexTasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class BulkImport<T>
    {
        public BulkImport(int partitionSize)
        {
            this.PartitionSize = partitionSize;
        }

        public int PartitionSize
        {
            get;
            set;
        }

        Dictionary<string, Queue<T>> _CurrentPartitions = new Dictionary<string, Queue<T>>();

        public void Add(string partitionName, T item)
        {
            Queue<T> partition = this.GetPartition(partitionName);
            partition.Enqueue(item);
            if (partition.Count >= this.PartitionSize)
            {
                T[] fullPartition = new T[this.PartitionSize];
                for (int i = 0 ; i < this.PartitionSize ; i++)
                {
                    fullPartition[i] = partition.Dequeue();
                }

                this._ReadyPartitions.Enqueue(Tuple.Create(partitionName, fullPartition));
            }
        }

        public void FlushUncompletePartitions()
        {
            foreach (KeyValuePair<string, Queue<T>> partition in this._CurrentPartitions)
            {
                while (partition.Value.Count != 0)
                {
                    T[] fullPartition = new T[Math.Min(this.PartitionSize, partition.Value.Count)];
                    for (int i = 0 ; i < fullPartition.Length ; i++)
                    {
                        fullPartition[i] = partition.Value.Dequeue();
                    }

                    this._ReadyPartitions.Enqueue(Tuple.Create(partition.Key, fullPartition));
                }
            }
        }

        internal Queue<Tuple<string, T[]>> _ReadyPartitions = new Queue<Tuple<string, T[]>>();

        private Queue<T> GetPartition(string partition)
        {
            Queue<T> result;
            if (!this._CurrentPartitions.TryGetValue(partition, out result))
            {
                result = new Queue<T>();
                this._CurrentPartitions.Add(partition, result);
            }

            return result;
        }

        public void MoveCurrentToReady()
        {

        }

        public Dictionary<string, Queue<T>> GetCurrentPartition()
        {
            return _CurrentPartitions;
        }

        public bool HasFullPartition => this._ReadyPartitions.Count > 0;

        public bool IsEmpty => this._CurrentPartitions.Count == 0;
    }
}
