import { createSelector, MemoizedSelector } from '@ngrx/store';
import { MultipleEntitiesState } from './multiple-entities-state';

export class MultipleEntitySelectorsHelper<TEntity, TEntityState extends MultipleEntitiesState<TEntity>> {
    public readonly getEntities = this.createGetEntities();
    public readonly getLoaded = this.createGetLoaded();
    public readonly getError = this.createGetError();

    constructor(private stateSelector: (state: any) => TEntityState) { }

    private createGetEntities(): MemoizedSelector<TEntityState, TEntity[]> {
        return createSelector(this.stateSelector, (state) => state.entities);
    }

    private createGetLoaded() {
        return createSelector(this.stateSelector, (state) => state.loaded);
    }

    private createGetError()  {
        return createSelector(this.stateSelector, (state) => state.error);
    }
}
