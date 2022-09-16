import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { VesselDuringInspectionDTO } from '@app/models/generated/dtos/VesselDuringInspectionDTO';

export class InspectedShipParams {
    public model: VesselDuringInspectionDTO | undefined;
    public ships!: NomenclatureDTO<number>[];
    public hasMap!: boolean;
    public readOnly!: boolean;

    public constructor(params?: Partial<InspectedShipParams>) {
        Object.assign(this, params);
    }
}