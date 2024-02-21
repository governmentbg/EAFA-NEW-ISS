

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { AdmissionLogBookPageRegisterDTO } from './AdmissionLogBookPageRegisterDTO';
import { TransportationLogBookPageRegisterDTO } from './TransportationLogBookPageRegisterDTO';
import { FirstSaleLogBookPageRegisterDTO } from './FirstSaleLogBookPageRegisterDTO';
import { LogBookPageStatusesEnum } from '@app/enums/log-book-page-statuses.enum';

export class ShipLogBookPageRegisterDTO { 
    public constructor(obj?: Partial<ShipLogBookPageRegisterDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public logBookId?: number;

    @StrictlyTyped(String)
    public pageNumber?: string;

    @StrictlyTyped(Boolean)
    public isLogBookFinishedOrSuspended?: boolean;

    @StrictlyTyped(Date)
    public outingStartDate?: Date;

    @StrictlyTyped(String)
    public fishingGear?: string;

    @StrictlyTyped(String)
    public portOfUnloading?: string;

    @StrictlyTyped(String)
    public unloadingInformation?: string;

    @StrictlyTyped(Number)
    public status?: LogBookPageStatusesEnum;

    @StrictlyTyped(String)
    public statusName?: string;

    @StrictlyTyped(Boolean)
    public hasOriginDeclaration?: boolean;

    @StrictlyTyped(String)
    public cancellationReason?: string;

    @StrictlyTyped(Boolean)
    public hasInspections?: boolean;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;

    @StrictlyTyped(AdmissionLogBookPageRegisterDTO)
    public admissionDeclarations?: AdmissionLogBookPageRegisterDTO[];

    @StrictlyTyped(TransportationLogBookPageRegisterDTO)
    public transportationDocuments?: TransportationLogBookPageRegisterDTO[];

    @StrictlyTyped(FirstSaleLogBookPageRegisterDTO)
    public firstSaleDocuments?: FirstSaleLogBookPageRegisterDTO[];
}