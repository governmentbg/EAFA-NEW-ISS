import { WaterInspectionVesselDTO } from '@app/models/generated/dtos/WaterInspectionVesselDTO';

export class WaterVesselTableParams {
    public readOnly: boolean = false;
    public model: WaterInspectionVesselDTO | undefined;

    public constructor(params?: Partial<WaterVesselTableParams>) {
        Object.assign(this, params);
    }
}