import { SearchTextService } from './search-text.service';

describe('SearchTextService', () => {
    let service: SearchTextService;

    beforeEach(() => {
        service = new SearchTextService();
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });

    it('searchText is set', () => {
        const searchText = 'the text';
        service.searchText = searchText;
        expect(service.searchText).toEqual(searchText);
    });

    it('searchTextStream pumps searchText', () => {
        const searchText = 'the text';
        let output;
        service.searchTextStream.subscribe(x => output = x);
        service.searchText = searchText;
        expect(output).toEqual(searchText);
    });

    it('searchTextStream pumps the last searchText value when subscribed', () => {
        const searchText = 'the text';
        service.searchText = searchText;
        let output;
        service.searchTextStream.subscribe(x => output = x);
        expect(output).toEqual(searchText);
    });
});
