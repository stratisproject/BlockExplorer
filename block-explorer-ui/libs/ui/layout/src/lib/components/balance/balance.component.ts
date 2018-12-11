import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'blockexplorer-balance',
  templateUrl: './balance.component.html',
  styleUrls: ['./balance.component.css']
})
export class BalanceComponent implements OnInit {

  @Input() balance = 0;
  @Input() symbol = 'STRAT';
  @Input() decimalPlaces = 4;

  constructor() { }

  ngOnInit() {
  }

  get whole() {
    return this.balance.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,').split('.')[0];
  }

  get decimal() {
    if (this.balance % 1 === 0) return ".0000";
    const fraction = parseInt(((this.balance % 1) * Math.pow(10, this.decimalPlaces)).toString(), 10);
    return `.${fraction}`;
  }

}
