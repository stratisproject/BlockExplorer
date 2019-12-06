/**
 * Used as a base state that represents a single entity that got loaded and selected
 * */
export interface SingleEntityState<T> {
    entity: T,
    loaded,
    error: Error | string
}
