import { Observable, throwError as _observableThrow, of as _observableOf } from 'rxjs';
import { InjectionToken } from '@angular/core';

export const API_BASE_URL = new InjectionToken<string>('API_BASE_URL');

export class BalanceResponseModel implements IBalanceResponseModel {
  continuation?: string | undefined;
  operations?: BalanceOperationModel[] | undefined;
  conflictedOperations?: BalanceOperationModel[] | undefined;

  constructor(data?: IBalanceResponseModel) {
      if (data) {
          for (const property in data) {
              if (data.hasOwnProperty(property))
                  (<any>this)[property] = (<any>data)[property];
          }
      }
  }

  static fromJS(data: any): BalanceResponseModel {
      data = typeof data === 'object' ? data : {};
      const result = new BalanceResponseModel();
      result.init(data);
      return result;
  }

  init(data?: any) {
      if (data) {
          this.continuation = data["continuation"];
          if (data["operations"] && data["operations"].constructor === Array) {
              this.operations = [];
              for (const item of data["operations"])
                  this.operations.push(BalanceOperationModel.fromJS(item));
          }
          if (data["ConflictedOperations"] && data["ConflictedOperations"].constructor === Array) {
              this.conflictedOperations = [];
              for (const item of data["ConflictedOperations"])
                  this.conflictedOperations.push(BalanceOperationModel.fromJS(item));
          }
      }
  }

  toJSON(data?: any) {
      data = typeof data === 'object' ? data : {};
      data["continuation"] = this.continuation;
      if (this.operations && this.operations.constructor === Array) {
          data["operations"] = [];
          for (const item of this.operations)
              data["operations"].push(item.toJSON());
      }
      if (this.conflictedOperations && this.conflictedOperations.constructor === Array) {
          data["ConflictedOperations"] = [];
          for (const item of this.conflictedOperations)
              data["ConflictedOperations"].push(item.toJSON());
      }
      return data;
  }
}

export interface IBalanceResponseModel {
  continuation?: string | undefined;
  operations?: BalanceOperationModel[] | undefined;
  conflictedOperations?: BalanceOperationModel[] | undefined;
}

export class BalanceOperationModel implements IBalanceOperationModel {
  amount?: MoneyModel | undefined;
  confirmations?: number | undefined;
  height?: number | undefined;
  blockId?: string | undefined;
  transactionId?: string | undefined;
  receivedCoins?: CoinModel[] | undefined;
  spentCoins?: CoinModel[] | undefined;
  firstSeen?: Date | undefined;
  transactionSummary?: TransactionSummaryModel | undefined;

  constructor(data?: IBalanceOperationModel) {
      if (data) {
          for (const property in data) {
              if (data.hasOwnProperty(property))
                  (<any>this)[property] = (<any>data)[property];
          }
      }
  }

  static fromJS(data: any): BalanceOperationModel {
      data = typeof data === 'object' ? data : {};
      const result = new BalanceOperationModel();
      result.init(data);
      return result;
  }

  init(data?: any) {
      if (data) {
          this.amount = data["amount"] ? MoneyModel.fromJS(data["amount"]) : <any>undefined;
          this.confirmations = data["confirmations"];
          this.height = data["height"];
          this.blockId = data["blockId"];
          this.transactionId = data["transactionId"];
          if (data["receivedCoins"] && data["receivedCoins"].constructor === Array) {
              this.receivedCoins = [];
              for (const item of data["receivedCoins"])
                  this.receivedCoins.push(CoinModel.fromJS(item));
          }
          if (data["spentCoins"] && data["spentCoins"].constructor === Array) {
              this.spentCoins = [];
              for (const item of data["spentCoins"])
                  this.spentCoins.push(CoinModel.fromJS(item));
          }
          this.firstSeen = data["firstSeen"] ? new Date(data["firstSeen"].toString()) : <any>undefined;
          this.transactionSummary = data["transactionSummary"] ? TransactionSummaryModel.fromJS(data["transactionSummary"]) : <any>undefined;
      }
  }

  toJSON(data?: any) {
      data = typeof data === 'object' ? data : {};
      data["amount"] = this.amount ? this.amount.toJSON() : <any>undefined;
      data["confirmations"] = this.confirmations;
      data["height"] = this.height;
      data["blockId"] = this.blockId;
      data["transactionId"] = this.transactionId;
      if (this.receivedCoins && this.receivedCoins.constructor === Array) {
          data["receivedCoins"] = [];
          for (const item of this.receivedCoins)
              data["receivedCoins"].push(item.toJSON());
      }
      if (this.spentCoins && this.spentCoins.constructor === Array) {
          data["spentCoins"] = [];
          for (const item of this.spentCoins)
              data["spentCoins"].push(item.toJSON());
      }
      data["firstSeen"] = this.firstSeen ? this.firstSeen.toISOString() : <any>undefined;
      data["transactionSummary"] = this.transactionSummary ? this.transactionSummary.toJSON() : <any>undefined;
      return data;
  }
}

export interface IBalanceOperationModel {
  amount?: MoneyModel | undefined;
  confirmations?: number | undefined;
  height?: number | undefined;
  blockId?: string | undefined;
  transactionId?: string | undefined;
  receivedCoins?: CoinModel[] | undefined;
  spentCoins?: CoinModel[] | undefined;
  firstSeen?: Date | undefined;
  transactionSummary?: TransactionSummaryModel | undefined;
}

export class MoneyModel implements IMoneyModel {
  satoshi?: number | undefined;

