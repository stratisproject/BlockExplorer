import { createAction, props } from '@ngrx/store';

export class EntityActionsHelper<TEntity> {
    //single entity
    public readonly loadEntityAction = this.loadEntity();
    public readonly entityLoadErrorAction = this.entityLoadError();
    public readonly entityLoadedAction = this.entityLoaded();

    //entity list
    public readonly loadEntitiesAction = this.loadEntities();
    public readonly entitiesLoadErrorAction = this.entitiesLoadError();
    public readonly entitiesLoadedAction = this.entitiesLoaded();

    constructor(public context: string) {
    }

    private loadEntity() {
        return createAction(
            `[${this.context}] Load Entity`,
            (id: string | number) => ({ id })
        );
    }

    private entityLoadError() {
        return createAction(
            `[${this.context}] Entity Load Error`,
            props<{ error: Error | string }>()
        );
    }

    private entityLoaded() {
        return createAction(
            `[${this.context}] Entity Loaded`,
            props<{ entity: TEntity }>()
        );
    }

    private loadEntities() {
        return createAction(
            `[${this.context}] Load Entities`,
            (from: number, records: number) => ({ from, records })
        );
    }

    private entitiesLoadError() {
        return createAction(
            `[${this.context}] Entities Load Error`,
            props<{ error: Error | string }>()
        );
    }

    private entitiesLoaded() {
        return createAction(
            `[${this.context}] Entities Loaded`,
            props<{ entities: TEntity[] }>()
        );
    }
}
