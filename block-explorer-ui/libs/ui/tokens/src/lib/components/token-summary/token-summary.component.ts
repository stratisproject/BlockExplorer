import { Component, OnInit, Input } from '@angular/core';
import { TokenDetail } from 'libs/state/tokens-state/src/lib/services/token-detail';

@Component({
  selector: 'blockexplorer-token-summary',
  templateUrl: './token-summary.component.html',
  styleUrls: ['./token-summary.component.css']
})
export class TokenSummaryComponent implements OnInit {

  @Input() address: string;
  @Input() token: TokenDetail;

  constructor() { }

  ngOnInit() {
  }

}
