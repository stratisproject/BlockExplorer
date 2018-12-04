import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'blockexplorer-menu-bar',
  templateUrl: './menu-bar.component.html',
  styleUrls: ['./menu-bar.component.css']
})
export class MenuBarComponent implements OnInit {

  @Input() searchText = '';
  @Output() search = new EventEmitter<string>();

  constructor() { }

  ngOnInit(): void {
  }

  enterPressed() {
    console.log("search for", this.searchText);
    this.search.emit(this.searchText);
  }
}
