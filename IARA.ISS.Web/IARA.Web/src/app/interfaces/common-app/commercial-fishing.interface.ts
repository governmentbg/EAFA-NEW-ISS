import { Observable } from 'rxjs';
import { PageCodeEnum } from '@app/enums/page-code.enum';

import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { CommercialFishingEditDTO } from '@app/models/generated/dtos/CommercialFishingEditDTO';
import { CommercialFishingPermitRegisterDTO } from '@app/models/generated/dtos/CommercialFishingPermitRegisterDTO';
import { CommercialFishingRegisterFilters } from '@app/models/generated/filters/CommercialFishingRegisterFilters';
import { IApplicationsActionsService } from './application-actions.interface';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { CommercialFishingApplicationEditDTO } from '@app/models/generated/dtos/CommercialFishingApplicationEditDTO';
import { SuspensionTypeNomenclatureDTO } from '@app/models/generated/dtos/SuspensionTypeNomenclatureDTO';
import { SuspensionReasonNomenclatureDTO } from '@app/models/generated/dtos/SuspensionReasonNomenclatureDTO';
import { PermitNomenclatureDTO } from '@app/models/generated/dtos/PermitNomenclatureDTO';
import { PermitLicenseForRenewalDTO } from '@app/models/generated/dtos/PermitLicenseForRenewalDTO';
import { PoundNetNomenclatureDTO } from '@app/models/generated/dtos/PoundNetNomenclatureDTO';
import { QualifiedFisherNomenclatureDTO } from '@app/models/generated/dtos/QualifiedFisherNomenclatureDTO';
import { PaymentTariffDTO } from '@app/models/generated/dtos/PaymentTariffDTO';
import { PermitLicenseTariffCalculationParameters } from '@app/components/common-app/commercial-fishing/models/permit-license-tariff-calculation-parameters.model';
import { SuspensionDataDTO } from '@app/models/generated/dtos/SuspensionDataDTO';
import { ISuspensionService } from './suspension.interface';
import { InspectedPermitLicenseNomenclatureDTO } from '@app/models/generated/dtos/InspectedPermitLicenseNomenclatureDTO';
import { FishingGearForChoiceDTO } from '@app/models/generated/dtos/FishingGearForChoiceDTO';
import { FishingGearDTO } from '@app/models/generated/dtos/FishingGearDTO';
import { PermitLicensesNomenclatureDTO } from '@app/models/generated/dtos/PermitLicensesNomenclatureDTO';
import { IdentifierTypeEnum } from '@app/enums/identifier-type.enum';
import { PersonFullDataDTO } from '@app/models/generated/dtos/PersonFullDataDTO';

export interface ICommercialFishingService extends IApplicationsActionsService, ISuspensionService {
    getAllPermits(request: GridRequestModel<CommercialFishingRegisterFilters>): Observable<GridResultModel<CommercialFishingPermitRegisterDTO>>;

    getRecord(id: number, pageCode: PageCodeEnum): Observable<CommercialFishingEditDTO>;

    addPermit(permit: CommercialFishingEditDTO, pageCode: PageCodeEnum, ignoreLogBookConflicts: boolean): Observable<number>;
    addAndDownloadRegister(model: CommercialFishingEditDTO, pageCode: PageCodeEnum, ignoreLogBookConflicts: boolean): Observable<boolean>;

    editPermit(permit: CommercialFishingEditDTO, pageCode: PageCodeEnum, ignoreLogBookConflicts: boolean): Observable<void>;
    editAndDownloadRegister(model: CommercialFishingEditDTO, pageCode: PageCodeEnum, ignoreLogBookConflicts: boolean): Observable<boolean>;

    downloadRegister(id: number, pageCode: PageCodeEnum): Observable<boolean>;
    downloadFile(fileId: number, fileName: string): Observable<boolean>;

    getPermitLicenseFisherPhoto(id: number): Observable<string>;
    getPermitLicenseFisherPhotoFromApplication(applicationId: number): Observable<string>;
    getPermitFisherPhoto(id: number): Observable<string>;
    getPermitFisherPhotoFromApplication(applicationId: number): Observable<string>;

