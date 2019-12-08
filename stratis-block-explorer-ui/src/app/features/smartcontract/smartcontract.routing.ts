import { Route, RouterModule } from '@angular/router';
import * as fromContainers from './containers';

export const smartContractRoutes: Route[] = [
    { path: 'smartcontracts', component: fromContainers.SmartContractsDashboardComponent, data: { breadcrumb: 'Smart Contracts' } },
    {
        path: 'smartcontract', data: { breadcrumb: 'Smart Contract' }, children: [
            { path: '', redirectTo: '/smartcontracts', pathMatch: "full" },
            { path: ':address', component: fromContainers.SmartContractActionComponent },
            {
                path: 'action', data: { breadcrumb: 'Action' }, children: [
                    { path: '', redirectTo: '/smartcontracts', pathMatch: "full" },
                    { path: ':txId', component: fromContainers.SmartContractActionComponent }
                ]
            }
        ]
    }
];
