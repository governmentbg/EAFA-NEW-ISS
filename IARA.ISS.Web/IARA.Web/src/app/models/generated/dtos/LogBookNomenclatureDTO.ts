

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO';
import { LogBookPagePersonTypesEnum } from '@app/enums/log-book-page-person-types.enum';
import { LogBookStatusesEnum } from '@app/enums/log-book-statuses.enum'; 

export class LogBookNomenclatureDTO extends NomenclatureDTO<number> {
    public constructor(obj?: Partial<LogBookNomenclatureDTO>) {
        if (obj != undefined) {
            super(obj as NomenclatureDTO<number>);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Number)
    public logBookTypeId?: number;

    @StrictlyTyped(String)
    public ownerName?: string;

    @StrictlyTyped(Number)
    public ownerType?: LogBookPagePersonTypesEnum;

    @StrictlyTyped(Number)
    public logBookPermitLicenseId?: number;

    @StrictlyTyped(String)
    public permitLicenseNumber?: string;

    @StrictlyTyped(Number)
    public permitLicenseId?: number;

    @StrictlyTyped(Number)
    public logBookStatus?: LogBookStatusesEnum;

    @StrictlyTyped(String)
    public logBookStatusName?: string;
}