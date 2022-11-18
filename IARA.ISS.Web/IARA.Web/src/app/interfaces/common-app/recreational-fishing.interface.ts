import { Observable } from 'rxjs';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { RecreationalFishingTicketApplicationDTO } from '@app/models/generated/dtos/RecreationalFishingTicketApplicationDTO';
import { RecreationalFishingTicketDTO } from '@app/models/generated/dtos/RecreationalFishingTicketDTO';
import { RecreationalFishingTicketPriceDTO } from '@app/models/generated/dtos/RecreationalFishingTicketPriceDTO';
import { RecreationalFishingTicketsDTO } from '@app/models/generated/dtos/RecreationalFishingTicketsDTO';
import { RecreationalFishingTicketValidationDTO } from '@app/models/generated/dtos/RecreationalFishingTicketValidationDTO';
import { RecreationalFishingTicketValidationResultDTO } from '@app/models/generated/dtos/RecreationalFishingTicketValidationResultDTO';
import { RecreationalFishingTicketValidToCalculationParamsDTO } from '@app/models/generated/dtos/RecreationalFishingTicketValidToCalculationParamsDTO';
import { RecreationalFishingTicketApplicationFilters } from '@app/models/generated/filters/RecreationalFishingTicketApplicationFilters';
import { RecreationalFishingTicketDeclarationParametersDTO } from '@app/models/generated/dtos/RecreationalFishingTicketDeclarationParametersDTO';
import { EgnLncDTO } from '@app/models/generated/dtos/EgnLncDTO';
import { RecreationalFishingTicketHolderDTO } from '@app/models/generated/dtos/RecreationalFishingTicketHolderDTO';
import { RecreationalFishingAddTicketsResultDTO } from '@app/models/generated/dtos/RecreationalFishingAddTicketsResultDTO';
import { RecreationalFishingTicketDuplicateDTO } from '@app/models/generated/dtos/RecreationalFishingTicketDuplicateDTO';
import { IBaseAuditService } from '../base-audit.interface';

export interface IRecreationalFishingService extends IBaseAuditService {
    // tickets
    getTicketPeriods(): Observable<NomenclatureDTO<number>[]>;
    getTicketTypes(): Observable<NomenclatureDTO<number>[]>;
    getTicketPrices(): Observable<RecreationalFishingTicketPriceDTO[]>;
    getAllFishingAssociations(): Observable<NomenclatureDTO<number>[]>;

    getTicket(id: number, getRegiXData: boolean): Observable<RecreationalFishingTicketDTO>;

    addTickets(ticket: RecreationalFishingTicketsDTO): Observable<RecreationalFishingAddTicketsResultDTO>;
    addTicketDuplicate(data: RecreationalFishingTicketDuplicateDTO): Observable<number>;
    editTicket(ticket: RecreationalFishingTicketDTO): Observable<void>;

    calculateTicketValidToDate(params: RecreationalFishingTicketValidToCalculationParamsDTO): Observable<Date>;
    checkEgnLncPurchaseAbility(data: RecreationalFishingTicketValidationDTO): Observable<RecreationalFishingTicketValidationResultDTO>;
    checkTicketNumbersAvailability(numbers: string[]): Observable<boolean[]>;

    downloadFile(fileId: number, fileName: string): Observable<boolean>;
    downloadFishingTicket(ticketId: number): Observable<boolean>;
    downloadTicketDeclaration(parameters: RecreationalFishingTicketDeclarationParametersDTO): Observable<boolean>;

    getPersonData(egnLnc: EgnLncDTO, associationId: number | undefined): Observable<RecreationalFishingTicketHolderDTO | undefined>;
    getPersonPhoto(egnLnc: EgnLncDTO, associationId: number | undefined): Observable<string>;
    getPhoto(fileId: number): Observable<string>;

    // applications
    getAllTicketApplications(request: GridRequestModel<RecreationalFishingTicketApplicationFilters>): Observable<GridResultModel<RecreationalFishingTicketApplicationDTO>>;

    deleteApplication(id: number): Observable<void>;
    undoDeleteApplication(id: number): Observable<void>;
}