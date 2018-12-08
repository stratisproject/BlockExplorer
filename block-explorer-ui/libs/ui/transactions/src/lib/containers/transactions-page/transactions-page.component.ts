import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { TransactionsState, TransactionsFacade } from '@blockexplorer/state/transactions-state';
import { Observable } from 'rxjs';
import { TransactionModel } from '@blockexplorer/shared/models';

@Component({
  selector: 'blockexplorer-transactions-page',
  templateUrl: './transactions-page.component.html',
  styleUrls: ['./transactions-page.component.css']
})
export class TransactionsPageComponent implements OnInit {
  transactions$: Observable<TransactionModel[]>;

  constructor(private transactionsFacade: TransactionsFacade) { }

  ngOnInit() {
    this.transactions$ = this.transactionsFacade.allTransactions$;
    this.transactionsFacade.loadAll();
  }

  selected(value: string){

  }
}
