import { Component, OnInit, Input, OnChanges, SimpleChanges } from '@angular/core';
import { PageEvent } from '@angular/material/paginator';
import * as fromModels from '../../models';
import { Transaction, TransactionSummaryModel } from '../../models';

@Component({
    selector: 'transaction-list',
    templateUrl: './transaction-list.component.html',
    styleUrls: ['./transaction-list.component.scss']
})
export class TransactionListComponent implements OnInit, OnChanges {

    @Input() title = 'Transactions';
    @Input() transactions: TransactionSummaryModel[] = null;
    @Input() pageSize = 20;
    @Input() showPaging = true;
    @Input() showTransactionHeader = true;
    @Input() showCount = true;

    transactionsOnCurrentPage: Transaction[] = [];
    currentPageIndex = 0;

    // MatPaginator Output
    pageEvent: PageEvent;

    constructor() { }

    ngOnInit() {
        this.loadCurrentPage();
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (changes.transactions) {
            this.loadCurrentPage();
        }
    }

    loadCurrentPage() {

        this.transactionsOnCurrentPage = this.transactions
            .slice(this.currentPageIndex * this.pageSize, (this.currentPageIndex + 1) * this.pageSize)
            .map((transaction, index) => fromModels.Transaction.fromTransactionSummaryModel(transaction), []);
    }

    get totalPages() {
        if (!this.transactions || this.transactions.length <= this.pageSize)
            return 1;

        const pages = Math.trunc(this.transactions.length / this.pageSize);

        if (this.transactions.length % this.pageSize === 0)
            return pages;

        return pages + 1;
    }

    public onPageEvent(event: PageEvent): PageEvent {
        this.pageSize = event.pageSize;
        this.currentPageIndex = event.pageIndex;
        this.loadCurrentPage();

        return event;
    }
}
