export interface IBlockHeaderResponseModel {
    version?: string | undefined;
    hash?: string | undefined;
    previous?: string | undefined;
    time?: string | undefined;
    nonce?: number | undefined;
    hashMerkelRoot?: string | undefined;
    bits?: string | undefined;
    difficulty?: number | undefined;
}

export class BlockHeaderResponseModel implements IBlockHeaderResponseModel {
    version?: string | undefined;
    hash?: string | undefined;
    previous?: string | undefined;
    time?: string | undefined;
    nonce?: number | undefined;
    hashMerkelRoot?: string | undefined;
    bits?: string | undefined;
    difficulty?: number | undefined;
}
