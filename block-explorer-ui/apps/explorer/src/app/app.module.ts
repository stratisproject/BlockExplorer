import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { NxModule } from '@nrwl/nx';
import { RouterModule } from '@angular/router';
import { StoreModule } from '@ngrx/store';
import { EffectsModule } from '@ngrx/effects';
import {
  APP_FEATURE_KEY,
  initialState as appInitialState,
  appReducer
} from './+state/app.reducer';
import { AppEffects } from './+state/app.effects';
import { AppFacade } from './+state/app.facade';
import { StoreDevtoolsModule } from '@ngrx/store-devtools';
import { environment } from '../environments/environment';
import { StoreRouterConnectingModule } from '@ngrx/router-store';
import { storeFreeze } from 'ngrx-store-freeze';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { UiLayoutModule } from '@blockexplorer/ui/layout';
import { FormsModule } from '@angular/forms';
import { uiTransactionsRoutes, UiTransactionsModule, TransactionsPageComponent } from '@blockexplorer/ui/transactions';
import { StateTransactionsStateModule } from '@blockexplorer/state/transactions-state';
import { SharedModelsModule, API_BASE_URL } from '@blockexplorer/shared/models';

@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserModule,
    NgbModule,
    FormsModule,
    UiLayoutModule,
    UiTransactionsModule,
    SharedModelsModule,
    StateTransactionsStateModule.forRoot(),
    NxModule.forRoot(),
    RouterModule.forRoot(
      [
        { path: '', component: TransactionsPageComponent },
        { path: 'transactions', children: uiTransactionsRoutes }
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
    AppFacade,
    { provide: API_BASE_URL, useValue: environment.apiBaseUrl }
  ],
  bootstrap: [AppComponent]
})
export class AppModule {}
