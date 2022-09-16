

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO';
import { InspConfiscationActionGroupsEnum } from '@app/enums/insp-confiscation-action-groups.enum'; 

export class AuanConfiscationActionsNomenclatureDTO extends NomenclatureDTO<number> {
    public constructor(obj?: Partial<AuanConfiscationActionsNomenclatureDTO>) {
        if (obj != undefined) {
            super(obj as NomenclatureDTO<number>);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Number)
    public actionGroup?: InspConfiscationActionGroupsEnum;
}