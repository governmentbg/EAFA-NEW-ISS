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
import { AuanInspectionDTO } from '@app/models/generated/dtos/AuanInspectionDTO';
import { AuanLawSectionDTO } from '@app/models/generated/dtos/AuanLawSectionDTO';
import { AuanDrafterNomenclatureDTO } from '@app/models/generated/dtos/AuanDrafterNomenclatureDTO';
import { InspectorUserNomenclatureDTO } from '@app/models/generated/dtos/InspectorUserNomenclatureDTO';

export interface IAuanRegisterService extends IBaseAuditService {
    getAllAuans(request: GridRequestModel<AuanRegisterFilters>): Observable<GridResultModel<AuanRegisterDTO>>;

    getAuan(id: number): Observable<AuanRegisterEditDTO>;
    addAuan(auan: AuanRegisterEditDTO): Observable<number>;
    editAuan(auan: AuanRegisterEditDTO): Observable<void>;
    deleteAuan(id: number): Observable<void>;
    undoDeleteAuan(id: number): Observable<void>;
    downloadAuan(auanId: number): Observable<boolean>;

    addAuanInspection(inspection: AuanInspectionDTO): Observable<number>;
    getAuanReportData(inspectionId: number): Observable<AuanReportDataDTO>;
    downloadFile(fileId: number, fileName: string): Observable<boolean>;

    getAllDrafters(): Observable<AuanDrafterNomenclatureDTO[]>;
    getInspectionDrafters(inspectionId: number): Observable<NomenclatureDTO<number>[]>;
    getAllInspectionReports(): Observable<NomenclatureDTO<number>[]>;

    getConfiscationActions(): Observable<AuanConfiscationActionsNomenclatureDTO[]>;
    getAuanDeliveryConfirmationTypes(): Observable<InspDeliveryTypesNomenclatureDTO[]>;
    getAuanLawSections(): Observable<AuanLawSectionDTO[]>;
    getConfiscatedAppliances(): Observable<NomenclatureDTO<number>[]>;
    getTurbotSizeGroups(): Observable<NomenclatureDTO<number>[]>;
    getInspectorUsernames(): Observable<InspectorUserNomenclatureDTO[]>;
}