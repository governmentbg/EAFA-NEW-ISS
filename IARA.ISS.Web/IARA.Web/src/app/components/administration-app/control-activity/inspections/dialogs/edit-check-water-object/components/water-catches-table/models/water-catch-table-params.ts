import { InspectionCatchMeasureDTO } from '@app/models/generated/dtos/InspectionCatchMeasureDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';

export class WaterCatchTableParams {
    public readOnly: boolean = false;
    public fishes: NomenclatureDTO<number>[] = [];
    public model: InspectionCatchMeasureDTO | undefined;

    public constructor(params?: Partial<WaterCatchTableParams>) {
        Object.assign(this, params);
    }
}