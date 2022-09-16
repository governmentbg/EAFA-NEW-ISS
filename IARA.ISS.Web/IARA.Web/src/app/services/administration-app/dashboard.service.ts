import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { PageCodeEnum } from '@app/enums/page-code.enum';
import { TicketTypeEnum } from '@app/enums/ticket-type.enum';
import { IDashboardService } from '@app/interfaces/administration-app/dashboard.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { ApplicationRegisterDTO } from '@app/models/generated/dtos/ApplicationRegisterDTO';
import { RecreationalFishingTicketApplicationDTO } from '@app/models/generated/dtos/RecreationalFishingTicketApplicationDTO';
import { StatusCountReportDataDTO } from '@app/models/generated/dtos/StatusCountReportDataDTO';
import { TicketTypesCountReportDTO } from '@app/models/generated/dtos/TicketTypesCountReportDTO';
import { TypesCountReportDTO } from '@app/models/generated/dtos/TypesCountReportDTO';
import { ApplicationsRegisterFilters } from '@app/models/generated/filters/ApplicationsRegisterFilters';
import { RecreationalFishingTicketApplicationFilters } from '@app/models/generated/filters/RecreationalFishingTicketApplicationFilters';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { CommonUtils } from '@app/shared/utils/common.utils';

@Injectable({
    providedIn: 'root'
})
export class DashboardService implements IDashboardService {
    private readonly area: AreaTypes = AreaTypes.Administrative;

    private controller: string = 'Dashboard';
    private requestService: RequestService;

    public constructor(requestService: RequestService) {
        this.requestService = requestService;
    }

    public getAllApplications(request: GridRequestModel<ApplicationsRegisterFilters>): Observable<GridResultModel<ApplicationRegisterDTO>> {
        return this.callGetAllApplications(request, 'GetAllApplications');
    }

    public getAllApplicationsByUserId(request: GridRequestModel<ApplicationsRegisterFilters>): Observable<GridResultModel<ApplicationRegisterDTO>> {
        return this.callGetAllApplications(request, 'GetAllApplicationsByUserId');
    }

    public getAllTicketApplications(request: GridRequestModel<RecreationalFishingTicketApplicationFilters>): Observable<GridResultModel<RecreationalFishingTicketApplicationDTO>> {
        return this.callGetAllTicketApplications(request, 'GetAllTicketApplications');
    }

    public getStatusCountReportData(shouldFilterByCurrentUser: boolean = false, isTickets: boolean = false): Observable<StatusCountReportDataDTO> {
        const params = new HttpParams()
            .append('shouldFilterByCurrentUser', shouldFilterByCurrentUser.toString())
            .append('isTickets', isTickets.toString());

        return this.requestService.get(this.area, this.controller, 'GetStatusCountReportData', {
            httpParams: params,
            responseTypeCtr: StatusCountReportDataDTO
        });
    }

    public getTicketTypesCountReport(): Observable<TicketTypesCountReportDTO[]> {
        return this.requestService.get<TicketTypesCountReportDTO[]>(this.area, this.controller, 'GetTicketTypesCountReport', {
            responseTypeCtr: TicketTypesCountReportDTO
        }).pipe(map((entries: TicketTypesCountReportDTO[]) => {
            for (const entry of entries) {
                switch (entry.ticketTypeCode) {
                    case TicketTypeEnum.ASSOCIATION:
                        entry.icon = 'fa-id-card';
                        break;
                    case TicketTypeEnum.BETWEEN14AND18:
                        entry.icon = 'fa-child';
                        break;
                    case TicketTypeEnum.DISABILITY:
                        entry.icon = 'fa-wheelchair';
                        break;
                    case TicketTypeEnum.ELDER:
                        entry.icon = 'fa-book-reader';
                        break;
                    case TicketTypeEnum.STANDARD:
                        entry.icon = 'fa-user';
                        break;
                    case TicketTypeEnum.UNDER14:
                        entry.icon = 'fa-baby';
                        break;
                    case TicketTypeEnum.BETWEEN14AND18ASSOCIATION:
                        entry.icon = 'fa-child';
                        break;
                    case TicketTypeEnum.ELDERASSOCIATION:
                        entry.icon = 'fa-book-reader';
                        break;
                    default:
                        entry.icon = 'fa-user-alt';
                        break;
                }
            }
            return entries;
        }));
    }

