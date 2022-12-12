

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class RegisterDTO<T> { 
    public constructor(obj?: Partial<RegisterDTO<T>>) {
        Object.assign(this, obj);
    }

    public dto?: T;
}