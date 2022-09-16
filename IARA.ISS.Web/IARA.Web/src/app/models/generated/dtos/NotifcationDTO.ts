

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class NotifcationDTO { 
    public constructor(obj?: Partial<NotifcationDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public title?: string;

    @StrictlyTyped(String)
    public text?: string;

    @StrictlyTyped(Boolean)
    public isRead?: boolean;
}