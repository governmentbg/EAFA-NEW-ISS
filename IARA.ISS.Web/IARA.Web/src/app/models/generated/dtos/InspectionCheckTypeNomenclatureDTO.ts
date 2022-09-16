

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { HasDescrNomenclatureDTO } from './HasDescrNomenclatureDTO';
import { InspectionCheckTypesEnum } from '@app/enums/inspection-check-types.enum'; 

export class InspectionCheckTypeNomenclatureDTO extends HasDescrNomenclatureDTO {
    public constructor(obj?: Partial<InspectionCheckTypeNomenclatureDTO>) {
        if (obj != undefined) {
            super(obj as HasDescrNomenclatureDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Number)
    public inspectionTypeId?: number;

    @StrictlyTyped(Boolean)
    public isMandatory?: boolean;

    @StrictlyTyped(Number)
    public checkType?: InspectionCheckTypesEnum;

    @StrictlyTyped(String)
    public descriptionLabel?: string;
}