import { ContractsComponent } from './contracts.component';
import { SearchTextService } from 'src/app/services/search-text.service';
import { FakeApiService } from 'src/app/services/api.service';
import { ContractsPageFactory } from './contracts.page';

describe('ContractsComponent', () => {
    let component: ContractsComponent;
    let apiService: FakeApiService;

    beforeEach(() => {
        apiService = new FakeApiService();
        const pageFactory = new ContractsPageFactory(apiService);
        component = new ContractsComponent(new SearchTextService(), apiService, pageFactory);
    });

    it('currentPageDisplay set to 1 when firstPageClicked', () => {
        component.firstPageClicked();
        expect(component.currentPageDisplay).toEqual('1');
    });

    it('currentPageDisplay set to 1 when firstPageClicked clicked in succession', () => {
        component.firstPageClicked();
        component.firstPageClicked();
        expect(component.currentPageDisplay).toEqual('1');
    });

    it('currentPageDisplay set to 2 when nextPageClicked clicked', () => {
        component.nextPageClicked();
        expect(component.currentPageDisplay).toEqual('2');
    });

    it('currentPageDisplay set to 1 when nextPageClicked followed by lastPageClicked', () => {
        component.nextPageClicked();
        component.previousPageClicked();
        expect(component.currentPageDisplay).toEqual('1');
    });

    it('currentPageDisplay set to lastPage when lastPageClicked', () => {
        component.lastPageClicked();
        expect(component.currentPageDisplay).toEqual(component.pageCount.toString());
    });

    it('currentPageDisplay set to lastPage when lastPageClicked clicked in succession', () => {
        component.lastPageClicked();
        component.lastPageClicked();
        expect(component.currentPageDisplay).toEqual(component.pageCount.toString());
    });
});
