

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class UserNewsDistrictSubscriptionDTO { 
    public constructor(obj?: Partial<UserNewsDistrictSubscriptionDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public name?: string;
}