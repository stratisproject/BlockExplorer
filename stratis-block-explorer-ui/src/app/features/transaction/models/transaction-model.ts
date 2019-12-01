import { MoneyModel } from '@shared/models/money.model';
import { TxInModel } from './tx-in-model';
import { TxOutModel } from './tx-out-model';

export interface ITransactionModel {
    rBF?: boolean | undefined;
    version?: number | undefined;
    time?: number | undefined;
    totalOut?: MoneyModel | undefined;
    inputs?: TxInModel[] | undefined;
    outputs?: TxOutModel[] | undefined;
    isCoinBase?: boolean | undefined;
    isCoinStake?: boolean | undefined;
    hasWitness?: boolean | undefined;
}

export class TransactionModel implements ITransactionModel {
    rBF?: boolean | undefined;
    version?: number | undefined;
    time?: number | undefined;
    totalOut?: MoneyModel | undefined;
    inputs?: TxInModel[] | undefined;
    outputs?: TxOutModel[] | undefined;
    isCoinBase?: boolean | undefined;
    isCoinStake?: boolean | undefined;
    hasWitness?: boolean | undefined;
}
