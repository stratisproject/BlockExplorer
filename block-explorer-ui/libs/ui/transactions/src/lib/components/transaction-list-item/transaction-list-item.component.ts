import { Component, OnInit, Input } from '@angular/core';
import { TransactionModel } from 'gen/nswag';

@Component({
  selector: 'blockexplorer-transaction-list-item',
  templateUrl: './transaction-list-item.component.html',
  styleUrls: ['./transaction-list-item.component.css']
})
export class TransactionListItemComponent implements OnInit {
  @Input() transaction: TransactionModel = new TransactionModel();

  constructor() { }

  ngOnInit() {
  }

}
