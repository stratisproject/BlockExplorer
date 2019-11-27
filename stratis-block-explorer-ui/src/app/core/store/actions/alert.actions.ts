import { createAction } from '@ngrx/store';

export const showSuccess = createAction(
    '[Alert] Show Confirm',
    (text: string, duration: number = 2000) => ({ text, duration })
);

export const showInformation = createAction(
    '[Alert] Show Information',
    (text: string, duration: number = 2000) => ({ text, duration })
);

export const showWarning = createAction(
    '[Alert] Show Warning',
    (text: string, duration: number = 2000) => ({ text, duration })
);

export const showError = createAction(
    '[Alert] Show Error',
    (text: string, error: Error | any, duration: number = 2000) => ({ text, error, duration })
);
