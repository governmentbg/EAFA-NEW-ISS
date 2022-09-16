import { InspectedFishingGearDTO } from '@app/models/generated/dtos/InspectedFishingGearDTO';

export class InspectedFishingGearTableModel {
    public DTO!: InspectedFishingGearDTO;
    public type: string | undefined;
    public count: number | undefined;
    public netEyeSize: number | undefined;
    public marksNumbers: string | undefined;
    public checkName: string | undefined;

    public constructor(params?: Partial<InspectedFishingGearTableModel>) {
        Object.assign(this, params);
    }
}