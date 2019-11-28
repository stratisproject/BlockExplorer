import { Component, OnInit, Input, OnDestroy, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';
import { takeUntilDestroyed } from '../../rxjs/operators/take-until-destroyed';
import { CoreStoreFacade } from '@core/store/core-store.facade';

@Component({
    selector: 'app-hash-view',
    templateUrl: './hash-view.component.html',
    styleUrls: ['./hash-view.component.scss'],
    changeDetection: ChangeDetectionStrategy.Default
})
export class HashViewComponent implements OnInit, OnDestroy {
    @Input() hash: string = null;
    @Input() label: string = null;
    @Input() enableRoute: boolean;
    @Input() hashType: "address" | "block" | "transaction" | "smartcontract" = "transaction";

    isCopied$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

    constructor(private coreFacade: CoreStoreFacade) { }

    ngOnInit() {
        this.enableRoute = this.enableRoute !== undefined;

        this.isCopied$
            .pipe(
                takeUntilDestroyed(this)
            )
            .subscribe(value => {
                if (value) {
                    this.coreFacade.showSuccess(`${this.hashType} hash copied to clipboard!`, () => setTimeout(() => this.isCopied$.next(false), 10));
                }
            });
    }

    ngOnDestroy(): void {
    }
}
