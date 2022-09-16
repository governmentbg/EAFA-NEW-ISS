import { LogBookPagePersonTypesEnum } from '@app/enums/log-book-page-person-types.enum';

export class OverlappingLogBooksParameters {
    public startPage!: number;
    public endPage!: number;
    public typeId!: number;
    public logBookId: number | undefined;
    public OwnerType: LogBookPagePersonTypesEnum | undefined;

    public constructor(obj?: Partial<OverlappingLogBooksParameters>) {
        Object.assign(this, obj);
    }
}