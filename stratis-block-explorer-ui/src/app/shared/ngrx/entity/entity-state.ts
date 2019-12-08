/**
 * Used as a base state that represents common entity implementations
 * */
export interface EntityState<T> {
    entity: T,
    entityLoaded,
    entityLoadError: string,

    entities: T[],
    entitiesLoaded: boolean,
    entitiesLoadError: string
}
