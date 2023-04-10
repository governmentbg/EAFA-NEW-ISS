

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO';
import { CrossChecksAutoExecFrequencyEnum } from '@app/enums/cross-checks-auto-exec-frequency.enum';

export class CrossCheckEditDTO { 
    public constructor(obj?: Partial<CrossCheckEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public code?: string;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(Boolean)
    public isNewReportGroup?: boolean;

    @StrictlyTyped(Number)
    public reportGroupId?: number;

    @StrictlyTyped(String)
    public reportGroupName?: string;

    @StrictlyTyped(Number)
    public errorLevel?: number;

    @StrictlyTyped(String)
    public dataSource?: string;

    @StrictlyTyped(String)
    public dataSourceColumns?: string;

    @StrictlyTyped(String)
    public checkSource?: string;

    @StrictlyTyped(String)
    public checkSourceColumns?: string;

    @StrictlyTyped(String)
    public checkTableName?: string;

    @StrictlyTyped(String)
    public purpose?: string;

    @StrictlyTyped(Number)
    public sourceId?: number;

    @StrictlyTyped(Number)
    public autoExecFrequency?: CrossChecksAutoExecFrequencyEnum;

    @StrictlyTyped(NomenclatureDTO)
    public users?: NomenclatureDTO<number>[];

    @StrictlyTyped(NomenclatureDTO)
    public roles?: NomenclatureDTO<number>[];

    @StrictlyTyped(String)
    public reportSQL?: string;
}