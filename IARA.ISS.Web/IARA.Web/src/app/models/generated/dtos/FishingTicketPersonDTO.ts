
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { FishingTicketPersonAddressDTO } from './FishingTicketPersonAddressDTO';

export class FishingTicketPersonDTO {
    public constructor(obj?: Partial<FishingTicketPersonDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public firstName?: string;

    @StrictlyTyped(String)
    public middleName?: string;

    @StrictlyTyped(String)
    public lastName?: string;

    @StrictlyTyped(String)
    public egn?: string;

    @StrictlyTyped(String)
    public idCard?: string;

    @StrictlyTyped(Number)
    public documentTypeId?: number;

    @StrictlyTyped(Number)
    public citizenshipId?: number;

    @StrictlyTyped(Date)
    public dateOfBirth?: Date;

    @StrictlyTyped(Date)
    public idCardDate?: Date;

    @StrictlyTyped(String)
    public idCardPublisher?: string;

    @StrictlyTyped(Boolean)
    public hasBulgarianAddressRegistration?: boolean;

    @StrictlyTyped(Boolean)
    public permanentAddressMatchWithCorrespondence?: boolean;

    @StrictlyTyped(FishingTicketPersonAddressDTO)
    public permanentAddress?: FishingTicketPersonAddressDTO;

    @StrictlyTyped(FishingTicketPersonAddressDTO)
    public correspondenceAddress?: FishingTicketPersonAddressDTO;
}