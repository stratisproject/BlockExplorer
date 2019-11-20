import { createAction, props } from '@ngrx/store';

export const identifyEntity = createAction(
   '[Main] Identify Entity',
   props<{ text: string }>()
);

export const identificationError = createAction(
   '[Main] Identification Error',
   props<{ error: Error | string }>()
);

export const identified = createAction(
   '[Main] Identified',
   props<{ payload: any }>()
);
