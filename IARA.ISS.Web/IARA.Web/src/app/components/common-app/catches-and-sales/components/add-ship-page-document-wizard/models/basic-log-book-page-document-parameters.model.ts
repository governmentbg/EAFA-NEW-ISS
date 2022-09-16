import { LogBookPagePersonTypesEnum } from '@app/enums/log-book-page-person-types.enum';
import { EgnLncDTO } from '@app/models/generated/dtos/EgnLncDTO';
import { LogBookPageDocumentTypesEnum } from '../../../enums/log-book-page-document-types.enum';

export class BasicLogBookPageDocumentParameters {
    public documentType!: LogBookPageDocumentTypesEnum;
    public ownerType!: LogBookPagePersonTypesEnum;
    public documentNumber!: number;
    public shipLogBookPageId!: number;
    public logBookId: number | undefined;

    public constructor(obj?: Partial<BasicLogBookPageDocumentParameters>) {
        Object.assign(this, obj);
    }
}