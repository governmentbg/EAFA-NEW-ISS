import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { IDeliveryService } from '@app/interfaces/common-app/delivery.interface';
import { ApplicationDeliveryDTO } from '@app/models/generated/dtos/ApplicationDeliveryDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestService } from '@app/shared/services/request.service';
import { BaseAuditService } from '@app/services/common-app/base-audit.service';
import { RequestProperties } from '@app/shared/services/request-properties';

@Injectable({
    providedIn: 'root'
})
export class DeliveryAdministrationService extends BaseAuditService implements IDeliveryService {
    protected readonly controller: string = 'DeliveryAdministration';

    public constructor(requestService: RequestService) {
        super(requestService, AreaTypes.Administrative);
    }

    public getDeliveryTypes(applicationId: number): Observable<NomenclatureDTO<number>[]> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        return this.requestService.get(this.area, this.controller, 'GetDeliveryTypes', {
            httpParams: params,
            responseTypeCtr: NomenclatureDTO
        });
    }

    public getDeliveryData(deliveryId: number): Observable<ApplicationDeliveryDTO> {
        const params = new HttpParams().append('deliveryId', deliveryId.toString());

        return this.requestService.get(this.area, this.controller, 'GetDeliveryData', {
            httpParams: params,
            responseTypeCtr: ApplicationDeliveryDTO
        });
    }

    public addDeliveryData(applicationId: number, deliveryData: ApplicationDeliveryDTO): Observable<number> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        return this.requestService.post(this.area, this.controller, 'AddDeliveryData', deliveryData, {
            httpParams: params
        });
    }

    public editDeliveryData(deliveryData: ApplicationDeliveryDTO, sendEDelivery: boolean): Observable<void> {
        const params = new HttpParams()
            .append('deliveryId', deliveryData.id!.toString())
            .append('sendEDelivery', sendEDelivery.toString());

        return this.requestService.post(this.area, this.controller, 'UpdateDeliveryData', deliveryData, {
            httpParams: params,
            successMessage: 'succ-updated-delivery-data',
            properties: new RequestProperties({
                asFormData: true
            })
        });
    }

    public downloadFile(fileId: number): Observable<boolean> {
        const params = new HttpParams().append('id', fileId.toString());
        return this.requestService.download(this.area, this.controller, 'DownloadFile', '', { httpParams: params });
    }
}