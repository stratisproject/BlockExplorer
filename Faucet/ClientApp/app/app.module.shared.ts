import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './components/app/app.component';
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { FaucetComponent } from './components/faucet/faucet.component';
import { AboutComponent } from './components/about/about.component'

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        FaucetComponent,
        AboutComponent
    ],
    imports: [
        CommonModule,
        HttpModule,
        FormsModule,
        ReactiveFormsModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'faucet', pathMatch: 'full' },
            { path: 'faucet', component: FaucetComponent},
            { path: 'about', component: AboutComponent },
            { path: '**', redirectTo: 'faucet' }
        ])
    ]
})
export class AppModuleShared {
}
