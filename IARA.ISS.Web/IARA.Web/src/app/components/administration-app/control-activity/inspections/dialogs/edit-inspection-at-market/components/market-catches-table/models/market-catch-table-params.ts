import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { InspectedDeclarationCatchDTO } from '@app/models/generated/dtos/InspectedDeclarationCatchDTO';

export class MarketCatchTableParams {
    public readOnly: boolean = false;
    public hasCatchType!: boolean;
    public hasUndersizedCheck!: boolean;
    public hasUnloadedQuantity!: boolean;
    public fishes: NomenclatureDTO<number>[] = [];
    public types: NomenclatureDTO<number>[] = [];
    public catchZones: NomenclatureDTO<number>[] = [];
    public presentations: NomenclatureDTO<number>[] = [];
    public model: InspectedDeclarationCatchDTO | undefined;

    public constructor(params?: Partial<MarketCatchTableParams>) {
        Object.assign(this, params);
    }
}