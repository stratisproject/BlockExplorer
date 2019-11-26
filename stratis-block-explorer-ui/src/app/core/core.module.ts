import { NgModule, ModuleWithProviders } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AppConfigService } from './services/app-config.service';
import { StyleManagerService } from './services/style-manager.service';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { StandardHeaderInterceptor } from './interceptors';
import { StoreModule } from '@ngrx/store';
import * as fromCore from './store/reducers'
import { EffectsModule } from '@ngrx/effects';
import { CoreStoreFacade } from './store/core-store.facade';

@NgModule({
    declarations: [],
    imports: [
        CommonModule,
    ]
    , exports: [
    ],
    providers: [
        AppConfigService,
        StyleManagerService,
        { provide: HTTP_INTERCEPTORS, useClass: StandardHeaderInterceptor, multi: true, }
    ]
})
export class CoreModule {
    static forRoot(): ModuleWithProviders {
        return {
            ngModule: CoreModule,
            providers: [
                { provide: HTTP_INTERCEPTORS, useClass: StandardHeaderInterceptor, multi: true },
            ],
        };
    }

    constructor(private facade: CoreStoreFacade) {
        this.facade.establishSignalRConnection();
    }
}

