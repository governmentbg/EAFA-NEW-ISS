import { Component, OnInit, ViewChild } from '@angular/core';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { LogBookPageDocumentTypesEnum } from '../../enums/log-book-page-document-types.enum';
import { StepperSelectionEvent } from '@angular/cdk/stepper';
import { MatStepper } from '@angular/material/stepper';
import { ICatchesAndSalesService } from '@app/interfaces/common-app/catches-and-sales.interface';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { LogBookPagePersonTypesEnum } from '@app/enums/log-book-page-person-types.enum';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { EditPageNumberDilogParamsModel } from '../../models/edit-page-number-dialog-params.model';
import { LogBookTypesEnum } from '@app/enums/log-book-types.enum';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { HttpErrorResponse } from '@angular/common/http';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { RequestProperties } from '@app/shared/services/request-properties';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { LogBookNomenclatureDTO } from '@app/models/generated/dtos/LogBookNomenclatureDTO';
import { PermitLicenseNomenclatureDTO } from '@app/models/generated/dtos/PermitLicenseNomenclatureDTO';
import { ShipNomenclatureDTO } from '@app/models/generated/dtos/ShipNomenclatureDTO';
import { ShipsUtils } from '@app/shared/utils/ships.utils';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';

@Component({
    selector: 'edit-log-book-page-number',
    templateUrl: './edit-log-book-page-number.component.html'
})
export class EditLogBookPageNumberComponent implements OnInit, IDialogComponent {
    public logBookType!: LogBookTypesEnum;
    public documentType: LogBookPageDocumentTypesEnum | undefined;
    public pageNumber!: number;
    public pageId!: number;
    public logBookId!: number;
    public hasMissingPagesRangePermission: boolean = false;
    public isAquacultureLogBookPage: boolean = false;
    public permitLicenseId: number | undefined;

    public preliminaryDataFormGroup!: FormGroup;
    public confirmationDataFormGroup!: FormGroup;

    public logBookOwnerTypes: NomenclatureDTO<LogBookPagePersonTypesEnum>[] = [];
    public possibleLogBooks: LogBookNomenclatureDTO[] = [];
    public permitLicenses: PermitLicenseNomenclatureDTO[] = [];
    public ships: ShipNomenclatureDTO[] = [];

    public service!: ICatchesAndSalesService;

    private readonly translate: FuseTranslationLoaderService;
    private snackbar: MatSnackBar;
    private confirmDialog: TLConfirmDialog;

    @ViewChild('stepper')
    private stepper!: MatStepper;

    public constructor(
        translationService: FuseTranslationLoaderService,
        snackbar: MatSnackBar,
        confirmDialog: TLConfirmDialog
    ) {
        this.translate = translationService;
        this.snackbar = snackbar;
        this.confirmDialog = confirmDialog;

        this.logBookOwnerTypes = [
            new NomenclatureDTO({
                value: LogBookPagePersonTypesEnum.Person,
                displayName: this.translate.getValue('catches-and-sales.log-book-page-person-person-type'),
                isActive: true
            }),
            new NomenclatureDTO({
                value: LogBookPagePersonTypesEnum.LegalPerson,
                displayName: this.translate.getValue('catches-and-sales.log-book-page-person-person-legal-type'),
                isActive: true
            }),
            new NomenclatureDTO({
                value: LogBookPagePersonTypesEnum.RegisteredBuyer,
                displayName: this.translate.getValue('catches-and-sales.log-book-page-person-registered-buyer-type'),
                isActive: true
            })
        ];

        this.buildForm();
    }

