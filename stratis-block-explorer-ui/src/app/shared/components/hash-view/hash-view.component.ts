import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';
import { takeUntilDestroyed } from '../../rxjs/operators/take-until-destroyed';
import { CoreStoreFacade } from '@core/store/core-store.facade';

@Component({
    selector: 'app-hash-view',
    templateUrl: './hash-view.component.html',
    styleUrls: ['./hash-view.component.scss']
})
export class HashViewComponent implements OnInit, OnDestroy {
    @Input() hash: string = null;
    @Input() label: string = null;
    isCopied$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

    constructor(private coreFacade: CoreStoreFacade) { }

    ngOnInit() {
        this.isCopied$
            .pipe(
                takeUntilDestroyed(this)
            )
            .subscribe(value => {
                if (value) {
                    this.coreFacade.showSuccess("Hash copied to clipboard!");
                }
            });
    }

    ngOnDestroy(): void {
    }
}
