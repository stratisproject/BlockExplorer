import { createAction, props } from '@ngrx/store';
import { BlockResponseModel } from '../models/block-response.model';

export const loadLastBlocks = createAction(
    '[Block] Load Last Blocks',
    (records: number) => ({ records })
);

export const loadLastBlocksError = createAction(
    '[Block] Load Last Blocks Error',
    props<{ error: Error | string }>()
);

export const lastBlocksLoaded = createAction(
    '[Block] Last Blocks Loaded',
    props<{ blocks: BlockResponseModel[] }>()
);

export const loadBlocks = createAction(
    '[Block] Load Blocks',
    (records: number) => ({ records })
);

export const loadBlocksError = createAction(
    '[Block] Load Blocks Error',
    props<{ error: Error | string }>()
);

export const blocksLoaded = createAction(
    '[Block] Blocks Loaded',
    props<{ blocks: BlockResponseModel[] }>()
);

export const loadBlock = createAction(
    '[Block] Load Block',
    (blockHash: string) => ({ blockHash })
);

export const loadBlockError = createAction(
    '[Block] Load Block Error',
    props<{ error: Error | string }>()
);

export const blockLoaded = createAction(
    '[Block] Block Loaded',
    props<{ block: BlockResponseModel }>()
);
