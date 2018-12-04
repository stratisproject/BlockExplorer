import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'blockexplorer-menu-bar',
  templateUrl: './menu-bar.component.html',
  styleUrls: ['./menu-bar.component.css']
})
export class MenuBarComponent implements OnInit {

  @Input() searchText = '';

  constructor() { }

  ngOnInit(): void {

  }

}
