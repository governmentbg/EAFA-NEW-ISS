

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO';
import { PatrolVehicleTypeEnum } from '@app/enums/patrol-vehicle-type.enum'; 

export class PatrolVehicleTypeNomenclatureDTO extends NomenclatureDTO<number> {
    public constructor(obj?: Partial<PatrolVehicleTypeNomenclatureDTO>) {
        if (obj != undefined) {
            super(obj as NomenclatureDTO<number>);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Number)
    public vehicleType?: PatrolVehicleTypeEnum;
}