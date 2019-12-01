export interface ISequenceModel {
    value?: number | undefined;
    isRelativeLock?: boolean | undefined;
    isRBF?: boolean | undefined;
}

export class SequenceModel implements ISequenceModel {
  value?: number | undefined;
  isRelativeLock?: boolean | undefined;
  isRBF?: boolean | undefined;

  constructor(data?: ISequenceModel) {
    if (data) {
      for (const property in data) {
        if (data.hasOwnProperty(property))
          (<any>this)[property] = (<any>data)[property];
      }
    }
  }
}
