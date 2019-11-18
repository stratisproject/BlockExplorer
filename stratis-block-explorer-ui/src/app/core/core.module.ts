import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AppConfigService } from './services/app-config.service';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
  ]
  , exports: [
  ],
  providers: [AppConfigService]
})
export class CoreModule { }