  constructor(data?: IMoneyModel) {
      if (data) {
          for (const property in data) {
              if (data.hasOwnProperty(property))
                  (<any>this)[property] = (<any>data)[property];
          }
      }
  }

  static fromJS(data: any): MoneyModel {
      data = typeof data === 'object' ? data : {};
      const result = new MoneyModel();
      result.init(data);
      return result;
  }

  init(data?: any) {
      if (data) {
          this.satoshi = data["satoshi"];
      }
  }

  toJSON(data?: any) {
      data = typeof data === 'object' ? data : {};
      data["satoshi"] = this.satoshi;
      return data;
  }
}

export interface IMoneyModel {
  satoshi?: number | undefined;
}

export class CoinModel implements ICoinModel {
  amount?: any | undefined;
  outpoint?: OutPointModel | undefined;
  txOut?: TxOutModel | undefined;

  constructor(data?: ICoinModel) {
      if (data) {
          for (const property in data) {
              if (data.hasOwnProperty(property))
                  (<any>this)[property] = (<any>data)[property];
          }
      }
  }

  static fromJS(data: any): CoinModel {
      data = typeof data === 'object' ? data : {};
      const result = new CoinModel();
      result.init(data);
      return result;
  }

  init(data?: any) {
      if (data) {
          this.amount = data["amount"];
          this.outpoint = data["outpoint"] ? OutPointModel.fromJS(data["outpoint"]) : <any>undefined;
          this.txOut = data["txOut"] ? TxOutModel.fromJS(data["txOut"]) : <any>undefined;
      }
  }

  toJSON(data?: any) {
      data = typeof data === 'object' ? data : {};
      data["amount"] = this.amount;
      data["outpoint"] = this.outpoint ? this.outpoint.toJSON() : <any>undefined;
      data["txOut"] = this.txOut ? this.txOut.toJSON() : <any>undefined;
      return data;
  }
}

export interface ICoinModel {
  amount?: any | undefined;
  outpoint?: OutPointModel | undefined;
  txOut?: TxOutModel | undefined;
}

export class TransactionSummaryModel implements ITransactionSummaryModel {
  hash?: string | undefined;
  isCoinbase?: boolean | undefined;
  isCoinstake?: boolean | undefined;
  isSmartContract: boolean;
  amount?: MoneyModel | undefined;
  fee?: MoneyModel | undefined;
  height?: number | undefined;
  time?: number | undefined;
  spent?: boolean | undefined;
  in?: LineItemModel[] | undefined;
  out?: LineItemModel[] | undefined;
  confirmations?: number | undefined;

  constructor(data?: ITransactionSummaryModel) {
    if (data) {
      for (const property in data) {
        if (data.hasOwnProperty(property))
          (<any>this)[property] = (<any>data)[property];
      }
    }
  }

  static fromJS(data: any): TransactionSummaryModel {
    data = typeof data === 'object' ? data : {};
    const result = new TransactionSummaryModel();
    result.init(data);
    return result;
  }

  init(data?: any) {
    if (data) {
      this.hash = data["hash"];
      this.isCoinbase = data["isCoinbase"];
      this.isCoinstake = data["isCoinstake"];
      this.isSmartContract = data["isSmartContract"];
      this.amount = data["amount"] ? MoneyModel.fromJS(data["amount"]) : <any>undefined;
      this.fee = data["fee"] ? MoneyModel.fromJS(data["fee"]) : <any>undefined;
      this.height = data["height"];
      this.time = data["time"];
      this.spent = data["spent"];
      if (data["in"] && data["in"].constructor === Array) {
          this.in = [];
          for (const item of data["in"])
              this.in.push(LineItemModel.fromJS(item));
      }
      if (data["out"] && data["out"].constructor === Array) {
          this.out = [];
          for (const item of data["out"])
              this.out.push(LineItemModel.fromJS(item));
      }
      this.confirmations = data["confirmations"];
    }
  }

  toJSON(data?: any) {
      data = typeof data === 'object' ? data : {};
      data["hash"] = this.hash;
      data["isCoinbase"] = this.isCoinbase;
      data["isCoinstake"] = this.isCoinstake;
      data["isSmartContract"] = this.isSmartContract;
      data["amount"] = this.amount ? this.amount.toJSON() : <any>undefined;
      data["fee"] = this.fee ? this.fee.toJSON() : <any>undefined;
      data["height"] = this.height;
      data["time"] = this.time;
      data["spent"] = this.spent;
      if (this.in && this.in.constructor === Array) {
          data["in"] = [];
          for (const item of this.in)
              data["in"].push(item.toJSON());
      }
      if (this.out && this.out.constructor === Array) {
          data["out"] = [];
          for (const item of this.out)
              data["out"].push(item.toJSON());
      }
      data["confirmations"] = this.confirmations;
      return data;
  }
}

export interface ITransactionSummaryModel {
  hash?: string | undefined;
  isCoinbase?: boolean | undefined;
  isCoinstake?: boolean | undefined;
  isSmartContract: boolean;
  amount?: MoneyModel | undefined;
  fee?: MoneyModel | undefined;
  height?: number | undefined;
  time?: number | undefined;
  spent?: boolean | undefined;
  in?: LineItemModel[] | undefined;
  out?: LineItemModel[] | undefined;
  confirmations?: number | undefined;
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

export interface ILineItemModel {
  hash?: string | undefined;
  amount?: MoneyModel | undefined;
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

export interface IOutPointModel {
  isNull?: boolean | undefined;
  hash?: string | undefined;
  n?: number | undefined;
}

export class TxOutModel implements ITxOutModel {
  scriptPubKey?: ScriptModel | undefined;
  isEmpty?: boolean | undefined;
  value?: MoneyModel | undefined;

