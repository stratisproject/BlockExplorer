import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { BlockResponseModel, TransactionSummaryModel } from '@blockexplorer/shared/models';
import * as moment from 'moment';

@Component({
  selector: 'blockexplorer-last-smart-contracts',
  templateUrl: './last-smart-contracts.component.html',
  styleUrls: ['./last-smart-contracts.component.css']
})
export class LastSmartContractsComponent implements OnInit {

  @Input() smartContracts: TransactionSummaryModel[] = [];
  @Input() loading = false;
  @Output() loadMore = new EventEmitter<number>();

  records = 10;
  constructor() { }

  ngOnInit() {
  }

  public getNext10() {
    this.records = this.records + 10;
    console.log(this.records);
    this.loadMore.emit(this.records);
  }
}
