

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { RegixPersonDataDTO } from './RegixPersonDataDTO';
import { AddressRegistrationDTO } from './AddressRegistrationDTO';
import { DuplicatesEntryDTO } from './DuplicatesEntryDTO';
import { FileInfoDTO } from './FileInfoDTO';
import { QualifiedFisherStatusesEnum } from '@app/enums/qualified-fisher-statuses.enum';

export class QualifiedFisherEditDTO { 
    public constructor(obj?: Partial<QualifiedFisherEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Date)
    public registrationDate?: Date;

    @StrictlyTyped(Boolean)
    public isOnlineApplication?: boolean;

    @StrictlyTyped(String)
    public registrationNum?: string;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(String)
    public egn?: string;

    @StrictlyTyped(Boolean)
    public isWithMaritimeEducation?: boolean;

    @StrictlyTyped(Number)
    public status?: QualifiedFisherStatusesEnum;

    @StrictlyTyped(Boolean)
    public hasExam?: boolean;

    @StrictlyTyped(Number)
    public examTerritoryUnitId?: number;

    @StrictlyTyped(String)
    public examProtocolNumber?: string;

    @StrictlyTyped(Date)
    public examDate?: Date;

    @StrictlyTyped(Boolean)
    public hasPassedExam?: boolean;

    @StrictlyTyped(String)
    public diplomaNumber?: string;

    @StrictlyTyped(Date)
    public diplomaDate?: Date;

    @StrictlyTyped(String)
    public diplomaIssuer?: string;

    @StrictlyTyped(String)
    public comments?: string;

    @StrictlyTyped(Number)
    public applicationId?: number;

    @StrictlyTyped(RegixPersonDataDTO)
    public submittedForRegixData?: RegixPersonDataDTO;

    @StrictlyTyped(AddressRegistrationDTO)
    public submittedForAddresses?: AddressRegistrationDTO[];

    @StrictlyTyped(DuplicatesEntryDTO)
    public duplicateEntries?: DuplicatesEntryDTO[];

    @StrictlyTyped(FileInfoDTO)
    public files?: FileInfoDTO[];
}