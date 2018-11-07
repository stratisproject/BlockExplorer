import { Injectable } from '@angular/core';

import { Observable, BehaviorSubject } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class SearchTextService {
    private _searchText: string;
    private readonly searchTextSubject: BehaviorSubject<string>;
    private readonly _searchTextStream: Observable<string>;

    constructor() { 
        this.searchTextSubject = new BehaviorSubject<string>(this._searchText);
        this._searchTextStream = this.searchTextSubject.asObservable();
    }

    get searchTextStream(): Observable<string> { return this._searchTextStream; }

    get searchText(): string { return this._searchText; }
    set searchText(value: string) {
        if (value !== this._searchText) {
            this._searchText = value;
            this.searchTextSubject.next(value);
        }
    }
}
