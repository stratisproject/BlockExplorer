import { MultipleEntitiesState } from './multiple-entities-state';
import { On, on } from '@ngrx/store';
import { MultipleEntitiesActionsHelper } from './multiple-entities-actions-helper';

/**
 * Helper to write less boilerplate code when writing reducers for states that have to track a multiple entity loading
 * */
export class MultipleEntitiesReducersHelper<T> {
    public getInitialState<TConcreteState extends MultipleEntitiesState<T>>(): TConcreteState {
        return <TConcreteState>{
            entities: [],
            loaded: false,
            error: null
        };
    }

    public applyLoadReducer<TConcreteState extends MultipleEntitiesState<T>>(state: TConcreteState): TConcreteState {
        return {
            ...state,
            entities: [],
            loaded: false,
            error: null
        };
    }

    public applyLoadedReducer<TConcreteState extends MultipleEntitiesState<T>>(state: TConcreteState, entities: T[]): TConcreteState {
        return ({
            ...state,
            entities: entities,
            loaded: true,
            error: null
        });
    }

    public applyLoadErrorReducer<TConcreteState extends MultipleEntitiesState<T>>(state: TConcreteState, error: Error | string): TConcreteState {
        return ({
            ...state,
            entities: [],
            loaded: false,
            error: error
        });
    }

    public getDefaultReducers<TConcreteState extends MultipleEntitiesState<T>>(actionHelper: MultipleEntitiesActionsHelper<T>): On<TConcreteState>[] {
        return <On<TConcreteState>[]>[
            on(actionHelper.loadAction, (state: TConcreteState): TConcreteState => this.applyLoadReducer(state)),
            on(actionHelper.loadedAction, (state: TConcreteState, action) => this.applyLoadedReducer(state, action.entities)),
            on(actionHelper.loadErrorAction, (state: TConcreteState, action) => this.applyLoadErrorReducer(state, action.error)),
        ]
    }
}
