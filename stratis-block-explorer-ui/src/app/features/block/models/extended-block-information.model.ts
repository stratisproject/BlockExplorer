import { MoneyModel } from '@shared/models/money.model';

export interface IExtendedBlockInformationModel {
   size?: number | undefined;
   strippedSize?: number | undefined;
   transactionCount?: number | undefined;
   blockSubsidy?: MoneyModel | undefined;
   blockReward?: MoneyModel | undefined;
}

export class ExtendedBlockInformationModel implements IExtendedBlockInformationModel {
   size?: number | undefined;
   strippedSize?: number | undefined;
   transactionCount?: number | undefined;
   blockSubsidy?: MoneyModel | undefined;
   blockReward?: MoneyModel | undefined;
}
