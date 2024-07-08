import { FishingCapacityRemainderActionEnum } from '@app/enums/fishing-capacity-remainder-action.enum';
import { FishingCapacityHolderDTO } from '@app/models/generated/dtos/FishingCapacityHolderDTO';

export class FreedCapacityTariffCalculationParameters {
    public applicationId: number | undefined;
    public hasFishingCapacity: boolean | undefined;
    public action: FishingCapacityRemainderActionEnum | undefined;
    public holders: FishingCapacityHolderDTO[] = [];
    public excludedTariffsIds: number[] | undefined;

    public constructor(obj?: Partial<FreedCapacityTariffCalculationParameters>) {
        Object.assign(this, obj);
    }
}