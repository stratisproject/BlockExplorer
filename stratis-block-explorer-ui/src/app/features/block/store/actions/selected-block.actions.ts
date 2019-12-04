import { createAction, props } from '@ngrx/store';
import { BlockResponseModel } from '../../models/block-response.model';

export const loadBlock = createAction(
    '[SelectedBlock] Load Block',
    (blockHash: string) => ({ blockHash })
);

export const loadBlockError = createAction(
    '[SelectedBlock] Load Block Error',
    props<{ error: Error | string }>()
);

export const blockLoaded = createAction(
    '[SelectedBlock] Block Loaded',
    props<{ block: BlockResponseModel }>()
);
