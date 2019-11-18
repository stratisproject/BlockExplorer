import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AppConfigService } from '@core/services/app-config.service';
import { Log } from '@shared/logger.service';

@Component({
  selector: 'app-menu-bar',
  templateUrl: './menu-bar.component.html',
  styleUrls: ['./menu-bar.component.scss']
})
export class MenuBarComponent implements OnInit {

  @Input() searchText = '';
  @Output() find = new EventEmitter<string>();

  links = [
    { title: "Stratis Mainnet", url: this.appConfig.Config.stratMainUrl || "https://stratisinttestbe-mainnet.azurewebsites.net/" },
    { title: "Cirrus Mainnet", url: this.appConfig.Config.sidechainMainUrl || "https://stratisinttestbe.azurewebsites.net/" },
    { title: "Stratis Testnet", url: this.appConfig.Config.stratTestUrl || "https://stratisinttestbe-testnet.azurewebsites.net/" },
    { title: "Cirrus Testnet", url: this.appConfig.Config.sidechainTestUrl || "https://stratisinttestbe-testnet.azurewebsites.net/" }
  ];

  constructor(private appConfig: AppConfigService, private log: Log) { }

  ngOnInit(): void {
  }

  get chain() {
    return this.appConfig.Config.chain;
  }

  changeBackground() {
    return { 'background-color': this.chain.indexOf('Cirrus') >= 0 ? this.appConfig.Config.sidechainColor : this.appConfig.Config.stratColor };
  }

  enterPressed() {
    this.log.info("search for", this.searchText);
    this.find.emit(this.searchText);
  }
}
