import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApplicationStatusesEnum } from '@app/enums/application-statuses.enum';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { IApplicationsService } from '@app/interfaces/administration-app/applications.interface';
import { ApplicationContentDTO } from '@app/models/generated/dtos/ApplicationContentDTO';
import { ApplicationForChoiceDTO } from '@app/models/generated/dtos/ApplicationForChoiceDTO';
import { ApplicationTypeDTO } from '@app/models/generated/dtos/ApplicationTypeDTO';
import { FileInfoDTO } from '@app/models/generated/dtos/FileInfoDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PaymentDataDTO } from '@app/models/generated/dtos/PaymentDataDTO';
import { ReasonDTO } from '@app/models/generated/dtos/ReasonDTO';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { ApplicationUtils } from '@app/shared/utils/application.utils';
import { CommonUtils } from '@app/shared/utils/common.utils';
import * as tlBoricaPayments from '@tl/tl-borica-payments';
import { TLEGovPaymentService } from '@tl/tl-egov-payments';
import { TLEPaymentService } from '@tl/tl-epay-payments';
import { Observable } from 'rxjs';
import { Environment } from '@env/environment';
import { BaseAuditService } from '../common-app/base-audit.service';
import { PaymentSummaryDTO } from '@app/models/generated/dtos/PaymentSummaryDTO';


@Injectable({
    providedIn: 'root'
})
export class ApplicationsPublicService extends BaseAuditService implements IApplicationsService {
    protected readonly controller: string = 'ApplicationsPublic';
    private applicationsProcessingController: string = 'ApplicationsProcessing';

    private eGovPaymentSevice: TLEGovPaymentService;
    private ePayBgPaymentService: TLEPaymentService;
    private ePosPaymentService: tlBoricaPayments.TLBoricaPaymentService

    public constructor(
        requestService: RequestService,
        eGovPaymentSevice: TLEGovPaymentService,
        ePayBgPaymentService: TLEPaymentService,
        ePosPaymentService: tlBoricaPayments.TLBoricaPaymentService
    ) {
        super(requestService, AreaTypes.Public);

        this.eGovPaymentSevice = eGovPaymentSevice;
        this.ePayBgPaymentService = ePayBgPaymentService;
        this.ePosPaymentService = ePosPaymentService;

        TLEGovPaymentService.setBaseServiceURL(Environment.Instance.apiBaseUrl + '/Public/EGovPayments');
        TLEPaymentService.setBaseServiceURL(Environment.Instance.apiBaseUrl + '/Public/EPay');
        tlBoricaPayments.TLBoricaPaymentService.setBaseURL(Environment.Instance.apiBaseUrl + '/Public/BoricaPayments');
    }

    public getApplicationsForChoice(pageCode: PageCodeEnum): Observable<ApplicationForChoiceDTO[]> {
        throw new Error('Method should not be called from public app');
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
        throw new Error('Method should not be called from public app');
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
        const params = new HttpParams().append('applicationTypeId', applicationTypeId.toString());

        return this.requestService.post(this.area, this.controller, 'AddApplication', null, {
            httpParams: params
        });
    }

    public enterEventisNumber(applicationId: number, eventisNumber: string): Observable<void> {
        throw new Error('Method should not be called from public app');
    }

    public saveDraftContent(applicationId: number, model: unknown): Observable<void> {
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

    // TODO
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
        throw new Error('Method should not be called from public app');
    }

    public confirmNoErrorsForApplication(applicationId: number): Observable<ApplicationStatusesEnum> {
        throw new Error('Method should not be called from public app');
    }

    public confirmApplicationDataIrregularity(applicationId: number, statusReason: string): Observable<ApplicationStatusesEnum> {
        throw new Error('Method should not be called from public app');
    }

    public confirmApplicationDataRegularity(applicationId: number): Observable<ApplicationStatusesEnum> {
        throw new Error('Method should not be called from public app');
    }

    public enterPaymentData(applicationId: number, paymentData: PaymentDataDTO): Observable<ApplicationStatusesEnum> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        return this.requestService.post(this.area, this.controller, 'EnterApplicationPaymentData', paymentData, {
            httpParams: params
        });
    }

    public renewApplicationPayment(applicationId: number): Observable<ApplicationStatusesEnum> {
        const params = new HttpParams().append('applicationId', applicationId.toString());
        return this.requestService.post(this.area, this.controller, 'RenewApplicationPayment', null, {
            httpParams: params
        });
    }

    public applicationPaymentRefusal(applicationId: number, reason: string): Observable<void> {
        throw new Error('Method should not be called from public app');
    }

    public downloadFile(fileId: number, fileName: string): Observable<boolean> {
        const params = new HttpParams().append('id', fileId.toString());
        return this.requestService.download(this.area, this.controller, 'DownloadFile', fileName, { httpParams: params });
    }

    public completeApplicationFillingByApplicant(applicationId: number): Observable<boolean> {
        const params = new HttpParams().append('applicationId', applicationId.toString());
        return this.requestService.download(this.area, this.controller, 'CompleteApplicationFillingByApplicantAndDownload', '', {
            httpParams: params
        });
    }

    public downloadGeneratedApplicationFile(applicationId: number): Observable<boolean> {
        const params = new HttpParams().append('applicationId', applicationId.toString());
        return this.requestService.download(this.area, this.controller, 'DownloadGeneratedApplicationFile', 'TODO-name again', {
            httpParams: params
        });
    }

    public goBackToFillApplicationByApplicant(applicationId: number): Observable<boolean> {
        const params = new HttpParams().append('applicationId', applicationId.toString());
        return this.requestService.patch(this.area, this.controller, 'GoBackToFillApplicationByApplicant', null, {
            httpParams: params
        });
    }

    public uploadSignedApplication(applicationId: number, file: FileInfoDTO): Observable<ApplicationStatusesEnum> {
        const params = new HttpParams().append('applicationId', applicationId.toString());
        return this.requestService.post(this.area, this.controller, 'UploadSignedApplicationAndStartRegixChecks', file, {
            httpParams: params,
            properties: new RequestProperties({
                asFormData: true
            })
        });
    }

    public manualRegixChecksStart(applicationId: number): Observable<ApplicationStatusesEnum> {
        throw new Error('Method should not be called from public app');
    }

    public updateStateMachineStatusMock(applicationId: number, wantedErrorLevel: string): Observable<void> {
        throw new Error('Method should not be called from public app');
    }
}