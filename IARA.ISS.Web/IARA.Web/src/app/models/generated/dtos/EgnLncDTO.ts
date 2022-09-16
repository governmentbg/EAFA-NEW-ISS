
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { IdentifierTypeEnum } from '@app/enums/identifier-type.enum';

export class EgnLncDTO {
    public constructor(obj?: Partial<EgnLncDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public egnLnc?: string;

    @StrictlyTyped(Number)
    public identifierType?: IdentifierTypeEnum;
}