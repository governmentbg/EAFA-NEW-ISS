

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO'; 

export class FishingCapacityCertificateNomenclatureDTO extends NomenclatureDTO<number> {
    public constructor(obj?: Partial<FishingCapacityCertificateNomenclatureDTO>) {
        if (obj != undefined) {
            super(obj as NomenclatureDTO<number>);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Number)
    public grossTonnage?: number;

    @StrictlyTyped(Number)
    public power?: number;

    @StrictlyTyped(Date)
    public validTo?: Date;
}