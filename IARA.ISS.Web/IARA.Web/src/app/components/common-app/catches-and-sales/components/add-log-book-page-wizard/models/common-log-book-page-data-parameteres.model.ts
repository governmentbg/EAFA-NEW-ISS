import { LogBookTypesEnum } from '@app/enums/log-book-types.enum';

export class CommonLogBookPageDataParameters {
    public logBookType: LogBookTypesEnum | undefined;
    public logBookId: number | undefined;
    public pageNumberToAdd: number | undefined;
    public originDeclarationNumber: number | undefined;
    public transportationDocumentNumber: number | undefined;
    public admissionDocumentNumber: number | undefined;

    public constructor(obj?: Partial<CommonLogBookPageDataParameters>) {
        Object.assign(this, obj);
    }
}