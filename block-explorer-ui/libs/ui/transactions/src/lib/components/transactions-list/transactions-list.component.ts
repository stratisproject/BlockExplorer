import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { TransactionSummaryModel } from '@blockexplorer/shared/models';

@Component({
  selector: 'blockexplorer-transactions-list',
  templateUrl: './transactions-list.component.html',
  styleUrls: ['./transactions-list.component.css']
})
export class TransactionsListComponent implements OnInit {
  @Input() transactions: TransactionSummaryModel[] = [];
  @Input() title = 'Transactions';
  @Input() showPaging = true;
  @Input() showTransactionHeader = true;
  @Input() showCount = true;
  @Output() selected = new EventEmitter<string>();

  transactionsOnCurrentPage: TransactionSummaryModel[] = [];

  currentPage = 1;

  constructor() { }

  ngOnInit() {
    this.page();
  }

  get noTransactions() {
    return !this.transactions || this.transactions.length === 0;
  }

  get totalPages() {
    if (!this.transactions || this.transactions.length <= 20) return 1;
    const pages = Math.trunc(this.transactions.length / 20);
    if (this.transactions.length % 20 === 0) return pages;
    return pages + 1;
  }

  first() {
    this.currentPage = 1;
    this.page();
  }

  last() {
    this.currentPage = this.transactions.length || 1;
    this.page();
  }

  next() {
    if (this.currentPage === this.transactions.length) { return; }
    this.currentPage++;
    this.page();
  }

  previous() {
    if (this.currentPage <= 1) { return; }
    this.currentPage--;
    this.page();
  }

  page() {
    const pageNumber = this.currentPage - 1;
    const pageSize = 20;
    this.transactionsOnCurrentPage = this.transactions.slice(pageNumber * pageSize, (pageNumber + 1) * pageSize);
  }
}
