import { InspectionCheckDTO } from '@app/models/generated/dtos/InspectionCheckDTO';
import { InspectionSubjectPersonnelDTO } from '@app/models/generated/dtos/InspectionSubjectPersonnelDTO';
import { VesselDuringInspectionDTO } from '@app/models/generated/dtos/VesselDuringInspectionDTO';
import { PortVisitDTO } from '@app/models/generated/dtos/PortVisitDTO';
import { InspectionCatchMeasureDTO } from '@app/models/generated/dtos/InspectionCatchMeasureDTO';
import { InspectedFishingGearDTO } from '@app/models/generated/dtos/InspectedFishingGearDTO';
import { InspectionLogBookDTO } from '@app/models/generated/dtos/InspectionLogBookDTO';
import { InspectionPermitDTO } from '@app/models/generated/dtos/InspectionPermitDTO';
import { InspectionObservationTextDTO } from '@app/models/generated/dtos/InspectionObservationTextDTO';

export class InspectedShipSectionsModel {
    public ship: VesselDuringInspectionDTO | undefined;
    public personnel: InspectionSubjectPersonnelDTO[] = [];
    public checks: InspectionCheckDTO[] = [];
    public port: PortVisitDTO | undefined;
    public permitLicenses: InspectionPermitDTO[] = [];
    public permits: InspectionPermitDTO[] = [];
    public catches: InspectionCatchMeasureDTO[] = [];
    public fishingGears: InspectedFishingGearDTO[] = [];
    public logBooks: InspectionLogBookDTO[] = [];
    public observationTexts: InspectionObservationTextDTO[] = [];

    public constructor(params?: Partial<InspectedShipSectionsModel>) {
        Object.assign(this, params);
    }
}