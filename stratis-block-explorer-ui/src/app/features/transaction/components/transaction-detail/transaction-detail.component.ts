import { Component, OnInit, Input, OnDestroy, ChangeDetectorRef, NgZone } from '@angular/core';
import { Transaction, ITransactionOut } from '../../models';
import { BehaviorSubject, Observable } from 'rxjs';
import { CoreStoreFacade } from '@core/store/core-store.facade';
import { faTwitter, faFacebook } from '@fortawesome/free-brands-svg-icons';

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
export class TransactionDetail implements OnInit, OnDestroy {
    @Input() transaction: Transaction;
    @Input() showHeader = true;

    opReturnCopied: boolean = false;
    shareOpReturnOnTwitter: boolean = false;

    outputType = OutputType;

    faTwitter = faTwitter;
    faFacebook = faFacebook;

    constructor(private coreFacade: CoreStoreFacade, private changeDetectorRef: ChangeDetectorRef, private ngZone: NgZone) { }

    ngOnInit() {
    }

    ngOnDestroy(): void {
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

    shareOnTwitter() {
        if (!this.shareOpReturnOnTwitter) {
            this.shareOpReturnOnTwitter = true;
            this.coreFacade.showSuccess("Share on twitter!", () => {
                this.shareOpReturnOnTwitter = false;
                this.changeDetectorRef.detectChanges();
            });
        }
    }

    onOpReturnCopied() {
        if (!this.opReturnCopied) {
            this.opReturnCopied = true;
            this.coreFacade.showSuccess("OP_RETURN content has been copied to clipboard!", () => {
                this.opReturnCopied = false;
                this.changeDetectorRef.detectChanges();
            });
        }
    }
}
