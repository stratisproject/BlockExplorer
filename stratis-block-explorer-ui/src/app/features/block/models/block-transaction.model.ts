import * as models from '@shared/models/transaction-summary.model';

export interface IBlockTransaction {
    txId: string;
    firstSeen: string;
    totalSpent: number;
    fee: number;
    isCoinbase: boolean;
    isCoinstake: boolean;
    inputs: IBlockTransactionIn[];
    outputs: IBlockTransactionOut[];
}

export interface IPreviousOutput {
    txId?: string;
    n?: number;
}

export interface IBlockTransactionIn {
    address: string;
    amount: number;
    prevOut?: IPreviousOutput;
}

export interface ISpentDetails {
    txId: string;
    n: number;
}

export interface IBlockTransactionOut {
    address?: string;
    amount: number;
    n: number;
    isUnspendable?: string;
    spentDetails?: ISpentDetails;
}



export class BlockTransaction implements IBlockTransaction {
    txId: string;
    firstSeen: string;
    totalSpent: number;
    fee: number;
    isCoinbase: boolean;
    isCoinstake: boolean;
    inputs: IBlockTransactionIn[];
    outputs: IBlockTransactionOut[];

    static fromTransactionModel(transaction: models.TransactionSummaryModel): IBlockTransaction {

        //let inputs = transaction.in.map(out => <IBlockTransactionIn>{
        //    address: out.hash,
        //    amount: out.amount.satoshi,
        //    prevOut: {
        //        n: out.n,
        //        txId: null //missing out.hash
        //    }
        //});

        //let outputs = transaction.out.map(out => <IBlockTransactionOut>{
        //    address: out.hash,
        //    amount: out.amount.satoshi,
        //    n: out.n,
        //    spentDetails: null,
        //    isUnspendable: out.isUnspendable
        //});

        return {
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
