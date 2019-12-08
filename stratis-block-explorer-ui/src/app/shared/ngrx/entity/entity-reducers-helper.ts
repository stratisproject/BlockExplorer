import { EntityState } from './entity-state';
import { On, on } from '@ngrx/store';
import { EntityActionsHelper } from './entity-actions-helper';

/**
 * Helper to write less boilerplate code when writing reducers for states that have to track a multiple entity loading
 * */
export class EntityReducersHelper<TEntity, TEntityState extends EntityState<TEntity>> {
    public getInitialState(): TEntityState {
        return <TEntityState>{
            entity: null,
            entityLoaded: false,
            entityLoadError: null,

            entities: [],
            entitiesLoaded: false,
            entitiesLoadError: null
        };
    }

    public applyLoadEntityReducer(state: TEntityState): TEntityState {
        return {
            ...state,
            entity: null,
            entityLoaded: false,
            entityLoadError: null
        };
    }

    public applyEntityLoadedReducer(state: TEntityState, entity: TEntity): TEntityState {
        return ({
            ...state,
            entity: entity,
            entityLoaded: true,
            entityLoadError: null
        });
    }

    public applyEntityLoadErrorReducer(state: TEntityState, error: Error | string): TEntityState {
        return ({
            ...state,
            entity: null,
            entityLoaded: true,
            entityLoadError: error
        });
    }

    public applyLoadEntitiesReducer(state: TEntityState): TEntityState {
        return {
            ...state,
            entities: [],
            entitiesLoaded: false,
            entitiesLoadError: null
        };
    }

    public applyEntitiesLoadedReducer(state: TEntityState, entities: TEntity[]): TEntityState {
        return ({
            ...state,
            entities: entities,
            entitiesLoaded: true,
            entitiesLoadError: null
        });
    }

    public applyEntitiesLoadErrorReducer(state: TEntityState, error: Error | string): TEntityState {
        return ({
            ...state,
            entities: [],
            entitiesLoaded: true,
            entitiesLoadError: error
        });
    }

    public getDefaultReducers(actionHelper: EntityActionsHelper<TEntity>): On<TEntityState>[] {
        return <On<TEntityState>[]>[
            on(actionHelper.loadEntityAction, (state: TEntityState) => this.applyLoadEntityReducer(state)),
            on(actionHelper.entityLoadedAction, (state: TEntityState, action) => this.applyEntityLoadedReducer(state, action.entity)),
            on(actionHelper.entityLoadErrorAction, (state: TEntityState, action) => this.applyEntityLoadErrorReducer(state, action.error)),
            on(actionHelper.loadEntitiesAction, (state: TEntityState) => this.applyLoadEntitiesReducer(state)),
            on(actionHelper.entitiesLoadedAction, (state: TEntityState, action) => this.applyEntitiesLoadedReducer(state, action.entities)),
            on(actionHelper.entitiesLoadErrorAction, (state: TEntityState, action) => this.applyEntitiesLoadErrorReducer(state, action.error)),
        ]
    }
}
