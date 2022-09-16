import { InspectionToggleTypesEnum } from '@app/enums/inspection-toggle-types.enum';
import { InspectionCheckTypeNomenclatureDTO } from '@app/models/generated/dtos/InspectionCheckTypeNomenclatureDTO';

export class InspectionCheckModel extends InspectionCheckTypeNomenclatureDTO {
    public description: string | undefined;
    public checkValue: InspectionToggleTypesEnum | undefined;

    public constructor(params?: Partial<InspectionCheckModel>) {
        super(params);
        Object.assign(this, params);
    }
}