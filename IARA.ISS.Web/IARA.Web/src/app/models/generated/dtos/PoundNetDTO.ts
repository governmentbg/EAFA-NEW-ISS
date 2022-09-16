

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { PoundNetStatusesEnum } from '@app/enums/pound-net-statuses.enum';

export class PoundNetDTO { 
    public constructor(obj?: Partial<PoundNetDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public number?: string;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(String)
    public seasonType?: string;

    @StrictlyTyped(String)
    public categoryType?: string;

    @StrictlyTyped(String)
    public muncipality?: string;

    @StrictlyTyped(Date)
    public registrationDate?: Date;

    @StrictlyTyped(String)
    public status?: string;

    @StrictlyTyped(Number)
    public statusCode?: PoundNetStatusesEnum;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}