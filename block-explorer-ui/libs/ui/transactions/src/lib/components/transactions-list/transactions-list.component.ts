import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { TransactionModel } from '@blockexplorer/shared/models';

@Component({
  selector: 'blockexplorer-transactions-list',
  templateUrl: './transactions-list.component.html',
  styleUrls: ['./transactions-list.component.css']
})
export class TransactionsListComponent implements OnInit {
  @Input() transactions: TransactionModel[] = [];
  @Output() selected = new EventEmitter<string>();

  constructor() {}

  ngOnInit() {}

  get noTransactions() {
    return  !this.transactions || this.transactions.length === 0;
  }
}
