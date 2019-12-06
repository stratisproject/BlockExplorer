import { createAction, props } from '@ngrx/store';

export class MultipleEntitiesActionsHelper<T> {
    public readonly loadAction = this.load();
    public readonly loadErrorAction = this.loadError();
    public readonly loadedAction = this.loaded();


    constructor(public context: string) {
    }

    private load() {
        return createAction(
            `[${this.context}] Load entities`,
            (from: number, records: number) => ({ from, records })
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
            props<{ entities: T[] }>()
        );
    }
}
