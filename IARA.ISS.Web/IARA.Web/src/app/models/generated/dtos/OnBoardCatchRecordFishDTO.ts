

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { CatchRecordFishDTO } from './CatchRecordFishDTO'; 

export class OnBoardCatchRecordFishDTO extends CatchRecordFishDTO {
    public constructor(obj?: Partial<OnBoardCatchRecordFishDTO>) {
        if (obj != undefined) {
            super(obj as CatchRecordFishDTO);
            Object.assign(this, obj);
        } 
        else {
            super();
        }
    }
  
    @StrictlyTyped(Number)
    public shipLogBookPageId?: number;

    @StrictlyTyped(String)
    public shipLogBookPageNumber?: string;

    @StrictlyTyped(Date)
    public tripStartDateTime?: Date;

    @StrictlyTyped(Date)
    public tripEndDateTime?: Date;

    @StrictlyTyped(Boolean)
    public isChecked?: boolean;
}