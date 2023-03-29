import { StepperSelectionEvent } from '@angular/cdk/stepper';
import { Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { MatStepper } from '@angular/material/stepper';
import { HttpErrorResponse } from '@angular/common/http';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { LogBookPageDocumentTypesEnum } from '../../enums/log-book-page-document-types.enum';
import { DialogWrapperComponent } from '@app/shared/components/dialog-wrapper/dialog-wrapper.component';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { ICatchesAndSalesService } from '@app/interfaces/common-app/catches-and-sales.interface';
import { EditFirstSaleLogBookPageComponent } from '../edit-first-sale-log-book/edit-first-sale-log-book-page.component';
import { EditAdmissionLogBookPageComponent } from '../edit-admission-log-book/edit-admission-log-book-page.component';
import { EditTransportationLogBookPageComponent } from '../edit-transporation-log-book/edit-transportation-log-book-page.component';
import { AddShipPageDocumentDialogParamsModel } from '../../models/add-ship-page-document-dialog-params.model';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { CatchesAndSalesDialogParamsModel } from '../../models/catches-and-sales-dialog-params.model';
import { FirstSaleLogBookPageEditDTO } from '@app/models/generated/dtos/FirstSaleLogBookPageEditDTO';
import { AdmissionLogBookPageEditDTO } from '@app/models/generated/dtos/AdmissionLogBookPageEditDTO';
import { TransportationLogBookPageEditDTO } from '@app/models/generated/dtos/TransportationLogBookPageEditDTO';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { BasicLogBookPageDocumentDataDTO } from '@app/models/generated/dtos/BasicLogBookPageDocumentDataDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { LogBookPagePersonTypesEnum } from '@app/enums/log-book-page-person-types.enum';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { LogBookTypesEnum } from '@app/enums/log-book-types.enum';
import { BasicLogBookPageDocumentParameters } from './models/basic-log-book-page-document-parameters.model';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { LogBookNomenclatureDTO } from '@app/models/generated/dtos/LogBookNomenclatureDTO';
import { IS_PUBLIC_APP } from '@app/shared/modules/application.modules';
import { MatSnackBar } from '@angular/material/snack-bar';
import { RequestProperties } from '@app/shared/services/request-properties';

@Component({
    selector: 'add-ship-page-document-wizard',
    templateUrl: './add-ship-page-document-wizard.component.html'
})
export class AddShipPageDocumentWizardComponent implements OnInit, IDialogComponent {
    public readonly documentTypesEnum: typeof LogBookPageDocumentTypesEnum = LogBookPageDocumentTypesEnum;
    public readonly logBookPagePersonTypesEnum: typeof LogBookPagePersonTypesEnum = LogBookPagePersonTypesEnum;

    public preliminaryDataFormGroup!: FormGroup;
    public confirmLogBookAndOwnerFormGroup!: FormGroup;
    public confirmationDataFormGroup!: FormGroup;

    public documentType!: LogBookPageDocumentTypesEnum;
    public ownerType: NomenclatureDTO<LogBookPagePersonTypesEnum> | undefined;
    public logBookType: LogBookTypesEnum | undefined;

    public registeredBuyers: NomenclatureDTO<number>[] = [];
    public logBookOwnerTypes: NomenclatureDTO<LogBookPagePersonTypesEnum>[] = [];
    public possibleLogBooks: LogBookNomenclatureDTO[] = [];

    public pageAlreadySubmittedOtherLogbook: string = '';

    public service!: ICatchesAndSalesService;

    private shipLogBookPageId!: number;
    private logBookPageDocumentData: BasicLogBookPageDocumentDataDTO | undefined;
    private dialogRef: MatDialogRef<DialogWrapperComponent<IDialogComponent>> | undefined;

    private readonly translationService: FuseTranslationLoaderService;
    private readonly firstSaleLogBookPageDialog: TLMatDialog<EditFirstSaleLogBookPageComponent>;
    private readonly admissionLogBookPageDialog: TLMatDialog<EditAdmissionLogBookPageComponent>;
    private readonly transportationLogBookPageDialog: TLMatDialog<EditTransportationLogBookPageComponent>;
    private readonly snackbar: MatSnackBar;

    @ViewChild('stepper')
    private stepper!: MatStepper;

    public constructor(
        translate: FuseTranslationLoaderService,
        firstSaleLogBookPageDialog: TLMatDialog<EditFirstSaleLogBookPageComponent>,
        admissionLogBookPageDialog: TLMatDialog<EditAdmissionLogBookPageComponent>,
        transportationLogBookPageDialog: TLMatDialog<EditTransportationLogBookPageComponent>,
        snackbar: MatSnackBar
    ) {
        this.translationService = translate;
        this.firstSaleLogBookPageDialog = firstSaleLogBookPageDialog;
        this.admissionLogBookPageDialog = admissionLogBookPageDialog;
        this.transportationLogBookPageDialog = transportationLogBookPageDialog;
        this.snackbar = snackbar;

        this.logBookOwnerTypes = [
            new NomenclatureDTO({
                value: LogBookPagePersonTypesEnum.Person,
                displayName: this.translationService.getValue('catches-and-sales.log-book-page-person-person-type'),
                isActive: true
            }),
            new NomenclatureDTO({
                value: LogBookPagePersonTypesEnum.LegalPerson,
                displayName: this.translationService.getValue('catches-and-sales.log-book-page-person-person-legal-type'),
                isActive: true
            }),
            new NomenclatureDTO({
                value: LogBookPagePersonTypesEnum.RegisteredBuyer,
                displayName: this.translationService.getValue('catches-and-sales.log-book-page-person-registered-buyer-type'),
                isActive: true
            })
        ];

        this.buildForm();
    }

    public async ngOnInit(): Promise<void> {
        this.registeredBuyers = await this.service.getRegisteredBuyersNomenclature().toPromise();

        this.setDocumentTypeControl();
    }

    public setData(data: AddShipPageDocumentDialogParamsModel, buttons: DialogWrapperData): void {
        this.documentType = data.documentType;
        this.shipLogBookPageId = data.shipLogBookPageId;
        this.service = data.service;

        this.dialogRef = buttons.dialogRef;

        this.setLogBookType();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.preliminaryDataFormGroup.markAllAsTouched();
        this.confirmLogBookAndOwnerFormGroup.markAllAsTouched();
        this.confirmationDataFormGroup.markAllAsTouched();

        if (this.preliminaryDataFormGroup.valid && this.confirmLogBookAndOwnerFormGroup.valid && this.confirmationDataFormGroup.valid) {
            switch (this.documentType) {
                case LogBookPageDocumentTypesEnum.FirstSaleDocument: {
                    this.openAddFirstSaleLogBookPageDialog();
                } break;
                case LogBookPageDocumentTypesEnum.AdmissionDocument: {
                    this.openAddAdmissionLogBookPageDialog();
                } break;
                case LogBookPageDocumentTypesEnum.TransportationDocument: {
                    this.openTransportationLogBookPageDialog();
                } break;
                default: {
                    dialogClose();
                } break;
            }
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public selectedStepChanged(stepperSelectionEvent: StepperSelectionEvent): void {
        if (stepperSelectionEvent.previouslySelectedIndex === 0 && stepperSelectionEvent.selectedIndex === 1) {
            if (this.preliminaryDataFormGroup.valid) {
                const documentNumber: number = Number(this.preliminaryDataFormGroup.get('documentNumberControl')!.value);
                const documentType: LogBookPageDocumentTypesEnum = this.documentType;

                this.service.getLogBookPageDocumentOwnerData(documentNumber, documentType).subscribe({
                    next: (result: LogBookNomenclatureDTO[]) => {
                        this.possibleLogBooks = result ?? [];

                        if (this.possibleLogBooks.length === 1) {
                            this.confirmLogBookAndOwnerFormGroup.get('possibleLogBookControl')!.setValue(this.possibleLogBooks[0]);
                        }
                    },
                    error: (errorResponse: HttpErrorResponse) => {
                        if ((errorResponse.error as ErrorModel)?.code === ErrorCode.LogBookNotFound) {
                            const message: string = this.translationService.getValue('catches-and-sales.log-book-page-person-cannot-find-log-book-error');
                            this.snackbar.open(message, undefined, {
                                duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                                panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
                            });

                            this.stepper.previous();
                        }
                    }
                });
            }
            else {
                setTimeout(() => {
                    this.stepper.previous();
                });
            }
        }
        else if (stepperSelectionEvent.previouslySelectedIndex === 1 && stepperSelectionEvent.selectedIndex === 2) {
            if (this.preliminaryDataFormGroup.valid && this.confirmLogBookAndOwnerFormGroup.valid) {
                const parameters: BasicLogBookPageDocumentParameters = new BasicLogBookPageDocumentParameters({
                    documentType: this.documentType,
                    shipLogBookPageId: this.shipLogBookPageId,
                    documentNumber: Number(this.preliminaryDataFormGroup.get('documentNumberControl')!.value),
                    logBookId: this.confirmLogBookAndOwnerFormGroup.get('possibleLogBookControl')!.value.value
                });

                this.service.getLogBookPageDocumentData(parameters).subscribe({
                    next: (result: BasicLogBookPageDocumentDataDTO) => {
                        this.logBookPageDocumentData = result;
                        this.fillConfirmationForm();
                    },
                    error: (errorResponse: HttpErrorResponse) => {
                        const error = errorResponse?.error as ErrorModel;
                        if (error?.code === ErrorCode.PageNumberNotInLogBookLicense) {
                            this.confirmLogBookAndOwnerFormGroup.setErrors({ 'pageNumberNotInLogBookLicense': true });
                            this.stepper.previous();
                        }
                        else if (error?.code === ErrorCode.LogBookPageAlreadySubmitted) {
                            this.confirmLogBookAndOwnerFormGroup.setErrors({ 'pageNumberAlreadySubmitted': true });
                            this.stepper.previous();
                        }
                        else if (error?.code === ErrorCode.LogBookPageAlreadySubmittedOtherLogBook) {
                            this.confirmLogBookAndOwnerFormGroup.setErrors({ 'pageNumberAlreadySubmittedOtherLogBook': true });
                            this.pageAlreadySubmittedOtherLogbook = error.messages[0];
                            this.stepper.previous();
                        }
                        else if (error?.code === ErrorCode.LogBookNotFound) {
                            const message: string = this.translationService.getValue('catches-and-sales.log-book-page-person-cannot-find-log-book-error');
                            this.snackbar.open(message, undefined, {
                                duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                                panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
                            });

                            this.stepper.previous();
                        }
                    }
                });
            }
            else {
                setTimeout(() => {
                    this.stepper.previous();
                });
            }
        }
    }

    private setLogBookType(): void {
        switch (this.documentType) {
            case LogBookPageDocumentTypesEnum.FirstSaleDocument: this.logBookType = LogBookTypesEnum.FirstSale; break;
            case LogBookPageDocumentTypesEnum.AdmissionDocument: this.logBookType = LogBookTypesEnum.Admission; break;
            case LogBookPageDocumentTypesEnum.TransportationDocument: this.logBookType = LogBookTypesEnum.Transportation; break;
        }
    }

    private buildForm(): void {
        this.preliminaryDataFormGroup = new FormGroup({
            documentTypeControl: new FormControl(undefined, Validators.required),
            documentNumberControl: new FormControl(undefined, Validators.required)
        });

        this.confirmLogBookAndOwnerFormGroup = new FormGroup({
            possibleLogBookControl: new FormControl(undefined, Validators.required)
        });

        this.confirmationDataFormGroup = new FormGroup({
            documentNumberControl: new FormControl(),
            logBookOwnerTypeControl: new FormControl(),
            sourceDataControl: new FormControl(undefined, Validators.required),
            personDataControl: new FormControl(undefined, Validators.required)
        });

        this.confirmLogBookAndOwnerFormGroup.get('possibleLogBookControl')!.valueChanges.subscribe({
            next: () => {
                this.confirmLogBookAndOwnerFormGroup.setErrors(null);
            }
        });
    }

    private fillConfirmationForm(): void {
        if (this.logBookPageDocumentData !== null && this.logBookPageDocumentData !== undefined) {
            this.ownerType = this.logBookOwnerTypes.find(x => x.value === this.logBookPageDocumentData!.ownerType)!;

            this.confirmationDataFormGroup.get('logBookOwnerTypeControl')!.setValue(this.ownerType);
            this.confirmationDataFormGroup.get('documentNumberControl')!.setValue(this.logBookPageDocumentData.documentNumber);
            this.confirmationDataFormGroup.get('sourceDataControl')!.setValue(this.logBookPageDocumentData.sourceData);
            this.confirmationDataFormGroup.get('personDataControl')!.setValue(this.logBookPageDocumentData.personData);
        }
        else {
            this.confirmationDataFormGroup.reset();
        }
    }

    private openAddFirstSaleLogBookPageDialog(): void {
        const logBook: LogBookNomenclatureDTO = this.confirmLogBookAndOwnerFormGroup.get('possibleLogBookControl')!.value;

        this.firstSaleLogBookPageDialog.openWithTwoButtons({
            title: this.translationService.getValue('catches-and-sales.add-first-sale-log-book-page-dialog-title'),
            TCtor: EditFirstSaleLogBookPageComponent,
            translteService: this.translationService,
            viewMode: false,
            headerCancelButton: {
                cancelBtnClicked: this.closeAddFirstSaleLogBookPageDialogBtnClicked.bind(this)
            },
            componentData: new CatchesAndSalesDialogParamsModel({
                shipPageDocumentData: this.logBookPageDocumentData,
                logBookId: logBook.value!,
                logBookTypeId: logBook.logBookTypeId!,
                service: this.service,
                viewMode: false
            }),
            disableDialogClose: true
        }, '1300px').subscribe({
            next: (result: FirstSaleLogBookPageEditDTO | undefined) => {
                if (result !== undefined) {
                    this.dialogRef!.close(result);
                }
            }
        });
    }

    private openAddAdmissionLogBookPageDialog(): void {
        const logBook: LogBookNomenclatureDTO = this.confirmLogBookAndOwnerFormGroup.get('possibleLogBookControl')!.value;

        this.admissionLogBookPageDialog.openWithTwoButtons({
            title: this.translationService.getValue('catches-and-sales.add-admission-log-book-page-dialog-title'),
            TCtor: EditAdmissionLogBookPageComponent,
            translteService: this.translationService,
            viewMode: false,
            headerCancelButton: {
                cancelBtnClicked: this.closeAddAdmissionLogBookPageDialogBtnClicked.bind(this)
            },
            componentData: new CatchesAndSalesDialogParamsModel({
                shipPageDocumentData: this.logBookPageDocumentData,
                service: this.service,
                logBookId: logBook.value!,
                logBookTypeId: logBook.logBookTypeId!,
                logBookPermitLicenseId: this.confirmLogBookAndOwnerFormGroup.get('possibleLogBookControl')!.value?.logBookPermitLicenseId,
                viewMode: false
            }),
            disableDialogClose: true
        }, '1300px').subscribe({
            next: (result: AdmissionLogBookPageEditDTO | undefined) => {
                if (result !== undefined) {
                    this.dialogRef!.close(result);
                }
            }
        });
    }

    private openTransportationLogBookPageDialog(): void {
        const logBook: LogBookNomenclatureDTO = this.confirmLogBookAndOwnerFormGroup.get('possibleLogBookControl')!.value;

        this.transportationLogBookPageDialog.openWithTwoButtons({
            title: this.translationService.getValue('catches-and-sales.add-transportation-log-book-page-dialog-title'),
            TCtor: EditTransportationLogBookPageComponent,
            translteService: this.translationService,
            viewMode: false,
            headerCancelButton: {
                cancelBtnClicked: this.closeAddTransportationLogBookPageDialogBtnClicked.bind(this)
            },
            componentData: new CatchesAndSalesDialogParamsModel({
                shipPageDocumentData: this.logBookPageDocumentData,
                service: this.service,
                logBookId: logBook.value!,
                logBookTypeId: logBook.logBookTypeId!,
                logBookPermitLicenseId: this.confirmLogBookAndOwnerFormGroup.get('possibleLogBookControl')!.value?.logBookPermitLicenseId,
                viewMode: false
            }),
            disableDialogClose: true
        }, '1300px').subscribe({
            next: (result: TransportationLogBookPageEditDTO | undefined) => {
                if (result !== undefined) {
                    this.dialogRef!.close(result);
                }
            }
        });
    }

    private setDocumentTypeControl(): void {
        let documentTypeValue: string = '';
        switch (this.documentType) {
            case LogBookPageDocumentTypesEnum.FirstSaleDocument:
                documentTypeValue = this.translationService.getValue('catches-and-sales.add-ship-page-document-wizard-first-sale-document-type'); break;
            case LogBookPageDocumentTypesEnum.AdmissionDocument:
                documentTypeValue = this.translationService.getValue('catches-and-sales.add-ship-page-document-wizard-admission-document-type'); break;
            case LogBookPageDocumentTypesEnum.TransportationDocument:
                documentTypeValue = this.translationService.getValue('catches-and-sales.add-ship-page-document-wizard-transportation-document-type'); break;
        }

        this.preliminaryDataFormGroup.get('documentTypeControl')!.setValue(documentTypeValue);
    }

    private closeAddFirstSaleLogBookPageDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private closeAddAdmissionLogBookPageDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private closeAddTransportationLogBookPageDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }
}