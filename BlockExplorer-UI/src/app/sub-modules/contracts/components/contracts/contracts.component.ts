import { Component } from '@angular/core';
import * as _ from 'lodash';
import { Subscription } from 'rxjs';

import { SearchTextService } from 'src/app/services/search-text.service';
import { ApiServiceBase } from 'src/app/services/api.service';
import { ContractsPage, ContractsPageFactory, Contract } from './contracts.page';
import { filter } from 'rxjs/operators';

@Component({
    selector: 'app-contracts',
    templateUrl: './contracts.component.html',
    styleUrls: ['./contracts.component.scss']
})
export class ContractsComponent {
    private readonly maxContractsPerPage = 8;
    private pageIndex = -1;
    private pages: ContractsPage[] = [];
    private contracts$: Subscription;
    private pageContracts: Contract[];

    constructor(readonly searchTextService: SearchTextService, readonly apiService: ApiServiceBase, readonly pageFactory: ContractsPageFactory) {
        searchTextService.searchTextStream.pipe(filter(_ => !!this.pageContracts)).subscribe(x => 
            this.filteredContracts = this.pageContracts.filter(x => this.isMatch(x)));
        apiService.getContractCountAsync().subscribe(x => this.processContractCount(x));

        this.setPage(0);
    }

    pageCount = 0;
    filteredContracts: Contract[];

    get currentPageDisplay(): string { return (this.pageIndex + 1).toString(); }
    
    firstPageClicked() {
        this.setPage(0);
    }

    previousPageClicked() {
        let page = this.pageIndex;
        if (page - 1 >= 0) {
            this.setPage(--page);
        }
    }

    nextPageClicked() {
        let page = this.pageIndex;
        if (page + 1 < this.pageCount) {
            this.setPage(++page);
        }
    }

    lastPageClicked() {
        this.setPage(this.pageCount - 1);
    }

    private setPage(pageIndex: number) {
        console.assert(pageIndex >= 0 && pageIndex < this.pageCount, `page out of range : ${pageIndex}`);
        if (pageIndex === this.pageIndex) {
            return;
        }
        let page = this.pages[pageIndex];
        if (!page) {
            page = this.pages[pageIndex] = this.pageFactory.New(pageIndex);
        }
        this.pageIndex = pageIndex;

        if (this.contracts$) {
            this.contracts$.unsubscribe();
        }
        this.contracts$ = page.getContractsAsync().subscribe(x => {
            this.pageContracts = x;
            this.filteredContracts = this.pageContracts.filter(c => this.isMatch(c));
        });
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

    private isMatch(contract: Contract): boolean {
        let searchText = this.searchTextService.searchText;
        if (!searchText) {
            return true;
        }
        searchText = searchText.toLowerCase();
        const matches = Object.keys(contract).map(x => contract[x].toLowerCase()).filter(x => x.includes(searchText));
        return matches.length > 0;
    }
}
