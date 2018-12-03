import { Action } from '@ngrx/store';
import { Entity } from './global.reducer';

export enum GlobalActionTypes {
  LoadGlobal = '[Global] Load Global',
  GlobalLoaded = '[Global] Global Loaded',
  GlobalLoadError = '[Global] Global Load Error'
}

export class LoadGlobal implements Action {
  readonly type = GlobalActionTypes.LoadGlobal;
}

export class GlobalLoadError implements Action {
  readonly type = GlobalActionTypes.GlobalLoadError;
  constructor(public payload: any) {}
}

export class GlobalLoaded implements Action {
  readonly type = GlobalActionTypes.GlobalLoaded;
  constructor(public payload: Entity[]) {}
}

export type GlobalAction = LoadGlobal | GlobalLoaded | GlobalLoadError;

export const fromGlobalActions = {
  LoadGlobal,
  GlobalLoaded,
  GlobalLoadError
};
