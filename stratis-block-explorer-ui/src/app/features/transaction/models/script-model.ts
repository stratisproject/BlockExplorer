export interface IScriptModel {
   length?: number | undefined;
   isPushOnly?: boolean | undefined;
   hasCanonicalPushes?: boolean | undefined;
   hash?: string | undefined;
   paymentScriptHash?: string | undefined;
   isUnspendable?: boolean | undefined;
   isSmartContract?: boolean | undefined;
   addresses?: string[] | undefined;
   isValid?: boolean | undefined;
}

export class ScriptModel implements IScriptModel {
   length?: number | undefined;
   isPushOnly?: boolean | undefined;
   hasCanonicalPushes?: boolean | undefined;
   hash?: string | undefined;
   paymentScriptHash?: string | undefined;
   isUnspendable?: boolean | undefined;
   isSmartContract?: boolean | undefined;
   addresses?: string[] | undefined;
   isValid?: boolean | undefined;
}
