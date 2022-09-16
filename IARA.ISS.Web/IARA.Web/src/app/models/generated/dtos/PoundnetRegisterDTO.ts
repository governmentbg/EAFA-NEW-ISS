

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { PoundnetCoordinateDTO } from './PoundnetCoordinateDTO';

export class PoundnetRegisterDTO { 
    public constructor(obj?: Partial<PoundnetRegisterDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public poundNetNum?: string;

    @StrictlyTyped(Date)
    public registrationDate?: Date;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(Number)
    public seasonTypeId?: number;

    @StrictlyTyped(Number)
    public categoryTypeId?: number;

    @StrictlyTyped(String)
    public activityOrderNum?: string;

    @StrictlyTyped(Date)
    public activityOrderDate?: Date;

    @StrictlyTyped(String)
    public comments?: string;

    @StrictlyTyped(String)
    public areaDescription?: string;

    @StrictlyTyped(Number)
    public depthFrom?: number;

    @StrictlyTyped(Number)
    public depthTo?: number;

    @StrictlyTyped(Number)
    public towelLength?: number;

    @StrictlyTyped(Number)
    public houseWidth?: number;

    @StrictlyTyped(Number)
    public houseLength?: number;

    @StrictlyTyped(Number)
    public bagEyeSize?: number;

    @StrictlyTyped(Number)
    public districtId?: number;

    @StrictlyTyped(Number)
    public municipalityId?: number;

    @StrictlyTyped(Number)
    public populatedAreaId?: number;

    @StrictlyTyped(String)
    public region?: string;

    @StrictlyTyped(String)
    public locationDescription?: string;

    @StrictlyTyped(Number)
    public statusId?: number;

    @StrictlyTyped(Number)
    public permitLicensePrice?: number;

    @StrictlyTyped(PoundnetCoordinateDTO)
    public poundnetCoordinates?: PoundnetCoordinateDTO[];
}