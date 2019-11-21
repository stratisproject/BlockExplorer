import { BlockHeaderModel } from './block-header.model';
import { TransactionSummaryModel } from '@shared/models/transaction-summary.model';

export class BlockModel implements IBlockModel {
  blockSize?: number | undefined;
  transactions?: TransactionSummaryModel[] | undefined;
  transactionIds?: string[] | undefined;
  headerOnly?: boolean | undefined;
  header?: BlockHeaderModel | undefined;

  constructor(data?: IBlockModel) {
    if (data) {
      for (const property in data) {
        if (data.hasOwnProperty(property))
          (<any>this)[property] = (<any>data)[property];
      }
    }
  }

  static fromJS(data: any): BlockModel {
    data = typeof data === 'object' ? data : {};
    const result = new BlockModel();
    result.init(data);
    return result;
  }

  init(data?: any) {
    if (data) {
      this.blockSize = data["blockSize"];
      if (data["transactions"] && data["transactions"].constructor === Array) {
        this.transactions = [];
        for (const item of data["transactions"])
          this.transactions.push(TransactionSummaryModel.fromJS(item));
      }
      if (data["transactionIds"] && data["transactionIds"].constructor === Array) {
        this.transactionIds = [];
        for (const item of data["transactionIds"])
          this.transactionIds.push(item);
      }
      this.headerOnly = data["headerOnly"];
      this.header = data["header"] ? BlockHeaderModel.fromJS(data["header"]) : <any>undefined;
    }
  }

  toJSON(data?: any) {
    data = typeof data === 'object' ? data : {};
    data["blockSize"] = this.blockSize;
    if (this.transactions && this.transactions.constructor === Array) {
      data["transactions"] = [];
      for (const item of this.transactions)
        data["transactions"].push(item.toJSON());
    }
    if (this.transactionIds && this.transactionIds.constructor === Array) {
      data["transactionIds"] = [];
      for (const item of this.transactionIds)
        data["transactionIds"].push(item);
    }
    data["headerOnly"] = this.headerOnly;
    data["header"] = this.header ? this.header.toJSON() : <any>undefined;
    return data;
  }
}

export interface IBlockModel {
  blockSize?: number | undefined;
  transactions?: TransactionSummaryModel[] | undefined;
  headerOnly?: boolean | undefined;
  header?: BlockHeaderModel | undefined;
}
