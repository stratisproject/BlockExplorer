import { Action, createReducer, on } from '@ngrx/store';
import * as Actions from '../actions/blocks.actions';
import * as fromModels from '../../models';
import { EntityState, EntityReducersHelper } from '@shared/ngrx';
import { StatsModel } from '../../models';

export interface BlocksState extends EntityState<fromModels.BlockResponseModel> {
   stats: StatsModel,
   statsLoaded: boolean,
   statsLoadError: boolean
}

let reducerHelper = new EntityReducersHelper<fromModels.BlockResponseModel, BlocksState>();

const innerReducer = createReducer(
   ({
      ...reducerHelper.getInitialState(),
      stats: null,
      statsLoaded: false,
      statsLoadError: null
   }),

   ...reducerHelper.getDefaultReducers(Actions.blockActionHelper),

   on(Actions.loadStats, state => state = ({
      ...state,
      stats: null,
      statsLoaded: false,
      statsLoadError: null
   })),

   on(Actions.statsLoaded, (state, action) => ({
      ...state,
      stats: action.stats,
      statsLoaded: true,
      statsLoadError: null
   })),

   on(Actions.loadStatsError, (state, action) => ({
      ...state,
      stats: null,
      statsLoaded: true,
      statsLoadError: action.error
   })),
);

export function reducer(state: BlocksState | undefined, action: Action) {
   return innerReducer(state, action);
}
