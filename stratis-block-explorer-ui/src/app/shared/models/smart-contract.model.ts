import { MoneyModel } from './money.model';

export interface ISmartContractModel {
   hash?: string | undefined;
   gasPrice?: MoneyModel | undefined;
   opCode?: string | undefined;
   methodName?: string | undefined;
   code?: string | undefined;
   isSuccessful?: boolean | undefined;
   isStandardToken?: boolean | undefined;
}

export class SmartContractModel implements ISmartContractModel {
   hash?: string | undefined;
   gasPrice?: MoneyModel | undefined;
   opCode?: string | undefined;
   methodName?: string | undefined;
   code?: string | undefined;
   isSuccessful?: boolean | undefined;
   isStandardToken?: boolean | undefined;

   constructor(data?: ISmartContractModel) {
      if (data) {
         for (const property in data) {
            if (data.hasOwnProperty(property))
               (<any>this)[property] = (<any>data)[property];
         }
      }
   }

   static fromJS(data: any): SmartContractModel {
      data = typeof data === 'object' ? data : {};
      const result = new SmartContractModel();
      result.init(data);
      return result;
   }

   init(data?: any) {
      if (data) {
         this.gasPrice = data["gasPrice"] ? MoneyModel.fromJS(data["gasPrice"]) : <any>undefined;
         this.hash = data["hash"];
         this.code = data["code"];
         this.opCode = data["opCode"];
         this.methodName = data["methodName"];
         this.isSuccessful = data["isSuccessful"];
         this.isStandardToken = data["isStandardToken"];
      }
   }

   toJSON(data?: any) {
      data = typeof data === 'object' ? data : {};
      data["gasPrice"] = this.gasPrice ? this.gasPrice.toJSON() : <any>undefined;
      data["hash"] = this.hash;
      data["code"] = this.code;
      data["opCode"] = this.opCode;
      data["methodName"] = this.methodName;
      return data;
   }
}
