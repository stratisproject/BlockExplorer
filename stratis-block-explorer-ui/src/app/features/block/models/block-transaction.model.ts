export interface IBlockTransaction {
    txId: string;
    time: string;
    totalSpent: number;
    fee: number;
    inputs: IBlockTransactionIn[];
    outputs: IBlockTransactionOut[];
}

export interface IPreviousOutput {
    txId?: string;
    n?: number;
}

export interface IBlockTransactionIn {
    address: string;
    amount?: number;
    prevOut?: IPreviousOutput;
}

export interface ISpentDetails {
    txId: string;
    n: number;
}

export interface IBlockTransactionOut {
    address?: string;
    amount?: number;
    spentDetails?: ISpentDetails;
}

export class BlockTransaction implements IBlockTransaction {
    txId: string;
    time: string;
    totalSpent: number;
    fee: number;
    inputs: IBlockTransactionIn[];
    outputs: IBlockTransactionOut[];
}
