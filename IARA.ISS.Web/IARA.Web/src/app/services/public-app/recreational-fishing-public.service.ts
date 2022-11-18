import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { IRecreationalFishingService } from '../../interfaces/common-app/recreational-fishing.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { ReasonDTO } from '@app/models/generated/dtos/ReasonDTO';
import { RecreationalFishingTicketApplicationDTO } from '@app/models/generated/dtos/RecreationalFishingTicketApplicationDTO';
import { RecreationalFishingTicketCardDTO } from '@app/models/generated/dtos/RecreationalFishingTicketCardDTO';
import { RecreationalFishingTicketDTO } from '@app/models/generated/dtos/RecreationalFishingTicketDTO';
import { RecreationalFishingTicketHolderDTO } from '@app/models/generated/dtos/RecreationalFishingTicketHolderDTO';
import { RecreationalFishingTicketPriceDTO } from '@app/models/generated/dtos/RecreationalFishingTicketPriceDTO';
import { RecreationalFishingTicketsDTO } from '@app/models/generated/dtos/RecreationalFishingTicketsDTO';
import { RecreationalFishingTicketValidationDTO } from '@app/models/generated/dtos/RecreationalFishingTicketValidationDTO';
import { RecreationalFishingTicketValidationResultDTO } from '@app/models/generated/dtos/RecreationalFishingTicketValidationResultDTO';
import { RecreationalFishingTicketValidToCalculationParamsDTO } from '@app/models/generated/dtos/RecreationalFishingTicketValidToCalculationParamsDTO';
import { RecreationalFishingTicketViewDTO } from '@app/models/generated/dtos/RecreationalFishingTicketViewDTO';
import { RecreationalFishingUserTicketDataDTO } from '@app/models/generated/dtos/RecreationalFishingUserTicketDataDTO';
import { RecreationalFishingTicketApplicationFilters } from '@app/models/generated/filters/RecreationalFishingTicketApplicationFilters';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { BaseAuditService } from '../common-app/base-audit.service';
import { RecreationalFishingCommonService } from '../common-app/recreational-fishing-common.service';
import { RecreationalFishingTicketDeclarationParametersDTO } from '../../models/generated/dtos/RecreationalFishingTicketDeclarationParametersDTO';
import { EgnLncDTO } from '@app/models/generated/dtos/EgnLncDTO';
import { RecreationalFishingAddTicketsResultDTO } from '@app/models/generated/dtos/RecreationalFishingAddTicketsResultDTO';
import { RecreationalFishingTicketDuplicateDTO } from '@app/models/generated/dtos/RecreationalFishingTicketDuplicateDTO';

@Injectable({
    providedIn: 'root'
})
export class RecreationalFishingPublicService extends BaseAuditService implements IRecreationalFishingService {
    public currentUserTickets = new BehaviorSubject<RecreationalFishingTicketViewDTO[]>([]);
    public currentUserAssociations = new BehaviorSubject<NomenclatureDTO<number>[]>([]);
    public currentUserChosenAssociation: NomenclatureDTO<number> | undefined;

    protected controller: string = 'RecreationalFishingPublic';

    private commonService: RecreationalFishingCommonService;

    public constructor(requestService: RequestService, commonService: RecreationalFishingCommonService) {
        super(requestService, AreaTypes.Public);

        this.commonService = commonService;
    }

    public getTicket(id: number, getRegiXData: boolean): Observable<RecreationalFishingTicketDTO> {
        return this.commonService.getTicket(this.area, this.controller, id, getRegiXData);
    }

    public addTickets(tickets: RecreationalFishingTicketsDTO): Observable<RecreationalFishingAddTicketsResultDTO> {
        return this.commonService.addTickets(this.area, this.controller, tickets);
    }

