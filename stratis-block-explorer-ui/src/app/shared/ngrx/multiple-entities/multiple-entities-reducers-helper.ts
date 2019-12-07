import { MultipleEntitiesState } from './multiple-entities-state';
import { On, on } from '@ngrx/store';
import { MultipleEntitiesActionsHelper } from './multiple-entities-actions-helper';

/**
 * Helper to write less boilerplate code when writing reducers for states that have to track a multiple entity loading
 * */
export class MultipleEntitiesReducersHelper<TEntity, TEntityState extends MultipleEntitiesState<TEntity>> {
    public getInitialState(): TEntityState {
        return <TEntityState>{
            entities: [],
            loaded: false,
            error: null
        };
    }

    public applyLoadReducer(state: TEntityState): TEntityState {
        return {
            ...state,
            entities: [],
            loaded: false,
            error: null
        };
    }

    public applyLoadedReducer(state: TEntityState, entities: TEntity[]): TEntityState {
        return ({
            ...state,
            entities: entities,
            loaded: true,
            error: null
        });
    }

    public applyLoadErrorReducer(state: TEntityState, error: Error | string): TEntityState {
        return ({
            ...state,
            entities: [],
            loaded: false,
            error: error
        });
    }

    public getDefaultReducers(actionHelper: MultipleEntitiesActionsHelper<TEntity>): On<TEntityState>[] {
        return <On<TEntityState>[]>[
            on(actionHelper.loadAction, (state: TEntityState) => this.applyLoadReducer(state)),
            on(actionHelper.loadedAction, (state: TEntityState, action) => this.applyLoadedReducer(state, action.entities)),
            on(actionHelper.loadErrorAction, (state: TEntityState, action) => this.applyLoadErrorReducer(state, action.error)),
        ]
    }
}
