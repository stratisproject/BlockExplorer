import { Component, OnInit, Input } from '@angular/core';
import { BlockResponseModel } from '../../models/block-response.model';
import { ChangeDetectionStrategy } from '@angular/core';

@Component({
    selector: 'app-block-summary',
    templateUrl: './block-summary.component.html',
    styleUrls: ['./block-summary.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class BlockSummaryComponent implements OnInit {
    @Input() block: BlockResponseModel = null;

    constructor() { }

    ngOnInit() {
    }

    getTotalAmount(block: BlockResponseModel) {
        return block.block.transactions.reduce((accumulator, currentValue) => accumulator + currentValue.amount.satoshi, 0)
    }

    getTotalFee(block: BlockResponseModel) {
        return block.block.transactions.reduce((accumulator, currentValue) => accumulator + currentValue.fee.satoshi, 0)
    }
}
