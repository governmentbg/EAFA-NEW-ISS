import { CommercialFishingLogBookEditDTO } from '@app/models/generated/dtos/CommercialFishingLogBookEditDTO';
import { ILogBookService } from '../../edit-log-book/interfaces/log-book.interface';

export class ChooseLogBookForRenewalDialogParams {
    public permitLicenseId: number | undefined;
    public service: ILogBookService | undefined;
    public saveToDB: boolean = false;
    public hasRenewMoreThanMaxPagesPermission: boolean = false;
    public logBooks: CommercialFishingLogBookEditDTO[] = [];

    public constructor(obj: Partial<ChooseLogBookForRenewalDialogParams>) {
        Object.assign(this, obj);
    }
}