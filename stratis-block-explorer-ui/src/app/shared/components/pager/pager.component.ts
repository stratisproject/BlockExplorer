import { Component, OnInit, EventEmitter, Output, Input } from '@angular/core';

@Component({
  selector: 'blockexplorer-pager',
  templateUrl: './pager.component.html',
  styleUrls: ['./pager.component.css']
})
export class PagerComponent implements OnInit {

  @Input() currentPage = 1;
  @Input() totalPages = 1;

  @Output() first = new EventEmitter();
  @Output() last = new EventEmitter();
  @Output() next = new EventEmitter();
  @Output() previous = new EventEmitter();

  constructor() { }

  ngOnInit() {
  }

}
