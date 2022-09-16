

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { BaseGridRequestModel } from '../../common/base-grid-request.model';
import { ExecutionParamDTO } from './ExecutionParamDTO';

export class ReportGridRequestDTO extends BaseGridRequestModel {
    public constructor(obj?: Partial<ReportGridRequestDTO>) {
        if (obj != undefined) {
            super();
            Object.assign(this, obj);
        }
        else {
            super();
        }
    }

    @StrictlyTyped(Number)
    public reportId?: number;

    @StrictlyTyped(String)
    public sqlQuery?: string;

    @StrictlyTyped(Number)
    public userId?: number;

    @StrictlyTyped(ExecutionParamDTO)
    public parameters?: ExecutionParamDTO[];
}