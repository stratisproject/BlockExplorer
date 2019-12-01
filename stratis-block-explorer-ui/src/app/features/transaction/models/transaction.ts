import { TransactionSummaryModel, ITransactionIn, ITransactionOut } from '../../transaction/models/transaction-summary.model';

export interface ITransaction {
    txId: string;
    blockHeight?: number;
    firstSeen: string;
    totalSpent: number;
    fee: number;
    isCoinbase: boolean;
    isCoinstake: boolean;
    inputs: ITransactionIn[];
    outputs: ITransactionOut[];
}

export class Transaction implements ITransaction {
    txId: string;
    blockHeight?: number;
    firstSeen: string;
    totalSpent: number;
    fee: number;
    isCoinbase: boolean;
    isCoinstake: boolean;
    inputs: ITransactionIn[];
    outputs: ITransactionOut[];

    static fromTransactionSummaryModel(transaction: TransactionSummaryModel): ITransaction {
        return {
            blockHeight: transaction.height,
            fee: transaction.fee.satoshi,
            isCoinbase: transaction.isCoinbase,
            isCoinstake: transaction.isCoinstake,
            firstSeen: transaction.firstSeen.toLocaleString(),
            txId: transaction.hash,
            totalSpent: transaction.out.reduce((accumulator, item) => accumulator + item.amount, 0),
            inputs: transaction.in,
            outputs: transaction.out
        };
    }
}
