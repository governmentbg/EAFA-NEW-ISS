

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { InspectorDuringInspectionDTO } from './InspectorDuringInspectionDTO';
import { AuanViolatedRegulationDTO } from './AuanViolatedRegulationDTO';
import { FileInfoDTO } from './FileInfoDTO';
import { InspectionSubjectPersonnelDTO } from './InspectionSubjectPersonnelDTO';
import { InspectionCheckDTO } from './InspectionCheckDTO';
import { VesselDuringInspectionDTO } from './VesselDuringInspectionDTO';
import { InspectionObservationTextDTO } from './InspectionObservationTextDTO';
import { InspectionStatesEnum } from '@app/enums/inspection-states.enum';
import { InspectionTypesEnum } from '@app/enums/inspection-types.enum';

export class InspectionEditDTO { 
    public constructor(obj?: Partial<InspectionEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public inspectionState?: InspectionStatesEnum;

    @StrictlyTyped(String)
    public reportNum?: string;

    @StrictlyTyped(Date)
    public startDate?: Date;

    @StrictlyTyped(Date)
    public endDate?: Date;

    @StrictlyTyped(Number)
    public inspectionType?: InspectionTypesEnum;

    @StrictlyTyped(Boolean)
    public byEmergencySignal?: boolean;

    @StrictlyTyped(String)
    public inspectorComment?: string;

    @StrictlyTyped(Boolean)
    public administrativeViolation?: boolean;

    @StrictlyTyped(String)
    public actionsTaken?: string;

    @StrictlyTyped(InspectorDuringInspectionDTO)
    public inspectors?: InspectorDuringInspectionDTO[];

    @StrictlyTyped(AuanViolatedRegulationDTO)
    public violatedRegulations?: AuanViolatedRegulationDTO[];

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];

    @StrictlyTyped(InspectionSubjectPersonnelDTO)
    public personnel?: InspectionSubjectPersonnelDTO[];

    @StrictlyTyped(InspectionCheckDTO)
    public checks?: InspectionCheckDTO[];

    @StrictlyTyped(VesselDuringInspectionDTO)
    public patrolVehicles?: VesselDuringInspectionDTO[];

    @StrictlyTyped(InspectionObservationTextDTO)
    public observationTexts?: InspectionObservationTextDTO[];

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}