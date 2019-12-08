import { Component, OnInit, OnDestroy } from '@angular/core';
import { Observable } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { SmartContractFacade } from '../../store/smartcontract-facade';
import * as fromModel from '../../models';

@Component({
    selector: 'smartcontract-action',
    templateUrl: './smartcontract-action.component.html',
    styleUrls: ['./smartcontract-action.component.scss']
})
export class SmartContractActionComponent implements OnInit {
    loaded$: Observable<boolean>;
    smartContractAction$: Observable<fromModel.SmartContractAction>;
    error$: Observable<string | Error>;

    constructor(private route: ActivatedRoute, private facade: SmartContractFacade) { }

    ngOnInit() {
        this.loadSmartContractAction();
    }

    private loadSmartContractAction() {
        // note: when the component is destroyed, ActivatedRoute instance dies with it, so there is no need to unsubscribe
        // see https://angular.io/guide/router#observable-parammap-and-component-reuse
        this.route.paramMap
            .subscribe((paramMap: any) => {
                if (!!paramMap.params.txId) {
                    let txId = paramMap.params.txId;
                    this.facade.getSmartContractAction(txId);
                }
            });

        this.loaded$ = this.facade.smartContractAction$;
        this.error$ = this.facade.smartContractActionError$;
        this.smartContractAction$ = this.facade.smartContractAction$;
    }
}
