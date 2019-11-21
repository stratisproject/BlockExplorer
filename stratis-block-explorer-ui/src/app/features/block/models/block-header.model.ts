export interface IBlockHeaderModel {
   currentVersion?: number | undefined;
   hashPrevBlock?: string | undefined;
   time?: number | undefined;
   bits?: string | undefined;
   version?: number | undefined;
   nonce?: number | undefined;
   hashMerkleRoot?: string | undefined;
   isNull?: boolean | undefined;
   blockTime?: Date | undefined;
}

export class BlockHeaderModel implements IBlockHeaderModel {
   currentVersion?: number | undefined;
   hashPrevBlock?: string | undefined;
   time?: number | undefined;
   bits?: string | undefined;
   version?: number | undefined;
   nonce?: number | undefined;
   hashMerkleRoot?: string | undefined;
   isNull?: boolean | undefined;
   blockTime?: Date | undefined;

   constructor(data?: IBlockHeaderModel) {
      if (data) {
         for (const property in data) {
            if (data.hasOwnProperty(property))
               (<any>this)[property] = (<any>data)[property];
         }
      }
   }

   static fromJS(data: any): BlockHeaderModel {
      data = typeof data === 'object' ? data : {};
      const result = new BlockHeaderModel();
      result.init(data);
      return result;
   }

   init(data?: any) {
      if (data) {
         this.currentVersion = data["currentVersion"];
         this.hashPrevBlock = data["hashPrevBlock"];
         this.time = data["time"];
         this.bits = data["bits"];
         this.version = data["version"];
         this.nonce = data["nonce"];
         this.hashMerkleRoot = data["hashMerkleRoot"];
         this.isNull = data["isNull"];
         this.blockTime = data["blockTime"] ? new Date(data["blockTime"].toString()) : <any>undefined;
      }
   }

   toJSON(data?: any) {
      data = typeof data === 'object' ? data : {};
      data["currentVersion"] = this.currentVersion;
      data["hashPrevBlock"] = this.hashPrevBlock;
      data["time"] = this.time;
      data["bits"] = this.bits;
      data["version"] = this.version;
      data["nonce"] = this.nonce;
      data["hashMerkleRoot"] = this.hashMerkleRoot;
      data["isNull"] = this.isNull;
      data["blockTime"] = this.blockTime ? this.blockTime.toISOString() : <any>undefined;
      return data;
   }
}
