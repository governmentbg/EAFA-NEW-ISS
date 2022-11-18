

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { AddressTypesEnum } from '@app/enums/address-types.enum';

export class AddressRegistrationDTO { 
    public constructor(obj?: Partial<AddressRegistrationDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public addressType?: AddressTypesEnum;

    @StrictlyTyped(Number)
    public countryId?: number;

    @StrictlyTyped(String)
    public countryName?: string;

    @StrictlyTyped(Number)
    public districtId?: number;

    @StrictlyTyped(String)
    public districtName?: string;

    @StrictlyTyped(Number)
    public municipalityId?: number;

    @StrictlyTyped(String)
    public municipalityName?: string;

    @StrictlyTyped(Number)
    public populatedAreaId?: number;

    @StrictlyTyped(String)
    public populatedAreaName?: string;

    @StrictlyTyped(String)
    public region?: string;

    @StrictlyTyped(String)
    public postalCode?: string;

    @StrictlyTyped(String)
    public street?: string;

    @StrictlyTyped(String)
    public streetNum?: string;

    @StrictlyTyped(String)
    public blockNum?: string;

    @StrictlyTyped(String)
    public entranceNum?: string;

    @StrictlyTyped(String)
    public floorNum?: string;

    @StrictlyTyped(String)
    public apartmentNum?: string;
}