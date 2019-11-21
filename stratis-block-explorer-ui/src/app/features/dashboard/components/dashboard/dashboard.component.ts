import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { BlockResponseModel } from '../../../block/models/block-response.model';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
   blocks$: Observable<BlockResponseModel[]>;
   blocksLoaded$: Observable<boolean>;

  constructor() { }

  ngOnInit() {
  }

}
