import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { LineItemModel, TransactionSummaryModel } from '@blockexplorer/shared/models';
import * as moment from 'moment';

@Component({
  selector: 'blockexplorer-transactions-table',
  templateUrl: './transactions-table.component.html',
  styleUrls: ['./transactions-table.component.css']
})
export class TransactionsTableComponent implements OnInit {
  @Input() transactions: TransactionSummaryModel[] = [];
  @Input() title = 'Transactions';
  @Input() showPaging = true;
  @Input() showTransactionHeader = true;
  @Input() showCount = true;
  @Output() selected = new EventEmitter<string>();

  constructor() { }

  ngOnInit() { }

  get noTransactions() {
    return !this.transactions || this.transactions.length === 0;
  }

  get totalPages() {
    if (!this.transactions || this.transactions.length <= 20) return 1;
    const pages = Math.trunc(this.transactions.length / 20);
    if (this.transactions.length % 20 === 0) return pages;
    return pages + 1;
  }

  public age(transaction: TransactionSummaryModel) {
    if (!transaction || !transaction.firstSeen) return 'Unknown';
    return moment(transaction.firstSeen).fromNow();
  }

  public transactionTime(transaction: TransactionSummaryModel) {
    if (!transaction || !transaction.firstSeen) return 'Unknown';
    return transaction.firstSeen.toLocaleString();
  }

  public transactionType(transaction: TransactionSummaryModel) {
    if (!transaction) return 'Unknown';
    if (transaction.isCoinbase) return 'Coinbase';
    if (transaction.isCoinstake) return 'Coinstake';
    if (transaction.height === -50) return 'Smart Contract'
    return 'Transaction';
  }

  public firstItem(items: LineItemModel[]) {
    if ((items || []).length === 0) return '';
    const nonEmptyItems = items.filter(i => !!i.hash);
    return nonEmptyItems.length === 0 ? '' : nonEmptyItems[0].hash;
  }

  public itemsTooltip(items: LineItemModel[]) {
    if ((items || []).length === 0) return '';
    return items.filter(i => !!i.hash).map(i => i.hash).join(', ');
  }
}