  constructor(data?: ITxOutModel) {
      if (data) {
          for (const property in data) {
              if (data.hasOwnProperty(property))
                  (<any>this)[property] = (<any>data)[property];
          }
      }
  }

  static fromJS(data: any): TxOutModel {
      data = typeof data === 'object' ? data : {};
      const result = new TxOutModel();
      result.init(data);
      return result;
  }

  init(data?: any) {
      if (data) {
          this.scriptPubKey = data["scriptPubKey"] ? ScriptModel.fromJS(data["scriptPubKey"]) : <any>undefined;
          this.isEmpty = data["isEmpty"];
          this.value = data["value"] ? MoneyModel.fromJS(data["value"]) : <any>undefined;
      }
  }

  toJSON(data?: any) {
      data = typeof data === 'object' ? data : {};
      data["scriptPubKey"] = this.scriptPubKey ? this.scriptPubKey.toJSON() : <any>undefined;
      data["isEmpty"] = this.isEmpty;
      data["value"] = this.value ? this.value.toJSON() : <any>undefined;
      return data;
  }
}

export interface ITxOutModel {
  scriptPubKey?: ScriptModel | undefined;
  isEmpty?: boolean | undefined;
  value?: MoneyModel | undefined;
}

export class ScriptModel implements IScriptModel {
  length?: number | undefined;
  isPushOnly?: boolean | undefined;
  hasCanonicalPushes?: boolean | undefined;
  hash?: string | undefined;
  paymentScriptHash?: string | undefined;
  isUnspendable?: boolean | undefined;
  addresses?: string[] | undefined;
  isValid?: boolean | undefined;

  constructor(data?: IScriptModel) {
      if (data) {
          for (const property in data) {
              if (data.hasOwnProperty(property))
                  (<any>this)[property] = (<any>data)[property];
          }
      }
  }

  static fromJS(data: any): ScriptModel {
      data = typeof data === 'object' ? data : {};
      const result = new ScriptModel();
      result.init(data);
      return result;
  }

  init(data?: any) {
      if (data) {
          this.length = data["length"];
          this.isPushOnly = data["isPushOnly"];
          this.hasCanonicalPushes = data["hasCanonicalPushes"];
          this.hash = data["hash"];
          this.paymentScriptHash = data["paymentScriptHash"];
          this.isUnspendable = data["isUnspendable"];
          if (data["addresses"] && data["addresses"].constructor === Array) {
              this.addresses = [];
              for (const item of data["addresses"])
                  this.addresses.push(item);
          }
          this.isValid = data["isValid"];
      }
  }

  toJSON(data?: any) {
      data = typeof data === 'object' ? data : {};
      data["length"] = this.length;
      data["isPushOnly"] = this.isPushOnly;
      data["hasCanonicalPushes"] = this.hasCanonicalPushes;
      data["hash"] = this.hash;
      data["paymentScriptHash"] = this.paymentScriptHash;
      data["isUnspendable"] = this.isUnspendable;
      if (this.addresses && this.addresses.constructor === Array) {
          data["addresses"] = [];
          for (const item of this.addresses)
              data["addresses"].push(item);
      }
      data["isValid"] = this.isValid;
      return data;
  }
}

export interface IScriptModel {
  length?: number | undefined;
  isPushOnly?: boolean | undefined;
  hasCanonicalPushes?: boolean | undefined;
  hash?: string | undefined;
  paymentScriptHash?: string | undefined;
  isUnspendable?: boolean | undefined;
  addresses?: string[] | undefined;
  isValid?: boolean | undefined;
}

export class BalanceSummaryModel implements IBalanceSummaryModel {
  unConfirmed?: BalanceSummaryDetailsModel | undefined;
  confirmed?: BalanceSummaryDetailsModel | undefined;
  spendable?: BalanceSummaryDetailsModel | undefined;
  immature?: BalanceSummaryDetailsModel | undefined;
  locator?: any | undefined;
  olderImmature?: number | undefined;
  cacheHit?: BalanceSummaryModelCacheHit | undefined;

  constructor(data?: IBalanceSummaryModel) {
      if (data) {
          for (const property in data) {
              if (data.hasOwnProperty(property))
                  (<any>this)[property] = (<any>data)[property];
          }
      }
  }

  static fromJS(data: any): BalanceSummaryModel {
      data = typeof data === 'object' ? data : {};
      const result = new BalanceSummaryModel();
      result.init(data);
      return result;
  }

  init(data?: any) {
      if (data) {
          this.unConfirmed = data["unConfirmed"] ? BalanceSummaryDetailsModel.fromJS(data["unConfirmed"]) : <any>undefined;
          this.confirmed = data["confirmed"] ? BalanceSummaryDetailsModel.fromJS(data["confirmed"]) : <any>undefined;
          this.spendable = data["spendable"] ? BalanceSummaryDetailsModel.fromJS(data["spendable"]) : <any>undefined;
          this.immature = data["immature"] ? BalanceSummaryDetailsModel.fromJS(data["immature"]) : <any>undefined;
          this.locator = data["locator"];
          this.olderImmature = data["olderImmature"];
          this.cacheHit = data["cacheHit"];
      }
  }

