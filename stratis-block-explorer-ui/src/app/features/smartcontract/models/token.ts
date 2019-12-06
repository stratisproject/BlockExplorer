export interface IToken {
    address: string;
    name: string;
    symbol: string;
    code: string;
}

export class Token implements IToken {
    address: string;
    name: string;
    symbol: string;
    code: string;
}
