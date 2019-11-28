import { Component, OnInit, Input } from '@angular/core';
import { IBlockTransaction } from '../../models/block-transaction.model';

@Component({
    selector: 'app-block-transactions-item',
    templateUrl: './block-transactions-item.component.html',
    styleUrls: ['./block-transactions-item.component.scss']
})
export class BlockTransactionsItemComponent implements OnInit {
    @Input() transaction: IBlockTransaction;
    @Input() showHeader = true;

    constructor() { }

    ngOnInit() {
    }

}
