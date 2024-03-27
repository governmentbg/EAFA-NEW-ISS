import { ReportSchema } from '@app/components/common-app/reports/models/report-schema.model';
import { ResultType } from '@app/components/common-app/reports/report-execution/report-execution.component';
import { ExecuteReportDTO } from '@app/models/generated/dtos/ExecuteReportDTO';
import { ExecutionReportInfoDTO } from '@app/models/generated/dtos/ExecutionReportInfoDTO';
import { ReportDTO } from '@app/models/generated/dtos/ReportDTO';
import { ReportGroupDTO } from '@app/models/generated/dtos/ReportGroupDTO';
import { ReportNodeDTO } from '@app/models/generated/dtos/ReportNodeDTO';
import { ReportParameterExecuteDTO } from '@app/models/generated/dtos/ReportParameterExecuteDTO';
import { Observable } from 'rxjs';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { ReportGridRequestDTO } from '@app/models/generated/dtos/ReportGridRequestDTO';
import { IBaseAuditService } from '../base-audit.interface';

export interface IBaseReportService extends IBaseAuditService {
    getReport(id: number): Observable<ReportDTO>;
    getGroup(id: number): Observable<ReportGroupDTO>;
    getReportNodes(): Observable<ReportNodeDTO[]>;
    getExecuteReport(id: number): Observable<ExecuteReportDTO>;
    getExecuteReportParameters(id: number): Observable<ReportParameterExecuteDTO>;
    getColumnNames(reportInfo: ExecutionReportInfoDTO): Observable<ReportSchema[]>;

    executeQuery(report: ExecutionReportInfoDTO): Observable<ResultType[]>;
    executeRawSql(reportInfo: ExecutionReportInfoDTO): Observable<ResultType[]>;
    executePagedQuery(report: ReportGridRequestDTO): Observable<GridResultModel<ResultType>>;
    generateCSV(reportInfo: ExecutionReportInfoDTO): Observable<boolean>;
    downloadReport(reportInfo: ExecutionReportInfoDTO): Observable<boolean>;
}