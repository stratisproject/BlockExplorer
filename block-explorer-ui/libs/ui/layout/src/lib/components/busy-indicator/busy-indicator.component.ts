import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'blockexplorer-busy-indicator',
  templateUrl: './busy-indicator.component.html',
  styleUrls: ['./busy-indicator.component.css']
})
export class BusyIndicatorComponent implements OnInit {
  @Input() text = 'Loading';
  constructor() { }

  ngOnInit() {
  }

}
