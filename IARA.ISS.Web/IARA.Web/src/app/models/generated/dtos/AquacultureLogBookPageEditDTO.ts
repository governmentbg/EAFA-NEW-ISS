

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { LogBookPagePersonDTO } from './LogBookPagePersonDTO';
import { LogBookPageProductDTO } from './LogBookPageProductDTO';
import { FileInfoDTO } from './FileInfoDTO';

export class AquacultureLogBookPageEditDTO { 
    public constructor(obj?: Partial<AquacultureLogBookPageEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public logBookId?: number;

    @StrictlyTyped(Number)
    public logBookTypeId?: number;

    @StrictlyTyped(Number)
    public pageNumber?: number;

    @StrictlyTyped(String)
    public status?: string;

    @StrictlyTyped(Date)
    public fillingDate?: Date;

    @StrictlyTyped(Date)
    public iaraAcceptanceDateTime?: Date;

    @StrictlyTyped(String)
    public aquacultureFacilityName?: string;

    @StrictlyTyped(LogBookPagePersonDTO)
    public buyer?: LogBookPagePersonDTO;

    @StrictlyTyped(LogBookPageProductDTO)
    public products?: LogBookPageProductDTO[];

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];
}