import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { MenuBarComponent } from './components/menu-bar/menu-bar.component';
import { ContractsModule } from './sub-modules/contracts/contracts.module';
import { ApiServiceBase, FakeApiService } from './services/api.service';

@NgModule({
  declarations: [
    AppComponent,
    MenuBarComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule, 
    FormsModule,
    ContractsModule
  ],
  providers: [{ provide: ApiServiceBase, useClass: FakeApiService }],
  bootstrap: [AppComponent]
})
export class AppModule { }
