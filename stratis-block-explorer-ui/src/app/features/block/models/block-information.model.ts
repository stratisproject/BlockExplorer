import { BlockHeaderModel } from './block-header.model';

export interface IBlockInformationModel {
    blockId?: string | undefined;
    blockHeader?: BlockHeaderModel | undefined;
    height?: number | undefined;
    confirmations?: number | undefined;
    medianTimePast?: string | undefined;
    blockTime?: string | undefined;
}

export class BlockInformationModel implements IBlockInformationModel {
    blockId?: string | undefined;
    blockHeader?: BlockHeaderModel | undefined;
    height?: number | undefined;
    confirmations?: number | undefined;
    medianTimePast?: string | undefined;
    blockTime?: string | undefined;
}
