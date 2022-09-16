import { Observable } from "rxjs";
import { GridRequestModel } from "@app/models/common/grid-request.model";
import { GridResultModel } from "@app/models/common/grid-result.model";
import { LegalEntityReportDTO } from "@app/models/generated/dtos/LegalEntityReportDTO";
import { PersonReportDTO } from "@app/models/generated/dtos/PersonReportDTO";
import { LegalEntitiesReportFilters } from "@app/models/generated/filters/LegalEntitiesReportFilters";
import { PersonsReportFilters } from "@app/models/generated/filters/PersonsReportFilters";
import { IBaseAuditService } from "../base-audit.interface";

export interface IPersonReportsService extends IBaseAuditService {
    getAllPersonsReport(request: GridRequestModel<PersonsReportFilters>): Observable<GridResultModel<PersonReportDTO>>;

    getAllLegalEntitiesReport(request: GridRequestModel<LegalEntitiesReportFilters>): Observable<GridResultModel<LegalEntityReportDTO>>;

    getPersonReport(id: number): Observable<PersonReportDTO>;

    getLegalEntityReport(id: number): Observable<LegalEntityReportDTO>;
}