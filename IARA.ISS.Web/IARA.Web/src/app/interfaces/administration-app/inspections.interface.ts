import { Observable } from "rxjs";

import { GridRequestModel } from "@app/models/common/grid-request.model";
import { GridResultModel } from "@app/models/common/grid-result.model";
import { InspectionDTO } from "@app/models/generated/dtos/InspectionDTO";
import { InspectionsFilters } from "@app/models/generated/filters/InspectionsFilters";
import { IBaseAuditService } from "@app/interfaces/base-audit.interface";
import { NomenclatureDTO } from "@app/models/generated/dtos/GenericNomenclatureDTO";
import { InspectorDTO } from "@app/models/generated/dtos/InspectorDTO";
import { VesselDTO } from "@app/models/generated/dtos/VesselDTO";
import { InspectionShipSubjectNomenclatureDTO } from "@app/models/generated/dtos/InspectionShipSubjectNomenclatureDTO";
import { InspectionCheckTypeNomenclatureDTO } from "@app/models/generated/dtos/InspectionCheckTypeNomenclatureDTO";
import { InspectionTypesEnum } from "@app/enums/inspection-types.enum";
import { InspectionShipLogBookDTO } from "@app/models/generated/dtos/InspectionShipLogBookDTO";
import { InspectionPermitLicenseDTO } from "@app/models/generated/dtos/InspectionPermitLicenseDTO";

export interface IInspectionsService extends IBaseAuditService {
    getAll(
        request: GridRequestModel<InspectionsFilters>
    ): Observable<GridResultModel<InspectionDTO>>;
    delete(id: number): Observable<void>;
    restore(id: number): Observable<void>;

    downloadFile(fileId: number, fileName: string): Observable<boolean>;

    getCurrentInspector(): Observable<InspectorDTO>;
    getInspector(id: number): Observable<InspectorDTO>;
    getInspectors(): Observable<NomenclatureDTO<number>[]>;
    getPatrolVehicles(
        isWaterVehicle: boolean
    ): Observable<NomenclatureDTO<number>[]>;
    getPatrolVehicle(id: number): Observable<VesselDTO>;
    getShip(id: number): Observable<VesselDTO>;
    getShipPersonnel(
        shipId: number
    ): Observable<InspectionShipSubjectNomenclatureDTO[]>;
    getCheckTypesForInspection(
        inspectionType: InspectionTypesEnum
    ): Observable<InspectionCheckTypeNomenclatureDTO[]>;
    getShipPermitLicenses(
        shipId: number
    ): Observable<InspectionPermitLicenseDTO[]>;
    getShipLogBooks(shipId: number): Observable<InspectionShipLogBookDTO[]>;
    canResolveCrossChecks(): Observable<boolean>;
}
