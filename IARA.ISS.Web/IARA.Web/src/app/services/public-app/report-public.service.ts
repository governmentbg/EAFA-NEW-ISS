import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReportSchema } from '@app/components/common-app/reports/models/report-schema.model';
import { ResultType } from '@app/components/common-app/reports/report-execution/report-execution.component';
import { ExecuteReportDTO } from '@app/models/generated/dtos/ExecuteReportDTO';
import { ExecutionReportInfoDTO } from '@app/models/generated/dtos/ExecutionReportInfoDTO';
import { ReportDTO } from '@app/models/generated/dtos/ReportDTO';
import { ReportGroupDTO } from '@app/models/generated/dtos/ReportGroupDTO';
import { ReportNodeDTO } from '@app/models/generated/dtos/ReportNodeDTO';
import { ReportParameterExecuteDTO } from '@app/models/generated/dtos/ReportParameterExecuteDTO';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestService } from '@app/shared/services/request.service';
import { Observable } from 'rxjs';
import { IBaseReportService } from '@app/interfaces/administration-app/base-report.interface';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { ReportGridRequestDTO } from '@app/models/generated/dtos/ReportGridRequestDTO';
import { RequestProperties } from '@app/shared/services/request-properties';
import { BaseAuditService } from '../common-app/base-audit.service';

@Injectable({
    providedIn: 'root'
})
export class ReportPublicService extends BaseAuditService implements IBaseReportService {
    protected controller: string = "ReportPublic";

    public constructor(requestService: RequestService) {
        super(requestService, AreaTypes.Public)
    }

    executeRawSql(reportInfo: ExecutionReportInfoDTO): Observable<ResultType[]> {
        throw new Error('Method not implemented.');
    }

    public executePagedQuery(report: ReportGridRequestDTO): Observable<GridResultModel<ResultType>> {
        return this.requestService.post(this.area, this.controller, 'ExecutePagedQuery', report, {
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

    public getReport(id: number): Observable<ReportDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetReport', {
            responseTypeCtr: ReportDTO,
            httpParams: params
        });
    }

    public getGroup(id: number): Observable<ReportGroupDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetGroup', {
            responseTypeCtr: ReportGroupDTO,
            httpParams: params
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
            properties: new RequestProperties({ showException: false, rethrowException: true })
        });
    }

    public getExecuteReportParameters(id: number): Observable<ReportParameterExecuteDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetExecuteReportParameters', {
            responseTypeCtr: ReportParameterExecuteDTO,
            httpParams: params
        });
    }

    public executeQuery(report: ExecutionReportInfoDTO): Observable<ResultType[]> {
        return this.requestService.post(this.area, this.controller, 'ExecuteQuery', report);
    }

    public getColumnNames(reportInfo: ExecutionReportInfoDTO): Observable<ReportSchema[]> {
        return this.requestService.post(this.area, this.controller, 'GetColumnNames', reportInfo, {
            responseTypeCtr: ReportSchema
        });
    }
}