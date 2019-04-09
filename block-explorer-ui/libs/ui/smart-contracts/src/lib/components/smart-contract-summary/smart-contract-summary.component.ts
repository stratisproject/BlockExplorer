import { Component, OnInit, Input } from '@angular/core';
import { SmartContractModel } from '@blockexplorer/shared/models';
import 'prismjs/components/prism-csharp';

@Component({
  selector: 'blockexplorer-smart-contract-summary',
  templateUrl: './smart-contract-summary.component.html',
  styleUrls: ['./smart-contract-summary.component.css']
})
export class SmartContractSummaryComponent implements OnInit {

  @Input() smartContract: SmartContractModel = null;

  constructor() { }

  ngOnInit() {
  }

  public get opCode() {
    return !!this.smartContract ? this.smartContract.opCode : '';
  }

  public get methodName() {
    return !!this.smartContract ? this.smartContract.methodName : '';
  }

  public get code() {
    return !!this.smartContract ? this.smartContract.code : '';
  }

  public get hash() {
    return !!this.smartContract ? this.smartContract.hash : '';
  }

  public get gasPrice() {
    return !!this.smartContract && !!this.smartContract.gasPrice
            ? this.smartContract.gasPrice.satoshi || 0
            : 0;
  }
}
