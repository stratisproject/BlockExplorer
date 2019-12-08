/**
 * Used as a base state that represents common entity implementations
 * */
export interface EntityState<T> {
    entity: T,
    entityLoaded,
    entityLoadError: Error | string,

    entities: T[],
    entitiesLoaded: boolean,
    entitiesLoadError: Error | string
}
