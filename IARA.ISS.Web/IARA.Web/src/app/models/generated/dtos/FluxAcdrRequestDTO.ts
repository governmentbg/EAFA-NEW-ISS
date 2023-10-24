

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { FluxAcdrReportDTO } from './FluxAcdrReportDTO';
import { FluxAcdrReportStatusEnum } from '@app/enums/flux-acdr-report-status.enum';

export class FluxAcdrRequestDTO { 
    public constructor(obj?: Partial<FluxAcdrRequestDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public fluxRequestId?: number;

    @StrictlyTyped(Boolean)
    public isOutgoing?: boolean;

    @StrictlyTyped(String)
    public webServiceName?: string;

    @StrictlyTyped(String)
    public requestUUID?: string;

    @StrictlyTyped(Date)
    public requestDateTime?: Date;

    @StrictlyTyped(Date)
    public periodStart?: Date;

    @StrictlyTyped(Date)
    public periodEnd?: Date;

    @StrictlyTyped(Number)
    public periodMonth?: number;

    @StrictlyTyped(Number)
    public periodYear?: number;

    @StrictlyTyped(String)
    public responseStatus?: string;

    @StrictlyTyped(String)
    public responseUUID?: string;

    @StrictlyTyped(Date)
    public responseDateTime?: Date;

    @StrictlyTyped(String)
    public errorDescription?: string;

    @StrictlyTyped(Number)
    public reportStatus?: FluxAcdrReportStatusEnum;

    @StrictlyTyped(String)
    public reportStatusName?: string;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;

    @StrictlyTyped(FluxAcdrReportDTO)
    public historyRecords?: FluxAcdrReportDTO[];
}