

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { BaseCrossCheckResultDTO } from './BaseCrossCheckResultDTO'; 

export class CrossCheckResultDTO extends BaseCrossCheckResultDTO {
    public constructor(obj?: Partial<CrossCheckResultDTO>) {
        if (obj != undefined) {
            super(obj as BaseCrossCheckResultDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public checkCode?: string;

    @StrictlyTyped(String)
    public checkName?: string;

    @StrictlyTyped(Date)
    public validFrom?: Date;

    @StrictlyTyped(Date)
    public validTo?: Date;

    @StrictlyTyped(Number)
    public assignedUserId?: number;

    @StrictlyTyped(String)
    public assignedUser?: string;

    @StrictlyTyped(String)
    public resolution?: string;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}