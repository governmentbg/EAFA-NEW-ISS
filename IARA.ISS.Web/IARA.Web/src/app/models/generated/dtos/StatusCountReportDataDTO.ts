
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { StatusCountReportDTO } from './StatusCountReportDTO';

export class StatusCountReportDataDTO {
    public constructor(obj?: Partial<StatusCountReportDataDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(StatusCountReportDTO)
    public series?: StatusCountReportDTO[];

    @StrictlyTyped(String)
    public categories?: string[];
}