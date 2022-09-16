
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class ErrorLogDTO {
    public constructor(obj?: Partial<ErrorLogDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Date)
    public logDate?: Date;

    @StrictlyTyped(String)
    public username?: string;

    @StrictlyTyped(String)
    public severity?: string;

    @StrictlyTyped(String)
    public client?: string;

    @StrictlyTyped(String)
    public class?: string;

    @StrictlyTyped(String)
    public method?: string;

    @StrictlyTyped(String)
    public exceptionSource?: string;

    @StrictlyTyped(String)
    public message?: string;

    @StrictlyTyped(String)
    public stackTrace?: string;
}