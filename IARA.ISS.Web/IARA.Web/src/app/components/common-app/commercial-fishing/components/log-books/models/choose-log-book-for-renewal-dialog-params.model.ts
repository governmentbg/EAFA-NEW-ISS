import { ILogBookService } from '../../edit-log-book/interfaces/log-book.interface';

export class ChooseLogBookForRenewalDialogParams {
    public permitLicenseId: number | undefined;
    public service: ILogBookService | undefined;
    public saveToDB: boolean = false;

    public constructor(obj: Partial<ChooseLogBookForRenewalDialogParams>) {
        Object.assign(this, obj);
    }
}