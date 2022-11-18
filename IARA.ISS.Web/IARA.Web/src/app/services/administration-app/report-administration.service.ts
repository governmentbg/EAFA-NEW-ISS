import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReportSchema } from '@app/components/common-app/reports/models/report-schema.model';
import { ResultType } from '@app/components/common-app/reports/report-execution/report-execution.component';
import { IReportService } from '@app/interfaces/administration-app/report.interface';
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
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { Observable } from 'rxjs';
import { ReportGridRequestDTO } from '@app/models/generated/dtos/ReportGridRequestDTO';
import { BaseAuditService } from '../common-app/base-audit.service';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';

@Injectable({
    providedIn: 'root'
})
export class ReportAdministrationService extends BaseAuditService implements IReportService {
    protected controller: string = 'ReportAdministration';
    protected roleController: string = 'Role';

    constructor(requestService: RequestService) {
        super(requestService, AreaTypes.Administrative)
    }

    public executePagedQuery(report: ReportGridRequestDTO): Observable<GridResultModel<ResultType>> {
        return this.requestService.post(this.area, this.controller, 'ExecutePagedQuery', report, {
            properties: new RequestProperties({ showException: true, rethrowException: true })
        });
    }

    public getAvailableNParameters(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetAvailableNParameters', {
            responseTypeCtr: NomenclatureDTO
        });
    }

    public getReportGroups(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetReportGroups', {
            responseTypeCtr: NomenclatureDTO
        });
    }

    public getAllNParameters(gridRequestModel: GridRequestModel<ReportParameterDefinitionFilters>): Observable<GridResultModel<NReportParameterDTO>> {
        return this.requestService.post(this.area, this.controller, 'GetAllNParameters', gridRequestModel, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        });
    }

    public getNParameter(id: number): Observable<NReportParameterEditDTO> {
        return this.requestService.get(this.area, this.controller, 'GetNParameter', {
            httpParams: new HttpParams().append('id', id.toString()),
            responseTypeCtr: NReportParameterEditDTO
        });
    }

    public addNParameter(nParameter: NReportParameterEditDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'AddNParameter', nParameter, {
            properties: new RequestProperties({ showException: true, rethrowException: true })
        });
    }

    public editNParameter(nParameter: NReportParameterEditDTO): Observable<void> {
        return this.requestService.put(this.area, this.controller, 'EditNParameter', nParameter, {
            properties: new RequestProperties({ showException: true, rethrowException: true })
        });
    }

    public deleteNParameter(id: number): Observable<void> {
        return this.requestService.delete(this.area, this.controller, 'DeleteNParameter', {
            httpParams: new HttpParams().append('id', id.toString())
        });
    }

    public getReport(id: number): Observable<ReportDTO> {
        return this.requestService.get(this.area, this.controller, 'GetReport', {
            responseTypeCtr: ReportDTO,
            httpParams: new HttpParams().append('id', id.toString())
        });
    }

    public addReport(report: ReportDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'AddReport', report, {
            properties: new RequestProperties({ showException: true, rethrowException: true })
        });
    }

    public editReport(report: ReportDTO): Observable<void> {
        return this.requestService.put(this.area, this.controller, 'EditReport', report, {
            properties: new RequestProperties({ showException: true, rethrowException: true })
        });
    }

    public deleteReport(id: number): Observable<void> {
        return this.requestService.delete(this.area, this.controller, 'DeleteReport', {
            httpParams: new HttpParams().append('id', id.toString())
        });
    }

    public undoDeletedReport(id: number): Observable<void> {
        return this.requestService.patch(this.area, this.controller, 'UndoDeletedReport', null, {
            httpParams: new HttpParams().append('id', id.toString())
        });
    }

    public getGroup(id: number): Observable<ReportGroupDTO> {
        return this.requestService.get(this.area, this.controller, 'GetGroup', {
            responseTypeCtr: ReportGroupDTO,
            httpParams: new HttpParams().append('id', id.toString())
        });
    }

    public addGroup(group: ReportGroupDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'AddGroup', group);
    }

    public editGroup(group: ReportGroupDTO): Observable<void> {
        return this.requestService.put(this.area, this.controller, 'EditGroup', group);
    }

    public deleteGroup(id: number): Observable<void> {
        return this.requestService.delete(this.area, this.controller, 'DeleteGroup', {
            httpParams: new HttpParams().append('id', id.toString())
        });
    }

    public undoDeletedGroup(id: number): Observable<void> {
        return this.requestService.patch(this.area, this.controller, 'UndoDeletedGroup', null, {
            httpParams: new HttpParams().append('id', id.toString())
        })
    }

    public getActiveRoles(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.roleController, 'GetAllActiveRoles', {
            responseTypeCtr: NomenclatureDTO
        });
    }

    public getTableNodes(): Observable<TableNodeDTO[]> {
        return this.requestService.get(this.area, this.controller, 'GetTableNodes', {
            responseTypeCtr: TableNodeDTO
        });
    }

    public getReportNodes(): Observable<ReportNodeDTO[]> {
        return this.requestService.get(this.area, this.controller, 'GetReportNodes', {
            responseTypeCtr: ReportNodeDTO
        });
    }

    public getExecuteReport(id: number): Observable<ExecuteReportDTO> {
        return this.requestService.get(this.area, this.controller, 'GetExecuteReport', {
            responseTypeCtr: ExecuteReportDTO,
            httpParams: new HttpParams().append('id', id.toString()),
            properties: new RequestProperties({ showException: true, rethrowException: true })
        });
    }

    public getExecuteReportParameters(id: number): Observable<ReportParameterExecuteDTO> {
        return this.requestService.get(this.area, this.controller, 'GetExecuteReportParameters', {
            responseTypeCtr: ReportParameterExecuteDTO,
            httpParams: new HttpParams().append('id', id.toString()),
            properties: new RequestProperties({ showException: true, rethrowException: true })
        });
    }

    public executeQuery(reportInfo: ExecutionReportInfoDTO): Observable<ResultType[]> {
        return this.requestService.post(this.area, this.controller, 'ExecuteQuery', reportInfo, {
            properties: new RequestProperties({ showException: true, rethrowException: true })
        });
    }

    public executeRawSql(reportInfo: ExecutionReportInfoDTO): Observable<ResultType[]> {
        return this.requestService.post(this.area, this.controller, 'ExecuteRawSql', reportInfo, {
            properties: new RequestProperties({ showException: true, rethrowException: true })
        });
    }

    public generateCSV(reportInfo: ExecutionReportInfoDTO): Observable<boolean> {
        const params: HttpParams = new HttpParams().append('reportId', reportInfo.reportId!.toString());
        return this.requestService.downloadPost(this.area, this.controller, 'GenerateCSV', reportInfo.name!, reportInfo.parameters, {
            properties: new RequestProperties({ showException: true, rethrowException: true }),
            httpParams: params
        });
    }

    public downloadReport(reportInfo: ExecutionReportInfoDTO): Observable<boolean> {
        const params: HttpParams = new HttpParams().append('id', reportInfo.reportId!.toString());
        return this.requestService.downloadPost(this.area, this.controller, 'DownloadReport', reportInfo.name!, reportInfo.parameters, {
            httpParams: params
        });
    }

    public getColumnNames(reportInfo: ExecutionReportInfoDTO): Observable<ReportSchema[]> {
        return this.requestService.post(this.area, this.controller, 'GetColumnNames', reportInfo, {
            responseTypeCtr: ReportSchema,
            properties: new RequestProperties({ showException: true, rethrowException: true })
        });
    }

    public getColumnNamesRawSql(reportInfo: ExecutionReportInfoDTO): Observable<ReportSchema[]> {
        return this.requestService.post(this.area, this.controller, 'GetColumnNames', reportInfo, {
            responseTypeCtr: ReportSchema,
            properties: new RequestProperties({ showException: true, rethrowException: true })
        });
    }

    public getReportAudit(id: number): Observable<SimpleAuditDTO> {
        const params: HttpParams = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetReportSimpleAudit', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }

    public getReportParametersAudit(id: number): Observable<SimpleAuditDTO> {
        const params: HttpParams = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetReportParametersSimpleAudit', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }

    public getParametersAudit(id: number): Observable<SimpleAuditDTO> {
        const params: HttpParams = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetParametersSimpleAudit', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }

    public getReportGroupsAudit(id: number): Observable<SimpleAuditDTO> {
        const params: HttpParams = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetReportGroupsSimpleAudit', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }
}