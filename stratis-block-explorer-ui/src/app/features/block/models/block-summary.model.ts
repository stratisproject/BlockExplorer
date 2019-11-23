import { BlockResponseModel } from './block-response.model';
import * as moment from 'moment';

export interface IBlockSummaryModel {
    height: number;
    hash: string;
    confirmations: number;
    size: number;
    age: string;
    time: string;
    transactions: number;
}

export class BlockSummaryModel implements IBlockSummaryModel {
    height: number;
    hash: string;
    confirmations: number;
    size: number;
    age: string;
    time: string;
    transactions: number;

    constructor(data?: IBlockSummaryModel) {
        if (data) {
            Object.assign(this, data);
        }
    }

    public static fromBlockResponseModel(block?: BlockResponseModel): BlockSummaryModel {
        if (block) {
            return {
                height: block.additionalInformation.height || 0,
                time: block.additionalInformation.blockTime.toLocaleTimeString(),
                age: moment(block.additionalInformation.blockTime).fromNow(),
                confirmations: block.additionalInformation.confirmations || 0,
                hash: block.additionalInformation.blockId || '',
                size: block.block.blockSize || 0,//block.extendedInformation.size || 0,
                transactions: block.block.transactionIds.length || 0
            };
        }
        else {
            return null;
        }
    }
}
