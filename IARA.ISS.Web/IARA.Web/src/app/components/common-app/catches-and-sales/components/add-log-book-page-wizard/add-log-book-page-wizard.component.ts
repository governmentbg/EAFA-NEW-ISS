import { StepperSelectionEvent } from '@angular/cdk/stepper';
import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { MatStepper } from '@angular/material/stepper';
import { HttpErrorResponse } from '@angular/common/http';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { LogBookTypesEnum } from '@app/enums/log-book-types.enum';
import { AddLogBookPageDialogParams } from './models/add-log-book-page-wizard-dialog-params.model';
import { DialogWrapperComponent } from '@app/shared/components/dialog-wrapper/dialog-wrapper.component';
import { ICatchesAndSalesService } from '@app/interfaces/common-app/catches-and-sales.interface';
import { CommonLogBookPageDataDTO } from '@app/models/generated/dtos/CommonLogBookPageDataDTO';
import { CommonLogBookPageDataParameters } from './models/common-log-book-page-data-parameteres.model';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { EditFirstSaleLogBookPageComponent } from '../edit-first-sale-log-book/edit-first-sale-log-book-page.component';
import { EditAdmissionLogBookPageComponent } from '../edit-admission-log-book/edit-admission-log-book-page.component';
import { EditTransportationLogBookPageComponent } from '../edit-transporation-log-book/edit-transportation-log-book-page.component';
import { CatchesAndSalesDialogParamsModel } from '../../models/catches-and-sales-dialog-params.model';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { FirstSaleLogBookPageEditDTO } from '@app/models/generated/dtos/FirstSaleLogBookPageEditDTO';
import { AdmissionLogBookPageEditDTO } from '@app/models/generated/dtos/AdmissionLogBookPageEditDTO';
import { TransportationLogBookPageEditDTO } from '@app/models/generated/dtos/TransportationLogBookPageEditDTO';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { LogBookPageStatusesEnum } from '@app/enums/log-book-page-statuses.enum';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { LogBookPageDocumentTypesEnum } from '../../enums/log-book-page-document-types.enum';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';
import { PossibleLogBooksForPageDTO } from '@app/models/generated/dtos/PossibleLogBooksForPageDTO';
import { IS_PUBLIC_APP } from '@app/shared/modules/application.modules';

@Component({
    selector: 'add-log-book-page-wizard',
    templateUrl: './add-log-book-page-wizard.component.html'
})
export class AddLogBookPageWizardComponent implements OnInit, AfterViewInit, IDialogComponent {
    public readonly logBookTypesEnum: typeof LogBookTypesEnum = LogBookTypesEnum;

    public preliminaryDataFormGroup!: FormGroup;
    public confirmationDataFormGroup!: FormGroup;

    public logBookType!: LogBookTypesEnum;
    public logBookId!: number;

    public selectedDocumentType: LogBookTypesEnum | undefined;

    public documentTypes: NomenclatureDTO<LogBookPageDocumentTypesEnum>[] = [];
    public possibleLogBooksForPage: NomenclatureDTO<number>[] = [];

    public originDeclarationTypeSelected: boolean = false;
    public transportationDocumentTypeSelected: boolean = false;
    public admissionDocumentTypeSelected: boolean = false;

    public hasInvalidOriginDeclarationNumber: boolean = false;
    public hasInvalidTransportationDocNumber: boolean = false;
    public hasInvalidAdmissionDocNumber: boolean = false;
    public hasPageNotInLogBookError: boolean = false;
    public hasPageAlreadySubmittedError: boolean = false;
    public hasPageAlreadySubmittedOtherLogBookError: boolean = false;
    public pageAlreadySubmittedOtherLogBook: string = '';

    private translationService: FuseTranslationLoaderService;
    private dialogRef: MatDialogRef<DialogWrapperComponent<IDialogComponent>> | undefined;
    private service!: ICatchesAndSalesService;
    private pageNumber: number | undefined;
    private pageStatus: LogBookPageStatusesEnum | undefined;
    private firstSaleLogBookPageDialog: TLMatDialog<EditFirstSaleLogBookPageComponent>;
    private admissionLogBookPageDialog: TLMatDialog<EditAdmissionLogBookPageComponent>;
    private transportationLogBookPageDialog: TLMatDialog<EditTransportationLogBookPageComponent>;

