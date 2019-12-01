import { createAction, props } from '@ngrx/store';
import { TransactionModel, TransactionResponseModel } from '../../models';

export const loadTransaction = createAction(
    '[Transaction] Load Transaction',
    (txId: string) => ({ txId })
);

export const loadTransactionError = createAction(
    '[Transaction] Load Transaction Error',
    props<{ error: Error | string }>()
);

export const transactionLoaded = createAction(
    '[Transaction] Transaction Loaded',
    props<{ transaction: TransactionResponseModel }>()
);
