import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Log } from '@blockexplorer/shared/utils';
import { APP_CONFIG } from '@blockexplorer/shared/models';

@Component({
  selector: 'blockexplorer-menu-bar',
  templateUrl: './menu-bar.component.html',
  styleUrls: ['./menu-bar.component.css']
})
export class MenuBarComponent implements OnInit {

  @Input() searchText = '';
  @Output() find = new EventEmitter<string>();

  links = [
    { title: "Stratis Mainnet", url: "https://stratisinttestbe-mainnet.azurewebsites.net/" },
    { title: "Stratis Testnet", url: "https://stratisinttestbe-testnet.azurewebsites.net/" },
    { title: "Cirrus Main", url: "https://stratisinttestbe.azurewebsites.net/" }
  ];

  constructor(private log: Log) { }

  ngOnInit(): void {
  }

  get chain() {
    return APP_CONFIG.chain;
  }

  enterPressed() {
    this.log.info("search for", this.searchText);
    this.find.emit(this.searchText);
  }
}
