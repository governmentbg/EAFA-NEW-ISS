import { OriginDeclarationFishDTO } from '@app/models/generated/dtos/OriginDeclarationFishDTO';

export class FishGroupedQuantitiesModel {
    public fishId!: number | undefined;
    public quantity: number | undefined;
    public catchQuadrantId: number | undefined;
    public turbotSizeGroupId: number | undefined;
    public turbotCount: number | undefined;
    public fishName: string | undefined;
    public catchZone: string | undefined;
    public turbotSizeGroupName: string | undefined;
    public isDiscarded: boolean = false;
    public catchFishStateId: number | undefined;
    public catchFishPresentationId: number | undefined;
    public catchFishPreservationId: number | undefined;
    public isActive: boolean | undefined;
    public isValid: boolean | undefined;
    public declarationFishes: OriginDeclarationFishDTO[] = [];

    public constructor(obj?: Partial<FishGroupedQuantitiesModel>) {
        Object.assign(this, obj);
    }
}