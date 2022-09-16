import { CommonDocumentDTO } from '@app/models/generated/dtos/CommonDocumentDTO';

export class CommonDocumentDialogParams {
    public model: CommonDocumentDTO | undefined;
    public viewMode: boolean = false;

    public constructor(obj?: Partial<CommonDocumentDialogParams>) {
        Object.assign(this, obj);
    }
}