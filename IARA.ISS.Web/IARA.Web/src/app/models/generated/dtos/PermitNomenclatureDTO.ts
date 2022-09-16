

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO';
import { CommercialFishingTypesEnum } from '@app/enums/commercial-fishing-types.enum'; 

export class PermitNomenclatureDTO extends NomenclatureDTO<number> {
    public constructor(obj?: Partial<PermitNomenclatureDTO>) {
        if (obj != undefined) {
            super(obj as NomenclatureDTO<number>);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Number)
    public type?: CommercialFishingTypesEnum;

    @StrictlyTyped(String)
    public captainName?: string;

    @StrictlyTyped(String)
    public shipOwnerName?: string;

    @StrictlyTyped(Number)
    public shipOwnerPersonId?: number;

    @StrictlyTyped(Number)
    public shipOwnerLegalId?: number;

    @StrictlyTyped(Number)
    public captainId?: number;
}