import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StoreModule } from '@ngrx/store';
import { EffectsModule } from '@ngrx/effects';
import * as components from './components';
import * as fromHome from './store/reducers/home.reducer';
import { HomeEffects } from './store/effects/home.effects';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { SharedModule } from '../../shared/shared.module';

@NgModule({
   declarations: [
      components.MenuBarComponent, components.BreadcrumbComponent, components.HomeComponent
   ],
   imports: [
      CommonModule,
      FormsModule,
      RouterModule,
      SharedModule,
      StoreModule.forFeature(fromHome.homeFeatureKey, fromHome.reducer),
      EffectsModule.forFeature([HomeEffects])
   ],
   exports: [
      components.MenuBarComponent, components.BreadcrumbComponent, components.HomeComponent
   ]
})
export class HomeModule { }
