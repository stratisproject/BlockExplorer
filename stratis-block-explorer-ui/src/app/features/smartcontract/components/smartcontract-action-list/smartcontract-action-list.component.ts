import { Component, OnInit, Input, OnChanges, SimpleChanges } from '@angular/core';
import { PageEvent } from '@angular/material/paginator';
import * as fromModels from '../../models';
import { SmartContractAction } from '../../models';

@Component({
    selector: 'smartcontract-action-list',
    templateUrl: './smartcontract-action-list.component.html',
    styleUrls: ['./smartcontract-action-list.component.scss']
})
export class SmartContractActionListComponent implements OnInit, OnChanges {

    @Input() title = 'Smart Contract Actions';
    @Input() smartContractActions: SmartContractAction[] = null;
    @Input() pageSize = 50;
    @Input() showPaging = true;
    @Input() showHeader = true;
    @Input() showCount = true;

    smartContractActionsOnCurrentPage: SmartContractAction[] = [];
    currentPageIndex = 0;

    // MatPaginator Output
    pageEvent: PageEvent;

    constructor() { }

    ngOnInit() {
        this.loadCurrentPage();
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (changes.transactions) {
            this.loadCurrentPage();
        }
    }

    loadCurrentPage() {

        this.smartContractActionsOnCurrentPage = this.smartContractActions
            .slice(this.currentPageIndex * this.pageSize, (this.currentPageIndex + 1) * this.pageSize);
    }

    get totalPages() {
        if (!this.smartContractActions || this.smartContractActions.length <= this.pageSize)
            return 1;

        const pages = Math.trunc(this.smartContractActions.length / this.pageSize);

        if (this.smartContractActions.length % this.pageSize === 0)
            return pages;

        return pages + 1;
    }

    public onPageEvent(event: PageEvent): PageEvent {
        this.pageSize = event.pageSize;
        this.currentPageIndex = event.pageIndex;
        this.loadCurrentPage();

        return event;
    }
}
