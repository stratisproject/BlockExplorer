import { Component, Input } from '@angular/core';

import { SearchTextService } from 'src/app/services/search-text.service';

@Component({
    selector: 'app-menu-bar',
    templateUrl: './menu-bar.component.html',
    styleUrls: ['./menu-bar.component.scss']
})
export class MenuBarComponent {
    constructor(readonly searchTextService: SearchTextService) { }

    get searchText(): string { return this.searchTextService.searchText; }
    @Input() set searchText(value: string) { this.searchTextService.searchText = value; }
}
