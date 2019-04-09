import { Injectable } from '@angular/core';
import { Effect, Actions } from '@ngrx/effects';
import { DataPersistence } from '@nrwl/nx';

import { TransactionsPartialState } from './transactions.reducer';
import {
  LoadTransactions,
  TransactionsLoaded,
  TransactionsLoadError,
  TransactionsActionTypes,
  GetAddress,
  AddressLoaded,
  AddressLoadError,
  GetAddressDetails,
  AddressDetailsLoaded,
  AddressDetailsLoadError,
  TransactionLoaded,
  TransactionLoadError,
  GetTransaction,
  BlockLoaded,
  GetBlock,
  BlockLoadError,
  GetBlockHeader,
  BlockHeaderLoaded,
  BlockHeaderLoadError,
  LoadLastBlocks,
  LastBlocksLoaded,
  LastBlocksLoadedError
} from './transactions.actions';
import { TransactionsService } from '../services/transactions.service';
import { map } from 'rxjs/operators';
import { BalancesService } from '../services/balances.service';
import { BlocksService } from '../services/blocks.service';

@Injectable()
export class TransactionsEffects {
  @Effect() loadTransactions$ = this.dataPersistence.fetch(TransactionsActionTypes.LoadTransactions, {
    run: (action: LoadTransactions, state: TransactionsPartialState) => {
      return this.transactionsService.transactions().pipe(
        map((transactions) => {
          return new TransactionsLoaded(transactions);
        })
      );
    },

    onError: (action: LoadTransactions, error) => {
      console.error('Error', error);
      return new TransactionsLoadError(error);
    }
  });

  @Effect() loadLastBlocks$ = this.dataPersistence.fetch(TransactionsActionTypes.LoadLastBlocks, {
    run: (action: LoadLastBlocks, state: TransactionsPartialState) => {
      return this.blocksService.blocks().pipe(
        map((blocks) => {
          return new LastBlocksLoaded(blocks);
        })
      );
    },

    onError: (action: LoadLastBlocks, error) => {
      console.error('Error', error);
      return new LastBlocksLoadedError(error);
    }
  });

  @Effect() getAddress$ = this.dataPersistence.fetch(TransactionsActionTypes.GetAddress, {
    run: (action: GetAddress, state: TransactionsPartialState) => {
      return this.balancesService.addressBalanceSummary(action.addressHash, undefined, false, false).pipe(
        map((balance) => {
          return new AddressLoaded(balance);
        })
      );
    },

    onError: (action: GetAddress, error) => {
      console.error('Error', error);
      return new AddressLoadError(error);
    }
  });

  @Effect() getAddressDetails$ = this.dataPersistence.fetch(TransactionsActionTypes.GetAddressDetails, {
    run: (action: GetAddressDetails, state: TransactionsPartialState) => {
      return this.balancesService.addressBalance(action.addressHash, undefined, undefined, undefined, false, false, false, true).pipe(
        map((balance) => {
          return new AddressDetailsLoaded(balance);
        })
      );
    },

    onError: (action: GetAddressDetails, error) => {
      console.error('Error', error);
      return new AddressDetailsLoadError(error);
    }
  });

  @Effect() getTransaction$ = this.dataPersistence.fetch(TransactionsActionTypes.GetTransaction, {
    run: (action: GetTransaction, state: TransactionsPartialState) => {
      return this.transactionsService.transaction(action.hash, false, true).pipe(
        map((balance) => {
          return new TransactionLoaded(balance);
        })
      );
    },

    onError: (action: GetTransaction, error) => {
      console.error('Error', error);
      return new TransactionLoadError(error);
    }
  });

  @Effect() getBlock$ = this.dataPersistence.fetch(TransactionsActionTypes.GetBlock, {
    run: (action: GetBlock, state: TransactionsPartialState) => {
      return this.blocksService.block(action.block, false, true).pipe(
        map((block) => {
          return new BlockLoaded(block);
        })
      );
    },

    onError: (action: GetBlock, error) => {
      console.error('Error', error);
      return new BlockLoadError(error);
    }
  });

  @Effect() getBlockHeader$ = this.dataPersistence.fetch(TransactionsActionTypes.GetBlockHeader, {
    run: (action: GetBlockHeader, state: TransactionsPartialState) => {
      return this.blocksService.blockHeader(action.block).pipe(
        map((block) => {
          return new BlockHeaderLoaded(block);
        })
      );
    },

    onError: (action: GetBlockHeader, error) => {
      console.error('Error', error);
      return new BlockHeaderLoadError(error);
    }
  });

  constructor(
    private actions$: Actions,
    private dataPersistence: DataPersistence<TransactionsPartialState>,
    private transactionsService: TransactionsService,
    private balancesService: BalancesService,
    private blocksService: BlocksService
  ) {}
}
