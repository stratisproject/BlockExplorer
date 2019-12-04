import { createAction, props } from '@ngrx/store';
import { BlockResponseModel } from '../../models/block-response.model';


export const loadBlocks = createAction(
    '[Blocks] Load Blocks',
    (records: number) => ({ records })
);

export const loadBlocksError = createAction(
    '[Blocks] Load Blocks Error',
    props<{ error: Error | string }>()
);

export const blocksLoaded = createAction(
    '[Blocks] Blocks Loaded',
    props<{ blocks: BlockResponseModel[] }>()
);
