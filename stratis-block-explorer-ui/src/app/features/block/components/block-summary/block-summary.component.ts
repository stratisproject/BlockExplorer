import { Component, OnInit, Input } from '@angular/core';
import { BlockResponseModel } from '../../models/block-response.model';

@Component({
    selector: 'app-block-summary',
    templateUrl: './block-summary.component.html',
    styleUrls: ['./block-summary.component.scss']
})
export class BlockSummaryComponent implements OnInit {
    @Input() block: BlockResponseModel = null;

    constructor() { }

    ngOnInit() {
    }

}
