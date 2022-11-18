import { AuanViolatedRegulationDTO } from '@app/models/generated/dtos/AuanViolatedRegulationDTO';
import { InspectionObservationTextDTO } from '@app/models/generated/dtos/InspectionObservationTextDTO';

export class InspectionAdditionalInfoModel {
    public violation: InspectionObservationTextDTO | undefined;
    public inspectorComment: string | undefined;
    public actionsTaken: string | undefined;
    public administrativeViolation: boolean = false;
    public violatedRegulations: AuanViolatedRegulationDTO[] = [];

    public constructor(params?: Partial<InspectionAdditionalInfoModel>) {
        Object.assign(this, params);
    }
}