

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class InspectionSubjectAddressDTO { 
    public constructor(obj?: Partial<InspectionSubjectAddressDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public countryId?: number;

    @StrictlyTyped(Number)
    public districtId?: number;

    @StrictlyTyped(Number)
    public municipalityId?: number;

    @StrictlyTyped(Number)
    public populatedAreaId?: number;

    @StrictlyTyped(String)
    public populatedArea?: string;

    @StrictlyTyped(String)
    public region?: string;

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

    @StrictlyTyped(String)
    public postCode?: string;
}