import { IRecreationalFishingService } from '@app/interfaces/common-app/recreational-fishing.interface';

export class EnterOnlineTicketNumberParams {
    public service!: IRecreationalFishingService;
    public ticketId: number;

    public constructor(service: IRecreationalFishingService, ticketId: number) {
        this.service = service;
        this.ticketId = ticketId;
    }
}