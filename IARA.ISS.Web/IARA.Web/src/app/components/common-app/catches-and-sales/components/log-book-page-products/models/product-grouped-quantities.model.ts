import { LogBookPageProductDTO } from '@app/models/generated/dtos/LogBookPageProductDTO';

export class ProductGroupedQuantitiesModel {
    public fishId!: number | undefined;
    public quantityKg: number | undefined;
    public turbotSizeGroupId: number | undefined;
    public unitCount: number | undefined;
    public fishName: string | undefined;
    public turbotSizeGroupName: string | undefined;
    public catchLocation: string | undefined;
    public productPresentationId: number | undefined;
    public productFreshnessId: number | undefined;
    public productPurposeId: number | undefined;
    public price: number | undefined;
    public totalPrice: string | undefined;
    public hasMissingProperties: boolean | undefined;
    public isActive: boolean | undefined;
    public products: LogBookPageProductDTO[] = [];

    public constructor(obj?: Partial<ProductGroupedQuantitiesModel>) {
        Object.assign(this, obj);
    }
}