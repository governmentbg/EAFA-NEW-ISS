import { Observable } from 'rxjs';

import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { AuanRegisterDTO } from '@app/models/generated/dtos/AuanRegisterDTO';
import { AuanRegisterEditDTO } from '@app/models/generated/dtos/AuanRegisterEditDTO';
import { AuanRegisterFilters } from '@app/models/generated/filters/AuanRegisterFilters';
import { IBaseAuditService } from '@app/interfaces/base-audit.interface';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { AuanReportDataDTO } from '@app/models/generated/dtos/AuanReportDataDTO';
import { AuanConfiscationActionsNomenclatureDTO } from '@app/models/generated/dtos/AuanConfiscationActionsNomenclatureDTO';
import { InspDeliveryTypesNomenclatureDTO } from '@app/models/generated/dtos/InspDeliveryTypesNomenclatureDTO';

export interface IAuanRegisterService extends IBaseAuditService {
    getAllAuans(request: GridRequestModel<AuanRegisterFilters>): Observable<GridResultModel<AuanRegisterDTO>>;

    getAuan(id: number): Observable<AuanRegisterEditDTO>;
    addAuan(auan: AuanRegisterEditDTO): Observable<number>;
    editAuan(auan: AuanRegisterEditDTO): Observable<void>;
    deleteAuan(id: number): Observable<void>;
    undoDeleteAuan(id: number): Observable<void>;
    downloadAuan(auanId: number): Observable<boolean>;

    getAuanReportData(inspectionId: number): Observable<AuanReportDataDTO>;
    downloadFile(fileId: number, fileName: string): Observable<boolean>;

    getAllDrafters(): Observable<NomenclatureDTO<number>[]>;
    getAllInspectionReports(): Observable<NomenclatureDTO<number>[]>;

    getConfiscationActions(): Observable<AuanConfiscationActionsNomenclatureDTO[]>;
    getAuanDeliveryTypes(): Observable<InspDeliveryTypesNomenclatureDTO[]>;
    getAuanDeliveryConfirmationTypes(): Observable<InspDeliveryTypesNomenclatureDTO[]>;
    getAuanStatuses(): Observable<NomenclatureDTO<number>[]>;
    getConfiscatedAppliances(): Observable<NomenclatureDTO<number>[]>;
    getTurbotSizeGroups(): Observable<NomenclatureDTO<number>[]>;
}