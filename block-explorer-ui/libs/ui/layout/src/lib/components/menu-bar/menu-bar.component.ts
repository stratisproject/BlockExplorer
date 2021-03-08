import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { APP_CONFIG } from '@blockexplorer/shared/models';
import { Log } from '@blockexplorer/shared/utils';

@Component({
  selector: 'blockexplorer-menu-bar',
  templateUrl: './menu-bar.component.html',
  styleUrls: ['./menu-bar.component.css']
})
export class MenuBarComponent implements OnInit {

  @Input() searchText = '';
  @Output() find = new EventEmitter<string>();

  links = [
    { title: "Stratis Mainnet", url: APP_CONFIG.stratMainUrl || "https://stratisinttestbe-mainnet.azurewebsites.net/" },
    { title: "Cirrus Mainnet", url: APP_CONFIG.sidechainMainUrl || "https://stratisinttestbe.azurewebsites.net/" },
    { title: "Stratis Testnet", url: APP_CONFIG.stratTestUrl || "https://stratisinttestbe-testnet.azurewebsites.net/" },
    { title: "Cirrus Testnet", url: APP_CONFIG.sidechainTestUrl || "https://stratisinttestbe-testnet.azurewebsites.net/" }
  ];

  constructor(private log: Log) { }

  ngOnInit(): void {
  }

  get chain() {
    return APP_CONFIG.chain;
  }

  changeBackground() {
    return { 'background-color': this.chain.indexOf('Cirrus') >= 0 ? APP_CONFIG.sidechainColor : APP_CONFIG.stratColor };
  }

  enterPressed() {
    this.log.info("search for", this.searchText);
    this.find.emit(this.searchText);
  }
}
