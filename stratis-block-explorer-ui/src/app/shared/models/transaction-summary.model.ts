import { MoneyModel } from './money.model';
import { LineItemModel } from './line-item.model';
import { SmartContractModel } from './smart-contract.model';

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
   firstSeen?: Date | undefined;
   smartContract?: SmartContractModel | undefined;

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
         this.smartContract = data["smartContract"] ? SmartContractModel.fromJS(data["smartContract"]) : <any>undefined;
         this.firstSeen = data["firstSeen"] ? new Date(data["firstSeen"].toString()) : <any>undefined;
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

      data["smartContract"] = this.smartContract ? this.smartContract.toJSON() : <any>undefined;
      data["firstSeen"] = this.firstSeen ? this.firstSeen.toISOString() : <any>undefined;
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
   firstSeen?: Date | undefined;
   smartContract?: SmartContractModel | undefined;
}
