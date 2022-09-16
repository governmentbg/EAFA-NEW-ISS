import { WaterInspectionEngineDTO } from '@app/models/generated/dtos/WaterInspectionEngineDTO';

export class WaterEngineTableParams {
    public readOnly: boolean = false;
    public model: WaterInspectionEngineDTO | undefined;

    public constructor(params?: Partial<WaterEngineTableParams>) {
        Object.assign(this, params);
    }
}