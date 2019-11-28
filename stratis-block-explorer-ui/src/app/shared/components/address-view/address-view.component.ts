import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { CoreStoreFacade } from '@core/store/core-store.facade';
import { takeUntilDestroyed } from '../../shared.module';

@Component({
    selector: 'app-address-view',
    templateUrl: './address-view.component.html',
    styleUrls: ['./address-view.component.scss']
})
export class AddressViewComponent implements OnInit, OnDestroy {
    @Input() address: string = null;
    @Input() label: string = null;
    @Input() enableRoute: boolean = false;

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
                    this.coreFacade.showSuccess("Address copied to clipboard!");
                }
            });
    }

    ngOnDestroy(): void {
    }
}
