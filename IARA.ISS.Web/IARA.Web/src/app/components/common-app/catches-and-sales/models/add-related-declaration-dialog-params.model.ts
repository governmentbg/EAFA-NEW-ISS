import { ShipLogBookPageEditDTO } from '@app/models/generated/dtos/ShipLogBookPageEditDTO';
import { ICatchesAndSalesService } from '@app/interfaces/common-app/catches-and-sales.interface';

export class AddRelatedDeclarationDialogParams {
    public service: ICatchesAndSalesService;
    public shipLogBookPage: ShipLogBookPageEditDTO;

    public constructor(service: ICatchesAndSalesService, page: ShipLogBookPageEditDTO) {
        this.service = service;
        this.shipLogBookPage = page;
    }
}