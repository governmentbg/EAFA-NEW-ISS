

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { InspectionEditDTO } from './InspectionEditDTO';
import { LocationDTO } from './LocationDTO';
import { InspectionLogBookPageDTO } from './InspectionLogBookPageDTO'; 

export class InspectionTransportVehicleDTO extends InspectionEditDTO {
    public constructor(obj?: Partial<InspectionTransportVehicleDTO>) {
        if (obj != undefined) {
            super(obj as InspectionEditDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Number)
    public vehicleTypeId?: number;

    @StrictlyTyped(Number)
    public countryId?: number;

    @StrictlyTyped(String)
    public tractorLicensePlateNum?: string;

    @StrictlyTyped(String)
    public trailerLicensePlateNum?: string;

    @StrictlyTyped(String)
    public tractorBrand?: string;

    @StrictlyTyped(String)
    public tractorModel?: string;

    @StrictlyTyped(Boolean)
    public isSealed?: boolean;

    @StrictlyTyped(Number)
    public sealInstitutionId?: number;

    @StrictlyTyped(String)
    public sealCondition?: string;

    @StrictlyTyped(String)
    public transporterComment?: string;

    @StrictlyTyped(String)
    public inspectionAddress?: string;

    @StrictlyTyped(LocationDTO)
    public inspectionLocation?: LocationDTO;

    @StrictlyTyped(String)
    public transportDestination?: string;

    @StrictlyTyped(InspectionLogBookPageDTO)
    public inspectionLogBookPages?: InspectionLogBookPageDTO[];
}