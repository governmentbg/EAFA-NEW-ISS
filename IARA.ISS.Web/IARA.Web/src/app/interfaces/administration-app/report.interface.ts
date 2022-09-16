import { ReportSchema } from '@app/components/common-app/reports/models/report-schema.model';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { ExecuteReportDTO } from '@app/models/generated/dtos/ExecuteReportDTO';
import { ExecutionReportInfoDTO } from '@app/models/generated/dtos/ExecutionReportInfoDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { NReportParameterDTO } from '@app/models/generated/dtos/NReportParameterDTO';
import { NReportParameterEditDTO } from '@app/models/generated/dtos/NReportParameterEditDTO';
import { ReportDTO } from '@app/models/generated/dtos/ReportDTO';
import { ReportGroupDTO } from '@app/models/generated/dtos/ReportGroupDTO';
import { ReportNodeDTO } from '@app/models/generated/dtos/ReportNodeDTO';
import { ReportParameterExecuteDTO } from '@app/models/generated/dtos/ReportParameterExecuteDTO';
import { TableNodeDTO } from '@app/models/generated/dtos/TableNodeDTO';
import { ReportParameterDefinitionFilters } from '@app/models/generated/filters/ReportParameterDefinitionFilters';
import { Observable } from "rxjs";
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { IBaseReportService } from './base-report.interface';

export interface IReportService extends IBaseReportService {
    getAvailableNParameters(): Observable<NomenclatureDTO<number>[]>;
    getReportGroups(): Observable<NomenclatureDTO<number>[]>;

    getAllNParameters(gridRequestModel: GridRequestModel<ReportParameterDefinitionFilters>): Observable<GridResultModel<NReportParameterDTO>>;
    getNParameter(id: number): Observable<NReportParameterEditDTO>;
    addNParameter(nParameter: NReportParameterEditDTO): Observable<number>;
    editNParameter(nParameter: NReportParameterEditDTO): Observable<void>;
    deleteNParameter(id: number): Observable<void>;
    undoDeletedNParameter(id: number): Observable<void>;

    getReport(id: number): Observable<ReportDTO>;
    addReport(report: ReportDTO): Observable<number>;
    editReport(report: ReportDTO): Observable<void>;
    deleteReport(id: number): Observable<void>;
    undoDeletedReport(id: number): Observable<void>;

    getGroup(id: number): Observable<ReportGroupDTO>;
    addGroup(group: ReportGroupDTO): Observable<number>;
    editGroup(group: ReportGroupDTO): Observable<void>;
    deleteGroup(id: number): Observable<void>;
    undoDeletedGroup(id: number): Observable<void>;

    getActiveRoles(): Observable<NomenclatureDTO<number>[]>;
    getTableNodes(): Observable<TableNodeDTO[]>;
    getReportNodes(): Observable<ReportNodeDTO[]>;

    getExecuteReport(id: number): Observable<ExecuteReportDTO>;
    getExecuteReportParameters(id: number): Observable<ReportParameterExecuteDTO>;

    getColumnNames(reportInfo: ExecutionReportInfoDTO): Observable<ReportSchema[]>;
    getColumnNamesRawSql(reportInfo: ExecutionReportInfoDTO): Observable<ReportSchema[]>;

    getReportAudit(id: number): Observable<SimpleAuditDTO>;
    getReportParametersAudit(id: number): Observable<SimpleAuditDTO>;
    getParametersAudit(id: number): Observable<SimpleAuditDTO>;
    getReportGroupsAudit(id: number): Observable<SimpleAuditDTO>;
}