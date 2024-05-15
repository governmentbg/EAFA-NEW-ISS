import { ICatchesAndSalesService } from '@app/interfaces/common-app/catches-and-sales.interface';

export class AddShipWizardDialogParams {
    public service!: ICatchesAndSalesService;
    public logBookId!: number;
    public pageNumber: string | undefined;
    public isEdit: boolean = false;
    public pageId: number | undefined;

    public constructor(params?: Partial<AddShipWizardDialogParams>) {
        Object.assign(this, params);
    }
}