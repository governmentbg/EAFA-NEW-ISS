

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO';
import { FileInfoDTO } from './FileInfoDTO';

export class YearlyQuotaEditDTO { 
    public constructor(obj?: Partial<YearlyQuotaEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public fishId?: number;

    @StrictlyTyped(Number)
    public year?: number;

    @StrictlyTyped(Number)
    public quotaValueKg?: number;

    @StrictlyTyped(Number)
    public leftoverValueKg?: number;

    @StrictlyTyped(String)
    public changeBasis?: string;

    @StrictlyTyped(NomenclatureDTO)
    public unloadPorts?: NomenclatureDTO<number>[];

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];
}