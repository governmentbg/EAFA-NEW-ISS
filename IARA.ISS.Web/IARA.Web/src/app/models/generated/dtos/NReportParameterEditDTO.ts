

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ReportParameterTypeEnum } from '@app/enums/report-parameter-type.enum';

export class NReportParameterEditDTO { 
    public constructor(obj?: Partial<NReportParameterEditDTO>) {
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

    @StrictlyTyped(Date)
    public dateFrom?: Date;

    @StrictlyTyped(Date)
    public dateTo?: Date;

    @StrictlyTyped(Number)
    public dataType?: ReportParameterTypeEnum;

    @StrictlyTyped(String)
    public nomenclatureSQL?: string;

    @StrictlyTyped(String)
    public defaultValue?: string;

    @StrictlyTyped(String)
    public pattern?: string;

    @StrictlyTyped(String)
    public errorMessage?: string;
}