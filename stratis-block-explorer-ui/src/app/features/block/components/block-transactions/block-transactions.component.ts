import { Component, OnInit, Input, OnChanges, SimpleChanges, SimpleChange } from '@angular/core';
import { TransactionSummaryModel } from '@shared/models/transaction-summary.model';
import { PageEvent } from '@angular/material/paginator';

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

    transactionsOnCurrentPage: TransactionSummaryModel[] = [];
    currentPage = 1;

    // MatPaginator Output
    pageEvent: PageEvent;

    constructor() { }

    ngOnInit() {
        this.loadCurrentPage();
    }

    ngOnChanges(changes: SimpleChanges): void {
        for (let propName in changes) {
            if (propName === 'pageSize') {
                this.currentPage = 1;
                this.loadCurrentPage();
            }
        }
    }

    loadCurrentPage() {
        const pageNumber = this.currentPage - 1;
        this.transactionsOnCurrentPage = this.transactions.slice(pageNumber * this.pageSize, (pageNumber + 1) * this.pageSize);
    }

    get totalPages() {
        if (!this.transactions || this.transactions.length <= this.pageSize)
            return 1;

        const pages = Math.trunc(this.transactions.length / this.pageSize);

        if (this.transactions.length % this.pageSize === 0)
            return pages;

        return pages + 1;
    }

    onPageEvent($event: PageEvent) {
        this.pageSize = $event.pageSize;
        this.currentPage = $event.pageIndex;
        this.loadCurrentPage();
    }
}
