import { UsageDocumentDTO } from '@app/models/generated/dtos/UsageDocumentDTO';

export class UsageDocumentDialogParams {
    public model: UsageDocumentDTO | undefined;
    public isIdReadOnly: boolean = false;
    public showOnlyRegiXData: boolean = false;
    public viewMode: boolean = false;

    public constructor(obj?: Partial<UsageDocumentDialogParams>) {
        Object.assign(this, obj);
    }
}