export interface ISmartContractAction {
    txId: string;
    opCode: string;
    successful: boolean;
    gasPrice: number;
    methodName: string;
    contractAddress: string;
    errorMessage: string;
}

export class SmartContractAction implements ISmartContractAction {
    txId: string;
    opCode: string;
    successful: boolean;
    gasPrice: number;
    methodName: string;
    contractAddress: string;
    errorMessage: string;
}