    @ViewChild('stepper')
    private stepper!: MatStepper;

    public constructor(
        translate: FuseTranslationLoaderService,
        firstSaleLogBookPageDialog: TLMatDialog<EditFirstSaleLogBookPageComponent>,
        admissionLogBookPageDialog: TLMatDialog<EditAdmissionLogBookPageComponent>,
        transportationLogBookPageDialog: TLMatDialog<EditTransportationLogBookPageComponent>
    ) {
        this.translationService = translate;
        this.firstSaleLogBookPageDialog = firstSaleLogBookPageDialog;
        this.admissionLogBookPageDialog = admissionLogBookPageDialog;
        this.transportationLogBookPageDialog = transportationLogBookPageDialog;

        this.buildForm();
    }

    public ngOnInit(): void {
        this.preliminaryDataFormGroup.get('pageNumberControl')!.setValue(this.pageNumber);

        this.documentTypes = [
            new NomenclatureDTO({
                value: LogBookPageDocumentTypesEnum.OriginDeclaration,
                displayName: this.translationService.getValue('catches-and-sales.add-log-book-page-wizard-origin-declaration-type'),
                isActive: true
            }),
            new NomenclatureDTO({
                value: LogBookPageDocumentTypesEnum.TransportationDocument,
                displayName: this.translationService.getValue('catches-and-sales.add-log-book-page-wizard-transportation-document-type'),
                isActive: true
            }),
            new NomenclatureDTO({
                value: LogBookPageDocumentTypesEnum.AdmissionDocument,
                displayName: this.translationService.getValue('catches-and-sales.add-log-book-page-wizard-admission-document-type'),
                isActive: true
            })
        ];
    }

    public ngAfterViewInit(): void {
        this.preliminaryDataFormGroup.get('originDeclarationNumberControl')!.valueChanges.subscribe({
            next: () => {
                this.hasInvalidOriginDeclarationNumber = false;
                this.preliminaryDataFormGroup.updateValueAndValidity({ emitEvent: false });
            }
        });

        if (this.logBookType === LogBookTypesEnum.FirstSale || this.logBookType === LogBookTypesEnum.Admission) {
            this.preliminaryDataFormGroup.get('documentTypeControl')!.setValidators(Validators.required);
            this.preliminaryDataFormGroup.get('documentTypeControl')!.markAsPending();

            this.preliminaryDataFormGroup.get('documentTypeControl')!.valueChanges.subscribe({
                next: (documentType: NomenclatureDTO<LogBookPageDocumentTypesEnum> | undefined) => {
                    this.confirmationDataFormGroup.reset();
                    this.setDocumentTypesValidators(documentType);

                    if (documentType !== null && documentType !== undefined) {
                        switch (documentType?.value) {
                            case LogBookPageDocumentTypesEnum.AdmissionDocument:
                                this.selectedDocumentType = LogBookTypesEnum.Admission; break;
                            case LogBookPageDocumentTypesEnum.OriginDeclaration:
                                this.selectedDocumentType = LogBookTypesEnum.Ship; break;
                            case LogBookPageDocumentTypesEnum.TransportationDocument:
                                this.selectedDocumentType = LogBookTypesEnum.Transportation; break;
                        }
                    }
                    else {
                        this.selectedDocumentType = undefined;
                    }
                }
            });

            this.preliminaryDataFormGroup.get('transportationDocumentNumberControl')!.valueChanges.subscribe({
                next: () => {
                    this.hasInvalidTransportationDocNumber = false;
                    this.preliminaryDataFormGroup.updateValueAndValidity({ emitEvent: false });
                }
            });

            if (this.logBookType === LogBookTypesEnum.Admission) {
                this.documentTypes = this.documentTypes.filter(x => x.value !== LogBookPageDocumentTypesEnum.AdmissionDocument);
            }
            else {
                this.preliminaryDataFormGroup.get('admissionDocumentNumberControl')!.valueChanges.subscribe({
                    next: () => {
                        this.hasInvalidAdmissionDocNumber = false;
                        this.preliminaryDataFormGroup.updateValueAndValidity({ emitEvent: false });
                    }
                });
            }
        }
        else if (this.logBookType === LogBookTypesEnum.Transportation) {
            this.selectedDocumentType = LogBookTypesEnum.Ship;
            this.preliminaryDataFormGroup.get('originDeclarationNumberControl')!.setValidators(Validators.required);
            this.preliminaryDataFormGroup.get('originDeclarationNumberControl')!.markAsPending();

            this.preliminaryDataFormGroup.get('isImportNotByShipControl')!.valueChanges.subscribe({
                next: (value: boolean | undefined) => {
                    if (value === true) {
                        this.preliminaryDataFormGroup.get('placeOfImportControl')!.setValidators([Validators.maxLength(500), Validators.required]);
                        this.preliminaryDataFormGroup.get('placeOfImportControl')!.markAsPending();

                        this.preliminaryDataFormGroup.get('originDeclarationNumberControl')!.clearValidators();
                        this.preliminaryDataFormGroup.get('originDeclarationNumberControl')!.reset();

                        this.selectedDocumentType = undefined;
                    }
                    else {
                        this.preliminaryDataFormGroup.get('placeOfImportControl')!.clearValidators();
                        this.preliminaryDataFormGroup.get('placeOfImportControl')!.reset();

                        this.preliminaryDataFormGroup.get('originDeclarationNumberControl')!.setValidators(Validators.required);
                        this.preliminaryDataFormGroup.get('originDeclarationNumberControl')!.markAsPending();

                        this.selectedDocumentType = LogBookTypesEnum.Ship;
                    }
                }
            });
        }
    }

