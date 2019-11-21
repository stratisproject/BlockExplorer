import { BlockHeaderModel } from './block-header.model';

export interface IBlockInformationModel {
  blockId?: string | undefined;
  blockHeader?: BlockHeaderModel | undefined;
  height?: number | undefined;
  confirmations?: number | undefined;
  medianTimePast?: Date | undefined;
  blockTime?: Date | undefined;
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
