

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { CommonLogBookPageDataDTO } from './CommonLogBookPageDataDTO';
import { LogBookPagePersonDTO } from './LogBookPagePersonDTO';
import { LogBookPageProductDTO } from './LogBookPageProductDTO';
import { FileInfoDTO } from './FileInfoDTO';

export class AdmissionLogBookPageEditDTO { 
    public constructor(obj?: Partial<AdmissionLogBookPageEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public logBookId?: number;

    @StrictlyTyped(Number)
    public logBookTypeId?: number;

    @StrictlyTyped(Number)
    public logBookPermitLicenseId?: number;

    @StrictlyTyped(Number)
    public pageNumber?: number;

    @StrictlyTyped(String)
    public status?: string;

    @StrictlyTyped(CommonLogBookPageDataDTO)
    public commonData?: CommonLogBookPageDataDTO;

    @StrictlyTyped(Date)
    public handoverDate?: Date;

    @StrictlyTyped(String)
    public storageLocation?: string;

    @StrictlyTyped(LogBookPagePersonDTO)
    public acceptingPerson?: LogBookPagePersonDTO;

    @StrictlyTyped(LogBookPageProductDTO)
    public products?: LogBookPageProductDTO[];

    @StrictlyTyped(LogBookPageProductDTO)
    public originalPossibleProducts?: LogBookPageProductDTO[];

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];
}