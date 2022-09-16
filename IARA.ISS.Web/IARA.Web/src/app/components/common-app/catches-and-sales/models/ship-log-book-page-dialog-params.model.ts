import { CatchesAndSalesDialogParamsModel } from './catches-and-sales-dialog-params.model';

export class ShipLogBookPageDialogParamsModel extends CatchesAndSalesDialogParamsModel {
    public permitLicenseId: number | undefined;

    public constructor(obj?: Partial<ShipLogBookPageDialogParamsModel>) {
        super(obj);
        Object.assign(this, obj);
    }
}