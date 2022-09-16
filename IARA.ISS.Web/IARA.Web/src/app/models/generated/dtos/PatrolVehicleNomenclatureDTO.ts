

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO';
import { PatrolVehicleTypeEnum } from '@app/enums/patrol-vehicle-type.enum'; 

export class PatrolVehicleNomenclatureDTO extends NomenclatureDTO<number> {
    public constructor(obj?: Partial<PatrolVehicleNomenclatureDTO>) {
        if (obj != undefined) {
            super(obj as NomenclatureDTO<number>);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(String)
    public registrationNumber?: string;

    @StrictlyTyped(Number)
    public institutionId?: number;

    @StrictlyTyped(Number)
    public flagId?: number;

    @StrictlyTyped(Number)
    public patrolVehicleTypeId?: number;

    @StrictlyTyped(Number)
    public vehicleType?: PatrolVehicleTypeEnum;
}