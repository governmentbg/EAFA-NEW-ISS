
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { PageCodeEnum } from '@app/enums/page-code.enum';

export class AssignedApplicationInfoDTO {
    public constructor(obj?: Partial<AssignedApplicationInfoDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public statusCode?: string;

    @StrictlyTyped(Number)
    public pageCode?: PageCodeEnum;
}