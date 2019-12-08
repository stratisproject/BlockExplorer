import { Component, OnInit, OnDestroy } from '@angular/core';
import { Observable } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { Log } from '@shared/logger.service';
import { SmartContractFacade } from '../../store/smartcontract-facade';
import * as fromModel from '../../models';

@Component({
    selector: 'smartcontract',
    templateUrl: './smartcontract.component.html',
    styleUrls: ['./smartcontract.component.scss']
})
export class SmartContractComponent implements OnInit {
    loaded$: Observable<boolean>;
    smartContract$: Observable<fromModel.SmartContract>;
    error$: Observable<string | Error>;

    constructor(private route: ActivatedRoute, private facade: SmartContractFacade, private log: Log) { }

    ngOnInit() {
        this.loadSmartContract();
    }

    private loadSmartContract() {
        // note: when the component is destroyed, ActivatedRoute instance dies with it, so there is no need to unsubscribe
        // see https://angular.io/guide/router#observable-parammap-and-component-reuse
        this.route.paramMap
            .subscribe((paramMap: any) => {
                if (!!paramMap.params.txId) {
                    let txId = paramMap.params.txId;
                    this.facade.getStandardTokens getSmartContract(txId);
                }
            });

        this.loaded$ = this.facade.smartContract$;
        this.error$ = this.facade.smartContractError$;
        this.smartContract$ = this.facade.smartContract$;
    }
}
