import { VesselDuringInspectionDTO } from '@app/models/generated/dtos/VesselDuringInspectionDTO';

export class PatrolVehicleTableParams {
    public isWaterVehicle: boolean = false;
    public readOnly: boolean = false;
    public isEdit: boolean = false;
    public model: VesselDuringInspectionDTO | undefined;
    public excludeIds: number[] = [];

    public constructor(params?: Partial<PatrolVehicleTableParams>) {
        Object.assign(this, params);
    }
}