  toJSON(data?: any) {
      data = typeof data === 'object' ? data : {};
      data["unConfirmed"] = this.unConfirmed ? this.unConfirmed.toJSON() : <any>undefined;
      data["confirmed"] = this.confirmed ? this.confirmed.toJSON() : <any>undefined;
      data["spendable"] = this.spendable ? this.spendable.toJSON() : <any>undefined;
      data["immature"] = this.immature ? this.immature.toJSON() : <any>undefined;
      data["locator"] = this.locator;
      data["olderImmature"] = this.olderImmature;
      data["cacheHit"] = this.cacheHit;
      return data;
  }
}

export interface IBalanceSummaryModel {
  unConfirmed?: BalanceSummaryDetailsModel | undefined;
  confirmed?: BalanceSummaryDetailsModel | undefined;
  spendable?: BalanceSummaryDetailsModel | undefined;
  immature?: BalanceSummaryDetailsModel | undefined;
  locator?: any | undefined;
  olderImmature?: number | undefined;
  cacheHit?: BalanceSummaryModelCacheHit | undefined;
}

export class BalanceSummaryDetailsModel implements IBalanceSummaryDetailsModel {
  transactionCount?: number | undefined;
  amount?: MoneyModel | undefined;
  received?: MoneyModel | undefined;
  assets?: AssetBalanceSummaryDetailsModel[] | undefined;

  constructor(data?: IBalanceSummaryDetailsModel) {
      if (data) {
          for (const property in data) {
              if (data.hasOwnProperty(property))
                  (<any>this)[property] = (<any>data)[property];
          }
      }
  }

  static fromJS(data: any): BalanceSummaryDetailsModel {
      data = typeof data === 'object' ? data : {};
      const result = new BalanceSummaryDetailsModel();
      result.init(data);
      return result;
  }

  init(data?: any) {
      if (data) {
          this.transactionCount = data["transactionCount"];
          this.amount = data["amount"] ? MoneyModel.fromJS(data["amount"]) : <any>undefined;
          this.received = data["received"] ? MoneyModel.fromJS(data["received"]) : <any>undefined;
          if (data["assets"] && data["assets"].constructor === Array) {
              this.assets = [];
              for (const item of data["assets"])
                  this.assets.push(AssetBalanceSummaryDetailsModel.fromJS(item));
          }
      }
  }

  toJSON(data?: any) {
      data = typeof data === 'object' ? data : {};
      data["transactionCount"] = this.transactionCount;
      data["amount"] = this.amount ? this.amount.toJSON() : <any>undefined;
      data["received"] = this.received ? this.received.toJSON() : <any>undefined;
      if (this.assets && this.assets.constructor === Array) {
          data["assets"] = [];
          for (const item of this.assets)
              data["assets"].push(item.toJSON());
      }
      return data;
  }
}

export interface IBalanceSummaryDetailsModel {
  transactionCount?: number | undefined;
  amount?: MoneyModel | undefined;
  received?: MoneyModel | undefined;
  assets?: AssetBalanceSummaryDetailsModel[] | undefined;
}

export class AssetBalanceSummaryDetailsModel implements IAssetBalanceSummaryDetailsModel {
  asset?: BitcoinAssetIdModel | undefined;
  quantity?: number | undefined;
  received?: number | undefined;

  constructor(data?: IAssetBalanceSummaryDetailsModel) {
      if (data) {
          for (const property in data) {
              if (data.hasOwnProperty(property))
                  (<any>this)[property] = (<any>data)[property];
          }
      }
  }

  static fromJS(data: any): AssetBalanceSummaryDetailsModel {
      data = typeof data === 'object' ? data : {};
      const result = new AssetBalanceSummaryDetailsModel();
      result.init(data);
      return result;
  }

  init(data?: any) {
      if (data) {
          this.asset = data["asset"] ? BitcoinAssetIdModel.fromJS(data["asset"]) : <any>undefined;
          this.quantity = data["quantity"];
          this.received = data["received"];
      }
  }

  toJSON(data?: any) {
      data = typeof data === 'object' ? data : {};
      data["asset"] = this.asset ? this.asset.toJSON() : <any>undefined;
      data["quantity"] = this.quantity;
      data["received"] = this.received;
      return data;
  }
}

export interface IAssetBalanceSummaryDetailsModel {
  asset?: BitcoinAssetIdModel | undefined;
  quantity?: number | undefined;
  received?: number | undefined;
}

export class BitcoinAssetIdModel implements IBitcoinAssetIdModel {
  assetId?: any | undefined;
  type?: BitcoinAssetIdModelType | undefined;

  constructor(data?: IBitcoinAssetIdModel) {
      if (data) {
          for (const property in data) {
              if (data.hasOwnProperty(property))
                  (<any>this)[property] = (<any>data)[property];
          }
      }
  }

  static fromJS(data: any): BitcoinAssetIdModel {
      data = typeof data === 'object' ? data : {};
      const result = new BitcoinAssetIdModel();
      result.init(data);
      return result;
  }

  init(data?: any) {
      if (data) {
          this.assetId = data["assetId"];
          this.type = data["type"];
      }
  }

  toJSON(data?: any) {
      data = typeof data === 'object' ? data : {};
      data["assetId"] = this.assetId;
      data["type"] = this.type;
      return data;
  }
}

export interface IBitcoinAssetIdModel {
  assetId?: any | undefined;
  type?: BitcoinAssetIdModelType | undefined;
}

export class TransactionResponseModel implements ITransactionResponseModel {
  transaction?: TransactionModel | undefined;
  transactionId?: string | undefined;
  isCoinbase?: boolean | undefined;
  block?: BlockInformationModel | undefined;
  spentCoins?: CoinModel[] | undefined;
  receivedCoins?: CoinModel[] | undefined;
  firstSeen?: Date | undefined;
  fees?: Money | undefined;

