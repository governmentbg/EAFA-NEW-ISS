import { FishingCapacityHolderDTO } from '@app/models/generated/dtos/FishingCapacityHolderDTO';
import { FishingCapacityHolderRegixDataDTO } from '@app/models/generated/dtos/FishingCapacityHolderRegixDataDTO';

export class TransferFishingCapacityTableEntryParams {
    public readOnly: boolean = false;
    public isEgnLncReadOnly: boolean = false;
    public showOnlyRegixData: boolean = false;
    public expectedResults: FishingCapacityHolderRegixDataDTO | undefined;
    public model: FishingCapacityHolderDTO | FishingCapacityHolderRegixDataDTO | undefined;

    public constructor(params?: Partial<TransferFishingCapacityTableEntryParams>) {
        Object.assign(this, params);
    }
}