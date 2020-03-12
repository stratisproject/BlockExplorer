import { Injectable } from "@angular/core";
import { Actions } from "@ngrx/effects";
import { DataPersistence } from "@nrwl/nx";
import { TokensPartialState } from "./tokens.reducer";
import { TokensActionTypes, LoadRecentTokenTransactions } from "./tokens.actions";

@Injectable()
export class TokensEffects {

    constructor(
        private actions$: Actions,
        private dataPersistence: DataPersistence<TokensPartialState>,        
      ) {}

    @Effect() 
    loadRecent$ = this.dataPersistence.fetch(TokensActionTypes.LoadRecentTokenTransactions, {
        run: (action: LoadRecentTokenTransactions, state: TokensPartialState) => {
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
  
}