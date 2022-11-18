import { FishingCapacityHolderDTO } from '@app/models/generated/dtos/FishingCapacityHolderDTO';
import { FishingCapacityHolderRegixDataDTO } from '@app/models/generated/dtos/FishingCapacityHolderRegixDataDTO';
import { ApplicationSubmittedByDTO } from '@app/models/generated/dtos/ApplicationSubmittedByDTO';

export class TransferFishingCapacityTableEntryParams {
    public readOnly: boolean = false;
    public isEgnLncReadOnly: boolean = false;
    public showOnlyRegixData: boolean = false;

    public remainingTonnage: number | undefined;
    public remainingPower: number | undefined;

    public submittedBy: ApplicationSubmittedByDTO | undefined;

    public expectedResults: FishingCapacityHolderRegixDataDTO | undefined;
    public model: FishingCapacityHolderDTO | FishingCapacityHolderRegixDataDTO | undefined;

    public constructor(params?: Partial<TransferFishingCapacityTableEntryParams>) {
        Object.assign(this, params);
    }
}