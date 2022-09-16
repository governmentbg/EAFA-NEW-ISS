import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { IRecreationalFishingService } from '@app/interfaces/common-app/recreational-fishing.interface';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { RecreationalFishingTicketDTO } from '@app/models/generated/dtos/RecreationalFishingTicketDTO';
import { RecreationalFishingTicketValidToCalculationParamsDTO } from '@app/models/generated/dtos/RecreationalFishingTicketValidToCalculationParamsDTO';
import { RegixPersonDataDTO } from '@app/models/generated/dtos/RegixPersonDataDTO';
import { FileInfoDTO } from '@app/models/generated/dtos/FileInfoDTO';
import { RecreationalFishingTicketDeclarationParametersDTO } from '@app/models/generated/dtos/RecreationalFishingTicketDeclarationParametersDTO';
import { TicketTypeEnum } from '@app/enums/ticket-type.enum';
import { TicketPeriodEnum } from '@app/enums/ticket-period.enum';
import { RecreationalFishingPublicService } from '@app/services/public-app/recreational-fishing-public.service';
import { IdentifierTypeEnum } from '@app/enums/identifier-type.enum';

@Component({
    selector: 'recreational-fishing-ticket-summary',
    templateUrl: './recreational-fishing-ticket-summary.component.html',
    styleUrls: ['./recreational-fishing-ticket-summary.component.scss']
})
export class RecreationalFishingTicketSummaryComponent implements OnChanges {
    @Input()
    public service!: IRecreationalFishingService;

    @Input()
    public type!: NomenclatureDTO<number>;

    @Input()
    public period!: NomenclatureDTO<number>;

    @Input()
    public ticket!: RecreationalFishingTicketDTO;

    @Input()
    public expectDeclaration: boolean = false;

    @Input()
    public showValidation: boolean = false;

    @Input()
    public isAssociation: boolean = false;

    @Input()
    public disable: boolean = false;

    public readonly identifierTypes: typeof IdentifierTypeEnum = IdentifierTypeEnum;

    public hasError: boolean = false;
    public validTo: Date | undefined;

    public constructor() {
        // nothing to do
    }

    public ngOnChanges(changes: SimpleChanges): void {
        if (this.expectDeclaration === true) {
            const showValidation: boolean | undefined = changes['showValidation']?.currentValue;
            if (showValidation === true) {
                this.hasError = this.ticket.declarationFile === null || this.ticket.declarationFile === undefined;
            }
            else {
                this.hasError = false;
            }
        }

        if (this.isValidState() && this.needsUpdate(changes)) {
            const params = new RecreationalFishingTicketValidToCalculationParamsDTO({
                typeId: this.type.value,
                periodId: this.period.value,
                validFrom: this.ticket.validFrom,
                birthDate: this.ticket.person?.birthDate,
                telkValidTo: this.ticket.telkData?.isIndefinite === true ? undefined : this.ticket.telkData?.validTo
            });

            this.service.calculateTicketValidToDate(params).subscribe({
                next: (validTo: Date) => {
                    this.validTo = validTo;
                }
            });
        }
    }

    public printApplication(): void {
        if (this.expectDeclaration) {
            const parameters = new RecreationalFishingTicketDeclarationParametersDTO({
                ticketNum: this.ticket.ticketNum,
                validFrom: this.ticket.validFrom,
                type: TicketTypeEnum[this.type.code as keyof typeof TicketTypeEnum],
                period: TicketPeriodEnum[this.period.code as keyof typeof TicketPeriodEnum],
                person: this.ticket.person,
                personAddressRegistrations: this.ticket.personAddressRegistrations,
                representativePerson: this.ticket.representativePerson,
                representativePersonAddressRegistrations: this.ticket.representativePersonAddressRegistrations
            });

            if (this.isAssociation) {
                parameters.associationId = (this.service as RecreationalFishingPublicService).currentUserChosenAssociation!.value!;
            }

            this.service.downloadTicketDeclaration(parameters).subscribe({
                next: () => {
                    // nothing to do
                }
            });
        }
    }

    public onFileUploaded(files: FileList): void {
        if (files) {
            const file: File = files[0];

            this.ticket.declarationFile = new FileInfoDTO({
                file: file,
                fileTypeId: undefined,
                size: file.size,
                contentType: file.type,
                name: file.name,
                uploadedOn: new Date()
            });

            this.hasError = false;
        }
    }

    private isValidState(): boolean {
        if (this.isNullOrUndefined(this.type)) {
            return false;
        }

        if (this.isNullOrUndefined(this.period)) {
            return false;
        }

        if (this.isNullOrUndefined(this.ticket.validFrom)) {
            return false;
        }

        if (this.isNullOrUndefined(this.ticket.person)) {
            return false;
        }

        return true;
    }

    private needsUpdate(changes: SimpleChanges): boolean {
        if (changes['type']?.currentValue !== changes['type']?.previousValue) {
            return true;
        }

        if (changes['period']?.currentValue !== changes['period']?.previousValue) {
            return true;
        }

        if (changes['validFrom']?.currentValue !== changes['validFrom']?.previousValue) {
            return true;
        }

        if (changes['person']?.currentValue !== changes['person']?.previousValue) {
            const curr = changes['person']?.currentValue as RegixPersonDataDTO;
            const prev = changes['person']?.previousValue as RegixPersonDataDTO;

            if (curr?.birthDate !== prev?.birthDate) {
                return true;
            }
        }
        return false;
    }

    private isNullOrUndefined(val: unknown): boolean {
        return val === null || val === undefined;
    }
}
