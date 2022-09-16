

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO';
import { ReportParameterTypeEnum } from '@app/enums/report-parameter-type.enum';

export class ReportParameterExecuteDTO { 
    public constructor(obj?: Partial<ReportParameterExecuteDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public dataType?: ReportParameterTypeEnum;

    @StrictlyTyped(Boolean)
    public isMandatory?: boolean;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public parameterId?: number;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(String)
    public code?: string;

    @StrictlyTyped(String)
    public defaultValue?: string;

    @StrictlyTyped(String)
    public pattern?: string;

    @StrictlyTyped(String)
    public description?: string;

    @StrictlyTyped(String)
    public errorMessage?: string;

    @StrictlyTyped(NomenclatureDTO)
    public parameterTypeNomenclatures?: NomenclatureDTO<number>[];

    @StrictlyTyped(String)
    public nomenclatureSQL?: string;
}