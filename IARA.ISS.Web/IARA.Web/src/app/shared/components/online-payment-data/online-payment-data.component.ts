import { AfterViewInit, Component, DoCheck, EventEmitter, Input, OnInit, Optional, Output, Self, ViewEncapsulation } from '@angular/core';
import { AbstractControl, ControlValueAccessor, FormControl, FormGroup, NgControl, ValidationErrors, Validator, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { IdentifierTypeEnum } from '@app/enums/identifier-type.enum';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { PaymentTypesEnum } from '@app/enums/payment-types.enum';
import { IApplicationsService } from '@app/interfaces/administration-app/applications.interface';
import { IEPaymentsService } from '@app/interfaces/common-app/e-payments.interface';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PaymentSummaryDTO } from '@app/models/generated/dtos/PaymentSummaryDTO';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { EPaymentsService } from '@app/services/common-app/e-payments.service';
import { UsersService } from '@app/services/common-app/users.service';
import { ApplicationsPublicService } from '@app/services/public-app/applications-public.service';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { FormDataModel, TLEGovPaymentService } from '@tl/tl-egov-payments';
import { GeneratedPaymentModel } from '@tl/tl-epay-payments';
import { DialogWrapperComponent } from '../dialog-wrapper/dialog-wrapper.component';
import { IActionInfo } from '../dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '../dialog-wrapper/interfaces/dialog-content.interface';
import { HeaderCloseFunction } from '../dialog-wrapper/interfaces/header-cancel-button.interface';
import { DialogWrapperData } from '../dialog-wrapper/models/dialog-action-buttons.model';
import { TLMatDialog } from '../dialog-wrapper/tl-mat-dialog';
import { EGovOfflinePaymenDataComponent } from './egov-offline-payment-data/egov-offline-payment-data.component';
import { EGovOfflinePaymentDataDialogParams } from './egov-offline-payment-data/models/egov-offline-payment-data-dialog-params.model';
import { OnlinePaymentDataDialogParams } from './egov-offline-payment-data/models/online-payment-data-dialog-params.model';

@Component({
    selector: 'online-payment-data',
    templateUrl: './online-payment-data.component.html',
    styleUrls: ['./online-payment-data.component.scss'],
    encapsulation: ViewEncapsulation.None
})
export class OnlinePaymentDataComponent implements OnInit, AfterViewInit, DoCheck, ControlValueAccessor, Validator, IDialogComponent {
    @Input()
    public applicationId!: number;

    @Input()
    public paymentRequestNum!: string;

    @Input()
    public okUrl: string = '';

    @Input()
    public cancelUrl: string = '';

    @Input()
    public removeOfflinePaymentOptions: boolean = false;

    @Output()
    public egovOfflinePaymentDataDialogClosed: EventEmitter<void> = new EventEmitter<void>();

    public performOnlinePaymentFormGroup: FormGroup;
    public paymentTypes!: NomenclatureDTO<number>[];
    public paymentTypesEnum: typeof PaymentTypesEnum = PaymentTypesEnum;
    public isComponentOpenedInDialog: boolean = false;

    public showEGovBankPaymentButton: boolean = false;
    public showEGovEPOSPaymentButton: boolean = false;
    public showEGovEPayPaymentButton: boolean = false;
    public showEPayPaymentButton: boolean = false;
    public showEPayDirectPaymentButton: boolean = false;

    public showTariffs: boolean = false;

    public paymentTypeControl!: FormControl;
    public isForeignPerson: boolean = false;

    private applicationsService: IApplicationsService;
    private ePaymentsService: IEPaymentsService;
    private matDialogRef: MatDialogRef<DialogWrapperComponent<OnlinePaymentDataComponent>>;
    private egovOfflinePaymentDataDialog: TLMatDialog<EGovOfflinePaymenDataComponent>;
    private translationService: FuseTranslationLoaderService;
    private commonNomenclaturesService: CommonNomenclatures;
    private egovService: TLEGovPaymentService;

