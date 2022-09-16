

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO'; 

export class FleetTypeNomenclatureDTO extends NomenclatureDTO<number> {
    public constructor(obj?: Partial<FleetTypeNomenclatureDTO>) {
        if (obj != undefined) {
            super(obj as NomenclatureDTO<number>);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Boolean)
    public hasControlCard?: boolean;

    @StrictlyTyped(Boolean)
    public hasFitnessCertificate?: boolean;

    @StrictlyTyped(Boolean)
    public hasFishingCapacity?: boolean;
}