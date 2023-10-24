

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { FishingActivityReportItemDTO } from './FishingActivityReportItemDTO';
import { FishingActivityReportPageDTO } from './FishingActivityReportPageDTO';

export class FishingActivityReportDTO { 
    public constructor(obj?: Partial<FishingActivityReportDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public tripIdentifier?: string;

    @StrictlyTyped(String)
    public shipCfr?: string;

    @StrictlyTyped(String)
    public shipName?: string;

    @StrictlyTyped(Date)
    public startTime?: Date;

    @StrictlyTyped(Date)
    public endTime?: Date;

    @StrictlyTyped(Date)
    public unloadTime?: Date;

    @StrictlyTyped(FishingActivityReportItemDTO)
    public items?: FishingActivityReportItemDTO[];

    @StrictlyTyped(FishingActivityReportPageDTO)
    public pages?: FishingActivityReportPageDTO[];
}