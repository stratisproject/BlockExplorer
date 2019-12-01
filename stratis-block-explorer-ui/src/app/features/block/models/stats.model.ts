export interface IStatsModel {
  blockCount?: number | undefined;
  txInputsCount?: number | undefined;
}

export class StatsModel implements IStatsModel {
  blockCount?: number | undefined;
  txInputsCount?: number | undefined;
}
