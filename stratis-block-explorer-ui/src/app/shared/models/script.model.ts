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
