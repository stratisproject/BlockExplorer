import { NgModule, Type } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StoreModule } from '@ngrx/store';
import { EffectsModule } from '@ngrx/effects';
import * as fromComponents from './components';
import * as fromMain from './store/reducers/main.reducer';
import { MainEffects } from './store/effects/main.effects';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { SharedModule } from '../../shared/shared.module';

const exportedComponents: Type<any>[] = [
   fromComponents.MenuBarComponent,
   fromComponents.BreadcrumbComponent,
   fromComponents.MainComponent
]


@NgModule({
   declarations: [
      ...exportedComponents
   ],
   imports: [
      CommonModule,
      FormsModule,
      RouterModule,
      SharedModule,
      StoreModule.forFeature(fromMain.mainFeatureKey, fromMain.reducer),
      EffectsModule.forFeature([MainEffects])
   ],
   exports: [
      ...exportedComponents
   ]
})
export class MainModule { }
