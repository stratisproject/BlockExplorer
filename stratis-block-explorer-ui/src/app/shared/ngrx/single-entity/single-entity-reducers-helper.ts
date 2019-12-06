import { SingleEntityState } from './single-entity-state';
import { On, on } from '@ngrx/store';
import { SingleEntityActionsHelper } from './single-entity-actions-helper';

/**
 * Helper to write less boilerplate code when writing reducers for states that have to track a single entity loading
 * */
export class SingleEntityReducersHelper<TEntity, TEntityState extends SingleEntityState<TEntity>> {
    public getInitialState(): TEntityState {
        return <TEntityState>{
            entity: null,
            loaded: false,
            error: null
        };
    }

    public applyLoadReducer(state: TEntityState): TEntityState {
        return {
            ...state,
            entity: null,
            loaded: false,
            error: null
        };
    }

    public applyLoadedReducer(state: TEntityState, entity: TEntity): TEntityState {
        return ({
            ...state,
            entity: entity,
            loaded: true,
            error: null
        });
    }

    public applyLoadErrorReducer(state: TEntityState, error: Error | string): TEntityState {
        return ({
            ...state,
            entity: null,
            loaded: false,
            error: error
        });
    }

    public getDefaultReducers(actionHelper: SingleEntityActionsHelper<TEntity>): On<TEntityState>[] {
        return <On<TEntityState>[]>[
            on(actionHelper.loadAction, (state: TEntityState) => this.applyLoadReducer(state)),
            on(actionHelper.loadedAction, (state: TEntityState, action) => this.applyLoadedReducer(state, action.entity)),
            on(actionHelper.loadErrorAction, (state: TEntityState, action) => this.applyLoadErrorReducer(state, action.error)),
        ]
    }
}
