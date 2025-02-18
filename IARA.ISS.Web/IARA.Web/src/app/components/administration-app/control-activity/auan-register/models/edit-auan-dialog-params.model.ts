import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { AuanStatusEnum } from '@app/enums/auan-status.enum';

export class EditAuanDialogParams extends DialogParamsModel {
    public inspectionId!: number;
    public isThirdParty: boolean = false;
    public canSaveAfterHours: boolean = false;

    public constructor(obj?: Partial<EditAuanDialogParams>) {
        super(obj);
        Object.assign(this, obj);
    }
}