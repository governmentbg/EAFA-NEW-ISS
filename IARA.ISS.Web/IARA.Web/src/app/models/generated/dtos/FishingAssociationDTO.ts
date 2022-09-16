
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class FishingAssociationDTO {
    public constructor(obj?: Partial<FishingAssociationDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}