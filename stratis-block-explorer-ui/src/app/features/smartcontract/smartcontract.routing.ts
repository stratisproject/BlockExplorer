import { Route, RouterModule } from '@angular/router';
import * as fromContainers from './containers';

export const smartContractRoutes: Route[] = [
    {
        path: 'smartcontract', data: { breadcrumb: 'Smart Contract' }, children: [
            { path: '', redirectTo: '/smartcontracts', pathMatch: "full" },
            { path: ':txId', component: fromContainers.SmartContractCallComponent }
        ]
    },
    {
        path: 'smartcontracts', data: { breadcrumb: 'Smart Contracts', component: fromContainers.SmartContractsDashboardComponent },
    }
];
