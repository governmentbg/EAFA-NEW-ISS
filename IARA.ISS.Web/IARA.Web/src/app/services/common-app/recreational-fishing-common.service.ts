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
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { TicketTypeEnum } from '@app/enums/ticket-type.enum';
import { map } from 'rxjs/operators';
import { TicketPeriodEnum } from '@app/enums/ticket-period.enum';
import { TerritoryUnitNomenclatureDTO } from '@app/models/generated/dtos/TerritoryUnitNomenclatureDTO';

@Injectable({
    providedIn: 'root'
})
export class RecreationalFishingCommonService {
    private http: RequestService;
    private translate: FuseTranslationLoaderService;

    public constructor(requestService: RequestService, translate: FuseTranslationLoaderService) {
        this.http = requestService;
        this.translate = translate;
    }

    public addTickets(area: AreaTypes, controller: string, tickets: RecreationalFishingTicketsDTO): Observable<RecreationalFishingAddTicketsResultDTO> {
        return this.http.post(area, controller, 'AddTickets', tickets, {
            properties: new RequestProperties({ asFormData: true }),
            responseTypeCtr: RecreationalFishingAddTicketsResultDTO
        });
    }

    public addTicketDuplicate(area: AreaTypes, controller: string, data: RecreationalFishingTicketDuplicateDTO): Observable<number> {
        return this.http.post(area, controller, 'AddTicketDuplicate', data, {
            properties: new RequestProperties({ asFormData: true }),
        });
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
        const resources: Record<string, string> = {};
        resources[TicketPeriodEnum[TicketPeriodEnum.ANNUAL]] = 'recreational-fishing.ticket-period-annual-resource';
        resources[TicketPeriodEnum[TicketPeriodEnum.HALFYEARLY]] = 'recreational-fishing.ticket-period-halfyearly-resource';
        resources[TicketPeriodEnum[TicketPeriodEnum.MONTHLY]] = 'recreational-fishing.ticket-period-monthly-resource';
        resources[TicketPeriodEnum[TicketPeriodEnum.WEEKLY]] = 'recreational-fishing.ticket-period-weekly-resource';
        resources[TicketPeriodEnum[TicketPeriodEnum.UNTIL14]] = 'recreational-fishing.ticket-period-until14-resource';
        resources[TicketPeriodEnum[TicketPeriodEnum.DISABILITY]] = 'recreational-fishing.ticket-period-disability-resource';
        resources[TicketPeriodEnum[TicketPeriodEnum.NOPERIOD]] = 'recreational-fishing.ticket-period-noperiod-resource';

        return this.http.get<NomenclatureDTO<number>[]>(area, controller, 'GetTicketPeriods', {
            responseTypeCtr: NomenclatureDTO
        }).pipe(map((ticketPeriods: NomenclatureDTO<number>[]) => {
            for (const period of ticketPeriods) {
                period.displayName = this.translate.getValue(resources[period.code!]);
            }

            return ticketPeriods;
        }));
    }

    public getTicketTypes(area: AreaTypes, controller: string): Observable<NomenclatureDTO<number>[]> {
        const resources: Record<string, string> = {};
        resources[TicketTypeEnum[TicketTypeEnum.STANDARD]] = 'recreational-fishing.ticket-type-standard-resource';
        resources[TicketTypeEnum[TicketTypeEnum.UNDER14]] = 'recreational-fishing.ticket-type-under14-resource';
        resources[TicketTypeEnum[TicketTypeEnum.BETWEEN14AND18]] = 'recreational-fishing.ticket-type-between14and18-resource';
        resources[TicketTypeEnum[TicketTypeEnum.ELDER]] = 'recreational-fishing.ticket-type-elder-resource';
        resources[TicketTypeEnum[TicketTypeEnum.DISABILITY]] = 'recreational-fishing.ticket-type-disability-resource';
        resources[TicketTypeEnum[TicketTypeEnum.ASSOCIATION]] = 'recreational-fishing.ticket-type-association-resource';
        resources[TicketTypeEnum[TicketTypeEnum.BETWEEN14AND18ASSOCIATION]] = 'recreational-fishing.ticket-type-between14and18association-resource';
        resources[TicketTypeEnum[TicketTypeEnum.ELDERASSOCIATION]] = 'recreational-fishing.ticket-type-elderassociation-resource';

        return this.http.get<NomenclatureDTO<number>[]>(area, controller, 'GetTicketTypes', {
            responseTypeCtr: NomenclatureDTO
        }).pipe(map((ticketTypes: NomenclatureDTO<number>[]) => {
            for (const type of ticketTypes) {
                type.displayName = this.translate.getValue(resources[type.code!]);
            }

            return ticketTypes;
        }));
    }

    public getTicketPrices(area: AreaTypes, controller: string): Observable<RecreationalFishingTicketPriceDTO[]> {
        return this.http.get(area, controller, 'GetTicketPrices', { responseTypeCtr: RecreationalFishingTicketPriceDTO });
    }

    public getAllFishingAssociations(area: AreaTypes, controller: string): Observable<NomenclatureDTO<number>[]> {
        return this.http.get(area, controller, 'GetAllFishingAssociations', { responseTypeCtr: NomenclatureDTO });
    }

    public getTicketTerritoryUnits(area: AreaTypes, controller: string): Observable<TerritoryUnitNomenclatureDTO[]> {
        const deliveryMsg1: string = this.translate.getValue('recreational-fishing.delivery-territory-unit-msg-1');
        const deliveryMsg2: string = this.translate.getValue('recreational-fishing.delivery-territory-unit-msg-2');
        const deliveryMsg3: string = this.translate.getValue('recreational-fishing.delivery-territory-unit-msg-3');
        const deliveryMsg4: string = this.translate.getValue('recreational-fishing.delivery-territory-unit-msg-4');

        return this.http.get<TerritoryUnitNomenclatureDTO[]>(area, controller, 'GetTicketTerritoryUnits', {
            responseTypeCtr: TerritoryUnitNomenclatureDTO
        }).pipe(map((territoryUnits: TerritoryUnitNomenclatureDTO[]) => {
            for (const territoryUnit of territoryUnits) {
                territoryUnit.deliveryTerritoryUniitMessage = `${deliveryMsg1}`;

                if (territoryUnit.address !== undefined && territoryUnit.address !== null && territoryUnit.address !== '') {
                    territoryUnit.deliveryTerritoryUniitMessage += ` - ${deliveryMsg2}: ${territoryUnit.address}`;
                }

                if (territoryUnit.phone !== undefined && territoryUnit.phone !== null && territoryUnit.phone !== '') {
                    territoryUnit.deliveryTerritoryUniitMessage += `, ${deliveryMsg3}: ${territoryUnit.phone}`;
                }

                if (territoryUnit.workingTime !== undefined && territoryUnit.workingTime !== null && territoryUnit.workingTime !== '') {
                    territoryUnit.deliveryTerritoryUniitMessage += `, ${deliveryMsg4}: ${territoryUnit.workingTime}`;
                }
            }

            return territoryUnits;
        }));
    }
}