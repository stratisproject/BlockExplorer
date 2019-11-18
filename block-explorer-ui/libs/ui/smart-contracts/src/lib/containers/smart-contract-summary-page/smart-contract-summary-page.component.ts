import { Component, OnInit } from '@angular/core';
import { Observable, ReplaySubject } from 'rxjs';
import { TransactionSummaryModel, SmartContractModel, BlockResponseModel } from '@blockexplorer/shared/models'

@Component({
  selector: 'blockexplorer-smart-contract-summary-page',
  templateUrl: './smart-contract-summary-page.component.html',
  styleUrls: ['./smart-contract-summary-page.component.css']
})
export class SmartContractSummaryPageComponent implements OnInit {
  blockLoaded$: Observable<boolean>;
  transactions: TransactionSummaryModel[] = [];
  destroyed$ = new ReplaySubject<any>();
  height = '';
  smartContract$: Observable<BlockResponseModel>;

  constructor() { }

  ngOnInit() {
  }

}
