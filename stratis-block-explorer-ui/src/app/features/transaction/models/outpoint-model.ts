export interface IOutPointModel {
   isNull?: boolean | undefined;
   hash?: string | undefined;
   n?: number | undefined;
}

export class OutPointModel implements IOutPointModel {
  isNull?: boolean | undefined;
  hash?: string | undefined;
  n?: number | undefined;
}
