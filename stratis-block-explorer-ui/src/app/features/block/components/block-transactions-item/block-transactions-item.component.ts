import { Component, OnInit, Input } from '@angular/core';
import { IBlockTransaction, IBlockTransactionOut } from '../../models/block-transaction.model';


enum OutputType {
    SmartContract,
    Unspendable,
    Address
}

@Component({
    selector: 'app-block-transactions-item',
    templateUrl: './block-transactions-item.component.html',
    styleUrls: ['./block-transactions-item.component.scss']
})
export class BlockTransactionsItemComponent implements OnInit {
    @Input() transaction: IBlockTransaction;
    @Input() showHeader = true;

    outputType = OutputType;

    constructor() { }

    ngOnInit() {
    }

    getOutputType(output: IBlockTransactionOut): OutputType {
        let value: OutputType;

        if (output.isSmartContract)
            value = OutputType.SmartContract
        else if (output.isUnspendable)
            value = OutputType.Unspendable
        else
            value = OutputType.Address

        return value;
    }
}