    public setData(data: AddLogBookPageDialogParams, buttons: DialogWrapperData): void {
        this.logBookType = data.logBookType;
        this.service = data.service;
        this.logBookId = data.logBookId;
        this.pageNumber = data.pageNumber;
        this.pageStatus = data.pageStatus;

        this.dialogRef = buttons.dialogRef;
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.preliminaryDataFormGroup.markAllAsTouched();
        this.preliminaryDataFormGroup.updateValueAndValidity();

        this.confirmationDataFormGroup.markAllAsTouched();
        this.confirmationDataFormGroup.updateValueAndValidity();

        if (this.preliminaryDataFormGroup.valid && this.confirmationDataFormGroup.valid) {
            switch (this.logBookType) {
                case LogBookTypesEnum.FirstSale: {
                    this.openAddFirstSaleLogBookPageDialog();
                } break;
                case LogBookTypesEnum.Admission: {
                    this.openAddAdmissionLogBookPageDialog();
                } break;
                case LogBookTypesEnum.Transportation: {
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
                if (this.logBookType === LogBookTypesEnum.Transportation
                    && this.preliminaryDataFormGroup.get('isImportNotByShipControl')!.value === true) {
                    this.confirmationDataFormGroup.get('commonLogBookPageDataControl')!.setValue(
                        new CommonLogBookPageDataDTO({
                            isImportNotByShip: this.preliminaryDataFormGroup.get('isImportNotByShipControl')!.value,
                            placeOfImport: this.preliminaryDataFormGroup.get('placeOfImportControl')!.value
                        })
                    );
                    setTimeout(() => {
                        this.stepper.next();
                    });
                }
                else {
                    this.service.getCommonLogBookPageData(this.getCommonLogBookPageParameters()).subscribe({
                        next: (result: CommonLogBookPageDataDTO) => {
                            if (result.possibleLogBooks !== null && result.possibleLogBooks !== undefined) {
                                this.buildPossibleLogBooksCollection(result.possibleLogBooks);

                                if (result.possibleLogBooks.length === 1) {
                                    this.confirmationDataFormGroup.get('possibleLogBooksForPageControl')!.setValue(this.possibleLogBooksForPage[0]);
                                }
                            }
                            else {
                                this.possibleLogBooksForPage = [];
                            }

                            this.confirmationDataFormGroup.get('commonLogBookPageDataControl')!.setValue(result);

                            setTimeout(() => {
                                this.stepper.next();
                            });
                        },
                        error: (errorResponse: HttpErrorResponse) => {
                            this.stepper.previous();
                            const error: ErrorModel | undefined = errorResponse.error as ErrorModel;
                            if (error !== null && error !== undefined) {
                                switch (error.code) {
                                    case ErrorCode.PageNumberNotInLogbook: {
                                        this.hasPageNotInLogBookError = true;
                                        this.preliminaryDataFormGroup.updateValueAndValidity({ emitEvent: false });
                                        this.preliminaryDataFormGroup.get('pageNumberControl')!.updateValueAndValidity({ emitEvent: false });
                                        this.preliminaryDataFormGroup.get('pageNumberControl')!.markAsTouched();
                                    } break;
                                    case ErrorCode.LogBookPageAlreadySubmitted: {
                                        this.hasPageAlreadySubmittedError = true;
                                        this.preliminaryDataFormGroup.get('pageNumberControl')!.updateValueAndValidity({ emitEvent: false });
                                        this.preliminaryDataFormGroup.get('pageNumberControl')!.markAsTouched();
                                    } break;
                                    case ErrorCode.LogBookPageAlreadySubmittedOtherLogBook: {
                                        this.hasPageAlreadySubmittedOtherLogBookError = true;
                                        this.pageAlreadySubmittedOtherLogBook = error.messages[0];
                                        this.preliminaryDataFormGroup.get('pageNumberControl')!.updateValueAndValidity({ emitEvent: false });
                                        this.preliminaryDataFormGroup.get('pageNumberControl')!.markAsTouched();
                                    } break;
                                    case ErrorCode.InvalidOriginDeclarationNumber: {
                                        this.hasInvalidOriginDeclarationNumber = true;
                                        this.preliminaryDataFormGroup.updateValueAndValidity({ emitEvent: false });
                                    } break;
                                    case ErrorCode.InvalidTransportationDocNumber: {
                                        this.hasInvalidTransportationDocNumber = true;
                                        this.preliminaryDataFormGroup.updateValueAndValidity({ emitEvent: false });
                                    } break;
                                    case ErrorCode.InvalidAdmissionDocNumber: {
                                        this.hasInvalidAdmissionDocNumber = true;
                                        this.preliminaryDataFormGroup.updateValueAndValidity({ emitEvent: false });
                                    } break;
                                }
                            }
                        }
                    });
                }
            }
            else {
                setTimeout(() => {
                    this.stepper.previous();
                });
            }
        }
    }

    private buildPossibleLogBooksCollection(possibleLogBooks: PossibleLogBooksForPageDTO[]): void {
        this.possibleLogBooksForPage = [];

        for (const possibleLogBook of possibleLogBooks) {
            const element: NomenclatureDTO<number> = new NomenclatureDTO({
                value: possibleLogBook.logBookPermitLicenseId,
                isActive: true
            });

            if (possibleLogBook.buyerNumber !== null && possibleLogBook.buyerNumber !== undefined) {
                const buyerLabel: string = this.translationService.getValue('catches-and-sales.add-log-book-page-wizard-page-registered-buyer');
                const buyerRegNumberLabel: string = this.translationService.getValue('catches-and-sales.add-log-book-page-wizard-page-registered-buyer-number');
                const buyerUrorrLabel: string = this.translationService.getValue('catches-and-sales.add-log-book-page-wizard-page-registered-buyer-urorr');
                element.displayName = `${possibleLogBook.logBookNumber}; ${buyerLabel}: ${possibleLogBook.buyerName}`;
                element.description = `${buyerRegNumberLabel}: ${possibleLogBook.buyerNumber}
                                       ${buyerUrorrLabel}: ${possibleLogBook.buyerUrorr}`;
            }
            else if (possibleLogBook.logBookPermitLicenseId !== null && possibleLogBook.logBookPermitLicenseId !== undefined) {
                const permitLicenseLabel: string = this.translationService.getValue('catches-and-sales.add-log-book-page-wizard-page-permit-license-number');
                const qualifiedFisherLabel: string = this.translationService.getValue('catches-and-sales.add-log-book-page-wizard-page-qualified-fisher');
                const lobBookOwner: string = this.translationService.getValue('catches-and-sales.add-log-book-page-wizard-page-lob-book-owner');
                element.displayName = `${possibleLogBook.logBookNumber}; ${permitLicenseLabel}: ${possibleLogBook.permitLicenseNumber}`;
                element.description = `${qualifiedFisherLabel}: ${possibleLogBook.qualifiedFisherName} (${possibleLogBook.qualifiedFisherNumber})
                                       ${lobBookOwner}: ${possibleLogBook.legalName ?? possibleLogBook.personName}`;
            }

            this.possibleLogBooksForPage.push(element);
        }
    }

    public getControlErrorLabelText(controlName: string, error: unknown, errorCode: string): TLError | undefined {
        if (errorCode === 'pageNotInLogBook') {
            return new TLError({
                type: 'error',
                text: this.translationService.getValue('catches-and-sales.add-log-book-page-wizard-page-not-in-log-book-error')
            });
        }
        else if (errorCode === 'pageAlreadySubmitted') {
            return new TLError({
                type: 'error',
                text: this.translationService.getValue('catches-and-sales.add-log-book-page-wizard-page-already-submitted-error')
            });
        }
        else if (errorCode === 'pageAlreadySubmittedOtherLogBook') {
            return new TLError({
                type: 'error',
                text: `${this.translationService.getValue('catches-and-sales.add-log-book-page-wizard-page-already-submitted-other-logbook-error')}: ${this.pageAlreadySubmittedOtherLogBook}`
            });
        }
        else {
            return undefined;
        }
    }

    private buildForm(): void {
        this.preliminaryDataFormGroup = new FormGroup({
            pageNumberControl: new FormControl(undefined, [Validators.required, TLValidators.number(0), this.pageNumberValidator()]),
            isImportNotByShipControl: new FormControl(false),
            documentTypeControl: new FormControl(null),
            placeOfImportControl: new FormControl(null, Validators.maxLength(500)),
            originDeclarationNumberControl: new FormControl(),
            transportationDocumentNumberControl: new FormControl(),
            admissionDocumentNumberControl: new FormControl()
        }, this.documentNumberExistanceValidator());

        this.confirmationDataFormGroup = new FormGroup({
            possibleLogBooksForPageControl: new FormControl(undefined, Validators.required),
            commonLogBookPageDataControl: new FormControl()
        });

        this.preliminaryDataFormGroup.get('pageNumberControl')!.valueChanges.subscribe({
            next: () => {
                this.hasPageNotInLogBookError = false;
                this.hasPageAlreadySubmittedError = false;
                this.hasPageAlreadySubmittedOtherLogBookError = false;
                this.pageAlreadySubmittedOtherLogBook = '';
            }
        });
    }

    private openAddFirstSaleLogBookPageDialog(): void {
        this.firstSaleLogBookPageDialog.openWithTwoButtons({
            title: this.translationService.getValue('catches-and-sales.add-first-sale-log-book-page-dialog-title'),
            TCtor: EditFirstSaleLogBookPageComponent,
            translteService: this.translationService,
            viewMode: false,
            headerCancelButton: {
                cancelBtnClicked: this.closeAddFirstSaleLogBookPageDialogBtnClicked.bind(this)
            },
            componentData: new CatchesAndSalesDialogParamsModel({
                logBookId: this.logBookId,
                service: this.service,
                pageNumber: Number(this.preliminaryDataFormGroup.get('pageNumberControl')!.value),
                pageStatus: this.pageStatus,
                viewMode: false,
                commonData: this.confirmationDataFormGroup.get('commonLogBookPageDataControl')!.value
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
        this.admissionLogBookPageDialog.openWithTwoButtons({
            title: this.translationService.getValue('catches-and-sales.add-admission-log-book-page-dialog-title'),
            TCtor: EditAdmissionLogBookPageComponent,
            translteService: this.translationService,
            viewMode: false,
            headerCancelButton: {
                cancelBtnClicked: this.closeAddAdmissionLogBookPageDialogBtnClicked.bind(this)
            },
            componentData: new CatchesAndSalesDialogParamsModel({
                logBookId: this.logBookId,
                service: this.service,
                pageNumber: Number(this.preliminaryDataFormGroup.get('pageNumberControl')!.value),
                pageStatus: this.pageStatus,
                viewMode: false,
                logBookPermitLicenseId: this.confirmationDataFormGroup.get('possibleLogBooksForPageControl')!.value!.value,
                commonData: this.confirmationDataFormGroup.get('commonLogBookPageDataControl')!.value
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
        this.transportationLogBookPageDialog.openWithTwoButtons({
            title: this.translationService.getValue('catches-and-sales.add-transportation-log-book-page-dialog-title'),
            TCtor: EditTransportationLogBookPageComponent,
            translteService: this.translationService,
            viewMode: false,
            headerCancelButton: {
                cancelBtnClicked: this.closeAddTransportationLogBookPageDialogBtnClicked.bind(this)
            },
            componentData: new CatchesAndSalesDialogParamsModel({
                logBookId: this.logBookId,
                service: this.service,
                pageNumber: Number(this.preliminaryDataFormGroup.get('pageNumberControl')!.value),
                pageStatus: this.pageStatus,
                viewMode: false,
                logBookPermitLicenseId: this.confirmationDataFormGroup.get('possibleLogBooksForPageControl')!.value!.value,
                commonData: this.confirmationDataFormGroup.get('commonLogBookPageDataControl')!.value
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

    private getCommonLogBookPageParameters(): CommonLogBookPageDataParameters {
        const parameters: CommonLogBookPageDataParameters = new CommonLogBookPageDataParameters({
            logBookType: this.logBookType,
            logBookId: this.logBookId
        });

        if (this.preliminaryDataFormGroup.get('pageNumberControl')!.value !== null && this.preliminaryDataFormGroup.get('pageNumberControl')!.value !== undefined) {
            parameters.pageNumberToAdd = Number(this.preliminaryDataFormGroup.get('pageNumberControl')!.value);

            if (isNaN(parameters.pageNumberToAdd)) {
                parameters.pageNumberToAdd = undefined;
            }
        }

        switch (this.logBookType) {
            case LogBookTypesEnum.FirstSale: {
                const documentType: LogBookPageDocumentTypesEnum = this.preliminaryDataFormGroup.get('documentTypeControl')!.value!.value;
                if (documentType === LogBookPageDocumentTypesEnum.OriginDeclaration) {
                    parameters.originDeclarationNumber = this.preliminaryDataFormGroup.get('originDeclarationNumberControl')!.value;
                }
                else if (documentType === LogBookPageDocumentTypesEnum.TransportationDocument) {
                    parameters.transportationDocumentNumber = Number(this.preliminaryDataFormGroup.get('transportationDocumentNumberControl')!.value);
                }
                else if (documentType === LogBookPageDocumentTypesEnum.AdmissionDocument) {
                    parameters.admissionDocumentNumber = Number(this.preliminaryDataFormGroup.get('admissionDocumentNumberControl')!.value);
                }
            } break;
            case LogBookTypesEnum.Admission: {
                const documentType: LogBookPageDocumentTypesEnum = this.preliminaryDataFormGroup.get('documentTypeControl')!.value!.value;
                if (documentType === LogBookPageDocumentTypesEnum.OriginDeclaration) {
                    parameters.originDeclarationNumber = this.preliminaryDataFormGroup.get('originDeclarationNumberControl')!.value;
                }
                else if (documentType === LogBookPageDocumentTypesEnum.TransportationDocument) {
                    parameters.transportationDocumentNumber = Number(this.preliminaryDataFormGroup.get('transportationDocumentNumberControl')!.value);
                }
            } break;
            case LogBookTypesEnum.Transportation: {
                parameters.originDeclarationNumber = this.preliminaryDataFormGroup.get('originDeclarationNumberControl')!.value;
            }
        }

        return parameters;
    }

    private setDocumentTypesValidators(documentType: NomenclatureDTO<LogBookPageDocumentTypesEnum> | undefined): void {
        if (documentType?.value === LogBookPageDocumentTypesEnum.OriginDeclaration) {
            this.originDeclarationTypeSelected = true;
            this.transportationDocumentTypeSelected = false;
            this.admissionDocumentTypeSelected = false;

            this.preliminaryDataFormGroup.get('originDeclarationNumberControl')!.reset();
            this.preliminaryDataFormGroup.get('originDeclarationNumberControl')!.setValidators(Validators.required);
            this.preliminaryDataFormGroup.get('originDeclarationNumberControl')!.markAsPending();

            this.preliminaryDataFormGroup.get('transportationDocumentNumberControl')!.clearValidators();
            this.preliminaryDataFormGroup.get('transportationDocumentNumberControl')!.reset();

            this.preliminaryDataFormGroup.get('admissionDocumentNumberControl')!.clearValidators();
            this.preliminaryDataFormGroup.get('admissionDocumentNumberControl')!.reset();
        }
        else if (documentType?.value === LogBookPageDocumentTypesEnum.TransportationDocument) {
            this.transportationDocumentTypeSelected = true;
            this.originDeclarationTypeSelected = false;
            this.admissionDocumentTypeSelected = false;

            this.preliminaryDataFormGroup.get('transportationDocumentNumberControl')!.reset();
            this.preliminaryDataFormGroup.get('transportationDocumentNumberControl')!.setValidators([Validators.required, TLValidators.number(0)]);
            this.preliminaryDataFormGroup.get('transportationDocumentNumberControl')!.markAsPending();

            this.preliminaryDataFormGroup.get('originDeclarationNumberControl')!.clearValidators();
            this.preliminaryDataFormGroup.get('originDeclarationNumberControl')!.reset();

            this.preliminaryDataFormGroup.get('admissionDocumentNumberControl')!.clearValidators();
            this.preliminaryDataFormGroup.get('admissionDocumentNumberControl')!.reset();
        }
        else if (documentType?.value === LogBookPageDocumentTypesEnum.AdmissionDocument) {
            this.admissionDocumentTypeSelected = true;
            this.originDeclarationTypeSelected = false;
            this.transportationDocumentTypeSelected = false;

            this.preliminaryDataFormGroup.get('admissionDocumentNumberControl')!.reset();
            this.preliminaryDataFormGroup.get('admissionDocumentNumberControl')!.setValidators([Validators.required, TLValidators.number(0)]);
            this.preliminaryDataFormGroup.get('admissionDocumentNumberControl')!.markAsPending();

            this.preliminaryDataFormGroup.get('originDeclarationNumberControl')!.clearValidators();
            this.preliminaryDataFormGroup.get('originDeclarationNumberControl')!.reset();

            this.preliminaryDataFormGroup.get('transportationDocumentNumberControl')!.clearValidators();
            this.preliminaryDataFormGroup.get('transportationDocumentNumberControl')!.reset();
        }
        else {
            this.originDeclarationTypeSelected = false;
            this.transportationDocumentTypeSelected = false;
            this.admissionDocumentTypeSelected = false;

            this.preliminaryDataFormGroup.get('originDeclarationNumberControl')!.clearValidators();
            this.preliminaryDataFormGroup.get('originDeclarationNumberControl')!.reset();

            this.preliminaryDataFormGroup.get('transportationDocumentNumberControl')!.clearValidators();
            this.preliminaryDataFormGroup.get('transportationDocumentNumberControl')!.reset();

            this.preliminaryDataFormGroup.get('admissionDocumentNumberControl')!.clearValidators();
            this.preliminaryDataFormGroup.get('admissionDocumentNumberControl')!.reset();
        }
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

    private pageNumberValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (control === null || control === undefined) {
                return null;
            }

            if (this.hasPageNotInLogBookError) {
                return { 'pageNotInLogBook': true };
            }
            else if (this.hasPageAlreadySubmittedError) {
                return { 'pageAlreadySubmitted': true };
            }
            else if (this.hasPageAlreadySubmittedOtherLogBookError) {
                return { 'pageAlreadySubmittedOtherLogBook': true };
            }
            else {
                return null;
            }
        }
    }

    private documentNumberExistanceValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (control === null || control === undefined) {
                return null;
            }

            if ((control.get('documentTypeControl')!.value?.value === LogBookPageDocumentTypesEnum.OriginDeclaration
                || this.logBookType === LogBookTypesEnum.Transportation)
                && this.hasInvalidOriginDeclarationNumber
            ) {
                return { 'invalidOriginDeclaratioNumber': true };
            }
            else if (control.get('documentTypeControl')!.value?.value === LogBookPageDocumentTypesEnum.TransportationDocument
                && this.hasInvalidTransportationDocNumber
            ) {
                return { 'invalidTransportationDocNumber': true };
            }
            else if (control.get('documentTypeControl')!.value?.value === LogBookPageDocumentTypesEnum.AdmissionDocument
                && this.hasInvalidAdmissionDocNumber
            ) {
                return { 'invalidAdmissionDocNumber': true };
            }
            else {
                return null;
            }
        }
    }
}