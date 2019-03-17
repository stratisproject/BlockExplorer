import { Component, OnInit, Input } from '@angular/core';
import { BlockResponseModel } from '@blockexplorer/shared/models';
import * as moment from 'moment';

@Component({
  selector: 'blockexplorer-last-blocks',
  templateUrl: './last-blocks.component.html',
  styleUrls: ['./last-blocks.component.css']
})
export class LastBlocksComponent implements OnInit {

  @Input() blocks: BlockResponseModel[] = [];

  constructor() { }

  ngOnInit() {
  }

  public getSize(block: BlockResponseModel) {
    return block.extendedInformation.size || 0;
  }

  public getTransactionCount(block: BlockResponseModel) {
    return block.extendedInformation.transactionCount || 0;
  }

  public getBlockReward(block: BlockResponseModel) {
    return !!block.extendedInformation && !!block.extendedInformation.blockReward
            ? block.extendedInformation.blockReward.satoshi || 0
            : 0;
  }

  public getConfirmations(block: BlockResponseModel) {
    return block.additionalInformation.confirmations || 0;
  }

  public getHeight(block: BlockResponseModel) {
    return block.additionalInformation.height || 0;
  }

  public getHash(block: BlockResponseModel) {
    return block.additionalInformation.blockId || '';
  }

  public getAge(block: BlockResponseModel) {
    return moment(block.additionalInformation.blockTime).fromNow();
  }

  public getTime(block: BlockResponseModel) {
    if (!block || !block.block.header.time) return 'Unknown';
    const date = new Date(1000 * block.block.header.time);

    return date.toString();
    // TODO: decide which format we want to show date in.
    // return `${date.toLocaleDateString()} ${date.toLocaleTimeString()}`;
  }
}
