import { createAction, props } from '@ngrx/store';

export class SingleEntityActionsHelper<TEntity> {
    public readonly loadAction = this.load();
    public readonly loadErrorAction = this.loadError();
    public readonly loadedAction = this.loaded();


    constructor(public context: string) {
    }

    private load() {
        return createAction(
            `[${this.context}] Load entity`,
            (id: string | number) => ({ id })
        );
    }

    private loadError() {
        return createAction(
            `[${this.context}] Load entity Error`,
            props<{ error: Error | string }>()
        );
    }

    private loaded() {
        return createAction(
            `[${this.context}] entity Loaded`,
            props<{ entity: TEntity }>()
        );
    }
}
