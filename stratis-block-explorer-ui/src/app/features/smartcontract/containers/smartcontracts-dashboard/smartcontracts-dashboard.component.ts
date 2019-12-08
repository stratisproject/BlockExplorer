import { Component, OnInit, Input } from '@angular/core';
import { Observable, of } from 'rxjs';
import { SmartContractFacade } from '../../store/smartcontract-facade';
import * as fromModel from '../../models';

@Component({
    selector: 'smartcontracts-dashboard',
    templateUrl: './smartcontracts-dashboard.component.html',
    styleUrls: ['./smartcontracts-dashboard.component.scss']
})
export class SmartContractsDashboardComponent implements OnInit {
    loaded$: Observable<boolean>;
    tokens$: Observable<fromModel.StandardToken[]>;
    error$: Observable<string | Error>;

    displayedColumns = ['address', 'name', 'symbol'];
    @Input() records: number = 50;

    constructor(private facade: SmartContractFacade) { }

    ngOnInit() {
        this.loadTokens();
    }

    private loadTokens() {
        this.loaded$ = this.facade.tokensLoaded$;
        this.error$ = this.facade.tokensError$
        this.tokens$ = this.facade.tokens$;

        this.facade.getStandardTokens(this.records);
    }
}
