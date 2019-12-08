import { Component, OnInit, OnDestroy } from '@angular/core';
import { Observable } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { TransactionStoreFacade } from '../../store/transaction-store.facade';
import * as fromModel from '../../models';

@Component({
    selector: 'app-transaction',
    templateUrl: './transaction.component.html',
    styleUrls: ['./transaction.component.scss']
})
export class TransactionComponent implements OnInit {
    isSelectedTransactionLoaded$: Observable<boolean>;
    selectedTransaction$: Observable<fromModel.Transaction>;
    selectedTransactionError$: Observable<string | Error>;

    constructor(private route: ActivatedRoute, private facade: TransactionStoreFacade) { }

    ngOnInit() {
        this.loadTransactionDetails();
    }

    private loadTransactionDetails() {
        // note: when the component is destroyed, ActivatedRoute instance dies with it, so there is no need to unsubscribe
        // see https://angular.io/guide/router#observable-parammap-and-component-reuse
        this.route.paramMap
            .subscribe((paramMap: any) => {
                if (!!paramMap.params.txId) {
                    let txId = paramMap.params.txId;
                    this.facade.loadTransaction(txId);
                }
            });

        this.isSelectedTransactionLoaded$ = this.facade.isSelectedTransactionLoaded$;
        this.selectedTransactionError$ = this.facade.selectedTransactionError$;
        this.selectedTransaction$ = this.facade.selectedTransaction$;
    }
}
