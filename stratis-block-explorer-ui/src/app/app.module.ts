import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { NgModule, APP_INITIALIZER } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { CoreModule } from './core/core.module';
import { AppConfigService } from './core/services/app-config.service';
import { HttpClientModule } from '@angular/common/http';
import { MainModule } from '@features/main/main.module';
import { StoreModule } from '@ngrx/store';
import { StoreDevtoolsModule } from '@ngrx/store-devtools';
import { environment } from '../environments/environment';
import { reducers } from './reducers';
import { EffectsModule } from '@ngrx/effects';
import { BlockModule } from './features/block/block.module';
import { DashboardModule } from './features/dashboard/dashboard.module';

@NgModule({
   declarations: [
      AppComponent,
   ],
   imports: [
      BrowserModule,
      BrowserAnimationsModule,
      AppRoutingModule,
      HttpClientModule,
      CoreModule,
      MainModule, BlockModule, DashboardModule,
      StoreModule.forRoot(reducers, {
         runtimeChecks: {
            strictStateImmutability: true,
            strictActionImmutability: true,
            strictStateSerializability: true,
            strictActionSerializability: true,
         }
      }),
      EffectsModule.forRoot([]),
      !environment.production ? StoreDevtoolsModule.instrument() : []
   ],
   providers: [
      AppConfigService,
      { provide: APP_INITIALIZER, useFactory: init_app, deps: [AppConfigService], multi: true }
   ],
   bootstrap: [AppComponent]
})
export class AppModule { }


export function init_app(appLoadService: AppConfigService) {
   return () => appLoadService.load();
}
