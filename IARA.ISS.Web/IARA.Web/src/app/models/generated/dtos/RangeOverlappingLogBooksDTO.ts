

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { OverlappingLogBookDTO } from './OverlappingLogBookDTO';

export class RangeOverlappingLogBooksDTO { 
    public constructor(obj?: Partial<RangeOverlappingLogBooksDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public startPage?: number;

    @StrictlyTyped(Number)
    public endPage?: number;

    @StrictlyTyped(String)
    public logBookNumber?: string;

    @StrictlyTyped(OverlappingLogBookDTO)
    public overlappingLogBooks?: OverlappingLogBookDTO[];
}