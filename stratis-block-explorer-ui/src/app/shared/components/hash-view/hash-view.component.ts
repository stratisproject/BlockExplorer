import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { takeUntilDestroyed } from '../../rxjs/operators/take-until-destroyed';
import { CoreStoreFacade } from '@core/store/core-store.facade';
import { coerceBooleanProperty } from '@angular/cdk/coercion';

@Component({
    selector: 'app-hash-view',
    templateUrl: './hash-view.component.html',
    styleUrls: ['./hash-view.component.scss'],
})
export class HashViewComponent implements OnInit, OnDestroy {
    @Input() hash: string = null;
    @Input() label: string = null;
    @Input() enableRoute: boolean;
    @Input() hashType: "address" | "block" | "transaction" | "smartcontract" = "transaction";

    isCopied$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

    constructor(private coreFacade: CoreStoreFacade) { }

    ngOnInit() {
        this.enableRoute = coerceBooleanProperty(this.enableRoute);

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
