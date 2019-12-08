import { createSelector, MemoizedSelector } from '@ngrx/store';
import { EntityState } from './entity-state';

export class EntitySelectorsHelper<TEntity, TEntityState extends EntityState<TEntity>> {
    public readonly getEntity = this.createGetEntity();
    public readonly getEntityLoaded = this.createGetEntityLoaded();
    public readonly getEntityLoadError = this.createGetEntityLoadError();

    public readonly getEntities = this.createGetEntities();
    public readonly getEntitiesLoaded = this.createGetEntitiesLoaded();
    public readonly getEntitiesLoadError = this.createGetEntitiesLoadError();

    constructor(private stateSelector: (state: any) => TEntityState) { }

    private createGetEntity(): MemoizedSelector<TEntityState, TEntity> {
        return createSelector(this.stateSelector, (state) => state.entity);
    }

    private createGetEntityLoaded() {
        return createSelector(this.stateSelector, (state) => state.entityLoaded);
    }

    private createGetEntityLoadError() {
        return createSelector(this.stateSelector, (state) => state.entityLoadError);
    }

    private createGetEntities(): MemoizedSelector<TEntityState, TEntity[]> {
        return createSelector(this.stateSelector, (state) => state.entities);
    }

    private createGetEntitiesLoaded() {
        return createSelector(this.stateSelector, (state) => state.entitiesLoaded);
    }

    private createGetEntitiesLoadError() {
        return createSelector(this.stateSelector, (state) => state.entitiesLoadError);
    }
}
