

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { CatchRecordDTO } from './CatchRecordDTO';
import { OriginDeclarationFishDTO } from './OriginDeclarationFishDTO';

export class ShipLogBookPageFLUXFieldsDTO { 
    public constructor(obj?: Partial<ShipLogBookPageFLUXFieldsDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public pageId?: number;

    @StrictlyTyped(Number)
    public departurePortId?: number;

    @StrictlyTyped(Number)
    public arrivalPortId?: number;

    @StrictlyTyped(Date)
    public pageFillDate?: Date;

    @StrictlyTyped(Number)
    public fishingGearRegisterId?: number;

    @StrictlyTyped(Number)
    public fishingGearCount?: number;

    @StrictlyTyped(Date)
    public fishTripStartDateTime?: Date;

    @StrictlyTyped(Date)
    public fishTripEndDateTime?: Date;

    @StrictlyTyped(CatchRecordDTO)
    public catchRecords?: CatchRecordDTO[];

    @StrictlyTyped(OriginDeclarationFishDTO)
    public originDeclarationFishes?: OriginDeclarationFishDTO[];

    @StrictlyTyped(Boolean)
    public isCancelled?: boolean;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}