import { FishingAssociationPersonDTO } from '@app/models/generated/dtos/FishingAssociationPersonDTO';

export class EditLegalAssociationPersonDialogParams {
    public model: FishingAssociationPersonDTO | undefined;
    public isEgnLncReadOnly: boolean = false;
    public readOnly: boolean = false;
    public showOnlyRegiXData: boolean = false;
    public expectedResults: FishingAssociationPersonDTO = new FishingAssociationPersonDTO();

    public constructor(obj?: Partial<EditLegalAssociationPersonDialogParams>) {
        Object.assign(this, obj);
    }
}