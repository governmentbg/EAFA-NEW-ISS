import { Observable } from 'rxjs';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { ScientificFishingOutingDTO } from '@app/models/generated/dtos/ScientificFishingOutingDTO';
import { ScientificFishingPermitDTO } from '@app/models/generated/dtos/ScientificFishingPermitDTO';
import { ScientificFishingPermitEditDTO } from '@app/models/generated/dtos/ScientificFishingPermitEditDTO';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { ScientificFishingFilters } from '@app/models/generated/filters/ScientificFishingFilters';
import { ScientificFishingPublicFilters } from '@app/models/generated/filters/ScientificFishingPublicFilters';
import { IApplicationsActionsService } from './application-actions.interface';
import { SciFiPrintTypesEnum } from '@app/enums/sci-fi-print-types.enum';
import { ApplicationSubmittedByDTO } from '@app/models/generated/dtos/ApplicationSubmittedByDTO';
import { ScientificFishingReasonNomenclatureDTO } from '@app/models/generated/dtos/ScientificFishingReasonNomenclatureDTO';
import { PrintConfigurationParameters } from '@app/components/common-app/applications/models/print-configuration-parameters.model';

export interface IScientificFishingService extends IApplicationsActionsService {
    getAllPermits(request: GridRequestModel<ScientificFishingFilters | ScientificFishingPublicFilters>): Observable<GridResultModel<ScientificFishingPermitDTO>>;
    getPermit(id: number): Observable<ScientificFishingPermitEditDTO>;
    getCurrentUserAsSubmittedBy(): Observable<ApplicationSubmittedByDTO>;

    addPermit(permit: ScientificFishingPermitEditDTO): Observable<number>;
    addAndDownloadRegister(model: ScientificFishingPermitEditDTO, printType: SciFiPrintTypesEnum, configurations: PrintConfigurationParameters): Observable<boolean>;
    editPermit(permit: ScientificFishingPermitEditDTO): Observable<void>;
    editAndDownloadRegister(model: ScientificFishingPermitEditDTO, printType: SciFiPrintTypesEnum, configurations: PrintConfigurationParameters): Observable<boolean>;

    deletePermit(id: number): Observable<void>;
    undoDeletePermit(id: number): Observable<void>;

    getPermitHolderPhoto(holderId: number): Observable<string>;
    getShipCaptainName(shipId: number): Observable<string>;

    addOuting(outing: ScientificFishingOutingDTO): Observable<number>;
    downloadFile(fileId: number, fileName: string): Observable<boolean>;
    downloadRegister(id: number, printType: SciFiPrintTypesEnum, configurations: PrintConfigurationParameters): Observable<boolean>;

    getPermitReasons(): Observable<ScientificFishingReasonNomenclatureDTO[]>;
    getPermitStatuses(): Observable<NomenclatureDTO<number>[]>;

    getSimpleAudit(id: number): Observable<SimpleAuditDTO>;
    getPermitHolderAudit(id: number): Observable<SimpleAuditDTO>;
    getPermitOutingAudit(id: number): Observable<SimpleAuditDTO>;
}