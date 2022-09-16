

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { FluxFlapRequestShipDTO } from './FluxFlapRequestShipDTO';
import { FluxFlapRequestTargetedQuotaDTO } from './FluxFlapRequestTargetedQuotaDTO';

export class FluxFlapRequestEditDTO { 
    public constructor(obj?: Partial<FluxFlapRequestEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Boolean)
    public isOutgoing?: boolean;

    @StrictlyTyped(String)
    public agreementTypeCode?: string;

    @StrictlyTyped(String)
    public coastalPartyCode?: string;

    @StrictlyTyped(String)
    public requestPurposeCode?: string;

    @StrictlyTyped(String)
    public requestPurposeText?: string;

    @StrictlyTyped(String)
    public fishingCategoryCode?: string;

    @StrictlyTyped(String)
    public fishingMethod?: string;

    @StrictlyTyped(String)
    public fishingArea?: string;

    @StrictlyTyped(String)
    public authorizedFishingGearCodes?: string[];

    @StrictlyTyped(FluxFlapRequestShipDTO)
    public ship?: FluxFlapRequestShipDTO;

    @StrictlyTyped(FluxFlapRequestShipDTO)
    public joinedShips?: FluxFlapRequestShipDTO[];

    @StrictlyTyped(Boolean)
    public isFirstApplication?: boolean;

    @StrictlyTyped(String)
    public remarks?: string;

    @StrictlyTyped(Number)
    public localSeamenCount?: number;

    @StrictlyTyped(Number)
    public acpSeamenCount?: number;

    @StrictlyTyped(Date)
    public authorizationStartDate?: Date;

    @StrictlyTyped(Date)
    public authorizationEndDate?: Date;

    @StrictlyTyped(FluxFlapRequestTargetedQuotaDTO)
    public targetedQuotas?: FluxFlapRequestTargetedQuotaDTO[];
}