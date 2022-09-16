

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ReportParameterExecuteDTO } from './ReportParameterExecuteDTO';
import { ReportTypesEnum } from '@app/enums/reports-type.enum';

export class ExecuteReportDTO {
    public constructor(obj?: Partial<ExecuteReportDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public reportGroupId?: number;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(String)
    public sqlQuery?: string;

    @StrictlyTyped(Number)
    public reportType?: ReportTypesEnum;

    @StrictlyTyped(ReportParameterExecuteDTO)
    public parameters?: ReportParameterExecuteDTO[];
}