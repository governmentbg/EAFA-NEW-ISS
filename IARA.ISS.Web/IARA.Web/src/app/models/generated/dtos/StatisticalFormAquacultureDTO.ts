

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO';
import { AquacultureSystemEnum } from '@app/enums/aquaculture-system.enum';

export class StatisticalFormAquacultureDTO { 
    public constructor(obj?: Partial<StatisticalFormAquacultureDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public aquacultureId?: number;

    @StrictlyTyped(String)
    public legalName?: string;

    @StrictlyTyped(String)
    public eik?: string;

    @StrictlyTyped(NomenclatureDTO)
    public facilityInstalations?: NomenclatureDTO<number>[];

    @StrictlyTyped(NomenclatureDTO)
    public fishTypes?: NomenclatureDTO<number>[];

    @StrictlyTyped(Number)
    public systemType?: AquacultureSystemEnum;
}