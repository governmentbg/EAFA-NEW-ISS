

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ExecutionParamDTO } from './ExecutionParamDTO';

export class ExecutionReportInfoDTO { 
    public constructor(obj?: Partial<ExecutionReportInfoDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public reportId?: number;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(String)
    public sqlQuery?: string;

    @StrictlyTyped(ExecutionParamDTO)
    public parameters?: ExecutionParamDTO[];
}