    public addTicketDuplicate(data: RecreationalFishingTicketDuplicateDTO): Observable<number> {
        return this.commonService.addTicketDuplicate(this.area, this.controller, data);
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

    public calculateTicketValidToDate(params: RecreationalFishingTicketValidToCalculationParamsDTO): Observable<Date> {
        return this.commonService.calculateTicketValidToDate(this.area, this.controller, params);
    }

    public checkEgnLncPurchaseAbility(data: RecreationalFishingTicketValidationDTO): Observable<RecreationalFishingTicketValidationResultDTO> {
        return this.requestService.post(this.area, this.controller, 'CheckEgnLncPurchaseAbility', data, {
            responseTypeCtr: RecreationalFishingTicketValidationResultDTO
        });
    }

    public checkTicketNumbersAvailability(numbers: string[]): Observable<boolean[]> {
        let params: HttpParams = new HttpParams();
        for (const num of numbers) {
            params = params.append('ticketNumbers', num);
        }

        return this.requestService.get(this.area, this.controller, 'CheckTicketNumbersAvailability', {
            httpParams: params
        });
    }

    public updateUserDataFromTicket(data: RecreationalFishingUserTicketDataDTO): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'UpdateUserDataFromTicket', data, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public getAllUserTickets(): Observable<RecreationalFishingTicketCardDTO[]> {
        return this.requestService.get(this.area, this.controller, 'GetAllUserTickets', { responseTypeCtr: RecreationalFishingTicketCardDTO });
    }

    public getUserRequestedOrActiveTickets(): Observable<void> {
        return this.requestService.get<RecreationalFishingTicketViewDTO[]>(this.area, this.controller, 'GetUserRequestedOrActiveTickets', {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: RecreationalFishingTicketViewDTO
        }).pipe(map((tickets: RecreationalFishingTicketViewDTO[]) => {
            this.currentUserTickets.next(tickets);
        }));
    }

    public getUserPersonData(): Observable<RecreationalFishingTicketHolderDTO> {
        return this.requestService.get(this.area, this.controller, 'GetUserPersonData', {
            responseTypeCtr: RecreationalFishingTicketHolderDTO
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

    public getPersonData(egnLnc: EgnLncDTO, associationId: number): Observable<RecreationalFishingTicketHolderDTO | undefined> {
        const params = new HttpParams()
            .append('egnLnc', egnLnc.egnLnc!)
            .append('idType', egnLnc.identifierType!.toString())
            .append('associationId', associationId.toString());

        return this.requestService.get(this.area, this.controller, 'GetPersonData', {
            httpParams: params,
            responseTypeCtr: RecreationalFishingTicketHolderDTO
        });
    }

    public getPersonPhoto(egnLnc: EgnLncDTO, associationId: number): Observable<string> {
        const params = new HttpParams()
            .append('egnLnc', egnLnc.egnLnc!)
            .append('idType', egnLnc.identifierType!.toString())
            .append('associationId', associationId.toString());

        return this.requestService.get(this.area, this.controller, 'GetPersonPhoto', {
            httpParams: params,
            responseType: 'text',
            properties: RequestProperties.NO_SPINNER
        });
    }

    public getUserPhoto(): Observable<string> {
        return this.requestService.get(this.area, this.controller, 'GetUserPhoto', {
            responseType: 'text',
            properties: RequestProperties.NO_SPINNER
        });
    }

    public getUserAssociations(): Observable<void> {
        return this.requestService.get<NomenclatureDTO<number>[]>(this.area, this.controller, 'GetUserAssociations', {
            responseTypeCtr: NomenclatureDTO
        }).pipe(map((assocs: NomenclatureDTO<number>[]) => {
            this.currentUserAssociations.next(assocs);
        }));
    }

    public downloadFile(fileId: number, fileName: string): Observable<boolean> {
        const params = new HttpParams().append('id', fileId.toString());
        return this.requestService.download(this.area, this.controller, 'DownloadFile', fileName, { httpParams: params });
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

    // applications
    public getAllTicketApplications(
        request: GridRequestModel<RecreationalFishingTicketApplicationFilters>
    ): Observable<GridResultModel<RecreationalFishingTicketApplicationDTO>> {
        throw new Error('This method should not be called from the public app');
    }

    public getAllAssociationTicketApplications(
        associationId: number,
        request: GridRequestModel<RecreationalFishingTicketApplicationFilters>
    ): Observable<GridResultModel<RecreationalFishingTicketApplicationDTO>> {
        const params = new HttpParams().append('associationId', associationId.toString());

        return this.requestService.post(this.area, this.controller, 'GetAllAssociationTicketApplications', request, {
            properties: RequestProperties.NO_SPINNER,
            httpParams: params,
            responseTypeCtr: GridResultModel
        });
    }

    public deleteApplication(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.delete(this.area, this.controller, 'DeleteApplication', { httpParams: params });
    }

    public undoDeleteApplication(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.patch(this.area, this.controller, 'UndoDeleteApplication', null, { httpParams: params });
    }
}