import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { RecreationalFishingTicketDTO } from '@app/models/generated/dtos/RecreationalFishingTicketDTO';
import { RecreationalFishingTicketPriceDTO } from '@app/models/generated/dtos/RecreationalFishingTicketPriceDTO';
import { RecreationalFishingTicketsDTO } from '@app/models/generated/dtos/RecreationalFishingTicketsDTO';
import { RecreationalFishingTicketValidToCalculationParamsDTO } from '@app/models/generated/dtos/RecreationalFishingTicketValidToCalculationParamsDTO';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { RecreationalFishingAddTicketsResultDTO } from '@app/models/generated/dtos/RecreationalFishingAddTicketsResultDTO';
import { RecreationalFishingTicketDuplicateDTO } from '@app/models/generated/dtos/RecreationalFishingTicketDuplicateDTO';

@Injectable({
    providedIn: 'root'
})
export class RecreationalFishingCommonService {
    private http: RequestService;

    public constructor(requestService: RequestService) {
        this.http = requestService;
    }

    public getTicket(area: AreaTypes, controller: string, id: number, getRegiXData: boolean): Observable<RecreationalFishingTicketDTO> {
        const params = new HttpParams()
            .append('id', id.toString())
            .append('getRegiXData', getRegiXData.toString());
        return this.http.get(area, controller, 'GetTicket', {
            httpParams: params,
            responseTypeCtr: RecreationalFishingTicketDTO
        });
    }

    public addTickets(area: AreaTypes, controller: string, tickets: RecreationalFishingTicketsDTO): Observable<RecreationalFishingAddTicketsResultDTO> {
        return this.http.post(area, controller, 'AddTickets', tickets, {
            properties: new RequestProperties({ asFormData: true }),
            responseTypeCtr: RecreationalFishingAddTicketsResultDTO
        });
    }

    public addTicketDuplicate(area: AreaTypes, controller: string, data: RecreationalFishingTicketDuplicateDTO): Observable<number> {
        return this.http.post(area, controller, 'AddTicketDuplicate', data);
    }

    public editTicket(area: AreaTypes, controller: string, ticket: RecreationalFishingTicketDTO): Observable<void> {
        return this.http.post(area, controller, 'EditTicket', ticket, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public calculateTicketValidToDate(area: AreaTypes, controller: string, params: RecreationalFishingTicketValidToCalculationParamsDTO): Observable<Date> {
        return this.http.post(area, controller, 'CalculateTicketValidToDate', params);
    }

    public getTicketPeriods(area: AreaTypes, controller: string): Observable<NomenclatureDTO<number>[]> {
        return this.http.get(area, controller, 'GetTicketPeriods', { responseTypeCtr: NomenclatureDTO });
    }

    public getTicketTypes(area: AreaTypes, controller: string): Observable<NomenclatureDTO<number>[]> {
        return this.http.get(area, controller, 'GetTicketTypes', { responseTypeCtr: NomenclatureDTO });
    }

    public getTicketPrices(area: AreaTypes, controller: string): Observable<RecreationalFishingTicketPriceDTO[]> {
        return this.http.get(area, controller, 'GetTicketPrices', { responseTypeCtr: RecreationalFishingTicketPriceDTO });
    }

    public getAllFishingAssociations(area: AreaTypes, controller: string): Observable<NomenclatureDTO<number>[]> {
        return this.http.get(area, controller, 'GetAllFishingAssociations', { responseTypeCtr: NomenclatureDTO });
    }
}