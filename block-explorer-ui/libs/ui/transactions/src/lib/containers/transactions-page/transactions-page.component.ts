import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { TransactionsFacade } from '@blockexplorer/state/transactions-state';
import { Observable } from 'rxjs';
import { TransactionModel, TransactionSummaryModel } from '@blockexplorer/shared/models';

@Component({
  selector: 'blockexplorer-transactions-page',
  templateUrl: './transactions-page.component.html',
  styleUrls: ['./transactions-page.component.css']
})
export class TransactionsPageComponent implements OnInit {
  transactions$: Observable<TransactionSummaryModel[]>;

  constructor(private transactionsFacade: TransactionsFacade) { }

  ngOnInit() {
    // this.transactions$ = this.transactionsFacade.allTransactions$;
    this.transactionsFacade.loadAll();
  }

  selected(value: string){

  }
}
