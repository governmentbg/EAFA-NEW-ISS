

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { FishingCapacityChangeTypeEnum } from '@app/enums/fishing-capacity-change-type.enum';

export class CapacityChangeHistoryDTO { 
    public constructor(obj?: Partial<CapacityChangeHistoryDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public applicationId?: number;

    @StrictlyTyped(Date)
    public dateOfChange?: Date;

    @StrictlyTyped(Number)
    public typeOfChange?: FishingCapacityChangeTypeEnum;

    @StrictlyTyped(Number)
    public shipCapacityId?: number;

    @StrictlyTyped(Number)
    public acquiredFishingCapacityId?: number;

    @StrictlyTyped(Number)
    public capacityCertificateTransferId?: number;

    @StrictlyTyped(Number)
    public grossTonnageChange?: number;

    @StrictlyTyped(Number)
    public powerChange?: number;

    @StrictlyTyped(String)
    public reasonOfChange?: string;

    @StrictlyTyped(Number)
    public capacityCertificateIds?: number[];

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}