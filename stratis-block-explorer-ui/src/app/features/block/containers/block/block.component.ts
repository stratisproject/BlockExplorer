import { Component, OnInit, OnDestroy } from '@angular/core';
import { BlockResponseModel } from '../../models/block-response.model';
import { Observable } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { Log } from '@shared/logger.service';
import { BlockStoreFacade } from '../../store/block-store.facade';

@Component({
    selector: 'app-block',
    templateUrl: './block.component.html',
    styleUrls: ['./block.component.scss']
})
export class BlockComponent implements OnInit {
    isSelectedBlockLoaded$: Observable<boolean>;
    selectedBlock$: Observable<BlockResponseModel>;
    selectedBlockError$: Observable<string | Error>;

    constructor(private route: ActivatedRoute, private facade: BlockStoreFacade, private log: Log) { }

    ngOnInit() {
        this.loadBlockDetails();
    }

    private loadBlockDetails() {
        //todo: substitute this with an effect that react to the route change?

        // note: when the component is destroyed, ActivatedRoute instance dies with it, so there is no need to unsubscribe
        // see https://angular.io/guide/router#observable-parammap-and-component-reuse
        this.route.paramMap
            .subscribe((paramMap: any) => {
                if (!!paramMap.params.blockHash) {
                    let hash = paramMap.params.blockHash;
                    this.facade.loadBlock(hash);
                }
            });

        this.isSelectedBlockLoaded$ = this.facade.isSelectedBlockLoaded$;
        this.selectedBlockError$ = this.facade.selectedBlockError$;
        this.selectedBlock$ = this.facade.selectedBlock$;
    }
}