    public ngOnInit(): void {
        if (this.isAquacultureLogBookPage) {
            this.service.getAquacultureLogBookPageOwnerData(this.pageNumber).subscribe({
                next: (possibleLogBooks: LogBookNomenclatureDTO[]) => {
                    this.possibleLogBooks = possibleLogBooks ?? [];
                }
            });
        }
        else if (this.documentType !== undefined && this.documentType !== null) {
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.LogBookPermitLicenses, this.service.getPermitLicenseNomenclatures.bind(this.service), false
            ).subscribe({
                next: (permitLicenses: PermitLicenseNomenclatureDTO[]) => {
                    this.permitLicenses = permitLicenses;
                }
            });

            this.service.getLogBookPageDocumentOwnerData(this.pageNumber, this.documentType!).subscribe({
                next: (possibleLogBooks: LogBookNomenclatureDTO[]) => {
                    this.possibleLogBooks = possibleLogBooks ?? [];
                }
            });
        }
    }

    public setData(data: EditPageNumberDilogParamsModel, wrapperData: DialogWrapperData): void {
        this.logBookType = data.logBookType;
        this.pageId = data.pageId;
        this.logBookId = data.logBookId;
        this.pageNumber = data.pageNumber;
        this.service = data.service;
        this.ships = data.ships;

        this.preliminaryDataFormGroup.get('oldPageNumberControl')!.setValue(data.pageNumber);

        if (data.logBookType === LogBookTypesEnum.Aquaculture) {
            this.isAquacultureLogBookPage = true;
        }
        else {
            this.setDocumentType();
        }
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (this.preliminaryDataFormGroup.valid) {
            const newPageNumber: number = this.preliminaryDataFormGroup.get('pageNumberControl')!.value;
            this.editPageNumber(newPageNumber, dialogClose);
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
                const logBook: LogBookNomenclatureDTO | undefined = this.possibleLogBooks.find(x => x.value === this.logBookId);
                this.confirmationDataFormGroup.get('possibleLogBookControl')!.setValue(logBook);
                this.permitLicenseId = logBook?.permitLicenseId;

                if (logBook?.permitLicenseId !== undefined && logBook?.permitLicenseId !== null) {
                    const permitLicense: PermitLicenseNomenclatureDTO = this.permitLicenses.find(x => x.value === logBook.permitLicenseId)!;

                    this.confirmationDataFormGroup.get('permitLicenseControl')!.setValue(logBook.permitLicenseNumber);
                    this.confirmationDataFormGroup.get('shipControl')!.setValue(ShipsUtils.get(this.ships, permitLicense.shipId!));
                }
            }
        }
    }

    private buildForm(): void {
        this.preliminaryDataFormGroup = new FormGroup({
            oldPageNumberControl: new FormControl(undefined),
            pageNumberControl: new FormControl(undefined, [Validators.required, TLValidators.number(0, undefined, 0)])
        });

        this.confirmationDataFormGroup = new FormGroup({
            possibleLogBookControl: new FormControl(undefined),
            permitLicenseControl: new FormControl(undefined),
            shipControl: new FormControl(undefined)
        });
    }

    private setDocumentType(): void {
        switch (this.logBookType) {
            case LogBookTypesEnum.Transportation: this.documentType = LogBookPageDocumentTypesEnum.TransportationDocument; break;
            case LogBookTypesEnum.Admission: this.documentType = LogBookPageDocumentTypesEnum.AdmissionDocument; break;
            case LogBookTypesEnum.FirstSale: this.documentType = LogBookPageDocumentTypesEnum.FirstSaleDocument; break;
        }
    }

    private editPageNumber(pageNumber: number, dialogClose: DialogCloseCallback): void {
        switch (this.logBookType) {
            case LogBookTypesEnum.Transportation:
                this.editTransportationPageNumber(pageNumber, dialogClose);
                break;
            case LogBookTypesEnum.Admission:
                this.editAdmissionPageNumber(pageNumber, dialogClose);
                break;
            case LogBookTypesEnum.FirstSale:
                this.editFirstSalePageNumber(pageNumber, dialogClose);
                break;
            case LogBookTypesEnum.Aquaculture:
                this.editAquaculturePageNumber(pageNumber, dialogClose);
                break;
            default: {
                dialogClose();
            } break;
        }
    }

    private editTransportationPageNumber(newPageNumber: number, dialogClose: DialogCloseCallback): void {
        this.service.editTransportationLogBookPageNumber(this.pageId, newPageNumber, this.hasMissingPagesRangePermission).subscribe({
            next: () => {
                dialogClose(newPageNumber);
            },
            error: (response: HttpErrorResponse) => {
                this.handleErrorResponse(response, dialogClose);
                this.stepper.previous();
            }
        });
    }

    private editAdmissionPageNumber(newPageNumber: number, dialogClose: DialogCloseCallback): void {
        this.service.editAdmissionLogBookPageNumber(this.pageId, newPageNumber, this.hasMissingPagesRangePermission).subscribe({
            next: () => {
                dialogClose(newPageNumber);
            },
            error: (response: HttpErrorResponse) => {
                this.handleErrorResponse(response, dialogClose);
                this.stepper.previous();
            }
        });
    }

    private editFirstSalePageNumber(newPageNumber: number, dialogClose: DialogCloseCallback): void {
        this.service.editFirstSaleLogBookPageNumber(this.pageId, newPageNumber, this.hasMissingPagesRangePermission).subscribe({
            next: () => {
                dialogClose(newPageNumber);
            },
            error: (response: HttpErrorResponse) => {
                this.handleErrorResponse(response, dialogClose);
                this.stepper.previous();
            }
        });
    }

    private editAquaculturePageNumber(newPageNumber: number, dialogClose: DialogCloseCallback): void {
        this.service.editAquacultureLogBookPageNumber(this.pageId, newPageNumber, this.hasMissingPagesRangePermission).subscribe({
            next: () => {
                dialogClose(newPageNumber);
            },
            error: (response: HttpErrorResponse) => {
                this.handleErrorResponse(response, dialogClose);
                this.stepper.previous();
            }
        });
    }

    private handleErrorResponse(response: HttpErrorResponse, dialogClose: DialogCloseCallback): void {
        const error: ErrorModel | undefined = response.error;
        const pageToAdd: number = this.preliminaryDataFormGroup.get('pageNumberControl')!.value!;

        if (error?.code === ErrorCode.PageNumberNotInLogbook) {
            this.snackbar.open(this.translate.getValue('catches-and-sales.ship-log-book-page-not-in-range-error'), undefined, {
                duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
            });
        }
        else if (error?.code === ErrorCode.PageNumberNotInLogBookLicense) {
            this.snackbar.open(
                this.translate.getValue('catches-and-sales.ship-log-book-page-not-in-log-book-license-range-error'),
                undefined,
                {
                    duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                    panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
                });
        }
        else if (error?.code === ErrorCode.LogBookPageAlreadySubmitted) {
            this.snackbar.open(this.translate.getValue('catches-and-sales.ship-log-book-page-already-submitted-error'), undefined, {
                duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
            });
        }
        else if (error?.code === ErrorCode.LogBookPageAlreadySubmittedOtherLogBook) {
            this.snackbar.open(
                `${this.translate.getValue('catches-and-sales.ship-log-book-page-already-submitted-other-logbook-error')}: ${error.messages[0]}`,
                undefined,
                {
                    duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                    panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
                });
        }
        else if (error?.code === ErrorCode.MaxNumberMissingPagesExceeded) {
            if (error!.messages === null || error!.messages === undefined || error!.messages.length < 2) {
                throw new Error('In MaxNumberMissingPagesExceeded exception at least the last used page number and a number saying the difference should be passed in the messages property.');
            }

            const lastUsedPageNum: number = Number(error!.messages[0]);

            //генерира се липсваща страница и за страницата, чийто номер е променен
            const diff: number = Number(error!.messages[1]) + 1;

            // confirmation message

            let message: string = ''; 

            const msg1: string = this.translate.getValue('catches-and-sales.edit-page-number-generate-missing-pages-permission-first-message');
            const msg2: string = this.translate.getValue('catches-and-sales.edit-page-number-generate-missing-pages-permission-second-message');
            const msg3: string = this.translate.getValue('catches-and-sales.edit-page-number-generate-missing-pages-permission-third-message');
            const msg4: string = this.translate.getValue('catches-and-sales.edit-page-number-generate-missing-pages-permission-forth-message');
            const msg5: string = this.translate.getValue('catches-and-sales.edit-page-number-generate-missing-pages-permission-fifth-message');
            const msg6: string = this.translate.getValue('catches-and-sales.edit-page-number-generate-missing-pages-permission-sixth-message');

            const msg7: string = this.translate.getValue('catches-and-sales.edit-page-number-generate-missing-pages-permission-one-third-message');
            const msg8: string = this.translate.getValue('catches-and-sales.edit-page-number-generate-missing-pages-permission-one-forth-message');

            if (diff === 1) {
                message = `${msg1} ${lastUsedPageNum} ${msg2} ${pageToAdd} ${msg7} ${diff} ${msg8}.\n\n${msg5} ${diff} ${msg8}.`;
            }
            else {
                message = `${msg1} ${lastUsedPageNum} ${msg2} ${pageToAdd} ${msg3} ${diff} ${msg4}.\n\n${msg5} ${diff} ${msg6}.`;
            }

            // button label

            const btnMsg1: string = this.translate.getValue('catches-and-sales.ship-log-book-page-permit-generate-missing-pages-first-part');
            const btnMsg2: string = this.translate.getValue('catches-and-sales.ship-log-book-page-permit-generate-missing-pages-second-part');

            this.confirmDialog.open({
                title: this.translate.getValue('catches-and-sales.ship-log-book-page-generate-missing-pages-permission-dialog-title'),
                message: message,
                okBtnLabel: `${btnMsg1} ${diff} ${btnMsg2}`,
                okBtnColor: 'warn'
            }).subscribe({
                next: (ok: boolean | undefined) => {
                    this.hasMissingPagesRangePermission = ok ?? false;

                    if (this.hasMissingPagesRangePermission) {
                        this.editPageNumber(pageToAdd, dialogClose!); // start edit page number method again
                    }
                }
            });
        }
        else if (error?.code === ErrorCode.MissingPageWithOldNumber) {
            const msg1: string = this.translate.getValue('catches-and-sales.edit-page-number-missing-page-with-old-number-first-part');
            const msg2: string = this.translate.getValue('catches-and-sales.edit-page-number-missing-page-with-old-number-second-part');
            const msg3: string = this.translate.getValue('catches-and-sales.edit-page-number-missing-page-with-old-number-third-part');
            const msg4: string = this.translate.getValue('catches-and-sales.edit-page-number-missing-page-with-old-number-forth-part');

            const message: string = `${msg1} ${pageToAdd} ${msg2} ${this.pageNumber} ${msg3} ${msg4}`;

            this.confirmDialog.open({
                title: this.translate.getValue('catches-and-sales.edit-page-number-missing-page-with-old-number-dialog-title'),
                message: message,
                okBtnLabel: this.translate.getValue('catches-and-sales.edit-page-number-missing-page-with-old-number-dialog-ok-button'),
                okBtnColor: 'warn'
            }).subscribe({
                next: (ok: boolean | undefined) => {
                    this.hasMissingPagesRangePermission = ok ?? false;

                    if (this.hasMissingPagesRangePermission) {
                        this.editPageNumber(pageToAdd, dialogClose!); // start edit page number method again
                    }
                }
            });
        }
    }
}