

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { VesselDuringInspectionDTO } from './VesselDuringInspectionDTO';
import { PortVisitDTO } from './PortVisitDTO';
import { InspectionCatchMeasureDTO } from './InspectionCatchMeasureDTO';
import { InspectionSubjectPersonnelDTO } from './InspectionSubjectPersonnelDTO';
import { InspectionCheckDTO } from './InspectionCheckDTO';
import { InspectionPermitDTO } from './InspectionPermitDTO';
import { InspectionLogBookDTO } from './InspectionLogBookDTO';

export class InspectionTransboardingShipDTO { 
    public constructor(obj?: Partial<InspectionTransboardingShipDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public nnnShipStatus?: string;

    @StrictlyTyped(String)
    public captainComment?: string;

    @StrictlyTyped(VesselDuringInspectionDTO)
    public inspectedShip?: VesselDuringInspectionDTO;

    @StrictlyTyped(PortVisitDTO)
    public lastPortVisit?: PortVisitDTO;

    @StrictlyTyped(InspectionCatchMeasureDTO)
    public catchMeasures?: InspectionCatchMeasureDTO[];

    @StrictlyTyped(InspectionSubjectPersonnelDTO)
    public personnel?: InspectionSubjectPersonnelDTO[];

    @StrictlyTyped(InspectionCheckDTO)
    public checks?: InspectionCheckDTO[];

    @StrictlyTyped(InspectionPermitDTO)
    public permitLicenses?: InspectionPermitDTO[];

    @StrictlyTyped(InspectionLogBookDTO)
    public logBooks?: InspectionLogBookDTO[];
}