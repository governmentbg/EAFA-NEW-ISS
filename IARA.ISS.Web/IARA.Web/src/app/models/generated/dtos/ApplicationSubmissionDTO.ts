
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { RegixPersonDataDTO } from './RegixPersonDataDTO';
import { FileInfoDTO } from './FileInfoDTO';
import { AddressRegistrationDTO } from './AddressRegistrationDTO';
import { LetterOfAttorneyDTO } from './LetterOfAttorneyDTO';
import { RegixLegalDataDTO } from './RegixLegalDataDTO';

export class ApplicationSubmissionDTO {
    public constructor(obj?: Partial<ApplicationSubmissionDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(RegixPersonDataDTO)
    public submittedByPerson?: RegixPersonDataDTO;

    @StrictlyTyped(FileInfoDTO)
    public submittedByPersonPhoto?: FileInfoDTO;

    @StrictlyTyped(AddressRegistrationDTO)
    public submittedByPersonAddresses?: AddressRegistrationDTO[];

    @StrictlyTyped(LetterOfAttorneyDTO)
    public submittedByLetterOfAttorney?: LetterOfAttorneyDTO;

    @StrictlyTyped(RegixPersonDataDTO)
    public submittedForPerson?: RegixPersonDataDTO;

    @StrictlyTyped(FileInfoDTO)
    public submittedForPersonPhoto?: FileInfoDTO;

    @StrictlyTyped(RegixLegalDataDTO)
    public submittedForLegal?: RegixLegalDataDTO;

    @StrictlyTyped(AddressRegistrationDTO)
    public submittedForAddresses?: AddressRegistrationDTO[];

    @StrictlyTyped(String)
    public applicationDraft?: string;
}