import { Action } from '@ngrx/store';
import { Entity } from './smart-contracts.reducer';

export enum SmartContractsActionTypes {
  LoadSmartContracts = '[SmartContracts] Load SmartContracts',
  SmartContractsLoaded = '[SmartContracts] SmartContracts Loaded',
  SmartContractsLoadError = '[SmartContracts] SmartContracts Load Error'
}

export class LoadSmartContracts implements Action {
  readonly type = SmartContractsActionTypes.LoadSmartContracts;
}

export class SmartContractsLoadError implements Action {
  readonly type = SmartContractsActionTypes.SmartContractsLoadError;
  constructor(public payload: any) {}
}

export class SmartContractsLoaded implements Action {
  readonly type = SmartContractsActionTypes.SmartContractsLoaded;
  constructor(public payload: Entity[]) {}
}

export type SmartContractsAction =
  | LoadSmartContracts
  | SmartContractsLoaded
  | SmartContractsLoadError;

export const fromSmartContractsActions = {
  LoadSmartContracts,
  SmartContractsLoaded,
  SmartContractsLoadError
};