  constructor(data?: ITransactionResponseModel) {
      if (data) {
          for (const property in data) {
              if (data.hasOwnProperty(property))
                  (<any>this)[property] = (<any>data)[property];
          }
      }
  }

  static fromJS(data: any): TransactionResponseModel {
      data = typeof data === 'object' ? data : {};
      const result = new TransactionResponseModel();
      result.init(data);
      return result;
  }

  init(data?: any) {
      if (data) {
          this.transaction = data["transaction"] ? TransactionModel.fromJS(data["transaction"]) : <any>undefined;
          this.transactionId = data["transactionId"];
          this.isCoinbase = data["isCoinbase"];
          this.block = data["block"] ? BlockInformationModel.fromJS(data["block"]) : <any>undefined;
          if (data["spentCoins"] && data["spentCoins"].constructor === Array) {
              this.spentCoins = [];
              for (const item of data["spentCoins"])
                  this.spentCoins.push(CoinModel.fromJS(item));
          }
          if (data["receivedCoins"] && data["receivedCoins"].constructor === Array) {
              this.receivedCoins = [];
              for (const item of data["receivedCoins"])
                  this.receivedCoins.push(CoinModel.fromJS(item));
          }
          this.firstSeen = data["firstSeen"] ? new Date(data["firstSeen"].toString()) : <any>undefined;
          this.fees = data["fees"] ? Money.fromJS(data["fees"]) : <any>undefined;
      }
  }

  toJSON(data?: any) {
      data = typeof data === 'object' ? data : {};
      data["transaction"] = this.transaction ? this.transaction.toJSON() : <any>undefined;
      data["transactionId"] = this.transactionId;
      data["isCoinbase"] = this.isCoinbase;
      data["block"] = this.block ? this.block.toJSON() : <any>undefined;
      if (this.spentCoins && this.spentCoins.constructor === Array) {
          data["spentCoins"] = [];
          for (const item of this.spentCoins)
              data["spentCoins"].push(item.toJSON());
      }
      if (this.receivedCoins && this.receivedCoins.constructor === Array) {
          data["receivedCoins"] = [];
          for (const item of this.receivedCoins)
              data["receivedCoins"].push(item.toJSON());
      }
      data["firstSeen"] = this.firstSeen ? this.firstSeen.toISOString() : <any>undefined;
      data["fees"] = this.fees ? this.fees.toJSON() : <any>undefined;
      return data;
  }
}

export interface ITransactionResponseModel {
  transaction?: TransactionModel | undefined;
  transactionId?: string | undefined;
  isCoinbase?: boolean | undefined;
  block?: BlockInformationModel | undefined;
  spentCoins?: CoinModel[] | undefined;
  receivedCoins?: CoinModel[] | undefined;
  firstSeen?: Date | undefined;
  fees?: Money | undefined;
}

export class TransactionModel implements ITransactionModel {
  rBF?: boolean | undefined;
  version?: number | undefined;
  time?: number | undefined;
  totalOut?: MoneyModel | undefined;
  inputs?: TxInModel[] | undefined;
  outputs?: TxOutModel[] | undefined;
  isCoinBase?: boolean | undefined;
  isCoinStake?: boolean | undefined;
  hasWitness?: boolean | undefined;

  constructor(data?: ITransactionModel) {
      if (data) {
          for (const property in data) {
              if (data.hasOwnProperty(property))
                  (<any>this)[property] = (<any>data)[property];
          }
      }
  }

  static fromJS(data: any): TransactionModel {
      data = typeof data === 'object' ? data : {};
      const result = new TransactionModel();
      result.init(data);
      return result;
  }

  init(data?: any) {
      if (data) {
          this.rBF = data["rBF"];
          this.version = data["version"];
          this.time = data["time"];
          this.totalOut = data["totalOut"] ? MoneyModel.fromJS(data["totalOut"]) : <any>undefined;
          if (data["inputs"] && data["inputs"].constructor === Array) {
              this.inputs = [];
              for (const item of data["inputs"])
                  this.inputs.push(TxInModel.fromJS(item));
          }
          if (data["outputs"] && data["outputs"].constructor === Array) {
              this.outputs = [];
              for (const item of data["outputs"])
                  this.outputs.push(TxOutModel.fromJS(item));
          }
          this.isCoinBase = data["isCoinBase"];
          this.isCoinStake = data["isCoinStake"];
          this.hasWitness = data["hasWitness"];
      }
  }

