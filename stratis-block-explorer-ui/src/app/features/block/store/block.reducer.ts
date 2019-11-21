import { Action, createReducer, on } from '@ngrx/store';
import * as BlockActions from './block.actions';
import { BlockResponseModel } from '../models/block-response.model';

export const blockFeatureKey = 'block';

export interface BlockState {
   lastBlocks: BlockResponseModel[],
   lastBlocksLoaded: boolean,
}

export const initialState: BlockState = {
   lastBlocks: [],
   lastBlocksLoaded: false,
};

const blockReducer = createReducer(
   initialState,

   on(BlockActions.loadLastBlocks, state => state = ({
      ...state,
      lastBlocksLoaded: false,
   })),

   on(BlockActions.lastBlocksLoaded, (state, action) => state = ({
      ...state,
      lastBlocksLoaded: true,
      lastBlocks: action.blocks,
   }))
);

export function reducer(state: BlockState | undefined, action: Action) {
   return blockReducer(state, action);
}
