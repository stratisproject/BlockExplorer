import { Action } from '@ngrx/store';
import { BalanceSummaryModel, BalanceResponseModel, TransactionSummaryModel, TransactionModel, BlockResponseModel, BlockHeaderResponseModel } from '@blockexplorer/shared/models';

export enum TransactionsActionTypes {
  LoadTransactions = '[Transactions] Load Transactions',
  LoadLastBlocks = '[Transactions] Load Last Blocks',
  LastBlocksLoadedError = '[Transactions] Load Last Blocks Error',
  GetAddress = '[Transactions] Get Address',
  GetTransaction = '[Transactions] Get Transaction',
  GetBlock = '[Transactions] Get Block',
  GetBlockHeader = '[Transactions] Get BlockHeader',
  GetAddressDetails = '[Transactions] Get Address Details',
  TransactionsLoaded = '[Transactions] Transactions Loaded',
  BlockLoaded = '[Transactions] Block Loaded',
  BlockHeaderLoaded = '[Transactions] Block Header Loaded',
  AddressLoaded = '[Transactions] Address Loaded',
  TransactionLoaded = '[Transactions] Transaction Loaded',
  LastBlocksLoaded = '[Transactions] Last Blocks Loaded',
  AddressDetailsLoaded = '[Transactions] Address Details Loaded',
  TransactionsLoadError = '[Transactions] Transactions Load Error',
  AddressLoadError = '[Transactions] Address Load Error',
  BlockLoadError = '[Transactions] Block Load Error',
  BlockHeaderLoadError = '[Transactions] Block Header Load Error',
  TransactionLoadError = '[Transactions] Transaction Load Error',
  AddressDetailsLoadError = '[Transactions] Address Details Load Error'
}

export class LoadTransactions implements Action {
  readonly type = TransactionsActionTypes.LoadTransactions;
}

export class LoadLastBlocks implements Action {
  readonly type = TransactionsActionTypes.LoadLastBlocks;
}

export class GetAddress implements Action {
  readonly type = TransactionsActionTypes.GetAddress;
  constructor(public addressHash: string) {}
}

export class GetTransaction implements Action {
  readonly type = TransactionsActionTypes.GetTransaction;
  constructor(public hash: string) {}
}

export class GetBlock implements Action {
  readonly type = TransactionsActionTypes.GetBlock;
  constructor(public block: string) {}
}

export class GetBlockHeader implements Action {
  readonly type = TransactionsActionTypes.GetBlockHeader;
  constructor(public block: string) {}
}

export class GetAddressDetails implements Action {
  readonly type = TransactionsActionTypes.GetAddressDetails;
  constructor(public addressHash: string) {}
}

export class TransactionsLoadError implements Action {
  readonly type = TransactionsActionTypes.TransactionsLoadError;
  constructor(public payload: any) {}
}

export class AddressLoadError implements Action {
  readonly type = TransactionsActionTypes.AddressLoadError;
  constructor(public payload: any) {}
}

export class TransactionLoadError implements Action {
  readonly type = TransactionsActionTypes.TransactionLoadError;
  constructor(public payload: any) {}
}

export class BlockLoadError implements Action {
  readonly type = TransactionsActionTypes.BlockLoadError;
  constructor(public payload: any) {}
}

export class BlockHeaderLoadError implements Action {
  readonly type = TransactionsActionTypes.BlockHeaderLoadError;
  constructor(public payload: any) {}
}

export class AddressDetailsLoadError implements Action {
  readonly type = TransactionsActionTypes.AddressDetailsLoadError;
  constructor(public payload: any) {}
}

export class AddressLoaded implements Action {
  readonly type = TransactionsActionTypes.AddressLoaded;
  constructor(public address: BalanceSummaryModel) {}
}

export class TransactionLoaded implements Action {
  readonly type = TransactionsActionTypes.TransactionLoaded;
  constructor(public transaction: TransactionSummaryModel) {}
}

export class BlockLoaded implements Action {
  readonly type = TransactionsActionTypes.BlockLoaded;
  constructor(public block: BlockResponseModel) {}
}

export class BlockHeaderLoaded implements Action {
  readonly type = TransactionsActionTypes.BlockHeaderLoaded;
  constructor(public block: BlockHeaderResponseModel) {}
}

export class AddressDetailsLoaded implements Action {
  readonly type = TransactionsActionTypes.AddressDetailsLoaded;
  constructor(public address: BalanceResponseModel) {}
}

export class TransactionsLoaded implements Action {
  readonly type = TransactionsActionTypes.TransactionsLoaded;
  constructor(public transactions: TransactionModel[]) {}
}

export class LastBlocksLoaded implements Action {
  readonly type = TransactionsActionTypes.LastBlocksLoaded;
  constructor(public blocks: BlockResponseModel[]) {}
}

export class LastBlocksLoadedError implements Action {
  readonly type = TransactionsActionTypes.LastBlocksLoadedError;
  constructor(public payload: any) {}
}

export type TransactionsAction =
  | LoadTransactions
  | LoadLastBlocks
  | LastBlocksLoadedError
  | LastBlocksLoaded
  | GetAddress
  | GetTransaction
  | GetBlock
  | GetBlockHeader
  | GetAddressDetails
  | TransactionsLoaded
  | TransactionsLoadError
  | AddressLoaded
  | TransactionLoaded
  | BlockLoaded
  | BlockHeaderLoaded
  | AddressLoadError
  | AddressDetailsLoaded
  | TransactionLoadError
  | BlockLoadError
  | BlockHeaderLoadError
  | AddressDetailsLoadError;

export const fromTransactionsActions = {
  LoadTransactions,
  LoadLastBlocks,
  LastBlocksLoadedError,
  LastBlocksLoaded,
  GetAddress,
  GetTransaction,
  GetAddressDetails,
  GetBlock,
  GetBlockHeader,
  TransactionsLoaded,
  TransactionsLoadError,
  AddressLoaded,
  TransactionLoaded,
  BlockLoaded,
  BlockHeaderLoaded,
  AddressLoadError,
  AddressDetailsLoaded,
  BlockLoadError,
  BlockHeaderLoadError,
  TransactionLoadError,
  AddressDetailsLoadError
};
