import { Component, OnInit, Input } from '@angular/core';
import { Transaction, ITransactionOut } from '../../models';


enum OutputType {
    SmartContract,
    Unspendable,
    Address
}

@Component({
    selector: 'transaction-detail',
    templateUrl: './transaction-detail.component.html',
    styleUrls: ['./transaction-detail.component.scss']
})
export class TransactionDetail implements OnInit {
    @Input() transaction: Transaction;
    @Input() showHeader = true;

    outputType = OutputType;

    constructor() { }

    ngOnInit() {
    }

    getOutputType(output: ITransactionOut): OutputType {
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
