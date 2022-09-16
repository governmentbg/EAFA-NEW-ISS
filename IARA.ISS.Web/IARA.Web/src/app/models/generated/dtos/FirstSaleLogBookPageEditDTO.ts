

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { CommonLogBookPageDataDTO } from './CommonLogBookPageDataDTO';
import { LogBookPageProductDTO } from './LogBookPageProductDTO';
import { FileInfoDTO } from './FileInfoDTO';

export class FirstSaleLogBookPageEditDTO { 
    public constructor(obj?: Partial<FirstSaleLogBookPageEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public logBookId?: number;

    @StrictlyTyped(Number)
    public pageNumber?: number;

    @StrictlyTyped(String)
    public status?: string;

    @StrictlyTyped(String)
    public logBookNumber?: string;

    @StrictlyTyped(CommonLogBookPageDataDTO)
    public commonData?: CommonLogBookPageDataDTO;

    @StrictlyTyped(Date)
    public saleDate?: Date;

    @StrictlyTyped(String)
    public saleContractNumber?: string;

    @StrictlyTyped(Date)
    public saleContractDate?: Date;

    @StrictlyTyped(String)
    public saleLocation?: string;

    @StrictlyTyped(Number)
    public buyerId?: number;

    @StrictlyTyped(String)
    public buyerName?: string;

    @StrictlyTyped(LogBookPageProductDTO)
    public products?: LogBookPageProductDTO[];

    @StrictlyTyped(LogBookPageProductDTO)
    public originalPossibleProducts?: LogBookPageProductDTO[];

    @StrictlyTyped(String)
    public productsTotalPrice?: string;

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];
}