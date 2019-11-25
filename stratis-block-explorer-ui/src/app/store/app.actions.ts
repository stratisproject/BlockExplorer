import { createAction, props } from '@ngrx/store';

export const showWarning = createAction(
    '[App] Show Warning',
    props<{ text: string }>()
);

export const showError = createAction(
    '[App] Show Error',
    props<{ text: string, error: Error | string }>()
);

export const showMessage = createAction(
    '[App] Show Message',
    props<{ text: string }>()
);
