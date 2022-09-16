import { InspectorDuringInspectionDTO } from '@app/models/generated/dtos/InspectorDuringInspectionDTO';

export class InspectionGeneralInfoModel {
    public reportNum: string | undefined;
    public startDate: Date | undefined;
    public endDate: Date | undefined;
    public byEmergencySignal: boolean = false;
    public inspectors: InspectorDuringInspectionDTO[] | undefined;

    public constructor(params?: Partial<InspectionGeneralInfoModel>) {
        Object.assign(this, params);
    }
}