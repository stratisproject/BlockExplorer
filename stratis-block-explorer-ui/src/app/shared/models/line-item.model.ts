import { MoneyModel } from './money.model';

export interface ILineItemModel {
   hash?: string | undefined;
   amount?: MoneyModel | undefined;
   n?: number | undefined;
}

export class LineItemModel implements ILineItemModel {
  hash?: string | undefined;
  amount?: MoneyModel | undefined;
  n?: number | undefined;

  constructor(data?: ILineItemModel) {
    if (data) {
      for (const property in data) {
        if (data.hasOwnProperty(property))
          (<any>this)[property] = (<any>data)[property];
      }
    }
  }

  static fromJS(data: any): LineItemModel {
    data = typeof data === 'object' ? data : {};
    const result = new LineItemModel();
    result.init(data);
    return result;
  }

  init(data?: any) {
    if (data) {
      this.hash = data["hash"];
      this.amount = data["amount"] ? MoneyModel.fromJS(data["amount"]) : <any>undefined;
      this.n = data["n"];
    }
  }

  toJSON(data?: any) {
    data = typeof data === 'object' ? data : {};
    data["hash"] = this.hash;
    data["amount"] = this.amount ? this.amount.toJSON() : <any>undefined;
    data["n"] = this.n;
    return data;
  }
}
