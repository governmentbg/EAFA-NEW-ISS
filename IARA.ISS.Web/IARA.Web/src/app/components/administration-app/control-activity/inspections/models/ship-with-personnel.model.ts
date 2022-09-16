import { VesselDuringInspectionDTO } from '@app/models/generated/dtos/VesselDuringInspectionDTO';
import { InspectionSubjectPersonnelDTO } from '@app/models/generated/dtos/InspectionSubjectPersonnelDTO';
import { InspectionCheckDTO } from '@app/models/generated/dtos/InspectionCheckDTO';
import { PortVisitDTO } from '@app/models/generated/dtos/PortVisitDTO';
import { InspectionObservationTextDTO } from '@app/models/generated/dtos/InspectionObservationTextDTO';

export class ShipWithPersonnelModel {
    public ship: VesselDuringInspectionDTO | undefined;
    public personnel: InspectionSubjectPersonnelDTO[] = [];
    public toggles: InspectionCheckDTO[] = [];
    public port: PortVisitDTO | undefined;
    public observationTexts: InspectionObservationTextDTO[] = [];

    public constructor(params?: Partial<ShipWithPersonnelModel>) {
        Object.assign(this, params);
    }
}