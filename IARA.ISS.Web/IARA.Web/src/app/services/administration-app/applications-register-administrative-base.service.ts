import { HttpParams } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { IApplicationsRegisterService } from '@app/interfaces/administration-app/applications-register.interface';
import { IApplicationRegister } from '@app/interfaces/common-app/application-register.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { ApplicationRegisterDTO } from '@app/models/generated/dtos/ApplicationRegisterDTO';
import { ApplicationsChangeHistoryDTO } from '@app/models/generated/dtos/ApplicationsChangeHistoryDTO';
import { AssignedApplicationInfoDTO } from '@app/models/generated/dtos/AssignedApplicationInfoDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { RegixChecksWrapperDTO } from '@app/models/generated/dtos/RegixChecksWrapperDTO';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { ApplicationsRegisterFilters } from '@app/models/generated/filters/ApplicationsRegisterFilters';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { BaseAuditService } from '../common-app/base-audit.service';
import { ApplicationsProcessingService } from './applications-processing.service';
import { PrintUserNomenclatureDTO } from '@app/models/generated/dtos/PrintUserNomenclatureDTO';
import { ApplicationsNotAssignedDTO } from '@app/models/generated/dtos/ApplicationsNotAssignedDTO';

export abstract class ApplicationsRegisterAdministrativeBaseService extends BaseAuditService implements IApplicationsRegisterService {
    protected readonly applicationsProcessingController: string = 'ApplicationsProcessing';

    protected readonly applicationsRegisterService: ApplicationsProcessingService | undefined;

    public constructor(requestService: RequestService, applicationsRegisterService?: ApplicationsProcessingService) {
        super(requestService, AreaTypes.Administrative);

        if (applicationsRegisterService) {
            this.applicationsRegisterService = applicationsRegisterService;
        }
    }

