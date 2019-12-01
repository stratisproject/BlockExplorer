import { MoneyModel } from '@shared/models/money.model';

export class TransactionSummaryModel implements ITransactionSummaryModel {
    hash?: string | undefined;
    isCoinbase?: boolean | undefined;
    isCoinstake?: boolean | undefined;
    isSmartContract: boolean;
    amount?: MoneyModel | undefined;
    fee?: MoneyModel | undefined;
    height?: number | undefined;
    time?: number | undefined;
    spent?: boolean | undefined;
    in?: ITransactionIn[] | undefined;
    out?: ITransactionOut[] | undefined;
    confirmations?: number | undefined;
    firstSeen?: Date | undefined;
}

export interface ITransactionSummaryModel {
    hash?: string | undefined;
    isCoinbase?: boolean | undefined;
    isCoinstake?: boolean | undefined;
    isSmartContract: boolean;
    amount?: MoneyModel | undefined;
    fee?: MoneyModel | undefined;
    height?: number | undefined;
    time?: number | undefined;
    spent?: boolean | undefined;
    in?: ITransactionIn[] | undefined;
    out?: ITransactionOut[] | undefined;
    confirmations?: number | undefined;
    firstSeen?: Date | undefined;
}


export interface IPreviousOutput {
    txId?: string;
    n?: number;
}

export interface ITransactionIn {
    address: string;
    amount: number;
    prevOut?: IPreviousOutput;
}

export interface ISpentDetails {
    txId: string;
    n: number;
}

export interface ITransactionOut {
    address?: string;
    amount: number;
    n: number;
    isUnspendable?: boolean;
    isSmartContract?: boolean;
    spentDetails?: ISpentDetails;
}
