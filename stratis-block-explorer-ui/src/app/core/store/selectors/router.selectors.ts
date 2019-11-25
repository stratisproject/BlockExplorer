import { createFeatureSelector, createSelector } from "@ngrx/store";
import * as fromRouter from "@ngrx/router-store";
import { getSelectors } from "@ngrx/router-store";

import { CoreState } from "../reducers";

export const selectRouter = createFeatureSelector<CoreState, fromRouter.RouterReducerState<any>>("router");


const {
    selectQueryParams, // select the current route query params
    selectRouteParams, // select the current route params
    selectRouteData, // select the current route data
    selectUrl // select the current url
} = getSelectors(selectRouter);
