export interface IBlockHeaderResponseModel {
   version?: string | undefined;
   hash?: string | undefined;
   previous?: string | undefined;
   time?: Date | undefined;
   nonce?: number | undefined;
   hashMerkelRoot?: string | undefined;
   bits?: string | undefined;
   difficulty?: number | undefined;
}

export class BlockHeaderResponseModel implements IBlockHeaderResponseModel {
  version?: string | undefined;
  hash?: string | undefined;
  previous?: string | undefined;
  time?: Date | undefined;
  nonce?: number | undefined;
  hashMerkelRoot?: string | undefined;
  bits?: string | undefined;
  difficulty?: number | undefined;

  constructor(data?: IBlockHeaderResponseModel) {
    if (data) {
      for (const property in data) {
        if (data.hasOwnProperty(property))
          (<any>this)[property] = (<any>data)[property];
      }
    }
  }

  static fromJS(data: any): BlockHeaderResponseModel {
    data = typeof data === 'object' ? data : {};
    const result = new BlockHeaderResponseModel();
    result.init(data);
    return result;
  }

  init(data?: any) {
    if (data) {
      this.version = data["version"];
      this.hash = data["hash"];
      this.previous = data["previous"];
      this.time = data["time"] ? new Date(data["time"].toString()) : <any>undefined;
      this.nonce = data["nonce"];
      this.hashMerkelRoot = data["hashMerkelRoot"];
      this.bits = data["bits"];
      this.difficulty = data["difficulty"];
    }
  }

  toJSON(data?: any) {
    data = typeof data === 'object' ? data : {};
    data["version"] = this.version;
    data["hash"] = this.hash;
    data["previous"] = this.previous;
    data["time"] = this.time ? this.time.toISOString() : <any>undefined;
    data["nonce"] = this.nonce;
    data["hashMerkelRoot"] = this.hashMerkelRoot;
    data["bits"] = this.bits;
    data["difficulty"] = this.difficulty;
    return data;
  }
}
