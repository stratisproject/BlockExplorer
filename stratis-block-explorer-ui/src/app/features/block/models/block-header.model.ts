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
}
