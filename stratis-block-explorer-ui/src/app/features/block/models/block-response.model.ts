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
}
