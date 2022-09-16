import { Observable } from "rxjs";
import { GridRequestModel } from "@app/models/common/grid-request.model";
import { GridResultModel } from "@app/models/common/grid-result.model";
import { ApplicationRegisterDTO } from "@app/models/generated/dtos/ApplicationRegisterDTO";
import { RecreationalFishingTicketApplicationDTO } from "@app/models/generated/dtos/RecreationalFishingTicketApplicationDTO";
import { StatusCountReportDataDTO } from "@app/models/generated/dtos/StatusCountReportDataDTO";
import { TicketTypesCountReportDTO } from "@app/models/generated/dtos/TicketTypesCountReportDTO";
import { TypesCountReportDTO } from "@app/models/generated/dtos/TypesCountReportDTO";
import { ApplicationsRegisterFilters } from "@app/models/generated/filters/ApplicationsRegisterFilters";
import { RecreationalFishingTicketApplicationFilters } from "@app/models/generated/filters/RecreationalFishingTicketApplicationFilters";

export interface IDashboardService {
    getAllApplicationsByUserId(request: GridRequestModel<ApplicationsRegisterFilters>): Observable<GridResultModel<ApplicationRegisterDTO>>;
    getAllApplications(request: GridRequestModel<ApplicationsRegisterFilters>): Observable<GridResultModel<ApplicationRegisterDTO>>;
    getAllTicketApplications(request: GridRequestModel<RecreationalFishingTicketApplicationFilters>): Observable<GridResultModel<RecreationalFishingTicketApplicationDTO>>;
    getStatusCountReportData(shouldFilterByCurrentUser: boolean, isTickets: boolean): Observable<StatusCountReportDataDTO>;
    getTypesCountReport(shouldFilterByCurrentUser: boolean): Observable<TypesCountReportDTO[]>;
    getTicketTypesCountReport(): Observable<TicketTypesCountReportDTO[]>;
}