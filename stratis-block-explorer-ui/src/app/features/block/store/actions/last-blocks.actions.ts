import { createAction, props } from '@ngrx/store';
import { BlockResponseModel } from '../../models/block-response.model';

export const loadLastBlocks = createAction(
    '[LastBlocks] Load Last Blocks',
    (records: number) => ({ records })
);

export const loadLastBlocksError = createAction(
    '[LastBlocks] Load Last Blocks Error',
    props<{ error: Error | string }>()
);

export const lastBlocksLoaded = createAction(
    '[LastBlocks] Last Blocks Loaded',
    props<{ blocks: BlockResponseModel[] }>()
);
