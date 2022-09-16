import { IRecreationalFishingService } from '@app/interfaces/common-app/recreational-fishing.interface';

export class IssueDuplicateTicketDialogParams {
    public ticketId!: number;
    public associationId: number | undefined;
    public service!: IRecreationalFishingService;

    public constructor(params?: Partial<IssueDuplicateTicketDialogParams>) {
        Object.assign(this, params);
    }
}