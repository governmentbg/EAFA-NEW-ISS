import { Observable } from 'rxjs';

import { CommercialFishingLogBookEditDTO } from '@app/models/generated/dtos/CommercialFishingLogBookEditDTO';
import { RangeOverlappingLogBooksDTO } from '@app/models/generated/dtos/RangeOverlappingLogBooksDTO';
import { OverlappingLogBooksParameters } from '@app/shared/components/overlapping-log-books/models/overlapping-log-books-parameters.model';
import { LogBookForRenewalDTO } from '@app/models/generated/dtos/LogBookForRenewalDTO';
import { LogBookEditDTO } from '@app/models/generated/dtos/LogBookEditDTO';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { ShipLogBookPageRegisterDTO } from '@app/models/generated/dtos/ShipLogBookPageRegisterDTO';
import { FirstSaleLogBookPageRegisterDTO } from '@app/models/generated/dtos/FirstSaleLogBookPageRegisterDTO';
import { AdmissionLogBookPageRegisterDTO } from '@app/models/generated/dtos/AdmissionLogBookPageRegisterDTO';
import { TransportationLogBookPageRegisterDTO } from '@app/models/generated/dtos/TransportationLogBookPageRegisterDTO';
import { AquacultureLogBookPageRegisterDTO } from '@app/models/generated/dtos/AquacultureLogBookPageRegisterDTO';
import { LogBookTypesEnum } from '@app/enums/log-book-types.enum';

export interface ILogBookService {
    getLogBooksForTable(permitLicenseId: number): Observable<CommercialFishingLogBookEditDTO[]>;

    getLogBook(logBookId: number): Observable<LogBookEditDTO>;
    getPermitLicenseLogBook(logBookPermitLicenseId: number): Observable<CommercialFishingLogBookEditDTO>;

    getLogBookPagesAndDeclarations(logBookId: number, permitLicenseId: number | undefined, logBookType: LogBookTypesEnum): Observable<ShipLogBookPageRegisterDTO[] | AdmissionLogBookPageRegisterDTO[] | TransportationLogBookPageRegisterDTO[]>;
    getLogBookPages(logBookId: number, logBookType: LogBookTypesEnum): Observable<FirstSaleLogBookPageRegisterDTO[]
        | AdmissionLogBookPageRegisterDTO[]
        | TransportationLogBookPageRegisterDTO[]
        | AquacultureLogBookPageRegisterDTO[]>;

    addLogBook(model: CommercialFishingLogBookEditDTO | LogBookEditDTO, registerId: number, ignoreLogBookConflicts: boolean): Observable<void>;
    editLogBook(model: CommercialFishingLogBookEditDTO | LogBookEditDTO, registerId: number, ignoreLogBookConflicts: boolean): Observable<void>;

    addLogBooksFromOldPermitLicenses(logBooks: CommercialFishingLogBookEditDTO[], permitLicenseId: number, ignoreLogBookConflicts: boolean): Observable<void>;

    getOverlappedLogBooks(parameters: OverlappingLogBooksParameters[]): Observable<RangeOverlappingLogBooksDTO[]>;

    getLogBooksForRenewal(permitLicenseRegisterId: number, showFinished: boolean): Observable<LogBookForRenewalDTO[]>;
    getLogBooksForRenewalByIds(permitLicenseRegisterIds: number[]): Observable<CommercialFishingLogBookEditDTO[] | LogBookEditDTO[]>;

    getLogBookAudit(id: number): Observable<SimpleAuditDTO>;

    deleteLogBook(logBookId: number): Observable<void>;
    deleteLogBookPermitLicense(id: number): Observable<void>;
    
    undoDeleteLogBook(logBookPermitLicenseId: number): Observable<void>;
    undoDeleteLogBookPermitLicense(logBookPermitLicenseId: number): Observable<void>;
}