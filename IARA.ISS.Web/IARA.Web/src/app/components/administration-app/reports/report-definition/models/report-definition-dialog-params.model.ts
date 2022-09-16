import { BaseDialogParamsModel } from '@app/models/common/base-dialog-params.model';
import { ReportParameterDTO } from '@app/models/generated/dtos/ReportParameterDTO';

export class ReportDefinitionDialogParams extends BaseDialogParamsModel {
    public parameter!: ReportParameterDTO;

    public constructor(obj?: Partial<ReportDefinitionDialogParams>) {
        super(obj);
        Object.assign(this, obj);
    }
}