import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-busy-indicator',
  templateUrl: './busy-indicator.component.html',
  styleUrls: ['./busy-indicator.component.scss']
})
export class BusyIndicatorComponent implements OnInit {
  @Input() text = 'Loading';
  constructor() { }

  ngOnInit() {
  }

}
