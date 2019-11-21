import { ExtendedBlockInformationModel } from './extended-block-information.model';
import { BlockInformationModel } from './block-information.model';
import { BlockModel } from './block.model';

export interface IBlockResponseModel {
   additionalInformation?: BlockInformationModel | undefined;
   extendedInformation?: ExtendedBlockInformationModel | undefined;
   block?: BlockModel | undefined;
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