  toJSON(data?: any) {
      data = typeof data === 'object' ? data : {};
      data["rBF"] = this.rBF;
      data["version"] = this.version;
      data["time"] = this.time;
      data["totalOut"] = this.totalOut ? this.totalOut.toJSON() : <any>undefined;
      if (this.inputs && this.inputs.constructor === Array) {
          data["inputs"] = [];
          for (const item of this.inputs)
              data["inputs"].push(item.toJSON());
      }
      if (this.outputs && this.outputs.constructor === Array) {
          data["outputs"] = [];
          for (const item of this.outputs)
              data["outputs"].push(item.toJSON());
      }
      data["isCoinBase"] = this.isCoinBase;
      data["isCoinStake"] = this.isCoinStake;
      data["hasWitness"] = this.hasWitness;
      return data;
  }
}

export interface ITransactionModel {
  rBF?: boolean | undefined;
  version?: number | undefined;
  time?: number | undefined;
  totalOut?: MoneyModel | undefined;
  inputs?: TxInModel[] | undefined;
  outputs?: TxOutModel[] | undefined;
  isCoinBase?: boolean | undefined;
  isCoinStake?: boolean | undefined;
  hasWitness?: boolean | undefined;
}

export class BlockInformationModel implements IBlockInformationModel {
  blockId?: string | undefined;
  blockHeader?: BlockHeaderModel | undefined;
  height?: number | undefined;
  confirmations?: number | undefined;
  medianTimePast?: Date | undefined;
  blockTime?: Date | undefined;

  constructor(data?: IBlockInformationModel) {
      if (data) {
          for (const property in data) {
              if (data.hasOwnProperty(property))
                  (<any>this)[property] = (<any>data)[property];
          }
      }
  }

  static fromJS(data: any): BlockInformationModel {
      data = typeof data === 'object' ? data : {};
      const result = new BlockInformationModel();
      result.init(data);
      return result;
  }

  init(data?: any) {
      if (data) {
          this.blockId = data["blockId"];
          this.blockHeader = data["blockHeader"] ? BlockHeaderModel.fromJS(data["blockHeader"]) : <any>undefined;
          this.height = data["height"];
          this.confirmations = data["confirmations"];
          this.medianTimePast = data["medianTimePast"] ? new Date(data["medianTimePast"].toString()) : <any>undefined;
          this.blockTime = data["blockTime"] ? new Date(data["blockTime"].toString()) : <any>undefined;
      }
  }

  toJSON(data?: any) {
      data = typeof data === 'object' ? data : {};
      data["blockId"] = this.blockId;
      data["blockHeader"] = this.blockHeader ? this.blockHeader.toJSON() : <any>undefined;
      data["height"] = this.height;
      data["confirmations"] = this.confirmations;
      data["medianTimePast"] = this.medianTimePast ? this.medianTimePast.toISOString() : <any>undefined;
      data["blockTime"] = this.blockTime ? this.blockTime.toISOString() : <any>undefined;
      return data;
  }
}

export interface IBlockInformationModel {
  blockId?: string | undefined;
  blockHeader?: BlockHeaderModel | undefined;
  height?: number | undefined;
  confirmations?: number | undefined;
  medianTimePast?: Date | undefined;
  blockTime?: Date | undefined;
}

export class Money implements IMoney {
  satoshi?: number | undefined;

  constructor(data?: IMoney) {
      if (data) {
          for (const property in data) {
              if (data.hasOwnProperty(property))
                  (<any>this)[property] = (<any>data)[property];
          }
      }
  }

  static fromJS(data: any): Money {
      data = typeof data === 'object' ? data : {};
      const result = new Money();
      result.init(data);
      return result;
  }

  init(data?: any) {
      if (data) {
          this.satoshi = data["satoshi"];
      }
  }

  toJSON(data?: any) {
      data = typeof data === 'object' ? data : {};
      data["satoshi"] = this.satoshi;
      return data;
  }
}

export interface IMoney {
  satoshi?: number | undefined;
}

export class TxInModel implements ITxInModel {
  sequence?: SequenceModel | undefined;
  prevOut?: OutPointModel | undefined;
  scriptSig?: ScriptModel | undefined;
  isFinal?: boolean | undefined;

  constructor(data?: ITxInModel) {
      if (data) {
          for (const property in data) {
              if (data.hasOwnProperty(property))
                  (<any>this)[property] = (<any>data)[property];
          }
      }
  }

  static fromJS(data: any): TxInModel {
      data = typeof data === 'object' ? data : {};
      const result = new TxInModel();
      result.init(data);
      return result;
  }

  init(data?: any) {
      if (data) {
          this.sequence = data["sequence"] ? SequenceModel.fromJS(data["sequence"]) : <any>undefined;
          this.prevOut = data["prevOut"] ? OutPointModel.fromJS(data["prevOut"]) : <any>undefined;
          this.scriptSig = data["scriptSig"] ? ScriptModel.fromJS(data["scriptSig"]) : <any>undefined;
          this.isFinal = data["isFinal"];
      }
  }

  toJSON(data?: any) {
      data = typeof data === 'object' ? data : {};
      data["sequence"] = this.sequence ? this.sequence.toJSON() : <any>undefined;
      data["prevOut"] = this.prevOut ? this.prevOut.toJSON() : <any>undefined;
      data["scriptSig"] = this.scriptSig ? this.scriptSig.toJSON() : <any>undefined;
      data["isFinal"] = this.isFinal;
      return data;
  }
}

export interface ITxInModel {
  sequence?: SequenceModel | undefined;
  prevOut?: OutPointModel | undefined;
  scriptSig?: ScriptModel | undefined;
  isFinal?: boolean | undefined;
}

export class BlockHeaderModel implements IBlockHeaderModel {
  currentVersion?: number | undefined;
  hashPrevBlock?: string | undefined;
  time?: number | undefined;
  bits?: TargetModel | undefined;
  version?: number | undefined;
  nonce?: number | undefined;
  hashMerkleRoot?: string | undefined;
  isNull?: boolean | undefined;
  blockTime?: Date | undefined;

  constructor(data?: IBlockHeaderModel) {
      if (data) {
          for (const property in data) {
              if (data.hasOwnProperty(property))
                  (<any>this)[property] = (<any>data)[property];
          }
      }
  }

