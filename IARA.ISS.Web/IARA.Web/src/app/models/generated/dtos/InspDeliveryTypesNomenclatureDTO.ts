

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO';
import { InspDeliveryTypeGroupsEnum } from '@app/enums/insp-delivery-type-groups.enum'; 

export class InspDeliveryTypesNomenclatureDTO extends NomenclatureDTO<number> {
    public constructor(obj?: Partial<InspDeliveryTypesNomenclatureDTO>) {
        if (obj != undefined) {
            super(obj as NomenclatureDTO<number>);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Number)
    public group?: InspDeliveryTypeGroupsEnum;
}