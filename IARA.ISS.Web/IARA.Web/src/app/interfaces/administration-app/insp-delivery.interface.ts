import { Observable } from 'rxjs';
import { AuanDeliveryDataDTO } from '@app/models/generated/dtos/AuanDeliveryDataDTO';
import { InspDeliveryTypesNomenclatureDTO } from '@app/models/generated/dtos/InspDeliveryTypesNomenclatureDTO';

export interface IInspDeliveryService {
    getDeliveryData(id: number): Observable<AuanDeliveryDataDTO>;
    addDeliveryData(registerId: number, deliveryData: AuanDeliveryDataDTO): Observable<number>;
    editDeliveryData(registerId: number, deliveryData: AuanDeliveryDataDTO, sendEDelivery: boolean): Observable<void>;
    downloadFile(fileId: number): Observable<boolean>;
    getDeliveryTypes(): Observable<InspDeliveryTypesNomenclatureDTO[]>;
}