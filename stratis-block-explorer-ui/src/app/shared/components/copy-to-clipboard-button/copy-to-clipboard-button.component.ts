import { Component, OnInit, Input, ChangeDetectorRef, EventEmitter, Output } from '@angular/core';
import { CoreStoreFacade } from '@core/store/core-store.facade';

@Component({
    selector: 'copy-to-clipboard',
    templateUrl: './copy-to-clipboard-button.component.html',
    styleUrls: ['./copy-to-clipboard-button.component.scss']
})
export class CopyToClipboardButtonComponent {
    @Input() cbContent: string;
    @Input() successMessage: string;

    @Output() cbOnSuccess: EventEmitter<boolean> = new EventEmitter<boolean>();

    copied: boolean = false;

    constructor(private coreFacade: CoreStoreFacade, private changeDetectorRef: ChangeDetectorRef, ) { }

    onCopied() {
        if (!this.copied) {

            this.copied = true;
            this.cbOnSuccess.emit(true);

            this.coreFacade.showSuccess(this.successMessage, () => {
                this.copied = false;
                this.changeDetectorRef.detectChanges();
            });
        }
    }
}
