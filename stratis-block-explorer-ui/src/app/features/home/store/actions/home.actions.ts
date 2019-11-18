import { createAction, props } from '@ngrx/store';

export const identifyEntity = createAction(
  '[Home] Identify Entity',
  props<{ text: string }>()
);

export const identificationError = createAction(
  '[Home] Identification Error',
  props<{ error: Error | any }>()
);

export const identified = createAction(
  '[Home] Identified',
  props<{ payload: any }>()
);
