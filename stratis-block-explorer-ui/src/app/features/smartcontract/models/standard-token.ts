export interface IStandardToken {
    address: string;
    name: string;
    symbol: string;
    code: string;
}

export class StandardToken implements IStandardToken {
    address: string;
    name: string;
    symbol: string;
    code: string;
}
