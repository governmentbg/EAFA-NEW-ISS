import { ILogBookService } from '../../edit-log-book/interfaces/log-book.interface';

export class ChooseLogBookForRenewalDialogParams {
    public permitLicenseId: number | undefined;
    public service: ILogBookService | undefined;

    public constructor(obj: Partial<ChooseLogBookForRenewalDialogParams>) {
        Object.assign(this, obj);
    }
}