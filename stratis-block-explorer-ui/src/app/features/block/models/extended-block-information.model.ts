import { MoneyModel } from '@shared/models/money.model';

export interface IExtendedBlockInformationModel {
   size?: number | undefined;
   strippedSize?: number | undefined;
   transactionCount?: number | undefined;
   blockSubsidy?: MoneyModel | undefined;
   blockReward?: MoneyModel | undefined;
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
