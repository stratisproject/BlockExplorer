import { NgModule, Type } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StoreModule } from '@ngrx/store';
import { EffectsModule } from '@ngrx/effects';
import * as fromComponents from './components';
import * as fromContainers from './containers';
import * as fromStore from './store';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { SharedModule } from '@shared/shared.module';
import { SidenavListComponent } from './components/sidenav-list/sidenav-list.component';


@NgModule({
    declarations: [
        fromComponents.MenuBarComponent,
        fromComponents.BreadcrumbComponent,
        fromComponents.SidenavListComponent,
        fromContainers.MainComponent,
        SidenavListComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        RouterModule,
        SharedModule,
        StoreModule.forFeature(fromStore.mainFeatureKey, fromStore.reducer),
        EffectsModule.forFeature([fromStore.MainEffects])
    ],
    exports: [
        fromContainers.MainComponent,
    ]
})
export class MainModule { }
