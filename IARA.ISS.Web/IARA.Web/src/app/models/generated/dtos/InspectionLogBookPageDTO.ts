

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { VesselDuringInspectionDTO } from './VesselDuringInspectionDTO';
import { InspectedDeclarationCatchDTO } from './InspectedDeclarationCatchDTO';
import { SubjectRoleEnum } from '@app/enums/subject-role.enum';
import { DeclarationLogBookTypeEnum } from '@app/enums/declaration-log-book-type.enum';

export class InspectionLogBookPageDTO { 
    public constructor(obj?: Partial<InspectionLogBookPageDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public inspectedShipType?: SubjectRoleEnum;

    @StrictlyTyped(Number)
    public shipId?: number;

    @StrictlyTyped(Number)
    public unregisteredShipId?: number;

    @StrictlyTyped(VesselDuringInspectionDTO)
    public originShip?: VesselDuringInspectionDTO;

    @StrictlyTyped(Number)
    public logBookType?: DeclarationLogBookTypeEnum;

    @StrictlyTyped(Number)
    public logBookId?: number;

    @StrictlyTyped(Number)
    public aquacultureId?: number;

    @StrictlyTyped(String)
    public unregisteredEntityData?: string;

    @StrictlyTyped(Date)
    public unregisteredPageDate?: Date;

    @StrictlyTyped(String)
    public unregisteredPageNum?: string;

    @StrictlyTyped(String)
    public unregisteredLogBookNum?: string;

    @StrictlyTyped(Number)
    public firstSaleLogBookPageId?: number;

    @StrictlyTyped(Number)
    public transportationLogBookPageId?: number;

    @StrictlyTyped(Number)
    public admissionLogBookPageId?: number;

    @StrictlyTyped(Number)
    public shipLogBookPageId?: number;

    @StrictlyTyped(Number)
    public aquacultureLogBookPageId?: number;

    @StrictlyTyped(InspectedDeclarationCatchDTO)
    public inspectionCatchMeasures?: InspectedDeclarationCatchDTO[];

    @StrictlyTyped(String)
    public catchMeasuresText?: string;
}