import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AppConfigService } from './services/app-config.service';
import { StyleManagerService } from './services/style-manager.service';

@NgModule({
   declarations: [],
   imports: [
      CommonModule,
   ]
   , exports: [
   ],
   providers: [AppConfigService, StyleManagerService]
})
export class CoreModule { }