    addSuspension(suspension: SuspensionDataDTO, id: number, pageCode: PageCodeEnum): Observable<void>;

    deletePermit(id: number, pageCode: PageCodeEnum): Observable<void>;
    undoDeletePermit(id: number, pageCode: PageCodeEnum): Observable<void>;

    deleteLogBookPermitLicense(logBookPermitLicenseId: number): Observable<void>;
    undoDeleteLogBookPermitLicense(logBookPermitLicenseId: number): Observable<void>;

    getPermitLicensesForRenewal(permitId: number | undefined, permitNumber: string | undefined, pageCode: PageCodeEnum): Observable<PermitLicenseForRenewalDTO[]>;
    getPermitLicenseData(permitLicenseId: number): Observable<CommercialFishingApplicationEditDTO>;
    getPermitLicenseApplicationDataFromPermitId(permitId: number, applicationId: number): Observable<CommercialFishingApplicationEditDTO>;
    getPermitLicenseApplicationDataFromPermitNumber(permitNumber: string, applicationId: number): Observable<CommercialFishingApplicationEditDTO>;

    addPermitApplicationAndStartPermitLicenseApplication(permit: CommercialFishingApplicationEditDTO): Observable<CommercialFishingApplicationEditDTO>;

    getPermitLicenseFromFishingGearsApplication(applicationId: number): Observable<CommercialFishingEditDTO>;
    completePermitLicenseFishingGearsApplication(permitLicense: CommercialFishingEditDTO): Observable<void>;

    getCommercialFishingPermitTypes(): Observable<NomenclatureDTO<number>[]>;
    getCommercialFishingPermitLicenseTypes(): Observable<NomenclatureDTO<number>[]>;
    getQualifiedFishers(): Observable<QualifiedFisherNomenclatureDTO[]>;
    getWaterTypes(): Observable<NomenclatureDTO<number>[]>;
    getFishingGearMarkStatuses(): Observable<NomenclatureDTO<number>[]>;
    getFishingGearPingerStatuses(): Observable<NomenclatureDTO<number>[]>;
    getHolderGroundForUseTypes(): Observable<NomenclatureDTO<number>[]>;
    getPoundNets(): Observable<PoundNetNomenclatureDTO[]>;
    getSuspensionTypes(): Observable<SuspensionTypeNomenclatureDTO[]>;
    getSuspensionReasons(): Observable<SuspensionReasonNomenclatureDTO[]>;
    getPermitNomenclatures(shipId: number, showPastPermits: boolean, onlyPoundNet: boolean): Observable<PermitNomenclatureDTO[]>;
    calculatePermitLicenseAppliedTariffs(tariffCalculationParameters: PermitLicenseTariffCalculationParameters): Observable<PaymentTariffDTO[]>;
    getShipPermitLicensesFromInspection(shipId: number): Observable<InspectedPermitLicenseNomenclatureDTO[]>;
    getShipFishingGearsFromInspection(inspectionId: number): Observable<FishingGearForChoiceDTO[]>;
    getFishingGearsForIds(gearIds: number[]): Observable<FishingGearDTO[]>;
    getPermitLicensesNomenclatures(shipId: number): Observable<PermitLicensesNomenclatureDTO[]>;
    getPermitLicenseFishingGears(permitLicenseId: number): Observable<FishingGearDTO[]>;
    getFishingGearsByPermitLicenseRegistrationNumber(permitLicenseNumber: string, shipId: number): Observable<FishingGearDTO[]>;
    tryGetQualifiedFisher(identifierType: IdentifierTypeEnum, identifier: string): Observable<PersonFullDataDTO | undefined>;

    getSimpleAudit(id: number): Observable<SimpleAuditDTO>;
    getPermitLicenseSimpleAudit(id: number): Observable<SimpleAuditDTO>;
    getPermitSuspensionAudit(id: number): Observable<SimpleAuditDTO>;
    getPermitLicenseSuspensionAudit(id: number): Observable<SimpleAuditDTO>;
    getFishingGearAudit(id: number): Observable<SimpleAuditDTO>;
    getLogBookAudit(id: number): Observable<SimpleAuditDTO>;

    downloadPermitFluxXml(permit: CommercialFishingEditDTO): Observable<boolean>;
}