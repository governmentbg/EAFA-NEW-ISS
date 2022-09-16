
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class ApplicationTariffDTO {
    public constructor(obj?: Partial<ApplicationTariffDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public code?: string;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(String)
    public description?: string;

    @StrictlyTyped(Number)
    public price?: number;

    @StrictlyTyped(String)
    public basedOnPlea?: string;
}