    public getAllApplications(request: GridRequestModel<ApplicationsRegisterFilters>): Observable<GridResultModel<ApplicationRegisterDTO>> {
        type Result = GridResultModel<ApplicationRegisterDTO>;
        type Body = GridRequestModel<ApplicationsRegisterFilters>;

        return this.requestService.post<Result, Body>(this.area, this.controller, 'GetAllApplications', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridRequestModel
        }).pipe(switchMap((entries: Result) => {
            const applicationIds: number[] = entries.records.map((application: ApplicationRegisterDTO) => {
                return application.id!;
            });

            if (applicationIds.length === 0) {
                return of(entries);
            }

            return this.getApplicationChangeHistory(applicationIds).pipe(map((changeHistoryRecords: ApplicationsChangeHistoryDTO[]) => {
                for (const changeHistory of changeHistoryRecords) {
                    const found: ApplicationRegisterDTO | undefined = entries.records.find((entry: ApplicationRegisterDTO) => {
                        return entry.id === changeHistory.applicationId;
                    });

                    if (found !== undefined) {
                        if (found.changeHistoryRecords !== undefined && found.changeHistoryRecords !== null) {
                            found.changeHistoryRecords.push(changeHistory);
                        }
                        else {
                            found.changeHistoryRecords = [changeHistory];
                        }
                    }
                }
                return entries;
            }));
        }));
    }

    public abstract getRegisterByApplicationId(applicationId: number, pageCode?: PageCodeEnum): Observable<unknown>;
    public abstract getApplication(id: number, getRegiXData: boolean, pageCode?: PageCodeEnum): Observable<IApplicationRegister>;
    public abstract getRegixData(id: number, pageCode?: PageCodeEnum): Observable<RegixChecksWrapperDTO<IApplicationRegister>>;
    public abstract getApplicationDataForRegister(applicationId: number, pageCode?: PageCodeEnum): Observable<IApplicationRegister>;
    public abstract addApplication(application: IApplicationRegister, pageCode?: PageCodeEnum): Observable<number>;
    public abstract editApplication(application: IApplicationRegister, pageCode?: PageCodeEnum, fromSaveAsDraft?: boolean | undefined): Observable<number>;
    public abstract editApplicationDataAndStartRegixChecks(model: IApplicationRegister, pageCode?: PageCodeEnum): Observable<void>;

    public deleteApplication(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.delete(this.area, this.controller, 'DeleteApplication', { httpParams: params });
    }

    public undoDeleteApplication(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.patch(this.area, this.controller, 'UndoDeleteApplication', null, { httpParams: params });
    }

    public assignApplicationViaAccessCode(accessCode: string): Observable<AssignedApplicationInfoDTO> {
        const params = new HttpParams().append('accessCode', accessCode);
        return this.requestService.post(this.area, this.controller, 'AssignApplicationViaAccessCode', null, { httpParams: params });
    }

    public assignApplicationViaUserId(applicationId: number, userId: number): Observable<AssignedApplicationInfoDTO> {
        const params: HttpParams = new HttpParams()
            .append('applicationId', applicationId.toString())
            .append('userId', userId.toString());

        return this.requestService.post(this.area, this.applicationsProcessingController, 'AssignApplicationViaUserId', undefined, {
            httpParams: params,
            responseTypeCtr: AssignedApplicationInfoDTO
        });
    }

    public confirmNoErrorsAndFillAdmAct(id: number, model: any, pageCode?: PageCodeEnum): Observable<void> {
        throw new Error(`Method not implemented for the choseen type of application (application id: ${id})`);
    }

    public downloadFile(fileId: number, fileName: string): Observable<boolean> {
        const params = new HttpParams().append('id', fileId.toString());
        return this.requestService.download(this.area, this.controller, 'DownloadFile', fileName, { httpParams: params });
    }

    public getApplicationStatuses(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetApplicationStatuses', { responseTypeCtr: NomenclatureDTO });
    }

    public getApplicationTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetApplicationTypes', { responseTypeCtr: NomenclatureDTO });
    }

    public getApplicationSources(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetApplicationSources', { responseTypeCtr: NomenclatureDTO });
    }

    public getUsersNomenclature(): Observable<PrintUserNomenclatureDTO[]> {
        return this.requestService.get(this.area, this.applicationsProcessingController, 'GetUsersNomenclature', {
            responseTypeCtr: PrintUserNomenclatureDTO
        });
    }

    public getMyTerritoryUnitUsersNomenclature(): Observable<PrintUserNomenclatureDTO[]> {
        return this.requestService.get(this.area, this.applicationsProcessingController, 'GetMyTerritoryUnitUsersNomenclature', {
            responseTypeCtr: PrintUserNomenclatureDTO
        });
    }

    public getApplicationHistorySimpleAudit(id: number): Observable<SimpleAuditDTO> {
        if (this.applicationsRegisterService) {
            return this.applicationsRegisterService.getApplicationHistorySimpleAudit(id);
        }
        throw new Error('"getApplicationHistorySimpleAudit" cannot be called because no valid instance of ApplicationsProcessingService is available');
    }

    public getApplicationsSimpleAudit(id: number): Observable<SimpleAuditDTO> {
        if (this.applicationsRegisterService) {
            return this.applicationsRegisterService.getApplicationsSimpleAudit(id);
        }
        throw new Error('"getApplicationSimpleAudit" cannot be called because no valid instance of ApplicationsProcessingService is available');
    }

    private getApplicationChangeHistory(applicationIds: number[]): Observable<ApplicationsChangeHistoryDTO[]> {
        return this.requestService.post(this.area, this.applicationsProcessingController, 'GetApplicationChangeHistoryRecords', applicationIds, {
            responseTypeCtr: ApplicationsChangeHistoryDTO,
            properties: RequestProperties.NO_SPINNER
        });
    }

    public getNotAssignedApplications(): Observable<ApplicationsNotAssignedDTO> {
        return this.requestService.get(this.area, this.controller, 'GetNotAssignedApplications', { responseTypeCtr: ApplicationsNotAssignedDTO });
    }
}