

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { FluxIsrTypesEnum } from '@app/enums/flux-isr-types.enum';

export class FLUXISRQueryRequestEditDTO { 
    public constructor(obj?: Partial<FLUXISRQueryRequestEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Date)
    public dateTimeFrom?: Date;

    @StrictlyTyped(Date)
    public dateTimeTo?: Date;

    @StrictlyTyped(Number)
    public queryType?: FluxIsrTypesEnum;

    @StrictlyTyped(String)
    public vesselCFR?: string;

    @StrictlyTyped(String)
    public vesselIRCS?: string;

    @StrictlyTyped(String)
    public vesselUVI?: string;

    @StrictlyTyped(String)
    public vesselExternalMark?: string;

    @StrictlyTyped(String)
    public flagStateCode?: string;

    @StrictlyTyped(String)
    public tractorIdentifier?: string;

    @StrictlyTyped(String)
    public trailerIdentifier?: string;

    @StrictlyTyped(String)
    public registrationCountryCode?: string;
}