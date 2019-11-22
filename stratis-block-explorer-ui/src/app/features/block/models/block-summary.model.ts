import { BlockResponseModel } from './block-response.model';
import * as moment from 'moment';

export interface IBlockSummaryModel {
    height: number;
    hash: string;
    confirmations: number;
    size: number;
    age: string;
    time: string;
    transactionCount: number;
}

export class BlockSummaryModel implements IBlockSummaryModel {
    height: number;
    hash: string;
    confirmations: number;
    size: number;
    age: string;
    time: string;
    transactionCount: number;

    constructor(data?: IBlockSummaryModel) {
        if (data) {
            Object.assign(this, data);
        }
    }

    public static fromBlockResponseModel(block?: BlockResponseModel): BlockSummaryModel {
        if (block) {
            let time = 'Unknown';

            if (block && block.block.header.time) {
                time = new Date(1000 * block.block.header.time).toString();
                // TODO: decide which format we want to show date in.
                // time = `${date.toLocaleDateString()} ${date.toLocaleTimeString()}`;
            }

            return {
                age: moment(block.additionalInformation.blockTime).fromNow(),
                confirmations: block.additionalInformation.confirmations || 0,
                hash: block.additionalInformation.blockId || '',
                height: block.additionalInformation.height || 0,
                size: block.extendedInformation.size || 0,
                time: time,
                transactionCount: block.block.transactionIds.length || 0
            };
        }
        else {
            return null;
        }
    }
}
