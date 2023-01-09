import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

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
import { BaseAuditService } from '@app/services/common-app/base-audit.service';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { PrintUserNomenclatureDTO } from '@app/models/generated/dtos/PrintUserNomenclatureDTO';

@Injectable({
    providedIn: 'root'
})
export class SubmittedApplicationsProcessingService extends BaseAuditService implements IApplicationsRegisterService {
    protected controller: string = 'SubmittedApplicationsProcessing';

    public constructor(requestService: RequestService) {
        super(requestService, AreaTypes.Public);
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
                    const found = entries.records.find((entry: ApplicationRegisterDTO) => {
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

    public getRegisterByApplicationId(applicationId: number, pageCode?: PageCodeEnum): Observable<unknown> {
        throw new Error('Method not implemented.');
    }

    public getApplicationChangeHistory(applicationIds: number[]): Observable<ApplicationsChangeHistoryDTO[]> {
        return this.requestService.post(this.area, this.controller, 'GetApplicationChangeHistoryRecords', applicationIds, {
            responseTypeCtr: ApplicationsChangeHistoryDTO
        });
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

    public getApplicationHistorySimpleAudit(id: number): Observable<SimpleAuditDTO> {
        throw new Error('This method should not be called from SubmittedApplicationsProcessing service.');
    }

    public deleteApplication(id: number): Observable<void> {
        throw new Error('This method should not be called from SubmittedApplicationsProcessing service.');
    }

    public undoDeleteApplication(id: number): Observable<void> {
        throw new Error('This method should not be called from SubmittedApplicationsProcessing service.');
    }

    public assignApplicationViaAccessCode(accessCode: string): Observable<AssignedApplicationInfoDTO> {
        throw new Error('This method should not be called from SubmittedApplicationsProcessing service.');
    }

    public assignApplicationViaUserId(applicationId: number, userId: number): Observable<AssignedApplicationInfoDTO> {
        throw new Error('This method should not be called from SubmittedApplicationsProcessing service.');
    }

    public getUsersNomenclature(): Observable<PrintUserNomenclatureDTO[]> {
        throw new Error('This method should not be called from SubmittedApplicationsProcessing service.');
    }

    public getMyTerritoryUnitUsersNomenclature(): Observable<PrintUserNomenclatureDTO[]> {
        throw new Error('This method should not be called from SubmittedApplicationsProcessing service.');
    }

    public getSimpleAudit(id: number): Observable<SimpleAuditDTO> {
        throw new Error('This method should not be called from SubmittedApplicationsProcessing service.');
    }

    public editApplicationDataAndStartRegixChecks(model: IApplicationRegister): Observable<void> {
        throw new Error('This method should not be called from SubmittedApplicationsProcessing service.');
    }

    public getApplication(id: number, getRegiXData: boolean): Observable<IApplicationRegister> {
        throw new Error('This method should not be called from SubmittedApplicationsProcessing service.');
    }

    public getRegixData(id: number): Observable<RegixChecksWrapperDTO<IApplicationRegister>> {
        throw new Error('This method should not be called from SubmittedApplicationsProcessing service.');
    }

    public getApplicationDataForRegister(applicationId: number): Observable<IApplicationRegister> {
        throw new Error('This method should not be called from SubmittedApplicationsProcessing service.');
    }

    public addApplication(application: IApplicationRegister): Observable<number> {
        throw new Error('This method should not be called from SubmittedApplicationsProcessing service.');
    }

    public editApplication(application: IApplicationRegister): Observable<number> {
        throw new Error('This method should not be called from SubmittedApplicationsProcessing service.');
    }

    public downloadFile(fileId: number, fileName: string): Observable<boolean> {
        throw new Error('This method should not be called from SubmittedApplicationsProcessing service.');
    }
}