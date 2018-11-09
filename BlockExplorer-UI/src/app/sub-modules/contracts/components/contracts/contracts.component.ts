import { Component } from '@angular/core';

import { SearchTextService } from 'src/app/services/search-text.service';
import { ApiServiceBase } from 'src/app/services/api.service';

@Component({
    selector: 'app-contracts',
    templateUrl: './contracts.component.html',
    styleUrls: ['./contracts.component.scss']
})
export class ContractsComponent  {
    private readonly maxContractsPerPage = 8;
    private currentPage = -1;

    constructor(readonly searchTextService: SearchTextService, readonly apiService: ApiServiceBase) {
        searchTextService.searchTextStream.subscribe(x => console.log(x));
        apiService.getContractCountAsync().subscribe(x => this.processContractCount(x));

        this.setPage(0);
    }

    pageCount = 0;

    get currentPageDisplay(): string { return (this.currentPage+1).toString(); }

    firstPageClicked() {
        this.setPage(0);
    }

    previousPageClicked() {
        let page = this.currentPage;
        if (page - 1 >= 0) {
            this.setPage(--page);
        }
    }

    nextPageClicked() {
        let page = this.currentPage;
        if (page + 1 < this.pageCount) {
            this.setPage(++page);
        }
    }

    lastPageClicked() {
        this.setPage(this.pageCount-1);
    }

    private setPage(page: number) {
        console.assert(page >= 0 && page < this.pageCount, `page out of range : ${page}`);
        if (page === this.currentPage) {
            return;
        }
        this.currentPage = page;
        console.log("Page : " + (this.currentPage+1).toString());
    }

    private processContractCount(contractCount: number) {
        this.pageCount = 0;
        if (contractCount > 0) {
            if (contractCount <= this.maxContractsPerPage) {
                this.pageCount = 1;
            } else {
                this.pageCount = Math.ceil(contractCount / this.maxContractsPerPage);
            }
        }
    }
}
