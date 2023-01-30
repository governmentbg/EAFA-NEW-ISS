import { Observable } from 'rxjs';

import { PageCodeEnum } from '@app/enums/page-code.enum';
import { SuspensionDataDTO } from '@app/models/generated/dtos/SuspensionDataDTO';
import { SuspensionReasonNomenclatureDTO } from '@app/models/generated/dtos/SuspensionReasonNomenclatureDTO';
import { SuspensionTypeNomenclatureDTO } from '@app/models/generated/dtos/SuspensionTypeNomenclatureDTO';

export interface ISuspensionService {
    getSuspensionTypes(): Observable<SuspensionTypeNomenclatureDTO[]>;
    getSuspensionReasons(): Observable<SuspensionReasonNomenclatureDTO[]>;

    addSuspension(suspension: SuspensionDataDTO, id: number, pageCode: PageCodeEnum): Observable<void>;
}