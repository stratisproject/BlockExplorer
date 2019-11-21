export interface IOutPointModel {
   isNull?: boolean | undefined;
   hash?: string | undefined;
   n?: number | undefined;
}

export class OutPointModel implements IOutPointModel {
  isNull?: boolean | undefined;
  hash?: string | undefined;
  n?: number | undefined;

  constructor(data?: IOutPointModel) {
    if (data) {
      for (const property in data) {
        if (data.hasOwnProperty(property))
          (<any>this)[property] = (<any>data)[property];
      }
    }
  }

  static fromJS(data: any): OutPointModel {
    data = typeof data === 'object' ? data : {};
    const result = new OutPointModel();
    result.init(data);
    return result;
  }

  init(data?: any) {
    if (data) {
      this.isNull = data["isNull"];
      this.hash = data["hash"];
      this.n = data["n"];
    }
  }

  toJSON(data?: any) {
    data = typeof data === 'object' ? data : {};
    data["isNull"] = this.isNull;
    data["hash"] = this.hash;
    data["n"] = this.n;
    return data;
  }
}
