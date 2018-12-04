import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import * as _ from 'lodash';

export abstract class ApiServiceBase {
    getContractCountAsync(): Observable<number> { return of(0); }
    getContractsAsync(skip: number, take: number): Observable<any> { return of({}); }
}

@Injectable()
export class FakeApiService implements ApiServiceBase {
    private readonly fakeContracts = {
        contracts: [
            { address: 'gfg', name: 'contract', hash: '#627' },
            { address: 'ads', name: 'contract', hash: '#628' },
            { address: 'hsj', name: 'contract', hash: '#629' },
            { address: 'mki', name: 'contract', hash: '#736' },
            { address: 'bhg', name: 'contract', hash: '#908' },
            { address: 'cdr', name: 'contract', hash: '#287' },
            { address: 'mop', name: 'contract', hash: '#976' },
            { address: 'xse', name: 'contract', hash: '#265' },
            { address: 'nhy', name: 'contract', hash: '#987' },
            { address: 'esw', name: 'contract', hash: '#242' },
            { address: 'bhy', name: 'contract', hash: '#525' },
            { address: 'nhu', name: 'contract', hash: '#987' },
            { address: 'swx', name: 'contract', hash: '#234' },
            { address: 'xde', name: 'contract', hash: '#123' },
            { address: 'nju', name: 'contract', hash: '#727' },
            { address: 'nhy', name: 'contract', hash: '#898' },
            { address: 'mju', name: 'contract', hash: '#888' },
            { address: 'bhg', name: 'contract', hash: '#999' },
            { address: 'nhy', name: 'contract', hash: '#000' },
            { address: 'cfe', name: 'contract', hash: '#282' },
            { address: 'mki', name: 'contract', hash: '#672' },
            { address: 'nju', name: 'contract', hash: '#972' },
            { address: 'bhy', name: 'contract', hash: '#732' },
            { address: 'cdh', name: 'contract', hash: '#952' },
            { address: 'nje', name: 'contract', hash: '#902' },
            { address: 'mkq', name: 'contract', hash: '#782' },
            { address: 'bfe', name: 'contract', hash: '#904' },
            { address: 'pol', name: 'contract', hash: '#629' },
            { address: 'nde', name: 'contract', hash: '#451' }
        ]
    }
    getContractCountAsync(): Observable<number> {
        return of(this.fakeContracts.contracts.length);
    }
    getContractsAsync(skip: number, take: number): Observable<any> {
        const contracts = this.fakeContracts.contracts;
        console.assert(skip >= 0 && skip < contracts.length);
        return of(_.slice(contracts, skip, skip+take));
    }
}

@Injectable()
export class ApiService implements ApiServiceBase {
    private fakeService = new FakeApiService();
    getContractCountAsync(): Observable<number> {
        return this.fakeService.getContractCountAsync();
    }
    getContractsAsync(skip: number, take: number): Observable<any> {
        return this.fakeService.getContractsAsync(skip, take);
    }
}
