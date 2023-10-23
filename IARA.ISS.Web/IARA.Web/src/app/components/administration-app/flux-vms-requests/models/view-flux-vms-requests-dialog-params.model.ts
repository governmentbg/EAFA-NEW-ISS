import { FluxAcdrReportStatusEnum } from '@app/enums/flux-acdr-report-status.enum';

export class ViewFluxVmsRequestsDialogParams {
    public id!: number;
    public acdrId: number | undefined;
    public reportStatus: FluxAcdrReportStatusEnum | undefined;

    constructor(obj?: Partial<ViewFluxVmsRequestsDialogParams>) {
        Object.assign(this, obj);
    }
}