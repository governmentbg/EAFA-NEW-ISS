import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { EGovPaymentRequestModel } from '@tl/tl-egov-payments';
import { ApplicationStatusesEnum } from '@app/enums/application-statuses.enum';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { IApplicationsService } from '@app/interfaces/administration-app/applications.interface';
import { ApplicationContentDTO } from '@app/models/generated/dtos/ApplicationContentDTO';
import { ApplicationForChoiceDTO } from '@app/models/generated/dtos/ApplicationForChoiceDTO';
import { ApplicationTypeDTO } from '@app/models/generated/dtos/ApplicationTypeDTO';
import { EventisDataDTO } from '@app/models/generated/dtos/EventisDataDTO';
import { FileInfoDTO } from '@app/models/generated/dtos/FileInfoDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PaymentDataDTO } from '@app/models/generated/dtos/PaymentDataDTO';
import { ReasonDTO } from '@app/models/generated/dtos/ReasonDTO';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { ApplicationUtils } from '@app/shared/utils/application.utils';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { PaymentSummaryDTO } from '@app/models/generated/dtos/PaymentSummaryDTO';
import { BaseAuditService } from '../common-app/base-audit.service';

@Injectable({
    providedIn: 'root'
})
export class ApplicationsAdministrationService extends BaseAuditService implements IApplicationsService {
    protected readonly controller: string = 'ApplicationsAdministration';
    private applicationsProcessingController: string = 'ApplicationsProcessing';

    public constructor(requestService: RequestService) {
        super(requestService, AreaTypes.Administrative);
    }

    public getApplicationsForChoice(pageCodes: PageCodeEnum[]): Observable<ApplicationForChoiceDTO[]> {
        return this.requestService.post(this.area, this.controller, 'GetApplicationsForChoice', pageCodes, {
            responseTypeCtr: ApplicationForChoiceDTO
        });
    }

