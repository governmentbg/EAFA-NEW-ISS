import { LogBookGroupsEnum } from '@app/enums/log-book-groups.enum';
import { ILogBookService } from '@app/components/common-app/commercial-fishing/components/edit-log-book/interfaces/log-book.interface';
import { OverlappingLogBooksParameters } from './overlapping-log-books-parameters.model';

export class OverlappingLogBooksDialogParamsModel {
    public service!: ILogBookService;
    public ranges!: OverlappingLogBooksParameters[];
    public logBookGroup!: LogBookGroupsEnum;

    public constructor(obj?: Partial<OverlappingLogBooksDialogParamsModel>) {
        Object.assign(this, obj);
    }
}