    private ngControl: NgControl;
    private onChanged: (value: NomenclatureDTO<number>) => void;
    private onTouched: (value: NomenclatureDTO<number>) => void;

    public constructor(
        @Optional() @Self() ngControl: NgControl,
        @Optional() matDialogRef: MatDialogRef<DialogWrapperComponent<OnlinePaymentDataComponent>>,
        egovOfflinePaymentDataDialog: TLMatDialog<EGovOfflinePaymenDataComponent>,
        applicationsService: ApplicationsPublicService,
        ePaymentsService: EPaymentsService,
        translationService: FuseTranslationLoaderService,
        commonNomenclaturesService: CommonNomenclatures,
        userService: UsersService,
        egovService: TLEGovPaymentService
    ) {
        this.ngControl = ngControl;
        this.matDialogRef = matDialogRef;

        if (this.matDialogRef !== null && this.matDialogRef !== undefined) {
            this.isComponentOpenedInDialog = true;
        }

        this.egovOfflinePaymentDataDialog = egovOfflinePaymentDataDialog;
        this.applicationsService = applicationsService;
        this.ePaymentsService = ePaymentsService;
        this.translationService = translationService;
        this.commonNomenclaturesService = commonNomenclaturesService;
        this.egovService = egovService;
        this.isForeignPerson = userService.User?.egnLnc?.identifierType === IdentifierTypeEnum.FORID;
        this.onChanged = (value: NomenclatureDTO<number>) => { return; };
        this.onTouched = (value: NomenclatureDTO<number>) => { return; };

        if (this.ngControl !== null && this.ngControl !== undefined) {
            this.ngControl.valueAccessor = this;
        }

        this.paymentTypeControl = new FormControl(null, Validators.required);

        this.performOnlinePaymentFormGroup = new FormGroup({
            paymentSummaryControl: new FormControl(null),
            paymentTypeControl: this.paymentTypeControl
        });
    }

