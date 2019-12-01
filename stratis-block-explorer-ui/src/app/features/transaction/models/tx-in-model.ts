import { SequenceModel } from './sequence-model';
import { OutPointModel } from './outpoint-model';
import { ScriptModel } from './script-model';

export interface ITxInModel {
    sequence?: SequenceModel | undefined;
    prevOut?: OutPointModel | undefined;
    scriptSig?: ScriptModel | undefined;
    isFinal?: boolean | undefined;
}

export class TxInModel implements ITxInModel {
  sequence?: SequenceModel | undefined;
  prevOut?: OutPointModel | undefined;
  scriptSig?: ScriptModel | undefined;
  isFinal?: boolean | undefined;
}
