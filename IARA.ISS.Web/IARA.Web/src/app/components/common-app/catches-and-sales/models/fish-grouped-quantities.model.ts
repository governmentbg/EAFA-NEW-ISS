export class FishGroupedQuantitiesModel {
    public fishId!: number | undefined;
    public quantity: number | undefined;
    public catchQuadrantId: number | undefined;
    public turbotSizeGroupId: number | undefined;
    public turbotCount: number | undefined;
    public fishName: string | undefined;
    public catchLocation: string | undefined;
    public turbotSizeGroupName: string | undefined;
    public isDiscarded: boolean = false;

    public constructor(obj?: Partial<FishGroupedQuantitiesModel>) {
        Object.assign(this, obj);
    }
}