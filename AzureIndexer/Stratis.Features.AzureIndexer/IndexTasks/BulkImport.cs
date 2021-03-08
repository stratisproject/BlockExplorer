namespace Stratis.Features.AzureIndexer.IndexTasks
{
    using System;
    using System.Collections.Generic;

    public class BulkImport<T>
    {
        public Dictionary<string, Queue<T>> currentPartitions = new Dictionary<string, Queue<T>>();

        public Queue<Tuple<string, T[]>> ReadyPartitions = new Queue<Tuple<string, T[]>>();

        public BulkImport(int partitionSize)
        {
            this.PartitionSize = partitionSize;
        }

        public int PartitionSize { get; set; }

        public bool HasFullPartition => this.ReadyPartitions.Count > 0;

        public bool IsEmpty => this.currentPartitions.Count == 0;

        public void Add(string partitionName, T item)
        {
            Queue<T> partition = this.GetPartition(partitionName);
            partition.Enqueue(item);
            if (partition.Count >= this.PartitionSize)
            {
                var fullPartition = new T[this.PartitionSize];
                for (var i = 0; i < this.PartitionSize; i++)
                {
                    fullPartition[i] = partition.Dequeue();
                }

                this.ReadyPartitions.Enqueue(Tuple.Create(partitionName, fullPartition));
            }
        }

        public void FlushUncompletePartitions()
        {
            foreach (KeyValuePair<string, Queue<T>> partition in this.currentPartitions)
            {
                while (partition.Value.Count != 0)
                {
                    T[] fullPartition = new T[Math.Min(this.PartitionSize, partition.Value.Count)];
                    for (int i = 0; i < fullPartition.Length; i++)
                    {
                        fullPartition[i] = partition.Value.Dequeue();
                    }

                    this.ReadyPartitions.Enqueue(Tuple.Create(partition.Key, fullPartition));
                }
            }
        }

        private Queue<T> GetPartition(string partition)
        {
            if (!this.currentPartitions.TryGetValue(partition, out Queue<T> result))
            {
                result = new Queue<T>();
                this.currentPartitions.Add(partition, result);
            }

            return result;
        }

        //public void MoveCurrentToReady()
        //{

        //}

        public Dictionary<string, Queue<T>> GetCurrentPartition()
        {
            return this.currentPartitions;
        }
    }
}
