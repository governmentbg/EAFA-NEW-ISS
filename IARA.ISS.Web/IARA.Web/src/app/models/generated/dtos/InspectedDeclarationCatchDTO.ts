

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { VesselDuringInspectionDTO } from './VesselDuringInspectionDTO';
import { DeclarationLogBookTypeEnum } from '@app/enums/declaration-log-book-type.enum';

export class InspectedDeclarationCatchDTO { 
    public constructor(obj?: Partial<InspectedDeclarationCatchDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public catchTypeId?: number;

    @StrictlyTyped(Number)
    public fishTypeId?: number;

    @StrictlyTyped(Number)
    public catchCount?: number;

    @StrictlyTyped(Number)
    public catchQuantity?: number;

    @StrictlyTyped(Number)
    public unloadedQuantity?: number;

    @StrictlyTyped(Number)
    public presentationId?: number;

    @StrictlyTyped(Number)
    public catchZoneId?: number;

    @StrictlyTyped(VesselDuringInspectionDTO)
    public originShip?: VesselDuringInspectionDTO;

    @StrictlyTyped(Number)
    public aquacultureId?: number;

    @StrictlyTyped(String)
    public unregisteredEntityData?: string;

    @StrictlyTyped(Number)
    public logBookPageId?: number;

    @StrictlyTyped(Number)
    public logBookType?: DeclarationLogBookTypeEnum;

    @StrictlyTyped(String)
    public unregisteredPageNum?: string;

    @StrictlyTyped(Date)
    public unregisteredPageDate?: Date;
}