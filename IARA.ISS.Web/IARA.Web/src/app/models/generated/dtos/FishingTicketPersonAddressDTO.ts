
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class FishingTicketPersonAddressDTO {
    public constructor(obj?: Partial<FishingTicketPersonAddressDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public countryId?: number;

    @StrictlyTyped(Number)
    public districtId?: number;

    @StrictlyTyped(Number)
    public municipalityId?: number;

    @StrictlyTyped(Number)
    public populatedAreaId?: number;

    @StrictlyTyped(String)
    public address?: string;
}