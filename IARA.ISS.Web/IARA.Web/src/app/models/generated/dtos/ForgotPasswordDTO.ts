

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class ForgotPasswordDTO { 
    public constructor(obj?: Partial<ForgotPasswordDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public email?: string;
}