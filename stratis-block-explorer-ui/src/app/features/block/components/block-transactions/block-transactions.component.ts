import { Component, OnInit, Input } from '@angular/core';
import { TransactionSummaryModel } from '@shared/models/transaction-summary.model';

@Component({
    selector: 'app-block-transactions',
    templateUrl: './block-transactions.component.html',
    styleUrls: ['./block-transactions.component.scss']
})
export class BlockTransactionsComponent implements OnInit {
    @Input() transactions: TransactionSummaryModel[] = null;

    constructor() { }

    ngOnInit() {
    }

}
