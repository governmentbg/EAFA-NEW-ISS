

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { FluxFAQueryTypesEnum } from '@app/enums/flux-fa-query-types.enum';

export class FluxFAQueryRequestEditDTO { 
    public constructor(obj?: Partial<FluxFAQueryRequestEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public queryType?: FluxFAQueryTypesEnum;

    @StrictlyTyped(Date)
    public dateTimeFrom?: Date;

    @StrictlyTyped(Date)
    public dateTimeTo?: Date;

    @StrictlyTyped(String)
    public vesselCFR?: string;

    @StrictlyTyped(String)
    public vesselIRCS?: string;

    @StrictlyTyped(String)
    public tripIdentifier?: string;

    @StrictlyTyped(Boolean)
    public consolidated?: boolean;
}