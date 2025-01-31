

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ShipLogBookPageRegisterDTO } from './ShipLogBookPageRegisterDTO';
import { AdmissionLogBookPageRegisterDTO } from './AdmissionLogBookPageRegisterDTO';
import { TransportationLogBookPageRegisterDTO } from './TransportationLogBookPageRegisterDTO';
import { FirstSaleLogBookPageRegisterDTO } from './FirstSaleLogBookPageRegisterDTO';
import { AquacultureLogBookPageRegisterDTO } from './AquacultureLogBookPageRegisterDTO';
import { LogBookTypesEnum } from '@app/enums/log-book-types.enum';
import { LogBookStatusesEnum } from '@app/enums/log-book-statuses.enum';
import { LogBookPagePersonTypesEnum } from '@app/enums/log-book-page-person-types.enum';

export class LogBookRegisterDTO { 
    public constructor(obj?: Partial<LogBookRegisterDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public number?: string;

    @StrictlyTyped(Number)
    public typeId?: number;

    @StrictlyTyped(String)
    public type?: string;

    @StrictlyTyped(Number)
    public typeCode?: LogBookTypesEnum;

    @StrictlyTyped(Number)
    public status?: LogBookStatusesEnum;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(Number)
    public startPageNum?: number;

    @StrictlyTyped(Number)
    public endPageNum?: number;

    @StrictlyTyped(Date)
    public issueDate?: Date;

    @StrictlyTyped(Date)
    public finishDate?: Date;

    @StrictlyTyped(Date)
    public logBookPermitLicenseValidFrom?: Date;

    @StrictlyTyped(String)
    public statusName?: string;

    @StrictlyTyped(Boolean)
    public isLogBookFinished?: boolean;

    @StrictlyTyped(Boolean)
    public isLogBookSuspended?: boolean;

    @StrictlyTyped(Boolean)
    public allowNewLogBookPages?: boolean;

    @StrictlyTyped(Boolean)
    public allowEditLogBookPages?: boolean;

    @StrictlyTyped(Date)
    public suspendedPermitLicenseValidTo?: Date;

    @StrictlyTyped(Boolean)
    public isOnline?: boolean;

    @StrictlyTyped(Number)
    public ownerType?: LogBookPagePersonTypesEnum;

    @StrictlyTyped(ShipLogBookPageRegisterDTO)
    public shipPages?: ShipLogBookPageRegisterDTO[];

    @StrictlyTyped(Boolean)
    public hasLogBookPermitLicenses?: boolean;

    @StrictlyTyped(AdmissionLogBookPageRegisterDTO)
    public admissionPages?: AdmissionLogBookPageRegisterDTO[];

    @StrictlyTyped(TransportationLogBookPageRegisterDTO)
    public transportationPages?: TransportationLogBookPageRegisterDTO[];

    @StrictlyTyped(FirstSaleLogBookPageRegisterDTO)
    public firstSalePages?: FirstSaleLogBookPageRegisterDTO[];

    @StrictlyTyped(AquacultureLogBookPageRegisterDTO)
    public aquaculturePages?: AquacultureLogBookPageRegisterDTO[];
}