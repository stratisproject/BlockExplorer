import { Route, RouterModule } from '@angular/router';
import * as fromContainers from './containers';

export const transactionRoutes: Route[] = [
    {
        path: 'transaction', data: { breadcrumb: 'Transaction' }, children: [
            { path: '', redirectTo: '/blocks', pathMatch: "full" },
            { path: ':txId', component: fromContainers.TransactionComponent }
        ]
    }
];
