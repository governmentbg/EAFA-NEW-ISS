import { Observable } from 'rxjs';

import { IApplicationsActionsService } from './application-actions.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { MaximumFishingCapacityDTO } from '@app/models/generated/dtos/MaximumFishingCapacityDTO';
import { MaximumFishingCapacityEditDTO } from '@app/models/generated/dtos/MaximumFishingCapacityEditDTO';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { MaximumFishingCapacityFilters } from '@app/models/generated/filters/MaximumFishingCapacityFilters';
import { FishingCapacityCertificateDTO } from '@app/models/generated/dtos/FishingCapacityCertificateDTO';
import { FishingCapacityCertificateEditDTO } from '@app/models/generated/dtos/FishingCapacityCertificateEditDTO';
import { IBaseAuditService } from '@app/interfaces/base-audit.interface';
import { LatestMaximumCapacityDTO } from '@app/models/generated/dtos/LatestMaximumCapacityDTO';
import { FishingCapacityCertificateNomenclatureDTO } from '@app/models/generated/dtos/FishingCapacityCertificateNomenclatureDTO';
import { FishingCapacityCertificatesFilters } from '@app/models/generated/filters/FishingCapacityCertificatesFilters';
import { FishingCapacityFilters } from '@app/models/generated/filters/FishingCapacityFilters';
import { ShipFishingCapacityDTO } from '@app/models/generated/dtos/ShipFishingCapacityDTO';
import { TransferFishingCapacityApplicationDTO } from '@app/models/generated/dtos/TransferFishingCapacityApplicationDTO';
import { FishingCapacityStatisticsDTO } from '@app/models/generated/dtos/FishingCapacityStatistics';
import { CapacityCertificateDuplicateApplicationDTO } from '@app/models/generated/dtos/CapacityCertificateDuplicateApplicationDTO';
import { ExcelExporterRequestModel } from '../../shared/components/data-table/models/excel-exporter-request-model.model';

export interface IFishingCapacityService extends IApplicationsActionsService, IBaseAuditService {
    getAllCapacityCertificates(request: GridRequestModel<FishingCapacityCertificatesFilters>): Observable<GridResultModel<FishingCapacityCertificateDTO>>;
    getCapacityCertificate(id: number): Observable<FishingCapacityCertificateEditDTO>;
    editCapacityCertificate(certificate: FishingCapacityCertificateEditDTO): Observable<void>;
    deleteCapacityCertificate(id: number): Observable<void>;
    undoDeleteCapacityCertificate(id: number): Observable<void>;
    getFishingCapacityCertificateSimpleAudit(id: number): Observable<SimpleAuditDTO>;
    getAllCapacityCertificateNomenclatures(): Observable<FishingCapacityCertificateNomenclatureDTO[]>;
    downloadFishingCapacityCertificate(certificateId: number): Observable<boolean>;
    downloadFishingCapacityCertificateExcel(request: ExcelExporterRequestModel<FishingCapacityCertificatesFilters>): Observable<boolean>;

    completeTransferFishingCapacityApplication(model: TransferFishingCapacityApplicationDTO): Observable<void>;
    completeCapacityCertificateDuplicateApplication(model: CapacityCertificateDuplicateApplicationDTO): Observable<void>;

    getAllShipCapacities(request: GridRequestModel<FishingCapacityFilters>): Observable<GridResultModel<ShipFishingCapacityDTO>>;

    getAllMaximumCapacities(request: GridRequestModel<MaximumFishingCapacityFilters>): Observable<GridResultModel<MaximumFishingCapacityDTO>>;
    getMaximumCapacity(id: number): Observable<MaximumFishingCapacityEditDTO>;
    getLatestMaximumCapacities(): Observable<LatestMaximumCapacityDTO>;
    addMaximumCapacity(capacity: MaximumFishingCapacityEditDTO): Observable<number>;
    editMaximumCapacity(capacity: MaximumFishingCapacityEditDTO): Observable<void>;
    getMaximumCapacitySimpleAudit(id: number): Observable<SimpleAuditDTO>;

    downloadFile(fileId: number, fileName: string): Observable<boolean>;

    getFishingCapacityStatistics(date: Date): Observable<FishingCapacityStatisticsDTO>;

    getFishingCapacityHolderAudit(id: number): Observable<SimpleAuditDTO>;
}