import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { MatStepper } from '@angular/material/stepper';
import { StepperSelectionEvent } from '@angular/cdk/stepper';
import { Subject } from 'rxjs';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { IRecreationalFishingService } from '@app/interfaces/common-app/recreational-fishing.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { RecreationalFishingTicketDuplicateDTO } from '@app/models/generated/dtos/RecreationalFishingTicketDuplicateDTO';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';
import { TariffNomenclatureDTO } from '@app/models/generated/dtos/TariffNomenclatureDTO';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { PaymentSummaryDTO } from '@app/models/generated/dtos/PaymentSummaryDTO';
import { TariffCodesEnum } from '@app/enums/tariff-codes.enum';
import { PaymentTariffDTO } from '@app/models/generated/dtos/PaymentTariffDTO';
import { IssueDuplicateTicketDialogParams } from '../../models/issue-duplicate-ticket-dialog-params.model';
import { GetControlErrorLabelTextCallback } from '@app/shared/components/input-controls/base-tl-control';

@Component({
    selector: 'issue-duplicate-ticket',
    templateUrl: './issue-duplicate-ticket.component.html'
})
export class IssueDuplicateTicketComponent implements OnInit, AfterViewInit, IDialogComponent {
    public readonly currentDate: Date = new Date();

    public ticketNumControl: FormControl;
    public paymentDataControl: FormControl;

    public paymentSummary!: PaymentSummaryDTO;
    public ticketNumApproved: boolean = true;

    public getControlErrorLabelTextForTicketNumberMethod: GetControlErrorLabelTextCallback  = this.getControlErrorLabelTextForTicketNumber.bind(this);

    @ViewChild('stepper')
    private stepper!: MatStepper;

    private ticketId!: number;
    private associationId: number | undefined;

    private service!: IRecreationalFishingService;
    private translate: FuseTranslationLoaderService;
    private nomenclatures: CommonNomenclatures;

    private ticketNumbersAvailability: Map<string, boolean> = new Map<string, boolean>();
    private ticketInvalid: Subject<void> = new Subject<void>();

    public constructor(translate: FuseTranslationLoaderService, nomenclatures: CommonNomenclatures) {
        this.translate = translate;
        this.nomenclatures = nomenclatures;

        this.ticketNumControl = new FormControl(null, [Validators.required, TLValidators.number(0), this.ticketNumberAlreadyInUse()]);
        this.paymentDataControl = new FormControl(null, Validators.required);

        this.ticketNumControl.valueChanges.subscribe({
            next: (ticketNum: string) => {
                this.ticketNumApproved = false;
            }
        });
    }

    public ngOnInit(): void {
        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.PaymentTariffs, this.nomenclatures.getPaymentTariffs.bind(this.nomenclatures), false
        ).subscribe({
            next: (tariffs: TariffNomenclatureDTO[]) => {
                const tariff: TariffNomenclatureDTO | undefined = tariffs.find(x => x.code === TariffCodesEnum[TariffCodesEnum.Ticket_duplicate]);

                if (tariff !== undefined) {
                    this.paymentSummary = new PaymentSummaryDTO({
                        tariffs: [
                            new PaymentTariffDTO({
                                tariffId: tariff.value,
                                tariffName: tariff.displayName,
                                tariffDescription: tariff.description,
                                tariffBasedOnPlea: tariff.basedOnPlea,
                                quantity: 1,
                                unitPrice: tariff.price,
                                price: tariff.price,
                                isCalculated: tariff.isCalculated,
                                isChecked: true
                            })
                        ],
                        totalPrice: tariff.price
                    });
                }
                else {
                    throw new Error('Could not find tariff for ticket duplicate!');
                }
            }
        });
    }

    public ngAfterViewInit(): void {
        this.ticketInvalid.subscribe({
            next: () => {
                setTimeout(() => {
                    this.stepper.previous();
                });
            }
        });
    }

    public setData(data: IssueDuplicateTicketDialogParams, wrapperData: DialogWrapperData): void {
        this.ticketId = data.ticketId;
        this.associationId = data.associationId;
        this.service = data.service;
    }

    public saveBtnClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.markAllAsTouched();

        if (this.isValid()) {
            this.service.addTicketDuplicate(this.getValue()).subscribe({
                next: (id: number) => {
                    dialogClose(true);
                }
            });
        }
    }

    public cancelBtnClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose(false);
    }

    public dialogButtonClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (action.id === 'save-and-print') {
            this.markAllAsTouched();

            if (this.isValid()) {
                this.service.addTicketDuplicate(this.getValue()).subscribe({
                    next: (id: number) => {
                        this.service.downloadFishingTicket(id).subscribe({
                            next: () => {
                                dialogClose(true);
                            }
                        });
                    }
                });
            }
        }
    }

    public onStepChange(step: StepperSelectionEvent): void {
        if (step.selectedIndex === 1) {
            this.checkTicketNumber();
        }
    }

    public getControlErrorLabelTextForTicketNumber(controlName: string, error: unknown, errorCode: string): TLError | undefined {
        if (errorCode === 'alreadyInUse') {
            return new TLError({
                text: this.translate.getValue('recreational-fishing.ticket-number-already-in-use'),
                type: 'error'
            });
        }
        return undefined;
    }

    private checkTicketNumber(): void {
        const ticketNum: string = this.ticketNumControl.value;

        if (this.ticketNumbersAvailability.has(ticketNum)) {
            this.updateTicketNumAndEmit();
        }
        else {
            this.service.checkTicketNumbersAvailability([ticketNum]).subscribe({
                next: (result: boolean[]) => {
                    this.ticketNumApproved = true;

                    this.ticketNumbersAvailability.set(ticketNum, result[0]);
                    this.updateTicketNumAndEmit();
                }
            });
        }
    }

    private updateTicketNumAndEmit(): void {
        this.ticketNumControl.markAsTouched();
        this.ticketNumControl.updateValueAndValidity();

        if (!this.ticketNumControl.valid) {
            this.ticketInvalid.next();
        }
    }

    private ticketNumberAlreadyInUse(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (this.ticketNumApproved === true) {
                const available: boolean | undefined = this.ticketNumbersAvailability.get(control.value);
                if (available !== undefined && available === false) {
                    return { alreadyInUse: true };
                }
            }

            return null;
        };
    }

    private getValue(): RecreationalFishingTicketDuplicateDTO {
        return new RecreationalFishingTicketDuplicateDTO({
            ticketId: this.ticketId,
            ticketNum: this.ticketNumControl.value,
            price: this.paymentSummary.totalPrice,
            paymentData: this.paymentDataControl.value,
            createdByAssociationId: this.associationId
        });
    }

    private markAllAsTouched(): void {
        this.ticketNumControl.markAsTouched();
        this.paymentDataControl.markAsTouched();
    }

    private isValid(): boolean {
        return this.ticketNumControl.valid && this.paymentDataControl.valid;
    }
}
