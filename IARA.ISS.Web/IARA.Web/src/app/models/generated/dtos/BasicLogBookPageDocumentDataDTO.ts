

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { LogBookPagePersonDTO } from './LogBookPagePersonDTO';
import { CommonLogBookPageDataDTO } from './CommonLogBookPageDataDTO';
import { LogBookPagePersonTypesEnum } from '@app/enums/log-book-page-person-types.enum';
import { LogBookPageStatusesEnum } from '@app/enums/log-book-page-statuses.enum';

export class BasicLogBookPageDocumentDataDTO { 
    public constructor(obj?: Partial<BasicLogBookPageDocumentDataDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public shipLogBookPageId?: number;

    @StrictlyTyped(Number)
    public logBookId?: number;

    @StrictlyTyped(String)
    public logBookNumber?: string;

    @StrictlyTyped(Number)
    public ownerType?: LogBookPagePersonTypesEnum;

    @StrictlyTyped(LogBookPagePersonDTO)
    public personData?: LogBookPagePersonDTO;

    @StrictlyTyped(Number)
    public registeredBuyerId?: number;

    @StrictlyTyped(Number)
    public documentNumber?: number;

    @StrictlyTyped(Number)
    public pageStatus?: LogBookPageStatusesEnum;

    @StrictlyTyped(CommonLogBookPageDataDTO)
    public sourceData?: CommonLogBookPageDataDTO;
}