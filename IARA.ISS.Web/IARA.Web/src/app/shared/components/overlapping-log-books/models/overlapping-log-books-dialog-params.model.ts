import { ICommercialFishingService } from '@app/interfaces/common-app/commercial-fishing.interface';
import { LogBookGroupsEnum } from '@app/enums/log-book-groups.enum';
import { OverlappingLogBooksParameters } from './overlapping-log-books-parameters.model';

export class OverlappingLogBooksDialogParamsModel {
    public service!: ICommercialFishingService;
    public ranges!: OverlappingLogBooksParameters[];
    public logBookGroup!: LogBookGroupsEnum;

    public constructor(obj?: Partial<OverlappingLogBooksDialogParamsModel>) {
        Object.assign(this, obj);
    }
}