import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { PageCodeEnum } from '@app/enums/page-code.enum';

export class InspectionDialogParamsModel extends DialogParamsModel {
    public canEditNumber: boolean = false;
    public pageCode: PageCodeEnum = PageCodeEnum.Inspections;

    public constructor(params?: Partial<InspectionDialogParamsModel>) {
        super(params);
        Object.assign(this, params);
    }
}