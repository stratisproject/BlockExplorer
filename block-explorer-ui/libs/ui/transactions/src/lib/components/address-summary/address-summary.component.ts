import { Component, OnInit, Input } from '@angular/core';
import { BalanceSummaryModel } from 'gen/nswag';

@Component({
  selector: 'blockexplorer-address-summary',
  templateUrl: './address-summary.component.html',
  styleUrls: ['./address-summary.component.css']
})
export class AddressSummaryComponent implements OnInit {

  @Input() address: BalanceSummaryModel = null;

  constructor() { }

  ngOnInit() {
  }
}
