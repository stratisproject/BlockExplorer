import { createSelector } from '@ngrx/store';
import { SingleEntityState } from './single-entity-state';

export class SingleEntitySelectorsHelper<TEntity, TEntityState extends SingleEntityState<TEntity>> {
    public readonly getEntity = this.createGetEntity();
    public readonly getLoaded = this.createGetLoaded();
    public readonly getError = this.createGetError();

    constructor(private stateSelector: (state: any) => TEntityState) { }

    private createGetEntity() {
        return createSelector(this.stateSelector, (state) => state.entity);
    }

    private createGetLoaded() {
        return createSelector(this.stateSelector, (state) => state.loaded);
    }

    private createGetError() {
        return createSelector(this.stateSelector, (state) => state.error);
    }
}
