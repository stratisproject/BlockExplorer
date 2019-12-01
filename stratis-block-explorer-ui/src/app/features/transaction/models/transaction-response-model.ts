import { TransactionModel } from './transaction-model';
import { BlockInformationModel } from '../../block/models/block-information.model';
import { CoinModel } from './coin-model';
import { MoneyModel } from '@shared/models/money.model';

export interface ITransactionResponseModel {
    transaction?: TransactionModel | undefined;
    transactionId?: string | undefined;
    isCoinbase?: boolean | undefined;
    block?: BlockInformationModel | undefined;
    spentCoins?: CoinModel[] | undefined;
    receivedCoins?: CoinModel[] | undefined;
    firstSeen?: Date | undefined;
    fees?: MoneyModel | undefined;
}

export class TransactionResponseModel implements ITransactionResponseModel {
    transaction?: TransactionModel | undefined;
    transactionId?: string | undefined;
    isCoinbase?: boolean | undefined;
    block?: BlockInformationModel | undefined;
    spentCoins?: CoinModel[] | undefined;
    receivedCoins?: CoinModel[] | undefined;
    firstSeen?: Date | undefined;
    fees?: MoneyModel | undefined;
}
