import { createAction, props } from '@ngrx/store';
import { BlockResponseModel } from '../models/block-response.model';

export const loadLastBlocks = createAction(
   '[Block] Load Last Blocks',
   (records: number) => ({ records })
);

export const lastBlocksLoadedError = createAction(
   '[Block] Load Last Blocks Error',
   props<{ error: Error | string }>()
);

export const lastBlocksLoaded = createAction(
   '[Block] Last Blocks Loaded',
   props<{ blocks: BlockResponseModel[] }>()
);
