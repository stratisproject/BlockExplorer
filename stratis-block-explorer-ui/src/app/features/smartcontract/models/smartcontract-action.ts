export interface ISmartContractAction {
   address: string;
   addressBase58: string;
   txId: string;
   opCode: string;
   gasPrice: number;
   methodName: string;
   isSuccessful: boolean;
   errorMessage: string;
   logs: string;

   isStandardToken?: boolean;
   code: string;
   contractName: string;
   contractSymbol: string;
}

export class SmartContractAction implements ISmartContractAction {
   address: string;
   addressBase58: string;
   txId: string;
   opCode: string;
   gasPrice: number;
   methodName: string;
   isSuccessful: boolean;
   errorMessage: string;
   logs: string;

   isStandardToken?: boolean;
   code: string;
   contractName: string;
   contractSymbol: string;
}
