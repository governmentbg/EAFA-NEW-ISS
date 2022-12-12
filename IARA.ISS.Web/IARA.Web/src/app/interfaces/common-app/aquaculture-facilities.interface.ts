import { Observable } from 'rxjs';

import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { AquacultureFacilityDTO } from '@app/models/generated/dtos/AquacultureFacilityDTO';
import { AquacultureFacilityEditDTO } from '@app/models/generated/dtos/AquacultureFacilityEditDTO';
import { AquacultureFacilitiesFilters } from '@app/models/generated/filters/AquacultureFacilitiesFilters';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { CancellationHistoryEntryDTO } from '@app/models/generated/dtos/CancellationHistoryEntryDTO';
import { IBaseAuditService } from '../base-audit.interface';
import { IApplicationsActionsService } from './application-actions.interface';
import { ExcelExporterRequestModel } from '@app/shared/components/data-table/models/excel-exporter-request-model.model';

export interface IAquacultureFacilitiesService extends IApplicationsActionsService, IBaseAuditService {
    // Register
    getAllAquacultures(request: GridRequestModel<AquacultureFacilitiesFilters>): Observable<GridResultModel<AquacultureFacilityDTO>>;
    getAquaculture(id: number): Observable<AquacultureFacilityEditDTO>;
    addAquaculture(aquaculture: AquacultureFacilityEditDTO, ignoreLogBookConflicts: boolean): Observable<number>;
    editAquaculture(aquaculture: AquacultureFacilityEditDTO, ignoreLogBookConflicts: boolean): Observable<void>;
    editAndDownloadAquaculture(aquaculture: AquacultureFacilityEditDTO, ignoreLogBookConflicts: boolean): Observable<boolean>;
    updateAquacultureStatus(aquacultureId: number, status: CancellationHistoryEntryDTO, applicationId?: number): Observable<void>;
    deleteAquaculture(id: number): Observable<void>;
    undoDeleteAquaculture(id: number): Observable<void>;
    downloadAquacultureFacility(aquacultureId: number): Observable<boolean>;
    downloadAquacultureFacilitiesExcel(request: ExcelExporterRequestModel<AquacultureFacilitiesFilters>): Observable<boolean>;

    getAquacultureFromChangeOfCircumstancesApplication(applicationId: number): Observable<AquacultureFacilityEditDTO>;
    getAquacultureFromDeregistrationApplication(applicationId: number): Observable<AquacultureFacilityEditDTO>;
    completeChangeOfCircumstancesApplication(aquaculture: AquacultureFacilityEditDTO, ignoreLogBookConflicts: boolean): Observable<void>;

    getInstallationAudit(id: number): Observable<SimpleAuditDTO>;
    getInstallationNetCageAudit(id: number): Observable<SimpleAuditDTO>;
    getUsageDocumentAudit(id: number): Observable<SimpleAuditDTO>;
    getWaterLawCertificateAudit(id: number): Observable<SimpleAuditDTO>;
    getOvosCertificateAudit(id: number): Observable<SimpleAuditDTO>;
    getBabhCertificateAudit(id: number): Observable<SimpleAuditDTO>;
    getLogBookAudit(id: number): Observable<SimpleAuditDTO>;

    // Nomenclatures
    getAllAquacultureNomenclatures(): Observable<NomenclatureDTO<number>[]>;
    getAquaculturePowerSupplyTypes(): Observable<NomenclatureDTO<number>[]>;
    getAquacultureWaterAreaTypes(): Observable<NomenclatureDTO<number>[]>;
    getWaterLawCertificateTypes(): Observable<NomenclatureDTO<number>[]>;
    getInstallationTypes(): Observable<NomenclatureDTO<number>[]>;
    getInstallationBasinPurposeTypes(): Observable<NomenclatureDTO<number>[]>;
    getInstallationBasinMaterialTypes(): Observable<NomenclatureDTO<number>[]>;
    getHatcheryEquipmentTypes(): Observable<NomenclatureDTO<number>[]>;
    getInstallationNetCageTypes(): Observable<NomenclatureDTO<number>[]>;
    getInstallationCollectorTypes(): Observable<NomenclatureDTO<number>[]>;
    getAquacultureStatusTypes(): Observable<NomenclatureDTO<number>[]>;
}