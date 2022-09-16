

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class NReportParameterDTO { 
    public constructor(obj?: Partial<NReportParameterDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public code?: string;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(String)
    public description?: string;

    @StrictlyTyped(String)
    public dataType?: string;

    @StrictlyTyped(String)
    public defaultValue?: string;

    @StrictlyTyped(String)
    public errorMessage?: string;

    @StrictlyTyped(String)
    public pattern?: string;

    @StrictlyTyped(Date)
    public dateFrom?: Date;

    @StrictlyTyped(Date)
    public dateTo?: Date;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}