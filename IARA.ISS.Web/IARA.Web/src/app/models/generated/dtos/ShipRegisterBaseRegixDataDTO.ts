

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { PageCodeEnum } from '@app/enums/page-code.enum';

export class ShipRegisterBaseRegixDataDTO { 
    public constructor(obj?: Partial<ShipRegisterBaseRegixDataDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public applicationId?: number;

    @StrictlyTyped(Number)
    public pageCode?: PageCodeEnum;

    @StrictlyTyped(String)
    public cfr?: string;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(String)
    public regLicenceNum?: string;

    @StrictlyTyped(String)
    public regLicencePublishVolume?: string;

    @StrictlyTyped(String)
    public regLicencePublishPage?: string;

    @StrictlyTyped(Number)
    public vesselTypeId?: number;

    @StrictlyTyped(String)
    public vesselTypeName?: string;

    @StrictlyTyped(Number)
    public grossTonnage?: number;

    @StrictlyTyped(Number)
    public netTonnage?: number;

    @StrictlyTyped(Number)
    public totalLength?: number;

    @StrictlyTyped(Number)
    public totalWidth?: number;

    @StrictlyTyped(Number)
    public boardHeight?: number;

    @StrictlyTyped(Number)
    public shipDraught?: number;

    @StrictlyTyped(Number)
    public lengthBetweenPerpendiculars?: number;

    @StrictlyTyped(Number)
    public fuelTypeId?: number;

    @StrictlyTyped(String)
    public fuelTypeName?: string;

    @StrictlyTyped(String)
    public hullNumber?: string;

    @StrictlyTyped(String)
    public mainEngineNum?: string;

    @StrictlyTyped(String)
    public statusReason?: string;
}