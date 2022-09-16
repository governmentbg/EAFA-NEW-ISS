import { WaterInspectionFishingGearDTO } from '@app/models/generated/dtos/WaterInspectionFishingGearDTO';

export class WaterFishingGearTableParams {
    public readOnly: boolean = false;
    public model: WaterInspectionFishingGearDTO | undefined;

    public constructor(params?: Partial<WaterFishingGearTableParams>) {
        Object.assign(this, params);
    }
}