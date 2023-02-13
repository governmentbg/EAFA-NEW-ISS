

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO';
import { IdentifierTypeEnum } from '@app/enums/identifier-type.enum';
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
    public personFirstName?: string;

    @StrictlyTyped(String)
    public personMiddleName?: string;

    @StrictlyTyped(String)
    public personLastName?: string;

    @StrictlyTyped(String)
    public eik?: string;

    @StrictlyTyped(String)
    public personEgnLnc?: string;

    @StrictlyTyped(Number)
    public personIdentifierType?: IdentifierTypeEnum;

    @StrictlyTyped(Boolean)
    public isPerson?: boolean;

    @StrictlyTyped(NomenclatureDTO)
    public facilityInstalations?: NomenclatureDTO<number>[];

    @StrictlyTyped(NomenclatureDTO)
    public fishTypes?: NomenclatureDTO<number>[];

    @StrictlyTyped(Number)
    public systemType?: AquacultureSystemEnum;
}