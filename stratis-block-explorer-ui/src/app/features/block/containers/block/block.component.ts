import { Component, OnInit, OnDestroy } from '@angular/core';
import { BlockResponseModel } from '../../models/block-response.model';
import { Observable } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { BlocksFacade } from '../../store/blocks.facade';

@Component({
    selector: 'app-block',
    templateUrl: './block.component.html',
    styleUrls: ['./block.component.scss']
})
export class BlockComponent implements OnInit {
    blockLoaded$: Observable<boolean>;
    block$: Observable<BlockResponseModel>;
    blockError$: Observable<string | Error>;

    constructor(private route: ActivatedRoute, private blocksFacade: BlocksFacade) { }

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
                    this.blocksFacade.getBlock(hash);
                }
            });

        this.blockLoaded$ = this.blocksFacade.blockLoaded$;
        this.blockError$ = this.blocksFacade.blockError$;
        this.block$ = this.blocksFacade.block$;
    }
}
