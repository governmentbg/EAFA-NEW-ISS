

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { CommonLogBookPageDataDTO } from './CommonLogBookPageDataDTO';
import { LogBookPagePersonDTO } from './LogBookPagePersonDTO';
import { LogBookPageProductDTO } from './LogBookPageProductDTO';
import { FileInfoDTO } from './FileInfoDTO';

export class TransportationLogBookPageEditDTO { 
    public constructor(obj?: Partial<TransportationLogBookPageEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public logBookId?: number;

    @StrictlyTyped(Number)
    public logBookPermitLicenseId?: number;

    @StrictlyTyped(Number)
    public pageNumber?: number;

    @StrictlyTyped(String)
    public status?: string;

    @StrictlyTyped(CommonLogBookPageDataDTO)
    public commonData?: CommonLogBookPageDataDTO;

    @StrictlyTyped(String)
    public vehicleIdentification?: string;

    @StrictlyTyped(String)
    public loadingLocation?: string;

    @StrictlyTyped(Date)
    public loadingDate?: Date;

    @StrictlyTyped(String)
    public deliveryLocation?: string;

    @StrictlyTyped(LogBookPagePersonDTO)
    public receiver?: LogBookPagePersonDTO;

    @StrictlyTyped(LogBookPageProductDTO)
    public products?: LogBookPageProductDTO[];

    @StrictlyTyped(LogBookPageProductDTO)
    public originalPossibleProducts?: LogBookPageProductDTO[];

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];
}