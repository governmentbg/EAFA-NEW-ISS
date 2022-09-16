

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { LetterOfAttorneyDTO } from './LetterOfAttorneyDTO';
import { SubmittedByRolesEnum } from '@app/enums/submitted-by-roles.enum';

export class ApplicantRelationToRecipientDTO { 
    public constructor(obj?: Partial<ApplicantRelationToRecipientDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public role?: SubmittedByRolesEnum;

    @StrictlyTyped(LetterOfAttorneyDTO)
    public letterOfAttorney?: LetterOfAttorneyDTO;
}