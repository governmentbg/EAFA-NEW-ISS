import { StepperSelectionEvent } from '@angular/cdk/stepper';
import { Component, OnDestroy, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatVerticalStepper } from '@angular/material/stepper';
import { Subscription } from 'rxjs';

import { ICatchesAndSalesService } from '@app/interfaces/common-app/catches-and-sales.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { ShipLogBookPageEditDTO } from '@app/models/generated/dtos/ShipLogBookPageEditDTO';
import { AddShipWizardDialogParams } from './models/add-ship-wizard-dialog-params.model';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { EditShipLogBookPageComponent } from '../ship-log-book/edit-ship-log-book-page.component';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { EditShipLogBookPageDialogParams } from '../ship-log-book/models/edit-ship-log-book-page-dialog-params.model';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { RequestProperties } from '@app/shared/services/request-properties';
import { LogBookPermitLicenseNomenclatureDTO } from '@app/models/generated/dtos/LogBookPermitLicenseNomenclatureDTO';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';

@Component({
    selector: 'add-ship-page-wizard',
    templateUrl: './add-ship-page-wizard.component.html'
})
export class AddShipPageWizardComponent implements IDialogComponent, OnDestroy {
    public preliminaryDataFormGroup!: FormGroup;
    public dataFormGroup!: FormGroup;

    public permitLicenses: LogBookPermitLicenseNomenclatureDTO[] = [];

    public pageNumberLabel!: string;
    public isEditNumber: boolean = false;

    @ViewChild(MatVerticalStepper)
    private stepper!: MatVerticalStepper;

    private service!: ICatchesAndSalesService;
    private logBookId!: number;
    private pageId: number | undefined;
    private hasMissingPagesRangePermission: boolean = false;

    private translate: FuseTranslationLoaderService;
    private snackbar: MatSnackBar;
    private editShipLogBookPageDialog: TLMatDialog<EditShipLogBookPageComponent>;
    private confirmDialog: TLConfirmDialog;

    private model!: ShipLogBookPageEditDTO;
    private dataCache: Map<string, ShipLogBookPageEditDTO[]> = new Map<string, ShipLogBookPageEditDTO[]>();

    private subscriptions: Subscription[] = [];

    public constructor(
        translate: FuseTranslationLoaderService,
        snackbar: MatSnackBar,
        editShipLogBookPageDialog: TLMatDialog<EditShipLogBookPageComponent>,
        confirmDialog: TLConfirmDialog
    ) {
        this.translate = translate;
        this.snackbar = snackbar;
        this.editShipLogBookPageDialog = editShipLogBookPageDialog;
        this.confirmDialog = confirmDialog;

        this.pageNumberLabel = this.isEditNumber
            ? this.translate.getValue('catches-and-sales.add-ship-page-wizard-new-page-number')
            : this.translate.getValue('catches-and-sales.add-ship-page-wizard-page-number');

        this.buildForm();
    }

    public ngOnDestroy(): void {
        for (const sub of this.subscriptions) {
            sub.unsubscribe();
        }
    }

    public saveBtnClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.preliminaryDataFormGroup.get('pageNumberControl')!.markAllAsTouched();
        this.preliminaryDataFormGroup.get('pageNumberControl')!.updateValueAndValidity();

        this.dataFormGroup.markAllAsTouched();
        this.dataFormGroup.updateValueAndValidity();