    public getApplicationTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.applicationsProcessingController, 'GetApplicationTypes', {
            responseTypeCtr: NomenclatureDTO
        });
    }

    public getApplicationTypesForChoice(): Observable<ApplicationTypeDTO[]> {
        return this.requestService.get(this.area, this.controller, 'GetApplicationTypesForChoice', {
            responseTypeCtr: ApplicationForChoiceDTO
        });
    }

    public getApplicationAccessCode(applicationId: number): Observable<string> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        return this.requestService.get(this.area, this.controller, 'GetApplicationAccessCode', {
            httpParams: params,
            responseType: 'text'
        });
    }

    public getOnlinePaymentTypes(): Observable<NomenclatureDTO<number>[]> {
        throw new Error('Method should not be called from administrative app');
    }

    public getApplicationChangeHistoryContent(applicationChangeHistoryId: number): Observable<ApplicationContentDTO> {
        const params = new HttpParams().append('applicationChangeHistoryId', applicationChangeHistoryId.toString());

        return this.requestService.get(this.area, this.controller, 'GetApplicationChangeHistoryContent', {
            httpParams: params,
            responseTypeCtr: ApplicationContentDTO
        });
    }

    public getApplicationPaymentSummary(applicationId: number): Observable<PaymentSummaryDTO> {
        const params: HttpParams = new HttpParams().append('applicationId', applicationId.toString());
        return this.requestService.get(this.area, this.controller, 'GetApplicationPaymentSummary', {
            httpParams: params,
            responseTypeCtr: PaymentSummaryDTO
        });
    }

    public addApplication(applicationTypeId: number): Observable<{ item1: number, item2: string }> {
        return this.requestService.post(this.area, this.controller, 'AddApplication', applicationTypeId);
    }

    public enterEventisNumber(applicationId: number, eventisNumber: string): Observable<void> {
        const params = new HttpParams().append('applicationId', applicationId.toString());
        const eventisData: EventisDataDTO = new EventisDataDTO({ eventisNumber: eventisNumber });

        return this.requestService.patch(this.area, this.controller, 'EnterEventisNumber', eventisData, {
            httpParams: params,
            successMessage: 'succ-filed-in-application'
        });
    }

    public saveDraftContent<T>(applicationId: number, model: T): Observable<void> {
        const files: FileInfoDTO[] = ApplicationUtils.spliceFilesFromModel(model);
        const httpParams = new HttpParams().append('applicationId', applicationId.toString());

        const camelCaseModel = CommonUtils.convertKeysToCamelCase(model);

        const applicationContent = new ApplicationContentDTO({
            applicationId: applicationId,
            draftContent: JSON.stringify(camelCaseModel),
            files: files
        });

        return this.requestService.post(this.area, this.controller, 'UpdateDraftContent', applicationContent, {
            httpParams: httpParams,
            successMessage: 'succ-updated-appl-draft-content',
            properties: new RequestProperties({
                asFormData: true
            })
        });
    }

    public applicationAnnulment(applicationId: number, reason: string): Observable<void> {
        const params = new HttpParams().append('applicationId', applicationId.toString());
        const reasonData: ReasonDTO = new ReasonDTO({ reason: reason });

        return this.requestService.patch(this.area, this.controller, 'ApplicationAnnulment', reasonData, {
            httpParams: params,
            successMessage: 'succ-appl-annulment'
        });
    }

    public initiateApplicationCorrections(applicationId: number): Observable<ApplicationStatusesEnum> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        return this.requestService.post(this.area, this.controller, 'InitiateApplicationCorrections', null, {
            httpParams: params
        });
    }

    public sendApplicationForUserCorrections(applicationId: number, statusReason: string): Observable<ApplicationStatusesEnum> {
        const params = new HttpParams().append('applicationId', applicationId.toString());
        const reasonData: ReasonDTO = new ReasonDTO({ reason: statusReason });

        return this.requestService.post(this.area, this.controller, 'SendApplicationForUserCorrections', reasonData, {
            httpParams: params
        });
    }

    public confirmNoErrorsForApplication(applicationId: number): Observable<ApplicationStatusesEnum> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        return this.requestService.post(this.area, this.controller, 'ConfirmNoErrorsForApplication', null, {
            httpParams: params
        });
    }

    public confirmApplicationDataIrregularity(applicationId: number, statusReason: string): Observable<ApplicationStatusesEnum> {
        const params = new HttpParams().append('applicationId', applicationId.toString());
        const reasonData: ReasonDTO = new ReasonDTO({ reason: statusReason });

        return this.requestService.post(this.area, this.controller, 'ConfirmApplicationDataIrregularity', reasonData, {
            httpParams: params
        });
    }

    public confirmApplicationDataRegularity(applicationId: number): Observable<ApplicationStatusesEnum> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        return this.requestService.post(this.area, this.controller, 'ConfirmApplicationDataRegularity', null, {
            httpParams: params
        });
    }

    public enterPaymentData(applicationId: number, paymentData: PaymentDataDTO): Observable<ApplicationStatusesEnum> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        return this.requestService.post(this.area, this.controller, 'EnterApplicationPaymentData', paymentData, {
            httpParams: params
        });
    }

    public renewApplicationPayment(applicationId: number): Observable<ApplicationStatusesEnum> {
        throw new Error('Method should not be called from administrative app');
    }

    public applicationPaymentRefusal(applicationId: number, reason: string): Observable<void> {
        const params = new HttpParams().append('applicationId', applicationId.toString());
        const reasonData: ReasonDTO = new ReasonDTO({ reason: reason });

        return this.requestService.patch(this.area, this.controller, 'ApplicationPaymentRefusal', reasonData, {
            httpParams: params,
            successMessage: 'succ-appl-annulment'
        });
    }

    public downloadFile(fileId: number, fileName: string): Observable<boolean> {
        const params = new HttpParams().append('id', fileId.toString());
        return this.requestService.download(this.area, this.controller, 'DownloadFile', fileName, { httpParams: params });
    }

    public uploadSignedApplication(applicationId: number, file: FileInfoDTO): Observable<ApplicationStatusesEnum> {
        throw new Error('Method should not be called from administrative app');
    }

    public manualRegixChecksStart(applicationId: number): Observable<ApplicationStatusesEnum> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        return this.requestService.post(this.area, this.controller, 'ManualRegixChecksStart', null, {
            httpParams: params,
            successMessage: 'succ-regix-checks-started'
        });
    }

    public completeApplicationFillingByApplicant(applicationId: number): Observable<boolean> {
        throw new Error('Method should not be called from administrative app');
    }

    public downloadGeneratedApplicationFile(applicationId: number): Observable<boolean> {
        throw new Error('Method should not be called from administrative app');
    }

    public goBackToFillApplicationByApplicant(applicationId: number): Observable<boolean> {
        throw new Error('Method should not be called from administrative app');
    }

    public initiateEGovPayment(paymentModel: EGovPaymentRequestModel): void {
        throw new Error('Method should not be called from administrative app');
    }

    public initiateEPayBGPayment(applicationId: number): void {
        throw new Error('Method should not be called from administrative app');
    }

    public updateStateMachineStatusMock(applicationId: number, wantedErrorLevel: string): Observable<void> {
        const params = new HttpParams().append('applicationId', applicationId.toString()).append('errorLevelToMock', wantedErrorLevel);

        return this.requestService.patch(this.area, this.controller, 'UpdateStateMachineStatusMock', null, {
            httpParams: params,
            properties: new RequestProperties({
                showProgressSpinner: false
            })
        });
    }
}