import { Component, OnInit, Input, OnChanges, SimpleChanges, SimpleChange } from '@angular/core';
import { TransactionSummaryModel } from '@shared/models/transaction-summary.model';
import { PageEvent } from '@angular/material/paginator';
import * as fromModels from '../../models';
import { BlockTransaction } from '../../models';

@Component({
    selector: 'app-block-transactions',
    templateUrl: './block-transactions.component.html',
    styleUrls: ['./block-transactions.component.scss']
})
export class BlockTransactionsComponent implements OnInit, OnChanges {

    @Input() title = 'Transactions';
    @Input() transactions: TransactionSummaryModel[] = null;
    @Input() pageSize = 20;
    @Input() showPaging = true;
    @Input() showTransactionHeader = true;
    @Input() showCount = true;

    transactionsOnCurrentPage: BlockTransaction[] = [];
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
            .map((transaction, index) => fromModels.BlockTransaction.fromTransactionModel(transaction), []);
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
