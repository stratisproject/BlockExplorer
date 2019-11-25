//import { Action, createReducer, on } from '@ngrx/store';
//import * as BlockActions from './block.actions';
//import { BlockResponseModel } from '../models/block-response.model';

//export const blockFeatureKey = 'block';

//export interface BlockState {
//    lastBlocks: BlockResponseModel[],
//    areLastBlocksLoaded: boolean,

//    blocks: BlockResponseModel[],
//    areBlocksLoaded: boolean

//    selectedBlock: BlockResponseModel,
//    isSelectedBlockLoaded,
//    selectedBlockError: Error | string
//}

//export const initialState: BlockState = {
//    lastBlocks: [],
//    areLastBlocksLoaded: false,
//    blocks: [],
//    areBlocksLoaded: false,
//    selectedBlock: null,
//    isSelectedBlockLoaded: false,
//    selectedBlockError: null
//};

//const blockReducer = createReducer(
//    initialState,

//    on(BlockActions.loadLastBlocks, state => state = ({
//        ...state,
//        areLastBlocksLoaded: false,
//    })),

//    on(BlockActions.lastBlocksLoaded, (state, action) => state = ({
//        ...state,
//        areLastBlocksLoaded: true,
//        lastBlocks: action.blocks,
//    })),

//    on(BlockActions.loadLastBlocksError, (state, action) => state = ({
//        ...state,
//        areLastBlocksLoaded: true,
//        lastBlocks: null,
//    })),

//    on(BlockActions.loadLastBlocks, state => state = ({
//        ...state,
//        areBlocksLoaded: false,
//    })),

//    on(BlockActions.blocksLoaded, (state, action) => state = ({
//        ...state,
//        areBlocksLoaded: true,
//        blocks: action.blocks,
//    })),

//    on(BlockActions.loadBlock, state => state = ({
//        ...state,
//        isSelectedBlockLoaded: false,
//        selectedBlockError: null
//    })),

//    on(BlockActions.blockLoaded, (state, action) => ({
//        ...state,
//        isSelectedBlockLoaded: true,
//        selectedBlock: action.block,
//        selectedBlockError: null
//    })),

//    on(BlockActions.loadBlockError, (state, action) => ({
//        ...state,
//        isSelectedBlockLoaded: true,
//        selectedBlock: null,
//        selectedBlockError: action.error
//    }))
//);

//export function reducer(state: BlockState | undefined, action: Action) {
//    return blockReducer(state, action);
//}