    public setData(data: OnlinePaymentDataDialogParams, buttons: DialogWrapperData): void {
        this.applicationId = data.applicationId;
        this.showTariffs = data.showTariffs;
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public cancelButtonClicked(): void {
        this.matDialogRef.close();
    }

    public ngOnInit(): void {
        if (this.ngControl?.control) {
            this.ngControl.control.validator = this.validate.bind(this);
        }

        NomenclatureStore.instance.getNomenclature<number>(
            NomenclatureTypes.OnlinePaymentTypes, this.commonNomenclaturesService.getOnlinePaymentTypes.bind(this.commonNomenclaturesService), false
        ).subscribe({
            next: (types: NomenclatureDTO<number>[]) => {
                const foreignPaymentMethods = ['ePay', 'ePayDirect'];

                this.paymentTypes = types.filter(x => x.isActive === true);

                if (this.isForeignPerson) {
                    this.paymentTypes = this.paymentTypes.filter(x => foreignPaymentMethods.some(y => y === x.code));
                }
                else {
                    this.paymentTypes = this.paymentTypes.filter(x => !foreignPaymentMethods.some(y => y === x.code));
                }

                if (this.removeOfflinePaymentOptions === true) {
                    this.paymentTypes = this.paymentTypes.filter(x => x.code !== 'PayEGovBank');
                }
            }
        });

        this.applicationsService.getApplicationPaymentSummary(this.applicationId).subscribe({
            next: (result: PaymentSummaryDTO) => {
                this.paymentRequestNum = result.paymentRequestNum!;
                this.performOnlinePaymentFormGroup.get('paymentSummaryControl')!.setValue(result);
            }
        });
    }

    public ngAfterViewInit(): void {
        this.performOnlinePaymentFormGroup.valueChanges.subscribe({
            next: () => {
                this.onChanged(this.getValue());
            }
        });

        this.performOnlinePaymentFormGroup.controls!.paymentTypeControl.valueChanges.subscribe({
            next: (paymentType: NomenclatureDTO<number>) => {
                if (paymentType?.code === PaymentTypesEnum[PaymentTypesEnum.PayEGovBank]) {
                    this.showEGovBankPaymentButton = true;
                }
                else {
                    this.showEGovBankPaymentButton = false;
                }

                const paymentTypeCode = PaymentTypesEnum[paymentType?.code as keyof typeof PaymentTypesEnum];

                switch (paymentTypeCode) {
                    case PaymentTypesEnum.ePay: {
                        this.showEGovBankPaymentButton = false;
                        this.showEGovEPOSPaymentButton = false;
                        this.showEGovEPayPaymentButton = false;
                        this.showEPayPaymentButton = true;
                        this.showEPayDirectPaymentButton = false;

                    } break;
                    case PaymentTypesEnum.ePayDirect: {
                        this.showEGovBankPaymentButton = false;
                        this.showEGovEPOSPaymentButton = false;
                        this.showEGovEPayPaymentButton = false;
                        this.showEPayPaymentButton = false;
                        this.showEPayDirectPaymentButton = true;
                    } break;
                    case PaymentTypesEnum.PayEGovBank: {
                        this.showEGovBankPaymentButton = true;
                        this.showEGovEPOSPaymentButton = false;
                        this.showEGovEPayPaymentButton = false;
                        this.showEPayPaymentButton = false;
                        this.showEPayDirectPaymentButton = false;
                    } break;
                    case PaymentTypesEnum.PayEGovePayBG: {
                        this.showEGovBankPaymentButton = false;
                        this.showEGovEPOSPaymentButton = false;
                        this.showEGovEPayPaymentButton = true;
                        this.showEPayPaymentButton = false;
                        this.showEPayDirectPaymentButton = false;
                    } break;
                    case PaymentTypesEnum.PayEGovePOS: {
                        this.showEGovBankPaymentButton = false;
                        this.showEGovEPOSPaymentButton = true;
                        this.showEGovEPayPaymentButton = false;
                        this.showEPayPaymentButton = false;
                        this.showEPayDirectPaymentButton = false;
                    } break;
                    default: {
                        this.showEGovBankPaymentButton = false;
                        this.showEGovEPOSPaymentButton = false;
                        this.showEGovEPayPaymentButton = false;
                        this.showEPayPaymentButton = false;
                        this.showEPayDirectPaymentButton = false;
                    } break;
                }

                this.onChanged(paymentType);
            }
        });
    }

    public ngDoCheck(): void {
        if (this.ngControl?.control?.touched) {
            this.performOnlinePaymentFormGroup.markAllAsTouched();
        }
    }

    public writeValue(value: NomenclatureDTO<number>): void {
        setTimeout(() => {
            if (value !== null && value !== undefined) {
                this.performOnlinePaymentFormGroup.get('paymentTypeControl')!.setValue(this.paymentTypes.find(x => x.value === value.value));
            }
        });
    }

    public registerOnChange(fn: (value: NomenclatureDTO<number>) => void): void {
        this.onChanged = fn;
    }

    public registerOnTouched(fn: (value: NomenclatureDTO<number>) => void): void {
        this.onTouched = fn;
    }

    public setDisabledState(isDisabled: boolean): void {
        if (isDisabled) {
            this.performOnlinePaymentFormGroup.disable();
        }
        else {
            this.performOnlinePaymentFormGroup.enable();
        }
    }

    public validate(control: AbstractControl): ValidationErrors | null {
        if (this.performOnlinePaymentFormGroup.valid) {
            return null;
        }

        const errors: ValidationErrors = {};
        Object.keys(this.performOnlinePaymentFormGroup.controls).forEach((key: string) => {
            const controlErrors: ValidationErrors | null = this.performOnlinePaymentFormGroup.controls[key].errors;
            if (controlErrors !== null) {
                errors[key] = controlErrors;
            }
        });
        return errors;
    }

    public eGovBankPaymentBtnClicked(): void {
        this.ePaymentsService!.initiateEGovBankPayment(this.paymentRequestNum).subscribe({
            next: (result: string) => {
                this.applicationsService.getApplicationPaymentSummary(this.applicationId).subscribe({
                    next: (result: PaymentSummaryDTO) => {
                        this.paymentRequestNum = result.paymentRequestNum!;
                    }
                });

                this.openOfflinePaymentDataDialog(result);
            }
        });
    }

    public onEgovPaymentButtonClicked(): void {
        const type: NomenclatureDTO<number> = this.performOnlinePaymentFormGroup.get('paymentTypeControl')!.value;

        if (type) {
            this.generatePayment(type);
        }
    }

    private getValue(): NomenclatureDTO<number> {
        return this.performOnlinePaymentFormGroup.get('paymentTypeControl')!.value
    }

    private generatePayment(paymentType: NomenclatureDTO<number>): void {
        const paymentTypeCode = PaymentTypesEnum[paymentType.code as keyof typeof PaymentTypesEnum];

        switch (paymentTypeCode) {
            case PaymentTypesEnum.ePay: {
                this.ePaymentsService.initiateEPayBGPayment(this.paymentRequestNum).subscribe((result: GeneratedPaymentModel) => {
                    this.applicationsService.getApplicationPaymentSummary(this.applicationId).subscribe({
                        next: (result: PaymentSummaryDTO) => {
                            this.paymentRequestNum = result.paymentRequestNum!;
                        }
                    });
                });
            } break;
            case PaymentTypesEnum.ePayDirect: {
                this.ePaymentsService.initiateEPayDirectPayment(this.paymentRequestNum).subscribe((result: GeneratedPaymentModel) => {
                    this.applicationsService.getApplicationPaymentSummary(this.applicationId).subscribe({
                        next: (result: PaymentSummaryDTO) => {
                            this.paymentRequestNum = result.paymentRequestNum!;
                        }
                    });
                });
            } break;
            case PaymentTypesEnum.PayEGovePayBG: {
                this.ePaymentsService!.initiateEGovEPayBGPayment(this.paymentRequestNum).subscribe((result: FormDataModel) => {
                    this.applicationsService.getApplicationPaymentSummary(this.applicationId).subscribe({
                        next: (summary: PaymentSummaryDTO) => {
                            this.paymentRequestNum = summary.paymentRequestNum!;
                            this.egovService.onPaymentGenerated.next(result);
                        }
                    });
                });
            } break;
            case PaymentTypesEnum.PayEGovePOS: {
                this.ePaymentsService!.initiateEGovEPOSPayment(this.paymentRequestNum).subscribe((result: FormDataModel) => {
                    this.applicationsService.getApplicationPaymentSummary(this.applicationId).subscribe({
                        next: (summary: PaymentSummaryDTO) => {
                            this.paymentRequestNum = summary.paymentRequestNum!;
                            this.egovService.onPaymentGenerated.next(result);
                        }
                    });
                });
            } break;
        }
    }

    private openOfflinePaymentDataDialog(paymentAccessCode: string): void {
        this.egovOfflinePaymentDataDialog.openWithTwoButtons({
            TCtor: EGovOfflinePaymenDataComponent,
            title: this.translationService.getValue('online-payment-data.egov-offile-payment-data-title'),
            translteService: this.translationService,
            componentData: new EGovOfflinePaymentDataDialogParams({ referenceNumber: paymentAccessCode }),
            headerCancelButton: {
                cancelBtnClicked: this.closeOfflinePaymentDataDialogBtnClicked.bind(this)
            },
            saveBtn: {
                id: 'save',
                color: 'accent',
                translateValue: this.translationService.getValue('online-payment-data.to-payment-page')
            }
        }, '700px').subscribe({
            next: (href: string | undefined) => {
                if (href) {
                    this.egovOfflinePaymentDataDialogClosed.emit();
                    if (this.matDialogRef !== null && this.matDialogRef !== undefined) {
                        this.matDialogRef.close();
                    }

                    window.open(href, '_self');
                }
            }
        });
    }

    private closeOfflinePaymentDataDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }
}