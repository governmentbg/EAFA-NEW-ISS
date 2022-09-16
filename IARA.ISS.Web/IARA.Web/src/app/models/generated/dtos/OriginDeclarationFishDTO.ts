

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class OriginDeclarationFishDTO { 
    public constructor(obj?: Partial<OriginDeclarationFishDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public originDeclarationId?: number;

    @StrictlyTyped(Number)
    public catchRecordFishId?: number;

    @StrictlyTyped(Number)
    public fishId?: number;

    @StrictlyTyped(String)
    public fishName?: string;

    @StrictlyTyped(String)
    public catchZone?: string;

    @StrictlyTyped(Number)
    public catchQuadrantId?: number;

    @StrictlyTyped(String)
    public catchQuadrant?: string;

    @StrictlyTyped(Number)
    public catchFishStateId?: number;

    @StrictlyTyped(String)
    public catchFishStateName?: string;

    @StrictlyTyped(Number)
    public catchFishPresentationId?: number;

    @StrictlyTyped(String)
    public catchFishPresentationName?: string;

    @StrictlyTyped(Boolean)
    public isProcessedOnBoard?: boolean;

    @StrictlyTyped(Number)
    public quantityKg?: number;

    @StrictlyTyped(Number)
    public unloadedProcessedQuantityKg?: number;

    @StrictlyTyped(Date)
    public transboradDateTime?: Date;

    @StrictlyTyped(Number)
    public transboardShipId?: number;

    @StrictlyTyped(Number)
    public transboardTargetPortId?: number;

    @StrictlyTyped(String)
    public comments?: string;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;

    @StrictlyTyped(Boolean)
    public isValid?: boolean;
}