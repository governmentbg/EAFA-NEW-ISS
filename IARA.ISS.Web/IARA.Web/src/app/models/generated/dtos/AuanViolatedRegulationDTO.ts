

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { AuanViolatedRegulationTypesEnum } from '@app/enums/auan-violated-regulation-types.enum';
import { ViolatedRegulationSectionTypesEnum } from '@app/enums/violated-regulation-section-types.enum';

export class AuanViolatedRegulationDTO { 
    public constructor(obj?: Partial<AuanViolatedRegulationDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public article?: string;

    @StrictlyTyped(String)
    public paragraph?: string;

    @StrictlyTyped(String)
    public section?: string;

    @StrictlyTyped(String)
    public letter?: string;

    @StrictlyTyped(Number)
    public type?: AuanViolatedRegulationTypesEnum;

    @StrictlyTyped(Number)
    public sectionType?: ViolatedRegulationSectionTypesEnum;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}