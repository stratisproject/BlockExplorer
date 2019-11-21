export interface IMoneyModel {
   satoshi?: number | undefined;
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
