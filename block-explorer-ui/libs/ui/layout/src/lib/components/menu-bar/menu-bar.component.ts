import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Log } from '@blockexplorer/shared/utils';

@Component({
  selector: 'blockexplorer-menu-bar',
  templateUrl: './menu-bar.component.html',
  styleUrls: ['./menu-bar.component.css']
})
export class MenuBarComponent implements OnInit {

  @Input() searchText = '';
  @Output() find = new EventEmitter<string>();

  constructor(private log: Log) { }

  ngOnInit(): void {
  }

  enterPressed() {
    this.log.info("search for", this.searchText);
    this.find.emit(this.searchText);
  }
}
