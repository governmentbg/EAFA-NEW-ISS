

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO';
import { FishQuotaDTO } from './FishQuotaDTO';
import { FishFamilyTypesEnum } from '@app/enums/fish-family-types.enum'; 

export class FishNomenclatureDTO extends NomenclatureDTO<number> {
    public constructor(obj?: Partial<FishNomenclatureDTO>) {
        if (obj != undefined) {
            super(obj as NomenclatureDTO<number>);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(FishQuotaDTO)
    public quotas?: FishQuotaDTO[];

    @StrictlyTyped(Number)
    public familyType?: FishFamilyTypesEnum;

    @StrictlyTyped(Number)
    public minCatchSize?: number;

    @StrictlyTyped(Boolean)
    public isDanube?: boolean;

    @StrictlyTyped(Boolean)
    public isBlackSea?: boolean;

    @StrictlyTyped(Boolean)
    public isInternal?: boolean;

    @StrictlyTyped(Boolean)
    public isCommon?: boolean;
}