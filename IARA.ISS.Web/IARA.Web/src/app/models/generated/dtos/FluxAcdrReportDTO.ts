

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { FluxAcdrReportStatusEnum } from '@app/enums/flux-acdr-report-status.enum';

export class FluxAcdrReportDTO { 
    public constructor(obj?: Partial<FluxAcdrReportDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public requestUUID?: string;

    @StrictlyTyped(String)
    public webServiceName?: string;

    @StrictlyTyped(String)
    public responseStatus?: string;

    @StrictlyTyped(Date)
    public responseDateTime?: Date;

    @StrictlyTyped(String)
    public errorDescription?: string;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;

    @StrictlyTyped(Date)
    public reportCreatedOn?: Date;

    @StrictlyTyped(Number)
    public requestId?: number;

    @StrictlyTyped(Boolean)
    public isModified?: boolean;

    @StrictlyTyped(Number)
    public reportStatus?: FluxAcdrReportStatusEnum;

    @StrictlyTyped(String)
    public reportStatusName?: string;

    @StrictlyTyped(Date)
    public periodStart?: Date;

    @StrictlyTyped(Date)
    public periodEnd?: Date;
}