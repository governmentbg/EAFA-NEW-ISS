import { InspectedCPFishingGearDTO } from '@app/models/generated/dtos/InspectedCPFishingGearDTO';

export class CPFishingGearTableModel extends InspectedCPFishingGearDTO {
    public fishingGearName: string | undefined;

    public constructor(params?: Partial<CPFishingGearTableModel>) {
        super(params);
        Object.assign(this, params);
    }
}