        if (this.preliminaryDataFormGroup.valid && this.dataFormGroup.valid) {
            if (this.isEditNumber) {
                if (this.pageId !== undefined && this.pageId !== null) {
                    this.editShipLogBookPageNumber(dialogClose);
                }
            }
            else {
                this.editShipLogBookPageDialog.openWithTwoButtons({
                    title: this.translate.getValue('catches-and-sales.add-fishing-log-book-page-dialog-title'),
                    TCtor: EditShipLogBookPageComponent,
                    translteService: this.translate,
                    viewMode: false,
                    headerCancelButton: {
                        cancelBtnClicked: this.closeEditShipLogBookPageDialogBtnClicked.bind(this)
                    },
                    componentData: new EditShipLogBookPageDialogParams({
                        service: this.service,
                        model: this.model,
                        viewMode: false
                    }),
                    disableDialogClose: true
                }, '1450px').subscribe({
                    next: (result: ShipLogBookPageEditDTO | undefined) => {
                        dialogClose(result);
                    }
                });
            }
        }
    }

    public cancelBtnClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public setData(data: AddShipWizardDialogParams, buttons: DialogWrapperData): void {
        this.service = data.service;
        this.logBookId = data.logBookId;
        this.isEditNumber = data.isEdit;
        this.pageId = data.pageId;

        if (data.isEdit) {
            this.preliminaryDataFormGroup.get('oldPageNumberControl')!.setValue(data.pageNumber);
        }
    }

    public stepSelected(event: StepperSelectionEvent): void {
        if (event.previouslySelectedIndex === 0 && event.selectedIndex === 1) {
            if (this.preliminaryDataFormGroup.get('pageNumberControl')!.valid) {
                const cached: ShipLogBookPageEditDTO[] | undefined = this.dataCache.get(this.preliminaryDataFormGroup.get('pageNumberControl')!.value);

                if (cached === undefined || cached === null) {
                    this.subscriptions.push(
                        this.service.getNewShipLogBookPages(this.preliminaryDataFormGroup.get('pageNumberControl')!.value, this.logBookId).subscribe({
                            next: (data: ShipLogBookPageEditDTO[]) => {
                                this.dataCache.set(this.preliminaryDataFormGroup.get('pageNumberControl')!.value, data);
                                this.setNewPageData(data);
                                this.fillDataForm();
                            },
                            error: (response: HttpErrorResponse) => {
                                const error: ErrorModel | undefined = response.error;

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

                                event.selectedStep.completed = false;
                                event.selectedStep.interacted = false;
                                this.stepper.previous();
                            }
                        })
                    );
                }
                else {
                    this.setNewPageData(cached);
                    this.fillDataForm();
                }
            }
        }
    }

    private setNewPageData(data: ShipLogBookPageEditDTO[]): void {
        this.fillPermitLicensesNomenclature(data);

        if (data.length === 1) {
            this.model = data[0];
            this.model.pageNumber = this.preliminaryDataFormGroup.get('pageNumberControl')!.value;
        }
        else {
            this.model = new ShipLogBookPageEditDTO({
                logBookId: data[0].logBookId,
                logBookTypeId: data[0].logBookTypeId,
                fillDate: data[0].fillDate,
                shipId: data[0].shipId,
                shipName: data[0].shipName,
                pageNumber: this.preliminaryDataFormGroup.get('pageNumberControl')!.value
            });
        }
    }

    private fillPermitLicensesNomenclature(data: ShipLogBookPageEditDTO[]): void {
        this.permitLicenses = [];

        for (const page of data) {
            this.permitLicenses.push(new LogBookPermitLicenseNomenclatureDTO({
                value: page.logBookPermitLicenseId,
                displayName: `${page.permitLicenseNumber}`,
                description: `${this.translate.getValue('catches-and-sales.ship-log-book-page-qualified-fisher')}: ${page.permitLicenseName}
                              ${this.translate.getValue('catches-and-sales.ship-log-book-page-water-type')}: ${page.permitLicenseWaterTypeName}`,
                permitLicenseId: page.permitLicenseId,
                permitLicenseName: page.permitLicenseName,
                permitLicenseWaterType: page.permitLicenseWaterType,
                permitLicenseWaterTypeName: page.permitLicenseWaterTypeName,
                permitLicenseNumber: page.permitLicenseNumber,
                logBookId: page.logBookId,
                isActive: true
            }));
        }

        this.permitLicenses = this.permitLicenses.slice();
    }

    private buildForm(): void {
        this.preliminaryDataFormGroup = new FormGroup({
            pageNumberControl: new FormControl(null, [Validators.required, TLValidators.number(0, undefined, 0)]),
            oldPageNumberControl: new FormControl(undefined)
        });

        this.dataFormGroup = new FormGroup({
            shipControl: new FormControl(undefined, Validators.required),
            permitLicenceControl: new FormControl(undefined, Validators.required)
        });

        this.dataFormGroup.get('permitLicenceControl')!.valueChanges.subscribe({
            next: (logBookPermitLicense: LogBookPermitLicenseNomenclatureDTO | null | undefined) => {
                if (logBookPermitLicense !== null && logBookPermitLicense !== undefined) {
                    this.model.logBookPermitLicenseId = logBookPermitLicense.value;
                    this.model.permitLicenseId = logBookPermitLicense.permitLicenseId;
                    this.model.permitLicenseName = logBookPermitLicense.permitLicenseName;
                    this.model.permitLicenseWaterType = logBookPermitLicense.permitLicenseWaterType;
                    this.model.permitLicenseWaterTypeName = logBookPermitLicense.permitLicenseWaterTypeName;
                }
                else {
                    this.model.logBookPermitLicenseId = undefined;
                    this.model.permitLicenseId = undefined;
                    this.model.permitLicenseName = undefined;
                    this.model.permitLicenseWaterType = undefined;
                    this.model.permitLicenseWaterTypeName = undefined;
                }
            }
        });
    }

    private fillDataForm(): void {
        this.dataFormGroup.get('shipControl')!.setValue(this.model.shipName);

        if (this.model.logBookPermitLicenseId !== null && this.model.logBookPermitLicenseId !== undefined) {
            const permitLicense: LogBookPermitLicenseNomenclatureDTO = this.permitLicenses.find(x => x.value == this.model.logBookPermitLicenseId)!;
            this.dataFormGroup.get('permitLicenceControl')!.setValue(permitLicense);
        }
    }

    private editShipLogBookPageNumber(dialogClose: DialogCloseCallback): void {
        this.service.editShipLogBookPageNumber(this.pageId!, this.preliminaryDataFormGroup.get('pageNumberControl')!.value!, this.hasMissingPagesRangePermission).subscribe({
            next: () => {
                dialogClose(this.model);
            },
            error: (response: HttpErrorResponse) => {
                this.handleEditPageNumberErrorResponse(response, dialogClose);
            }
        });
    }

    private handleEditPageNumberErrorResponse(response: HttpErrorResponse, dialogClose: DialogCloseCallback): void {
        const error: ErrorModel | undefined = response.error;

        const pageToAdd: string = this.model.pageNumber!;
        const oldPageNum: number | undefined = this.preliminaryDataFormGroup.get('oldPageNumberControl')!.value;

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
            const message: string = this.translate.getValue('catches-and-sales.ship-log-book-page-already-submitted-other-logbook-error');
            this.snackbar.open(
                `${message}: ${error.messages[0]}`,
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
            let diff: number = Number(error!.messages[1]);

            // confirmation message

            let message: string = ''; 

            //генерира се липсваща страница и за страницата, чийто номер е променен
            if (this.isEditNumber) {
                diff = diff + 1;
            }

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
                        this.editShipLogBookPageNumber(dialogClose!); // start edit page number method again
                    }
                }
            });
        }
        else if (error?.code === ErrorCode.MissingPageWithOldNumber) {
            const msg1: string = this.translate.getValue('catches-and-sales.edit-page-number-missing-page-with-old-number-first-part');
            const msg2: string = this.translate.getValue('catches-and-sales.edit-page-number-missing-page-with-old-number-second-part');
            const msg3: string = this.translate.getValue('catches-and-sales.edit-page-number-missing-page-with-old-number-third-part');
            const msg4: string = this.translate.getValue('catches-and-sales.edit-page-number-missing-page-with-old-number-forth-part');

            const message: string = `${msg1} ${pageToAdd} ${msg2} ${oldPageNum} ${msg3} ${msg4}`;

            this.confirmDialog.open({
                title: this.translate.getValue('catches-and-sales.edit-page-number-missing-page-with-old-number-dialog-title'),
                message: message,
                okBtnLabel: this.translate.getValue('catches-and-sales.edit-page-number-missing-page-with-old-number-dialog-ok-button'),
                okBtnColor: 'warn'
            }).subscribe({
                next: (ok: boolean | undefined) => {
                    this.hasMissingPagesRangePermission = ok ?? false;

                    if (this.hasMissingPagesRangePermission) {
                        this.editShipLogBookPageNumber(dialogClose!); // start edit page number method again
                    }
                }
            });
        }
    }

    private closeEditShipLogBookPageDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }
}