  static fromJS(data: any): BlockHeaderModel {
      data = typeof data === 'object' ? data : {};
      const result = new BlockHeaderModel();
      result.init(data);
      return result;
  }

  init(data?: any) {
      if (data) {
          this.currentVersion = data["currentVersion"];
          this.hashPrevBlock = data["hashPrevBlock"];
          this.time = data["time"];
          this.bits = data["bits"] ? TargetModel.fromJS(data["bits"]) : <any>undefined;
          this.version = data["version"];
          this.nonce = data["nonce"];
          this.hashMerkleRoot = data["hashMerkleRoot"];
          this.isNull = data["isNull"];
          this.blockTime = data["blockTime"] ? new Date(data["blockTime"].toString()) : <any>undefined;
      }
  }

  toJSON(data?: any) {
      data = typeof data === 'object' ? data : {};
      data["currentVersion"] = this.currentVersion;
      data["hashPrevBlock"] = this.hashPrevBlock;
      data["time"] = this.time;
      data["bits"] = this.bits ? this.bits.toJSON() : <any>undefined;
      data["version"] = this.version;
      data["nonce"] = this.nonce;
      data["hashMerkleRoot"] = this.hashMerkleRoot;
      data["isNull"] = this.isNull;
      data["blockTime"] = this.blockTime ? this.blockTime.toISOString() : <any>undefined;
      return data;
  }
}

export interface IBlockHeaderModel {
  currentVersion?: number | undefined;
  hashPrevBlock?: string | undefined;
  time?: number | undefined;
  bits?: TargetModel | undefined;
  version?: number | undefined;
  nonce?: number | undefined;
  hashMerkleRoot?: string | undefined;
  isNull?: boolean | undefined;
  blockTime?: Date | undefined;
}

export class SequenceModel implements ISequenceModel {
  value?: number | undefined;
  isRelativeLock?: boolean | undefined;
  isRBF?: boolean | undefined;

  constructor(data?: ISequenceModel) {
      if (data) {
          for (const property in data) {
              if (data.hasOwnProperty(property))
                  (<any>this)[property] = (<any>data)[property];
          }
      }
  }

  static fromJS(data: any): SequenceModel {
      data = typeof data === 'object' ? data : {};
      const result = new SequenceModel();
      result.init(data);
      return result;
  }

  init(data?: any) {
      if (data) {
          this.value = data["value"];
          this.isRelativeLock = data["isRelativeLock"];
          this.isRBF = data["isRBF"];
      }
  }

  toJSON(data?: any) {
      data = typeof data === 'object' ? data : {};
      data["value"] = this.value;
      data["isRelativeLock"] = this.isRelativeLock;
      data["isRBF"] = this.isRBF;
      return data;
  }
}

export interface ISequenceModel {
  value?: number | undefined;
  isRelativeLock?: boolean | undefined;
  isRBF?: boolean | undefined;
}

export class TargetModel implements ITargetModel {
  difficulty?: number | undefined;

  constructor(data?: ITargetModel) {
      if (data) {
          for (const property in data) {
              if (data.hasOwnProperty(property))
                  (<any>this)[property] = (<any>data)[property];
          }
      }
  }

  static fromJS(data: any): TargetModel {
      data = typeof data === 'object' ? data : {};
      const result = new TargetModel();
      result.init(data);
      return result;
  }

  init(data?: any) {
      if (data) {
          this.difficulty = data["difficulty"];
      }
  }

  toJSON(data?: any) {
      data = typeof data === 'object' ? data : {};
      data["difficulty"] = this.difficulty;
      return data;
  }
}

export interface ITargetModel {
  difficulty?: number | undefined;
}

export class BlockResponseModel implements IBlockResponseModel {
  additionalInformation?: BlockInformationModel | undefined;
  extendedInformation?: ExtendedBlockInformationModel | undefined;
  block?: BlockModel | undefined;

  constructor(data?: IBlockResponseModel) {
      if (data) {
          for (const property in data) {
              if (data.hasOwnProperty(property))
                  (<any>this)[property] = (<any>data)[property];
          }
      }
  }

  static fromJS(data: any): BlockResponseModel {
      data = typeof data === 'object' ? data : {};
      const result = new BlockResponseModel();
      result.init(data);
      return result;
  }

  init(data?: any) {
      if (data) {
          this.additionalInformation = data["additionalInformation"] ? BlockInformationModel.fromJS(data["additionalInformation"]) : <any>undefined;
          this.extendedInformation = data["extendedInformation"] ? ExtendedBlockInformationModel.fromJS(data["extendedInformation"]) : <any>undefined;
          this.block = data["block"] ? BlockModel.fromJS(data["block"]) : <any>undefined;
      }
  }

  toJSON(data?: any) {
      data = typeof data === 'object' ? data : {};
      data["additionalInformation"] = this.additionalInformation ? this.additionalInformation.toJSON() : <any>undefined;
      data["extendedInformation"] = this.extendedInformation ? this.extendedInformation.toJSON() : <any>undefined;
      data["block"] = this.block ? this.block.toJSON() : <any>undefined;
      return data;
  }
}

export interface IBlockResponseModel {
  additionalInformation?: BlockInformationModel | undefined;
  extendedInformation?: ExtendedBlockInformationModel | undefined;
  block?: BlockModel | undefined;
}

export class ExtendedBlockInformationModel implements IExtendedBlockInformationModel {
  size?: number | undefined;
  strippedSize?: number | undefined;
  transactionCount?: number | undefined;
  blockSubsidy?: MoneyModel | undefined;
  blockReward?: MoneyModel | undefined;

