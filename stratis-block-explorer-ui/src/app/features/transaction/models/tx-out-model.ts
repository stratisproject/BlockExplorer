import { ScriptModel } from './script-model';
import { MoneyModel } from '@shared/models/money.model';

export class TxOutModel implements ITxOutModel {
  scriptPubKey?: ScriptModel | undefined;
  isEmpty?: boolean | undefined;
  value?: MoneyModel | undefined;
}

export interface ITxOutModel {
  scriptPubKey?: ScriptModel | undefined;
  isEmpty?: boolean | undefined;
  value?: MoneyModel | undefined;
}
