import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { TokenTransactionResponse } from 'libs/state/tokens-state/src/lib/services/token-transaction-response';
import * as moment from 'moment';
import { TokenDetail } from 'libs/state/tokens-state/src/lib/services/token-detail';

@Component({
  selector: 'blockexplorer-token-transactions-table',
  templateUrl: './token-transactions-table.component.html',
  styleUrls: ['./token-transactions-table.component.css']
})
export class TokenTransactionsTableComponent implements OnInit {
  @Input() transactions: TokenTransactionResponse[] = [];
  @Input() title = 'Token Transactions';
  @Input() symbol = ' ';
  @Input() tokenAddress: string;
  @Input() token: TokenDetail;
  @Output() selected = new EventEmitter<string>();
  
  constructor() { }

  ngOnInit() {
  }

  public age(transaction: TokenTransactionResponse) {
    if (!transaction || !transaction.time) return 'Unknown';
    return moment(transaction.time).fromNow();
  }

}
