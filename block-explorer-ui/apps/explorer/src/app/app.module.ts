import { HttpClientModule } from '@angular/common/http';
import { APP_INITIALIZER, NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';
import { SharedModelsModule } from '@blockexplorer/shared/models';
import { ENVIRONMENT, SharedUtilsModule } from '@blockexplorer/shared/utils';
import { StateGlobalStateModule } from '@blockexplorer/state/global-state';
import { StateTransactionsStateModule } from '@blockexplorer/state/transactions-state';
import { UiLayoutModule } from '@blockexplorer/ui/layout';
import { UiSmartContractsModule, uiSmartContractsRoutes } from '@blockexplorer/ui/smart-contracts';
import {
  TransactionsPageComponent,
  uiAddressesRoutes,
  uiBlockRoutes,
  uiOtherRoutes,
  UiTransactionsModule,
  uiTransactionsRoutes,
} from '@blockexplorer/ui/transactions';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { EffectsModule } from '@ngrx/effects';
import { StoreRouterConnectingModule } from '@ngrx/router-store';
import { StoreModule } from '@ngrx/store';
import { StoreDevtoolsModule } from '@ngrx/store-devtools';
import { PrismModule } from '@ngx-prism/core';
import { NxModule } from '@nrwl/nx';
import { storeFreeze } from 'ngrx-store-freeze';
import { ClipboardModule } from 'ngx-clipboard';

import { environment } from '../environments/environment';
import { AppConfigService } from './+state/app-config.service';
import { AppEffects } from './+state/app.effects';
import { AppFacade } from './+state/app.facade';
import { appReducer, initialState as appInitialState } from './+state/app.reducer';
import { AppComponent } from './app.component';

/**
* Exported function so that it works with AOT
* @param {AppConfigService} configService
* @returns {Function}
*/
export function loadConfigService(configService: AppConfigService): Function {
  return () => {
    return configService.load();
  };
}

@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserModule,
    NgbModule,
    HttpClientModule,
    ClipboardModule,
    FormsModule,
    UiLayoutModule,
    PrismModule,
    UiTransactionsModule,
    UiSmartContractsModule,
    SharedModelsModule,
    SharedUtilsModule.forRoot(),
    StateGlobalStateModule.forRoot(),
    StateTransactionsStateModule.forRoot(),
    NxModule.forRoot(),
    RouterModule.forRoot(
      [
        { path: '', component: TransactionsPageComponent, data: { breadcrumb: 'Dashboard' } },
        { path: 'transactions', children: uiTransactionsRoutes, data: { breadcrumb: 'Transactions' } },
        { path: 'addresses', children: uiAddressesRoutes, data: { breadcrumb: 'Address' } },
        { path: 'blocks', children: uiBlockRoutes, data: { breadcrumb: 'Block' } },
        { path: 'smartcontracts', children: uiSmartContractsRoutes, data: { breadcrumb: 'Smart Contract' } },
        { path: 'search', children: uiOtherRoutes, data: { breadcrumb: 'Not Found' } }
      ],
      {
        initialNavigation: 'enabled',
        onSameUrlNavigation: 'reload'
      }
    ),
    StoreModule.forRoot(
      { app: appReducer },
      {
        initialState: { app: appInitialState },
        metaReducers: !environment.production ? [storeFreeze] : []
      }
    ),
    EffectsModule.forRoot([AppEffects]),
    !environment.production ? StoreDevtoolsModule.instrument() : [],
    StoreRouterConnectingModule
  ],
  providers: [
    AppConfigService,
    AppFacade,
    { provide: ENVIRONMENT, useValue: environment.production ? 'prod' : 'dev' },
    { provide: APP_INITIALIZER, useFactory: loadConfigService, deps: [AppConfigService], multi: true },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
