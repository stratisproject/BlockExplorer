import { Action } from '@ngrx/store';
import { Entity } from './global.reducer';

export enum GlobalActionTypes {
  IndentifyEntity = '[Global] Indentify Entity',
  Identified = '[Global] Identified',
  IdentificationError = '[Global] Identification Error'
}

export class IndentifyEntity implements Action {
  readonly type = GlobalActionTypes.IndentifyEntity;
  constructor(public id: string) {}
}

export class IdentificationError implements Action {
  readonly type = GlobalActionTypes.IdentificationError;
  constructor(public payload: any) {}
}

export class Identified implements Action {
  readonly type = GlobalActionTypes.Identified;
  constructor(public payload: any) {}
}

export type GlobalAction = IndentifyEntity | Identified | IdentificationError;

export const fromGlobalActions = {
  IndentifyEntity,
  Identified,
  IdentificationError
};
