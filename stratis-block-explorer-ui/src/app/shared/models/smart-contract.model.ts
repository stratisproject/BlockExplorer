import { MoneyModel } from './money.model';

export interface ISmartContractModel {
   hash?: string | undefined;
   gasPrice?: MoneyModel | undefined;
   opCode?: string | undefined;
   methodName?: string | undefined;
   code?: string | undefined;
   isSuccessful?: boolean | undefined;
   isStandardToken?: boolean | undefined;
}

export class SmartContractModel implements ISmartContractModel {
   hash?: string | undefined;
   gasPrice?: MoneyModel | undefined;
   opCode?: string | undefined;
   methodName?: string | undefined;
   code?: string | undefined;
   isSuccessful?: boolean | undefined;
   isStandardToken?: boolean | undefined;
}
