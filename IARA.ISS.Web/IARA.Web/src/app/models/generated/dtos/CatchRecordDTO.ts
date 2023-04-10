

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { CatchRecordFishDTO } from './CatchRecordFishDTO';

export class CatchRecordDTO { 
    public constructor(obj?: Partial<CatchRecordDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public catchOperationsCount?: number;

    @StrictlyTyped(Number)
    public depth?: number;

    @StrictlyTyped(Boolean)
    public hasGearEntry?: boolean;

    @StrictlyTyped(Date)
    public gearEntryTime?: Date;

    @StrictlyTyped(Boolean)
    public hasGearExit?: boolean;

    @StrictlyTyped(Date)
    public gearExitTime?: Date;

    @StrictlyTyped(Number)
    public transboardedFromShipId?: number;

    @StrictlyTyped(String)
    public totalTime?: string;

    @StrictlyTyped(CatchRecordFishDTO)
    public catchRecordFishes?: CatchRecordFishDTO[];

    @StrictlyTyped(String)
    public catchRecordFishesSummary?: string;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}