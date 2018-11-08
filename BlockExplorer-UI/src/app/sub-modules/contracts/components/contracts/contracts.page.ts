import { ApiServiceBase } from "src/app/services/api.service";

export class Contract {
    constructor(public address: string, public name: string, public hash: string) {}
}

export class ContractsPage {
    constructor(readonly apiService: ApiServiceBase, public pageNumber: number) { }

    
}
