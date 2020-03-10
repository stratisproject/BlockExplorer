import { TokensActions, TokensActionTypes } from "./tokens.actions";

export const TOKENS_FEATURE_KEY = 'tokens';

export interface TokensState {
    // list: TransactionModel[];
    // lastBlocks: BlockResponseModel[];
    // smartContractTransactions: TransactionSummaryModel[];
    // smartContractTransactionsLoaded: boolean;
    // lastBlocksLoaded: boolean;
    // statsLoaded: boolean;
    // selectedId?: string | number;
    // selectedAddress?: BalanceSummaryModel;
    // stats?: StatsModel;
    // selectedTransaction?: TransactionSummaryModel;
    // selectedBlock?: BlockResponseModel;
    // selectedBlockHeader?: BlockHeaderResponseModel;
    // selectedAddressDetails?: BalanceResponseModel;
    // loadedTransactions: boolean;
    // loadedAddress: boolean;
    // loadedBlockData: boolean;
    // loadedAddressDetails: boolean;
    // error?: any;
  }

  export interface TokensPartialState {
    readonly [TOKENS_FEATURE_KEY]: TokensState;
  }

  export const initialState: TokensState = {
    // list: [],
    // lastBlocks: [],
    // smartContractTransactions: [],
    // smartContractTransactionsLoaded: false,
    // lastBlocksLoaded: false,
    // statsLoaded: false,
    // selectedAddress: null,
    // stats: null,
    // selectedTransaction: null,
    // selectedBlock: null,
    // selectedBlockHeader: null,
    // selectedAddressDetails: null,
    // loadedTransactions: false,
    // loadedAddress: false,
    // loadedAddressDetails: false,
    // loadedBlockData: false
  };
  
  export function tokensReducer(state: TokensState = initialState, action: TokensActions // TODO
  ): TokensState {

      switch(action.type) {

        case (TokensActionTypes.LoadTokens):
            break;
      }

      return state;
  }