import { Route } from '@angular/router';
import * as fromContainers from './containers';

export const mainRoutes: Route[] = [
    {
        path: 'search', data: { breadcrumb: 'Search' }, children: [
            { path: '', redirectTo: '/', pathMatch: "full" },
            { path: 'not-found', component: fromContainers.NotFoundPageComponent }
        ]
    }
];
