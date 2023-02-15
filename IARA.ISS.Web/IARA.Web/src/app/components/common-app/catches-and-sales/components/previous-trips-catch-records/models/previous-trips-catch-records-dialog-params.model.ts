import { ICatchesAndSalesService } from '@app/interfaces/common-app/catches-and-sales.interface';

export class PreviousTripsCatchRecordsDialogParams {
    public shipId!: number;
    public service!: ICatchesAndSalesService;
    public currentPageId: number | undefined;

    public constructor(obj?: Partial<PreviousTripsCatchRecordsDialogParams>) {
        Object.assign(this, obj);
    }
}