

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class TableNodeDTO { 
    public constructor(obj?: Partial<TableNodeDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(String)
    public displayName?: string;

    @StrictlyTyped(TableNodeDTO)
    public children?: TableNodeDTO[];
}