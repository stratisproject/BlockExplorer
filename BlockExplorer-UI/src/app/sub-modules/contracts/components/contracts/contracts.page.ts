import { ApiServiceBase } from "src/app/services/api.service";
import { of, Subscription, Observable } from "rxjs";
import { map } from 'rxjs/operators';
import { Injectable } from "@angular/core";

export class Contract {
    constructor(public address: string, public name: string, public hash: string) { }
}

export class ContractsPage {
    private contracts: Contract[];
    constructor(readonly apiService: ApiServiceBase, public pageIndex: number, public takeCount: number = 8) { }

    getContractsAsync(): Observable<Contract[]> {
        if (this.contracts) {
            return of(this.contracts);
        }
        const skip = this.pageIndex * this.takeCount;
        return this.apiService.getContractsAsync(skip, this.takeCount)
                              .pipe(map(x => this.processContracts(x)));
    }

    private processContracts(response: any) {
        const contracts = [];
        for (const contract of response) {
            contracts.push(new Contract(contract.address, contract.name, contract.hash))
        }
        this.contracts = contracts.slice();
        return this.contracts;
    }
}

@Injectable({
    providedIn: 'root'
})
export class ContractsPageFactory {
    constructor(readonly apiService: ApiServiceBase) { }
    New = (pageIndex: number, takeCount: number = 8): ContractsPage =>
        new ContractsPage(this.apiService, pageIndex, takeCount);
}
