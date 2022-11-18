import { InspectionCatchMeasureDTO } from '@app/models/generated/dtos/InspectionCatchMeasureDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';

export class InspectedCatchTableModel extends InspectionCatchMeasureDTO {
    public fish: NomenclatureDTO<number> | undefined;
    public type: NomenclatureDTO<number> | undefined;
    public catchZone: NomenclatureDTO<number> | undefined;
    public turbotSizeGroup: NomenclatureDTO<number> | undefined;

    public constructor(params?: Partial<InspectedCatchTableModel>) {
        super(params);
        Object.assign(this, params);
    }
}
