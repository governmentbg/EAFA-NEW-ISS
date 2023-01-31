import { VesselDuringInspectionDTO } from '@app/models/generated/dtos/VesselDuringInspectionDTO';

export class PatrolVehicleTableParams {
    public isWaterVehicle: boolean | undefined = false;
    public readOnly: boolean = false;
    public isEdit: boolean = false;
    public model: VesselDuringInspectionDTO | undefined;
    public excludeIds: number[] = [];
    public hasCoordinates: boolean = true;

    public constructor(params?: Partial<PatrolVehicleTableParams>) {
        Object.assign(this, params);
    }
}