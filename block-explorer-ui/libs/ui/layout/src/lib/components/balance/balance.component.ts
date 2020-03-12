import { APP_CONFIG } from '@blockexplorer/shared/models';
import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'blockexplorer-balance',
  templateUrl: './balance.component.html',
  styleUrls: ['./balance.component.css']
})
export class BalanceComponent implements OnInit {

  _symbol: string;

  @Input() balance = 0;
  @Input() decimalPlaces = 4;
  
  @Input() 
  set symbol(value: string) {
    this._symbol = value;
  }


  constructor() { }

  ngOnInit() {
  }

  get value() {
    return this.balance / Math.pow(10, 8);
  }

  get symbol() {
    return this._symbol || APP_CONFIG.symbol;
  }

  get whole() {
    return this.value.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,').split('.')[0];
  }

  get decimal() {
    if (this.value % 1 === 0) return ".0000";
    const fraction = parseInt(((this.value % 1) * Math.pow(10, this.decimalPlaces)).toString(), 10);
    return `.${fraction}`;
  }

}
