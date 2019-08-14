import { Action } from '@ngrx/store';
import { BalanceSummaryModel, BalanceResponseModel, TransactionSummaryModel, TransactionModel, BlockResponseModel, BlockHeaderResponseModel, StatsModel } from '@blockexplorer/shared/models';

export enum TransactionsActionTypes {
  LoadTransactions = '[Transactions] Load Transactions',
  LoadStats = '[Transactions] Load Stats',
  LoadSmartContractTransactions = '[Transactions] Smart Contract Transactions',
  LoadLastBlocks = '[Transactions] Load Last Blocks',
  LastBlocksLoadedError = '[Transactions] Load Last Blocks Error',
  GetAddress = '[Transactions] Get Address',
  GetTransaction = '[Transactions] Get Transaction',
  GetBlock = '[Transactions] Get Block',
  GetBlockHeader = '[Transactions] Get BlockHeader',
  GetAddressDetails = '[Transactions] Get Address Details',
  TransactionsLoaded = '[Transactions] Transactions Loaded',
  StatsLoaded = '[Transactions] Stats Loaded',
  BlockLoaded = '[Transactions] Block Loaded',
  BlockHeaderLoaded = '[Transactions] Block Header Loaded',
  AddressLoaded = '[Transactions] Address Loaded',
  TransactionLoaded = '[Transactions] Transaction Loaded',
  SmartContractTransactionsLoaded = '[Transactions] SmartContract Transactions Loaded',
  LastBlocksLoaded = '[Transactions] Last Blocks Loaded',
  AddressDetailsLoaded = '[Transactions] Address Details Loaded',
  TransactionsLoadError = '[Transactions] Transactions Load Error',
  SmartContractTransactionsLoadError = '[Transactions] SmartContract Transactions Load Error',
  StatsLoadError = '[Transactions] Address Load Error',
  AddressLoadError = '[Transactions] Address Load Error',
  BlockLoadError = '[Transactions] Block Load Error',
  BlockHeaderLoadError = '[Transactions] Block Header Load Error',
  TransactionLoadError = '[Transactions] Transaction Load Error',
  AddressDetailsLoadError = '[Transactions] Address Details Load Error'
}

export class LoadTransactions implements Action {
  readonly type = TransactionsActionTypes.LoadTransactions;
}

export class LoadStats implements Action {
  readonly type = TransactionsActionTypes.LoadStats;
}

export class LoadSmartContractTransactions implements Action {
  readonly type = TransactionsActionTypes.LoadSmartContractTransactions;
  constructor(public records: number) {}
}

export class LoadLastBlocks implements Action {
  readonly type = TransactionsActionTypes.LoadLastBlocks;
  constructor(public records: number) {}
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

export class StatsLoadError implements Action {
  readonly type = TransactionsActionTypes.StatsLoadError;
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

export class StatsLoaded implements Action {
  readonly type = TransactionsActionTypes.StatsLoaded;
  constructor(public stats: StatsModel) {}
}

export class TransactionLoaded implements Action {
  readonly type = TransactionsActionTypes.TransactionLoaded;
  constructor(public transaction: TransactionSummaryModel) {}
}

export class SmartContractTransactionsLoaded implements Action {
  readonly type = TransactionsActionTypes.SmartContractTransactionsLoaded;
  constructor(public transactions: TransactionSummaryModel[]) {}
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

export class SmartContractTransactionsLoadError implements Action {
  readonly type = TransactionsActionTypes.SmartContractTransactionsLoadError;
  constructor(public payload: any) {}
}

export type TransactionsAction =
  | LoadTransactions
  | LoadStats
  | LoadSmartContractTransactions
  | LoadLastBlocks
  | LastBlocksLoadedError
  | StatsLoadError
  | StatsLoaded
  | LastBlocksLoaded
  | GetAddress
  | GetTransaction
  | GetBlock
  | GetBlockHeader
  | GetAddressDetails
  | TransactionsLoaded
  | SmartContractTransactionsLoaded
  | TransactionsLoadError
  | SmartContractTransactionsLoadError
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
  LoadStats,
  StatsLoaded,
  StatsLoadError,
  LoadSmartContractTransactions,
  LoadLastBlocks,
  LastBlocksLoadedError,
  LastBlocksLoaded,
  GetAddress,
  GetTransaction,
  GetAddressDetails,
  GetBlock,
  GetBlockHeader,
  TransactionsLoaded,
  SmartContractTransactionsLoaded,
  TransactionsLoadError,
  SmartContractTransactionsLoadError,
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
