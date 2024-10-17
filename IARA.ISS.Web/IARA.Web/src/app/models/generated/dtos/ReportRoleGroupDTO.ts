

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO';

export class ReportRoleGroupDTO { 
    public constructor(obj?: Partial<ReportRoleGroupDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public groupId?: number;

    @StrictlyTyped(String)
    public parentGroup?: string;

    @StrictlyTyped(Boolean)
    public expanded?: boolean;

    @StrictlyTyped(NomenclatureDTO)
    public reports?: NomenclatureDTO<number>[];
}