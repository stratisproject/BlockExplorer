import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { StatsModel } from '@blockexplorer/shared/models';

@Component({
  selector: 'blockexplorer-stats',
  templateUrl: './stats.component.html',
  styleUrls: ['./stats.component.css']
})
export class StatsComponent implements OnInit {

  @Input() stats: StatsModel = undefined;
  @Input() loading = false;

  records = 10;
  constructor() { }

  ngOnInit() {
  }
}
