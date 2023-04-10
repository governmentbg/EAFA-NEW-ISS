

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ReportParameterDTO } from './ReportParameterDTO';
import { NomenclatureDTO } from './GenericNomenclatureDTO';

export class ReportDTO { 
    public constructor(obj?: Partial<ReportDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public reportGroupId?: number;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(String)
    public description?: string;

    @StrictlyTyped(String)
    public reportType?: string;

    @StrictlyTyped(Date)
    public lastRunDateTime?: Date;

    @StrictlyTyped(String)
    public lastRunUsername?: string;

    @StrictlyTyped(String)
    public iconName?: string;

    @StrictlyTyped(Number)
    public lastRunDurationSec?: number;

    @StrictlyTyped(Number)
    public orderNum?: number;

    @StrictlyTyped(ReportParameterDTO)
    public parameters?: ReportParameterDTO[];

    @StrictlyTyped(String)
    public code?: string;

    @StrictlyTyped(NomenclatureDTO)
    public roles?: NomenclatureDTO<number>[];

    @StrictlyTyped(NomenclatureDTO)
    public users?: NomenclatureDTO<number>[];

    @StrictlyTyped(String)
    public reportSQL?: string;
}