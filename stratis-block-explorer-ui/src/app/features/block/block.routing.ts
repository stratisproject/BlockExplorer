import { Route, RouterModule } from '@angular/router';
import * as fromContainers from './containers';

export const blockRoutes: Route[] = [
    { path: 'blocks', component: fromContainers.BlocksComponent, data: { breadcrumb: 'Blocks' } },
    {
        path: 'block', data: { breadcrumb: 'Block' }, children: [
            { path: '', redirectTo: '/blocks', pathMatch: "full" },
            { path: ':blockHash', component: fromContainers.BlockComponent }
        ]
    }
];
