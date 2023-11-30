import { Observable } from 'rxjs';
import { ApplicationStatusesEnum } from '@app/enums/application-statuses.enum';
import { ApplicationContentDTO } from '@app/models/generated/dtos/ApplicationContentDTO';
import { ApplicationTypeDTO } from '@app/models/generated/dtos/ApplicationTypeDTO';
import { FileInfoDTO } from '@app/models/generated/dtos/FileInfoDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PaymentDataDTO } from '@app/models/generated/dtos/PaymentDataDTO';
import { PaymentSummaryDTO } from '@app/models/generated/dtos/PaymentSummaryDTO';
import { AddApplicationResultDTO } from '@app/models/generated/dtos/AddApplicationResultDTO';

export interface IApplicationsService {
    getApplicationTypes(): Observable<NomenclatureDTO<number>[]>;
    getApplicationTypesForChoice(): Observable<ApplicationTypeDTO[]>;
    getApplicationChangeHistoryContent(applicationChangeHistoryId: number): Observable<ApplicationContentDTO>;
    getApplicationPaymentSummary(applicationId: number): Observable<PaymentSummaryDTO>;
    addApplication(applicationTypeId: number): Observable<AddApplicationResultDTO>;
    enterEventisNumber(applicationId: number, eventisNumber: string): Observable<void>;
    saveDraftContent<T>(applicationId: number, model: T): Observable<void>;
    applicationAnnulment(applicationId: number, reason: string): Observable<void>;
    initiateApplicationCorrections(applicationId: number): Observable<ApplicationStatusesEnum>;
    sendApplicationForUserCorrections(applicationId: number, statusReason: string): Observable<ApplicationStatusesEnum>;
    confirmNoErrorsForApplication(applicationId: number): Observable<ApplicationStatusesEnum>;
    confirmApplicationDataIrregularity(applicationId: number, statusReason: string): Observable<ApplicationStatusesEnum>;
    confirmApplicationDataRegularity(applicationId: number): Observable<ApplicationStatusesEnum>;
    enterPaymentData(applicationId: number, paymentData: PaymentDataDTO): Observable<ApplicationStatusesEnum>;
    renewApplicationPayment(applicationId: number): Observable<ApplicationStatusesEnum>;
    applicationPaymentRefusal(applicationId: number, reason: string): Observable<void>;
    downloadFile(fileId: number, fileName: string): Observable<boolean>;
    uploadSignedApplication(applicationId: number, file: FileInfoDTO): Observable<ApplicationStatusesEnum>;
    manualRegixChecksStart(applicationId: number): Observable<ApplicationStatusesEnum>;
    completeApplicationFillingByApplicant(applicationId: number): Observable<boolean>;
    downloadGeneratedApplicationFile(applicationId: number): Observable<boolean>;
    goBackToFillApplicationByApplicant(applicationId: number): Observable<boolean>;
    getApplicationAccessCode(applicationId: number): Observable<string>;

    updateStateMachineStatusMock(applicationId: number, wantedErrorLevel: string): Observable<void>;
}