    public getTypesCountReport(shouldFilterByCurrentUser: boolean = false): Observable<TypesCountReportDTO[]> {
        const params = new HttpParams().append('shouldFilterByCurrentUser', shouldFilterByCurrentUser.toString());

        return this.requestService.get<TypesCountReportDTO[]>(this.area, this.controller, 'GetTypesCountReport', {
            httpParams: params,
            responseTypeCtr: TypesCountReportDTO
        }).pipe(map((entries: TypesCountReportDTO[]) => {
            for (const entry of entries) {
                switch (entry.pageCode) {
                    case PageCodeEnum.RegVessel:
                    case PageCodeEnum.ShipRegChange:
                    case PageCodeEnum.DeregShip:
                        entry.icon = CommonUtils.MENU_ICONS_MAP.get('fishing_vessels');
                        break;
                    case PageCodeEnum.CommFish:
                    case PageCodeEnum.RightToFishResource:
                    case PageCodeEnum.RightToFishThirdCountry:
                    case PageCodeEnum.PoundnetCommFish:
                    case PageCodeEnum.PoundnetCommFishLic:
                    case PageCodeEnum.CatchQuataSpecies:
                    case PageCodeEnum.DupCommFish:
                    case PageCodeEnum.DupRightToFishThirdCountry:
                    case PageCodeEnum.DupPoundnetCommFish:
                    case PageCodeEnum.DupRightToFishResource:
                    case PageCodeEnum.DupPoundnetCommFishLic:
                    case PageCodeEnum.DupCatchQuataSpecies:
                        entry.icon = CommonUtils.MENU_ICONS_MAP.get('commercial_fishing');
                        break;
                    case PageCodeEnum.AptitudeCourceExam:
                    case PageCodeEnum.CommFishLicense:
                    case PageCodeEnum.CompetencyDup:
                        entry.icon = CommonUtils.MENU_ICONS_MAP.get('qualified_fishers');
                        break;
                    case PageCodeEnum.ChangeFirstSaleBuyer:
                    case PageCodeEnum.ChangeFirstSaleCenter:
                    case PageCodeEnum.RegFirstSaleBuyer:
                    case PageCodeEnum.RegFirstSaleCenter:
                    case PageCodeEnum.TermFirstSaleBuyer:
                    case PageCodeEnum.TermFirstSaleCenter:
                    case PageCodeEnum.DupFirstSaleBuyer:
                    case PageCodeEnum.DupFirstSaleCenter:
                        entry.icon = CommonUtils.MENU_ICONS_MAP.get('buyers_and_sales_centers');
                        break;
                    case PageCodeEnum.SciFi:
                        entry.icon = CommonUtils.MENU_ICONS_MAP.get('scientific_fishing');
                        break;
                    case PageCodeEnum.LE:
                        entry.icon = CommonUtils.MENU_ICONS_MAP.get('personal_data_legal_entities_and_persons_reports');
                        break;
                    case PageCodeEnum.AquaFarmReg:
                    case PageCodeEnum.AquaFarmChange:
                    case PageCodeEnum.AquaFarmDereg:
                        entry.icon = CommonUtils.MENU_ICONS_MAP.get('aqua_culture_farms');
                        break;
                    case PageCodeEnum.IncreaseFishCap:
                    case PageCodeEnum.ReduceFishCap:
                    case PageCodeEnum.TransferFishCap:
                    case PageCodeEnum.CapacityCertDup:
                        entry.icon = CommonUtils.MENU_ICONS_MAP.get('fishing-capacity');
                        break;
                    case PageCodeEnum.StatFormAquaFarm:
                    case PageCodeEnum.StatFormFishVessel:
                    case PageCodeEnum.StatFormRework:
                        entry.icon = CommonUtils.MENU_ICONS_MAP.get('statistical_forms');
                        break;
                    default:
                        entry.icon = CommonUtils.MENU_ICONS_MAP.get('application_processing');
                        break;
                }
            }
            return entries;
        }));
    }

    private callGetAllApplications(request: GridRequestModel<ApplicationsRegisterFilters>, serviceMethod: string): Observable<GridResultModel<ApplicationRegisterDTO>> {
        if (request.filters === null || request.filters === undefined) {
            request.filters = new ApplicationsRegisterFilters({ showOnlyNotFinished: true });
        }
        else {
            request.filters.showOnlyNotFinished = true;
        }

        return this.requestService.post(this.area, this.controller, serviceMethod, request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridRequestModel
        });
    }

    private callGetAllTicketApplications(
        request: GridRequestModel<RecreationalFishingTicketApplicationFilters>,
        serviceMethod: string
    ): Observable<GridResultModel<RecreationalFishingTicketApplicationDTO>> {
        if (request.filters === null || request.filters === undefined) {
            request.filters = new RecreationalFishingTicketApplicationFilters({ showOnlyNotFinished: true });
        }
        else {
            request.filters.showOnlyNotFinished = true;
        }

        return this.requestService.post(this.area, this.controller, serviceMethod, request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        });
    }
}