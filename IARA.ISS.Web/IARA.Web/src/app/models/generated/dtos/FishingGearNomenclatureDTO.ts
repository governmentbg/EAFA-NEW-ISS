

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO';
import { FishingGearParameterTypesEnum } from '@app/enums/fishing-gear-parameter-types.enum'; 

export class FishingGearNomenclatureDTO extends NomenclatureDTO<number> {
    public constructor(obj?: Partial<FishingGearNomenclatureDTO>) {
        if (obj != undefined) {
            super(obj as NomenclatureDTO<number>);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Number)
    public type?: FishingGearParameterTypesEnum;

    @StrictlyTyped(Boolean)
    public isForMutualFishing?: boolean;

    @StrictlyTyped(Boolean)
    public hasHooks?: boolean;

    @StrictlyTyped(Number)
    public permitLicenseIds?: number[];
}