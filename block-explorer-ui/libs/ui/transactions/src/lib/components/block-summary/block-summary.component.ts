import { Component, Input, OnInit } from '@angular/core';
import { BlockResponseModel } from '@blockexplorer/shared/models';

@Component({
    selector: 'blockexplorer-block-summary',
    templateUrl: './block-summary.component.html',
    styleUrls: ['./block-summary.component.css']
})
export class BlockSummaryComponent implements OnInit {

    @Input() block: BlockResponseModel = null;

    constructor() { }

    ngOnInit() {
    }

    public get size() {
        return this.block.extendedInformation.size || 0;
    }

    public get transactionCount() {
        return this.block.extendedInformation.transactionCount || 0;
    }

    public get blockReward() {
        return !!this.block.extendedInformation && !!this.block.extendedInformation.blockReward
            ? this.block.extendedInformation.blockReward.satoshi || 0
            : 0;
    }

    public get blockSubsidy() {
        return !!this.block.extendedInformation && !!this.block.extendedInformation.blockSubsidy
            ? this.block.extendedInformation.blockSubsidy.satoshi || 0
            : 0;
    }

    public get totalBlockReward() {
        return this.blockReward + this.blockSubsidy;
    }

    public get confirmations() {
        return this.block.additionalInformation.confirmations || 0;
    }

    public get height() {
        return this.block.additionalInformation.height || 0;
    }

    public get hash() {
        return this.block.additionalInformation.blockId || '';
    }

    public get time() {
        if (!this.block || !this.block.block.header.time) return 'Unknown';
        const date = new Date(1000 * this.block.block.header.time);

        return date.toLocaleString();
        // TODO: decide which format we want to show date in.
        // return `${date.toLocaleDateString()} ${date.toLocaleTimeString()}`;
    }
}
