import { Component, OnInit } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { StepperSelectionEvent } from '@angular/cdk/stepper';

import { IRecreationalFishingService } from '@app/interfaces/common-app/recreational-fishing.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { RecreationalFishingTicketDuplicateDTO } from '@app/models/generated/dtos/RecreationalFishingTicketDuplicateDTO';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { TariffNomenclatureDTO } from '@app/models/generated/dtos/TariffNomenclatureDTO';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { PaymentSummaryDTO } from '@app/models/generated/dtos/PaymentSummaryDTO';
import { TariffCodesEnum } from '@app/enums/tariff-codes.enum';
import { PaymentTariffDTO } from '@app/models/generated/dtos/PaymentTariffDTO';
import { IssueDuplicateTicketDialogParams } from '../../models/issue-duplicate-ticket-dialog-params.model';
import { FileInfoDTO } from '@app/models/generated/dtos/FileInfoDTO';
import { PaymentTypesEnum } from '@app/enums/payment-types.enum';
import { PaymentDataDTO } from '@app/models/generated/dtos/PaymentDataDTO';

@Component({
    selector: 'issue-duplicate-ticket',
    templateUrl: './issue-duplicate-ticket.component.html'
})
export class IssueDuplicateTicketComponent implements OnInit, IDialogComponent {
    public readonly currentDate: Date = new Date();

    public ticketNumControl: FormControl;
    public paymentDataControl: FormControl;

    public paymentSummary!: PaymentSummaryDTO;
    public payment: PaymentDataDTO | undefined;

    private ticketId!: number;
    private associationId: number | undefined;
    private photo: FileInfoDTO | undefined;

    private service!: IRecreationalFishingService;
    private nomenclatures: CommonNomenclatures;

    public constructor(nomenclatures: CommonNomenclatures) {
        this.nomenclatures = nomenclatures;

        this.ticketNumControl = new FormControl(null, [Validators.required, TLValidators.number(0)]);
        this.paymentDataControl = new FormControl(null, Validators.required);
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

    public setData(data: IssueDuplicateTicketDialogParams, wrapperData: DialogWrapperData): void {
        this.ticketId = data.ticketId;
        this.associationId = data.associationId;
        this.service = data.service;
        this.photo = data.photo;
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

            this.paymentDataControl.setValue(new PaymentDataDTO({
                paymentType: PaymentTypesEnum.CASH,
                paymentDateTime: this.currentDate,
                totalPaidPrice: this.paymentSummary.totalPrice
            }));
        }
    }

    private checkTicketNumber(): void {
        this.updateTicketNumAndEmit();
    }

    private updateTicketNumAndEmit(): void {
        this.ticketNumControl.markAsTouched();
        this.ticketNumControl.updateValueAndValidity();
    }

    private getValue(): RecreationalFishingTicketDuplicateDTO {
        return new RecreationalFishingTicketDuplicateDTO({
            ticketId: this.ticketId,
            ticketNum: this.ticketNumControl.value,
            price: this.paymentSummary.totalPrice,
            paymentData: this.paymentDataControl.value,
            createdByAssociationId: this.associationId,
            personPhoto: this.photo
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
