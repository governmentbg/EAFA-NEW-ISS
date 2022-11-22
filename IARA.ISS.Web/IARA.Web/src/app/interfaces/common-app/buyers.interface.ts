import { Observable } from "rxjs";

import { GridRequestModel } from "@app/models/common/grid-request.model";
import { GridResultModel } from "@app/models/common/grid-result.model";
import { BuyerDTO } from "@app/models/generated/dtos/BuyerDTO";
import { BuyersFilters } from "@app/models/generated/filters/BuyersFilters";
import { IApplicationsActionsService } from "@app/interfaces/common-app/application-actions.interface";
import { BuyerEditDTO } from '@app/models/generated/dtos/BuyerEditDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { CancellationHistoryEntryDTO } from '@app/models/generated/dtos/CancellationHistoryEntryDTO';
import { BuyerTypesEnum } from '@app/enums/buyer-types.enum';
import { PrintConfigurationParameters } from '@app/components/common-app/applications/models/print-configuration-parameters.model';

export interface IBuyersService extends IApplicationsActionsService {
    getAll(request: GridRequestModel<BuyersFilters>): Observable<GridResultModel<BuyerDTO>>;
    get(id: number): Observable<BuyerEditDTO>;
    edit(item: BuyerDTO, ignoreLogBookConflicts: boolean): Observable<number>;
    editAndDownloadRegister(model: BuyerEditDTO, configurations: PrintConfigurationParameters, ignoreLogBookConflicts: boolean): Observable<boolean>;
    add(item: BuyerDTO, ignoreLogBookConflicts: boolean): Observable<number>;
    addAndDownloadRegister(model: BuyerEditDTO, configurations: PrintConfigurationParameters, ignoreLogBookConflicts: boolean): Observable<boolean>;
    downloadRegister(id: number, buyerType: BuyerTypesEnum, configurations: PrintConfigurationParameters): Observable<boolean>;
    updateBuyerStatus(buyerId: number, status: CancellationHistoryEntryDTO, applicationId?: number): Observable<void>;

    getBuyerTypes(): Observable<NomenclatureDTO<number>[]>;
    getAllBuyersNomenclatures(): Observable<NomenclatureDTO<number>[]>;
    getAllFirstSaleCentersNomenclatures(): Observable<NomenclatureDTO<number>[]>;
    getBuyerStatuses(): Observable<NomenclatureDTO<number>[]>;

    getBuyerFromChangeOfCircumstancesApplication(applicationId: number): Observable<BuyerEditDTO>;
    getBuyerFromTerminationApplication(applicationId: number): Observable<BuyerEditDTO>;
    completeBuyerChangeOfCircumstancesApplication(buyer: BuyerEditDTO, ignoreLogBookConflicts: boolean): Observable<void>;

    getPremiseUsageDocumentAudit(id: number): Observable<SimpleAuditDTO>;
    getLogBookAudit(id: number): Observable<SimpleAuditDTO>;
    getBuyerLicensesAudit(id: number): Observable<SimpleAuditDTO>;

}