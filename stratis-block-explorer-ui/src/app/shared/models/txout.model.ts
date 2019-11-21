import { ScriptModel } from './script.model';
import { MoneyModel } from './money.model';

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
