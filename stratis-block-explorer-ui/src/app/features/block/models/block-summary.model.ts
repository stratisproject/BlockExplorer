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

            let size = 0;
            let transactions = 0;
            if (block.block) {
                size = block.block.blockSize;
                transactions = block.block.transactionIds.length;
            }

            return {
                height: block.additionalInformation.height || 0,
                time: moment(block.additionalInformation.blockTime).toLocaleString(), //block.additionalInformation.blockTime.toLocaleTimeString(),
                age: moment(block.additionalInformation.blockTime).fromNow(),
                confirmations: block.additionalInformation.confirmations || 0,
                hash: block.additionalInformation.blockId || '',
                size: size,
                transactions: transactions
            };
        }
        else {
            return null;
        }
    }
}
