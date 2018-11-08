import { Component, OnInit } from '@angular/core';

import { SearchTextService } from 'src/app/services/search-text.service';
import { ApiServiceBase } from 'src/app/services/api.service';

@Component({
    selector: 'app-contracts',
    templateUrl: './contracts.component.html',
    styleUrls: ['./contracts.component.scss']
})
export class ContractsComponent implements OnInit {
    private pageCount = 0;
    private readonly maxContractsPerPage = 8;

    constructor(readonly searchTextService: SearchTextService, readonly apiService: ApiServiceBase) {
        searchTextService.searchTextStream.subscribe(x => console.log(x));
        apiService.getContractCountAsync().subscribe(x => this.processContractCount(x));
    }

    ngOnInit() {
    }

    private processContractCount(contractCount: number) {
        if (contractCount > 0) {
            if (contractCount <= this.maxContractsPerPage) {
                this.pageCount = 1;
            } else {
                this.pageCount = Math.ceil(contractCount / this.maxContractsPerPage);
            }
        }
    }
}
