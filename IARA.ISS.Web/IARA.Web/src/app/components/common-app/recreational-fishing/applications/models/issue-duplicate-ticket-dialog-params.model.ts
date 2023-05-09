import { IRecreationalFishingService } from '@app/interfaces/common-app/recreational-fishing.interface';
import { FileInfoDTO } from '@app/models/generated/dtos/FileInfoDTO';

export class IssueDuplicateTicketDialogParams {
    public ticketId!: number;
    public associationId: number | undefined;
    public service!: IRecreationalFishingService;
    public photo: FileInfoDTO | undefined;

    public constructor(params?: Partial<IssueDuplicateTicketDialogParams>) {
        Object.assign(this, params);
    }
}