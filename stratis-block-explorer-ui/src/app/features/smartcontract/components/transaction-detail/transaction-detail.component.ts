import { Component, OnInit, Input, OnDestroy, ChangeDetectorRef, NgZone } from '@angular/core';
import { Transaction, ITransactionOut } from '../../models';
import { CoreStoreFacade } from '@core/store/core-store.facade';
import { faTwitter, faFacebook } from '@fortawesome/free-brands-svg-icons';
import { faExchangeAlt, faCheck } from '@fortawesome/free-solid-svg-icons';

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
export class TransactionDetail {
    @Input() transaction: Transaction;
    @Input() showHeader = true;

    opReturnCopied: boolean = false;
    shareOpReturnOnTwitter: boolean = false;

    outputType = OutputType;

    faTwitter = faTwitter;
    faExchangeAlt = faExchangeAlt;
    faCheck = faCheck;

    constructor() { }

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

    /**
     * Converts hex to string
     * @param hex
     */
    getOpReturnHexToAscii(txOut: ITransactionOut): string {
        let hexData = txOut.scriptPubKey.hash.substr(9); //removes "OP_RETURN " part before decode
        var str = '';
        for (var n = 0; n < hexData.length; n += 2) {
            str += String.fromCharCode(parseInt(hexData.substr(n, 2), 16));
        }
        return str;
    }
}
