import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { IApplicationRegister } from '../../interfaces/common-app/application-register.interface';
import { IRecreationalFishingService } from '../../interfaces/common-app/recreational-fishing.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { ApplicationRegisterDTO } from '@app/models/generated/dtos/ApplicationRegisterDTO';
import { AssignedApplicationInfoDTO } from '@app/models/generated/dtos/AssignedApplicationInfoDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { RecreationalFishingTicketApplicationDTO } from '@app/models/generated/dtos/RecreationalFishingTicketApplicationDTO';
import { RecreationalFishingTicketBaseRegixDataDTO } from '@app/models/generated/dtos/RecreationalFishingTicketBaseRegixDataDTO';
import { RecreationalFishingTicketDTO } from '@app/models/generated/dtos/RecreationalFishingTicketDTO';
import { RecreationalFishingTicketPriceDTO } from '@app/models/generated/dtos/RecreationalFishingTicketPriceDTO';
import { RecreationalFishingTicketsDTO } from '@app/models/generated/dtos/RecreationalFishingTicketsDTO';
import { RecreationalFishingTicketValidationDTO } from '@app/models/generated/dtos/RecreationalFishingTicketValidationDTO';
import { RecreationalFishingTicketValidationResultDTO } from '@app/models/generated/dtos/RecreationalFishingTicketValidationResultDTO';
import { RecreationalFishingTicketValidToCalculationParamsDTO } from '@app/models/generated/dtos/RecreationalFishingTicketValidToCalculationParamsDTO';
import { RegixChecksWrapperDTO } from '@app/models/generated/dtos/RegixChecksWrapperDTO';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { ApplicationsRegisterFilters } from '@app/models/generated/filters/ApplicationsRegisterFilters';
import { RecreationalFishingTicketApplicationFilters } from '@app/models/generated/filters/RecreationalFishingTicketApplicationFilters';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { RecreationalFishingCommonService } from '../common-app/recreational-fishing-common.service';
import { ApplicationsRegisterAdministrativeBaseService } from './applications-register-administrative-base.service';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { RecreationalFishingTicketDeclarationParametersDTO } from '@app/models/generated/dtos/RecreationalFishingTicketDeclarationParametersDTO';
import { EgnLncDTO } from '@app/models/generated/dtos/EgnLncDTO';
import { RecreationalFishingAddTicketsResultDTO } from '@app/models/generated/dtos/RecreationalFishingAddTicketsResultDTO';
import { RecreationalFishingTicketDuplicateDTO } from '@app/models/generated/dtos/RecreationalFishingTicketDuplicateDTO';
import { ReasonDTO } from '@app/models/generated/dtos/ReasonDTO';
import { TicketStatusEnum } from '@app/enums/ticket-status.enum';
import { TerritoryUnitNomenclatureDTO } from '@app/models/generated/dtos/TerritoryUnitNomenclatureDTO';

@Injectable({
    providedIn: 'root'
})
export class RecreationalFishingAdministrationService extends ApplicationsRegisterAdministrativeBaseService implements IRecreationalFishingService {
    protected controller: string = 'RecreationalFishingAdministration';

    private commonService: RecreationalFishingCommonService;
    private translate: FuseTranslationLoaderService;

    public constructor(
        requestService: RequestService,
        translate: FuseTranslationLoaderService,
        commonService: RecreationalFishingCommonService,
    ) {
        super(requestService);

        this.translate = translate;
        this.commonService = commonService;
    }

    public getRegisterByApplicationId(applicationId: number, pageCode?: PageCodeEnum): Observable<unknown> {
        throw new Error('Method not implemented.');
    }

    public getApplication(id: number, getRegiXData: boolean): Observable<IApplicationRegister> {
        throw new Error('Method not implemented.');
    }

    public getApplicationDataForRegister(applicationId: number): Observable<IApplicationRegister> {
        throw new Error('Method not implemented.');
    }

    public getApplicationHistorySimpleAudit(id: number): Observable<SimpleAuditDTO> {
        throw new Error('Method not implemented.');
    }

    public addApplication(application: IApplicationRegister): Observable<number> {
        throw new Error('Method not implemented.');
    }

