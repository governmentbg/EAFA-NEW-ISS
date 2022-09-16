

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { TransportationLogBookPageRegisterDTO } from './TransportationLogBookPageRegisterDTO';
import { AdmissionLogBookPageRegisterDTO } from './AdmissionLogBookPageRegisterDTO';
import { FirstSaleLogBookPageRegisterDTO } from './FirstSaleLogBookPageRegisterDTO';
import { AquacultureLogBookPageRegisterDTO } from './AquacultureLogBookPageRegisterDTO';
import { LogBookPagePersonTypesEnum } from '@app/enums/log-book-page-person-types.enum';

export class LogBookEditDTO { 
    public constructor(obj?: Partial<LogBookEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public logBookId?: number;

    @StrictlyTyped(Number)
    public logBookTypeId?: number;

    @StrictlyTyped(Number)
    public statusId?: number;

    @StrictlyTyped(Number)
    public ownerType?: LogBookPagePersonTypesEnum;

    @StrictlyTyped(String)
    public logbookNumber?: string;

    @StrictlyTyped(Date)
    public issueDate?: Date;

    @StrictlyTyped(Date)
    public finishDate?: Date;

    @StrictlyTyped(Number)
    public startPageNumber?: number;

    @StrictlyTyped(Number)
    public endPageNumber?: number;

    @StrictlyTyped(Boolean)
    public isOnline?: boolean;

    @StrictlyTyped(Boolean)
    public logBookIsActive?: boolean;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;

    @StrictlyTyped(Number)
    public price?: number;

    @StrictlyTyped(String)
    public comment?: string;

    @StrictlyTyped(Boolean)
    public hasError?: boolean;

    @StrictlyTyped(TransportationLogBookPageRegisterDTO)
    public transportationPagesAndDeclarations?: TransportationLogBookPageRegisterDTO[];

    @StrictlyTyped(AdmissionLogBookPageRegisterDTO)
    public admissionPagesAndDeclarations?: AdmissionLogBookPageRegisterDTO[];

    @StrictlyTyped(FirstSaleLogBookPageRegisterDTO)
    public firstSalePages?: FirstSaleLogBookPageRegisterDTO[];

    @StrictlyTyped(AquacultureLogBookPageRegisterDTO)
    public aquaculturePages?: AquacultureLogBookPageRegisterDTO[];
}