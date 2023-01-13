import { ICatchesAndSalesService } from '@app/interfaces/common-app/catches-and-sales.interface';
import { OriginDeclarationFishDTO } from '@app/models/generated/dtos/OriginDeclarationFishDTO';

export class OriginDeclarationDialogParamsModel {
    public model: OriginDeclarationFishDTO | undefined;
    public service!: ICatchesAndSalesService;
    public viewMode: boolean = true;
    public isAllCatchTransboarded: boolean = false;

    public constructor(obj?: Partial<OriginDeclarationDialogParamsModel>) {
        Object.assign(this, obj);
    }

}