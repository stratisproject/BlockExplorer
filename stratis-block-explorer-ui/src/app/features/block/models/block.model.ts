import { BlockHeaderModel } from './block-header.model';
import { TransactionSummaryModel } from '../../transaction/models/transaction-summary.model';

export class BlockModel implements IBlockModel {
  blockSize?: number | undefined;
  transactions?: TransactionSummaryModel[] | undefined;
  transactionIds?: string[] | undefined;
  headerOnly?: boolean | undefined;
  header?: BlockHeaderModel | undefined;
}

export interface IBlockModel {
  blockSize?: number | undefined;
  transactions?: TransactionSummaryModel[] | undefined;
  headerOnly?: boolean | undefined;
  header?: BlockHeaderModel | undefined;
}
