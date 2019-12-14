export interface IBlockStat {
   size: number;
   transactionCount: number;
   inputsCount: number;
   outputCounts: number;
}

export interface IStatsModel {
   blocksStats: IBlockStat[];
   transactionCount: number;
   inputsCount: number;
   outputCounts: number;
}


export class StatsModel implements IStatsModel {
   blocksStats: IBlockStat[];
   transactionCount: number;
   inputsCount: number;
   outputCounts: number;
}
