
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { EgnLncDTO } from './EgnLncDTO';
import { PersonDocumentDTO } from './PersonDocumentDTO';

export class RegixPersonDataDTO {
    public constructor(obj?: Partial<RegixPersonDataDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(EgnLncDTO)
    public egnLnc?: EgnLncDTO;

    @StrictlyTyped(String)
    public firstName?: string;

    @StrictlyTyped(String)
    public middleName?: string;

    @StrictlyTyped(String)
    public lastName?: string;

    @StrictlyTyped(PersonDocumentDTO)
    public document?: PersonDocumentDTO;

    @StrictlyTyped(Number)
    public citizenshipCountryId?: number;

    @StrictlyTyped(String)
    public citizenshipCountryName?: string;

    @StrictlyTyped(String)
    public phone?: string;

    @StrictlyTyped(String)
    public email?: string;

    @StrictlyTyped(Date)
    public birthDate?: Date;

    @StrictlyTyped(Boolean)
    public hasBulgarianAddressRegistration?: boolean;

    @StrictlyTyped(Number)
    public genderId?: number;

    @StrictlyTyped(String)
    public genderName?: string;
}