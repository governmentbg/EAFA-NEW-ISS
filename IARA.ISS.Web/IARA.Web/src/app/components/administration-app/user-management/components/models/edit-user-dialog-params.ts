import { IUserManagementService } from "@app/interfaces/administration-app/user-management.interface";
import { DialogParamsModel } from "@app/models/common/dialog-params.model";

export class EditUserDialogParams extends DialogParamsModel {
    public service: IUserManagementService;
    public isInternalUser: boolean;
    public isAddUser: boolean;

    public constructor(id: number, service: IUserManagementService, isInternalUser: boolean, isAddUser: boolean, readOnly: boolean = false) {
        super({ id: id, isReadonly: readOnly });
        this.service = service;
        this.isInternalUser = isInternalUser;
        this.isAddUser = isAddUser;
    }
}