import { IApplicationsService } from "@app/interfaces/administration-app/applications.interface";
import { PermittedFileTypeDTO } from "@app/models/generated/dtos/PermittedFileTypeDTO";

export class UploadFileDialogParams {
    public applicationId!: number;
    public service!: IApplicationsService;

    public constructor(obj?: Partial<UploadFileDialogParams>) {
        Object.assign(this, obj);
    }
}