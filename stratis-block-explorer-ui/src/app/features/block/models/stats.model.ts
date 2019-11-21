export interface IStatsModel {
  blockCount?: number | undefined;
  txInputsCount?: number | undefined;
}

export class StatsModel implements IStatsModel {
  blockCount?: number | undefined;
  txInputsCount?: number | undefined;

  constructor(data?: IStatsModel) {
    if (data) {
      for (const property in data) {
        if (data.hasOwnProperty(property))
          (<any>this)[property] = (<any>data)[property];
      }
    }
  }

  static fromJS(data: any): StatsModel {
    data = typeof data === 'object' ? data : {};
    const result = new StatsModel();
    result.init(data);
    return result;
  }

  init(data?: any) {
    if (data) {
      this.blockCount = data["blockCount"];
      this.txInputsCount = data["txInputsCount"];
    }
  }

  toJSON(data?: any) {
    data = typeof data === 'object' ? data : {};
    data["blockCount"] = this.blockCount;
    data["txInputsCount"] = this.txInputsCount;
    return data;
  }
}
