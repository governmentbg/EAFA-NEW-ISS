

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { FluxSalesTypeEnum } from '@app/enums/flux-sales-type.enum';

export class FluxSalesQueryRequestEditDTO { 
    public constructor(obj?: Partial<FluxSalesQueryRequestEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public queryType?: FluxSalesTypeEnum;

    @StrictlyTyped(Date)
    public dateTimeFrom?: Date;

    @StrictlyTyped(Date)
    public dateTimeTo?: Date;

    @StrictlyTyped(String)
    public vesselCFR?: string;

    @StrictlyTyped(String)
    public salesID?: string;

    @StrictlyTyped(String)
    public tripID?: string;
}