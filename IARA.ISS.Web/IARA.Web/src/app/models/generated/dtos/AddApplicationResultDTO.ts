

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ApplicationHierarchyTypesEnum } from '@app/enums/application-hierarchy-types.enum';

export class AddApplicationResultDTO { 
    public constructor(obj?: Partial<AddApplicationResultDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public applicationId?: number;

    @StrictlyTyped(String)
    public accessCode?: string;

    @StrictlyTyped(Number)
    public applicationHierarchyType?: ApplicationHierarchyTypesEnum;
}