  constructor(data?: IExtendedBlockInformationModel) {
      if (data) {
          for (const property in data) {
              if (data.hasOwnProperty(property))
                  (<any>this)[property] = (<any>data)[property];
          }
      }
  }

  static fromJS(data: any): ExtendedBlockInformationModel {
      data = typeof data === 'object' ? data : {};
      const result = new ExtendedBlockInformationModel();
      result.init(data);
      return result;
  }

  init(data?: any) {
      if (data) {
          this.size = data["size"];
          this.strippedSize = data["strippedSize"];
          this.transactionCount = data["transactionCount"];
          this.blockSubsidy = data["blockSubsidy"] ? MoneyModel.fromJS(data["blockSubsidy"]) : <any>undefined;
          this.blockReward = data["blockReward"] ? MoneyModel.fromJS(data["blockReward"]) : <any>undefined;
      }
  }

  toJSON(data?: any) {
      data = typeof data === 'object' ? data : {};
      data["size"] = this.size;
      data["strippedSize"] = this.strippedSize;
      data["transactionCount"] = this.transactionCount;
      data["blockSubsidy"] = this.blockSubsidy ? this.blockSubsidy.toJSON() : <any>undefined;
      data["blockReward"] = this.blockReward ? this.blockReward.toJSON() : <any>undefined;
      return data;
  }
}

export interface IExtendedBlockInformationModel {
  size?: number | undefined;
  strippedSize?: number | undefined;
  transactionCount?: number | undefined;
  blockSubsidy?: MoneyModel | undefined;
  blockReward?: MoneyModel | undefined;
}

export class BlockModel implements IBlockModel {
  blockSize?: number | undefined;
  transactions?: TransactionSummaryModel[] | undefined;
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

export class BlockHeaderResponseModel implements IBlockHeaderResponseModel {
  version?: string | undefined;
  hash?: string | undefined;
  previous?: string | undefined;
  time?: Date | undefined;
  nonce?: number | undefined;
  hashMerkelRoot?: string | undefined;
  bits?: string | undefined;
  difficulty?: number | undefined;

  constructor(data?: IBlockHeaderResponseModel) {
      if (data) {
          for (const property in data) {
              if (data.hasOwnProperty(property))
                  (<any>this)[property] = (<any>data)[property];
          }
      }
  }

  static fromJS(data: any): BlockHeaderResponseModel {
      data = typeof data === 'object' ? data : {};
      const result = new BlockHeaderResponseModel();
      result.init(data);
      return result;
  }

  init(data?: any) {
      if (data) {
          this.version = data["version"];
          this.hash = data["hash"];
          this.previous = data["previous"];
          this.time = data["time"] ? new Date(data["time"].toString()) : <any>undefined;
          this.nonce = data["nonce"];
          this.hashMerkelRoot = data["hashMerkelRoot"];
          this.bits = data["bits"];
          this.difficulty = data["difficulty"];
      }
  }

  toJSON(data?: any) {
      data = typeof data === 'object' ? data : {};
      data["version"] = this.version;
      data["hash"] = this.hash;
      data["previous"] = this.previous;
      data["time"] = this.time ? this.time.toISOString() : <any>undefined;
      data["nonce"] = this.nonce;
      data["hashMerkelRoot"] = this.hashMerkelRoot;
      data["bits"] = this.bits;
      data["difficulty"] = this.difficulty;
      return data;
  }
}

export interface IBlockHeaderResponseModel {
  version?: string | undefined;
  hash?: string | undefined;
  previous?: string | undefined;
  time?: Date | undefined;
  nonce?: number | undefined;
  hashMerkelRoot?: string | undefined;
  bits?: string | undefined;
  difficulty?: number | undefined;
}

export enum BalanceSummaryModelCacheHit {
  _0 = 0,
  _1 = 1,
  _2 = 2,
}

export enum BitcoinAssetIdModelType {
  _0 = 0,
  _1 = 1,
  _2 = 2,
  _3 = 3,
  _4 = 4,
  _5 = 5,
  _6 = 6,
  _7 = 7,
  _8 = 8,
  _9 = 9,
  _10 = 10,
  _11 = 11,
  _12 = 12,
}

export class SwaggerException extends Error {
  message: string;
  status: number;
  response: string;
  headers: { [key: string]: any; };
  result: any;

  constructor(message: string, status: number, response: string, headers: { [key: string]: any; }, result: any) {
      super();

      this.message = message;
      this.status = status;
      this.response = response;
      this.headers = headers;
      this.result = result;
  }

  protected isSwaggerException = true;

  static isSwaggerException(obj: any): obj is SwaggerException {
      return obj.isSwaggerException === true;
  }
}

export function throwException(message: string, status: number, response: string, headers: { [key: string]: any; }, result?: any): Observable<any> {
  if(result !== null && result !== undefined)
      return _observableThrow(result);
  else
      return _observableThrow(new SwaggerException(message, status, response, headers, null));
}

export function blobToText(blob: any): Observable<string> {
  return new Observable<string>((observer: any) => {
      if (!blob) {
          observer.next("");
          observer.complete();
      } else {
          const reader = new FileReader();
          reader.onload = event => {
              observer.next((<any>event.target).result);
              observer.complete();
          };
          reader.readAsText(blob);
      }
  });
}
