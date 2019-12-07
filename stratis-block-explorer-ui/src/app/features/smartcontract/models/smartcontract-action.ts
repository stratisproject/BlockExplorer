export interface ISmartContractAction {
    address: string;
    txId: string;
    opCode: string;
    gasPrice: number;
    methodName: string;
    isSuccessful: boolean;
    errorMessage: string;

    isStandardToken?: boolean;
    code: string;
    contractName: string;
    contractSymbol: string;
}

export class SmartContractAction implements ISmartContractAction {
    address: string;
    txId: string;
    opCode: string;
    gasPrice: number;
    methodName: string;
    isSuccessful: boolean;
    errorMessage: string;

    isStandardToken?: boolean;
    code: string;
    contractName: string;
    contractSymbol: string;
}
