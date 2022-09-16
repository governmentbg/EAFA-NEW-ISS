
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ReportHistoryDTO } from './ReportHistoryDTO';

export class LegalEntityReportDTO {
    public constructor(obj?: Partial<LegalEntityReportDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public legalName?: string;

    @StrictlyTyped(String)
    public eik?: string;

    @StrictlyTyped(String)
    public populatedArea?: string;

    @StrictlyTyped(String)
    public street?: string;

    @StrictlyTyped(String)
    public number?: string;

    @StrictlyTyped(ReportHistoryDTO)
    public history?: ReportHistoryDTO[];
}