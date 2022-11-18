import { FishingAssociationPersonDTO } from '@app/models/generated/dtos/FishingAssociationPersonDTO';

export class EditLegalAssociationPersonResult {
    public person: FishingAssociationPersonDTO | undefined;
    public isTouched: boolean;

    public constructor(person: FishingAssociationPersonDTO | undefined, isTouched: boolean) {
        this.person = person;
        this.isTouched = isTouched;
    }
}