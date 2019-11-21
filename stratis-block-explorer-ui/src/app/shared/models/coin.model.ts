import { OutPointModel } from './outpoint.model';
import { TxOutModel } from './txout.model';

export interface ICoinModel {
  amount?: any | undefined;
  outpoint?: OutPointModel | undefined;
  txOut?: TxOutModel | undefined;
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
