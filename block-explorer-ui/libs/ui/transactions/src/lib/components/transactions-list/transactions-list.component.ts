import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
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

  constructor() {}

  ngOnInit() {}

  get noTransactions() {
    return  !this.transactions || this.transactions.length === 0;
  }

  get totalPages() {
    if (!this.transactions || this.transactions.length <= 20) return 1;
    const pages = Math.trunc(this.transactions.length / 20);
    if (this.transactions.length % 20 === 0) return pages;
    return pages + 1;
  }
}
