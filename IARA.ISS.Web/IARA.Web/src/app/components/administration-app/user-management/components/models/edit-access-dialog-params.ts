import { IUserManagementService } from "@app/interfaces/administration-app/user-management.interface";
import { DialogParamsModel } from "@app/models/common/dialog-params.model";

export class EditAccessDialogParams extends DialogParamsModel {
    public service: IUserManagementService;
    public matCardTitleLabel: string;
    public userFullName: string;

    public constructor(id: number,
        service: IUserManagementService,
        matCardTitleLabel: string,
        userFullName: string,
        readOnly: boolean = false) {
        super({ id: id, isReadonly: readOnly });
        this.service = service;
        this.matCardTitleLabel = matCardTitleLabel;
        this.userFullName = userFullName;
    }
}