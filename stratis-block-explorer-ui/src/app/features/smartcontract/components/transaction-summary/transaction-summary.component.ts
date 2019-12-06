import { Component, OnInit, Input } from '@angular/core';
import { Transaction } from '../../models';
import { ChangeDetectionStrategy } from '@angular/core';

@Component({
    selector: 'app-transaction-summary',
    templateUrl: './transaction-summary.component.html',
    styleUrls: ['./transaction-summary.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class TransactionSummaryComponent implements OnInit {
    @Input() transaction: Transaction = null;

    constructor() { }

    ngOnInit() {
    }

    public getTransactionTags(transaction: Transaction) {
        let tags: { color: string, text: string }[] = [];

        if (this.transaction.isCoinbase)
            tags.push({ color: "primary", text: "Coinbase" });

        if (this.transaction.isCoinstake)
            tags.push({ color: "accent", text: "Coinstake" });

        if (this.transaction.outputs.findIndex(out => out.isSmartContract) > -1)
            tags.push({ color: "accent", text: "Smart Contract" });

        return tags;
    }
}
