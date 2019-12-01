import { OutPointModel } from './outpoint-model';
import { TxOutModel } from './tx-out-model';

export interface ICoinModel {
  amount?: any | undefined;
  outpoint?: OutPointModel | undefined;
  txOut?: TxOutModel | undefined;
}

export class CoinModel implements ICoinModel {
   amount?: any | undefined;
   outpoint?: OutPointModel | undefined;
   txOut?: TxOutModel | undefined;
}
