

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class InspectedEntityControlActivityInfoDTO { 
    public constructor(obj?: Partial<InspectedEntityControlActivityInfoDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public auanNumbers?: string;

    @StrictlyTyped(String)
    public penalDecreeNumbers?: string;

    @StrictlyTyped(String)
    public agreementNumbers?: string;

    @StrictlyTyped(String)
    public warningNumbers?: string;

    @StrictlyTyped(String)
    public resolutionNumbers?: string;
}