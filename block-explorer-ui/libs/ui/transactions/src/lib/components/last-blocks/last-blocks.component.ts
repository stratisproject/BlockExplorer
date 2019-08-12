import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { BlockResponseModel } from '@blockexplorer/shared/models';
import * as moment from 'moment';

@Component({
  selector: 'blockexplorer-last-blocks',
  templateUrl: './last-blocks.component.html',
  styleUrls: ['./last-blocks.component.css']
})
export class LastBlocksComponent implements OnInit {

  @Input() blocks: BlockResponseModel[] = [];
  @Input() loading = false;
  @Output() loadMore = new EventEmitter<number>();

  private records = 10;
  constructor() { }

  ngOnInit() {
  }

  public getSize(block: BlockResponseModel) {
    return block.extendedInformation.size || 0;
  }

  public getTransactionCount(block: BlockResponseModel) {
    return block.extendedInformation.transactionCount || 0;
  }

  public getBlockTransactions(block: BlockResponseModel) {
    return !!block.extendedInformation
            ? block.extendedInformation.transactionCount || 0
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

  public getNext10() {
    this.records = this.records + 10;
    console.log(this.records);
    this.loadMore.emit(this.records);
  }
}