    public editApplication(application: IApplicationRegister, pageCode?: PageCodeEnum, fromSaveAsDraft?: boolean): Observable<number> {
        throw new Error('Method not implemented.');
    }

    public getTicket(id: number, getRegiXData: boolean): Observable<RecreationalFishingTicketDTO> {
        const params = new HttpParams()
            .append('id', id.toString())
            .append('getRegiXData', getRegiXData.toString());

        return this.requestService.get(this.area, this.controller, 'GetTicket', {
            httpParams: params,
            responseTypeCtr: RecreationalFishingTicketDTO
        });
    }

    public editTicket(ticket: RecreationalFishingTicketDTO): Observable<void> {
        return this.commonService.editTicket(this.area, this.controller, ticket);
    }

    public cancelTicket(id: number, reason: string): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        const reasonData: ReasonDTO = new ReasonDTO({ reason: reason });

        return this.requestService.patch(this.area, this.controller, 'CancelTicket', reasonData, {
            httpParams: params
        });
    }

    public cancelTicketRegister(id: number, reason: string): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        const reasonData: ReasonDTO = new ReasonDTO({ reason: reason });

        return this.requestService.patch(this.area, this.controller, 'CancelTicketRegister', reasonData, {
            httpParams: params
        });
    }

    public getRegixData(id: number): Observable<RegixChecksWrapperDTO<RecreationalFishingTicketBaseRegixDataDTO>> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get<RegixChecksWrapperDTO<RecreationalFishingTicketBaseRegixDataDTO>>(this.area, this.controller, 'GetTicketRegixData', {
            httpParams: params,
            responseTypeCtr: RegixChecksWrapperDTO
        }).pipe(map((result: RegixChecksWrapperDTO<RecreationalFishingTicketBaseRegixDataDTO>) => {
            result.dialogDataModel = new RecreationalFishingTicketBaseRegixDataDTO(result.dialogDataModel);
            result.regiXDataModel = new RecreationalFishingTicketBaseRegixDataDTO(result.regiXDataModel);

            return result;
        }));
    }

    public addTickets(tickets: RecreationalFishingTicketsDTO): Observable<RecreationalFishingAddTicketsResultDTO> {
        return this.commonService.addTickets(this.area, this.controller, tickets);
    }

    public addTicketDuplicate(data: RecreationalFishingTicketDuplicateDTO): Observable<number> {
        return this.commonService.addTicketDuplicate(this.area, this.controller, data);
    }

    public calculateTicketValidToDate(params: RecreationalFishingTicketValidToCalculationParamsDTO): Observable<Date> {
        return this.commonService.calculateTicketValidToDate(this.area, this.controller, params);
    }

    public enterOnlineTicketOfflineNumber(id: number, ticketNum: string): Observable<boolean> {
        const params = new HttpParams()
            .append('id', id.toString())
            .append('ticketNum', ticketNum);

        return this.requestService.patch(this.area, this.controller, 'EnterOnlineTicketOfflineNumber', null, {
            httpParams: params
        });
    }

    public checkEgnLncPurchaseAbility(data: RecreationalFishingTicketValidationDTO): Observable<RecreationalFishingTicketValidationResultDTO> {
        return this.requestService.post(this.area, this.controller, 'CheckEgnLncPurchaseAbility', data, {
            responseTypeCtr: RecreationalFishingTicketValidationResultDTO
        });
    }

    public getPhoto(fileId: number): Observable<string> {
        const params = new HttpParams().append('fileId', fileId.toString());
        return this.requestService.get(this.area, this.controller, 'GetPhoto', {
            httpParams: params,
            responseType: 'text',
            properties: RequestProperties.NO_SPINNER
        });
    }

    public getPersonPhoto(egnLnc: EgnLncDTO): Observable<string> {
        const params = new HttpParams()
            .append('egnLnc', egnLnc.egnLnc!)
            .append('idType', egnLnc.identifierType!.toString());

        return this.requestService.get(this.area, this.controller, 'GetPersonPhoto', {
            httpParams: params,
            responseType: 'text',
            properties: RequestProperties.NO_SPINNER
        });
    }

    public downloadFishingTicket(ticketId: number): Observable<boolean> {
        const params = new HttpParams().append('ticketId', ticketId.toString());
        return this.requestService.download(this.area, this.controller, 'DownloadFishingTicket', 'ticket', { httpParams: params });
    }

    public downloadTicketDeclaration(parameters: RecreationalFishingTicketDeclarationParametersDTO): Observable<boolean> {
        return this.requestService.downloadPost(this.area, this.controller, 'DownloadTicketDeclaration', 'declaration', parameters);
    }

    public getTicketPeriods(): Observable<NomenclatureDTO<number>[]> {
        return this.commonService.getTicketPeriods(this.area, this.controller);
    }

    public getTicketTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.commonService.getTicketTypes(this.area, this.controller);
    }

    public getTicketPrices(): Observable<RecreationalFishingTicketPriceDTO[]> {
        return this.commonService.getTicketPrices(this.area, this.controller);
    }

    public getAllFishingAssociations(): Observable<NomenclatureDTO<number>[]> {
        return this.commonService.getAllFishingAssociations(this.area, this.controller);
    }

    public getTicketTerritoryUnits(): Observable<TerritoryUnitNomenclatureDTO[]> {
        return this.commonService.getTicketTerritoryUnits(this.area, this.controller);
    }

    // applications
    public getAllTicketApplications(request: GridRequestModel<RecreationalFishingTicketApplicationFilters>): Observable<GridResultModel<RecreationalFishingTicketApplicationDTO>> {
        type Result = GridResultModel<RecreationalFishingTicketApplicationDTO>;
        type Body = GridRequestModel<RecreationalFishingTicketApplicationFilters>;

        return this.requestService.post<Result, Body>(this.area, this.controller, 'GetAllTicketApplications', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        }).pipe(switchMap((entries: Result) => {
            for (const entry of entries.records) {
                if (entry.isDuplicate) {
                    entry.ticketType += ` - ${this.translate.getValue('recreational-fishing.ticket-status-duplicate')}`;
                }
            }

            return of(entries);
        }));
    }

    public getAllTicketOnlineApplications(request: GridRequestModel<RecreationalFishingTicketApplicationFilters>): Observable<GridResultModel<RecreationalFishingTicketApplicationDTO>> {
        type Result = GridResultModel<RecreationalFishingTicketApplicationDTO>;
        type Body = GridRequestModel<RecreationalFishingTicketApplicationFilters>;

        return this.requestService.post<Result, Body>(this.area, this.controller, 'GetAllTicketOnlineApplications', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        }).pipe(switchMap((entries: Result) => {
            for (const entry of entries.records) {
                if (entry.ticketStatus === TicketStatusEnum.APPROVED) {
                    entry.ticketStatusName = this.translate.getValue('recreational-fishing.ticket-status-offline-number-expected');
                }

                if (entry.isDuplicate) {
                    entry.ticketType += ` - ${this.translate.getValue('recreational-fishing.ticket-status-duplicate')}`;
                }
            }

            return of(entries);
        }));
    }

    public getAllTicketStatuses(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetAllTicketStatuses', { responseTypeCtr: NomenclatureDTO });
    }

    public getAllApplications(request: GridRequestModel<ApplicationsRegisterFilters>): Observable<GridResultModel<ApplicationRegisterDTO>> {
        throw new Error('Method not implemented.');
    }

    public getApplicationStatuses(): Observable<NomenclatureDTO<number>[]> {
        throw new Error('Method not implemented.');
    }

    public getApplicationTypes(): Observable<NomenclatureDTO<number>[]> {
        throw new Error('Method not implemented.');
    }

    public getApplicationSources(): Observable<NomenclatureDTO<number>[]> {
        throw new Error('Method not implemented.');
    }

    public assignApplicationViaAccessCode(accessCode: string): Observable<AssignedApplicationInfoDTO> {
        throw new Error('Method not implemented.');
    }

    public editApplicationDataAndStartRegixChecks(model: IApplicationRegister): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'EditTicketRegixData', model, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public getOnlinePaidTicketsCount(): Observable<number> {
        return this.requestService.get(this.area, this.controller, 'GetOnlinePaidTicketsCount');
    }
}