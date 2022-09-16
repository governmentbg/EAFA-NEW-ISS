
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class RecreationalFishingPossibleAssociationLegalDTO {
    public constructor(obj?: Partial<RecreationalFishingPossibleAssociationLegalDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public eik?: string;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(Boolean)
    public hasPermissions?: boolean;

    @StrictlyTyped(Boolean)
    public isChecked?: boolean;
}