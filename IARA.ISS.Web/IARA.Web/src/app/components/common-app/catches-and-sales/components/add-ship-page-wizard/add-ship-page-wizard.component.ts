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
import { FuseTranslationLoaderService } from '@app/../@fuse/services/translation-loader.service';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { EditShipLogBookPageDialogParams } from '../ship-log-book/models/edit-ship-log-book-page-dialog-params.model';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { RequestProperties } from '@app/shared/services/request-properties';
import { LogBookPermitLicenseNomenclatureDTO } from '@app/models/generated/dtos/LogBookPermitLicenseNomenclatureDTO';

@Component({
    selector: 'add-ship-page-wizard',
    templateUrl: './add-ship-page-wizard.component.html'
})
export class AddShipPageWizardComponent implements IDialogComponent, OnDestroy {
    public pageNumberControl!: FormControl;
    public dataFormGroup!: FormGroup;

    public permitLicenses: LogBookPermitLicenseNomenclatureDTO[] = [];

    @ViewChild(MatVerticalStepper)
    private stepper!: MatVerticalStepper;

    private service!: ICatchesAndSalesService;
    private logBookId!: number;

    private translate: FuseTranslationLoaderService;
    private snackbar: MatSnackBar;
    private editShipLogBookPageDialog: TLMatDialog<EditShipLogBookPageComponent>;

    private model!: ShipLogBookPageEditDTO;
    private dataCache: Map<string, ShipLogBookPageEditDTO[]> = new Map<string, ShipLogBookPageEditDTO[]>();

    private subscriptions: Subscription[] = [];

    public constructor(
        translate: FuseTranslationLoaderService,
        snackbar: MatSnackBar,
        editShipLogBookPageDialog: TLMatDialog<EditShipLogBookPageComponent>
    ) {
        this.translate = translate;
        this.snackbar = snackbar;
        this.editShipLogBookPageDialog = editShipLogBookPageDialog;

        this.buildForm();
    }

    public ngOnDestroy(): void {
        for (const sub of this.subscriptions) {
            sub.unsubscribe();
        }
    }

    public saveBtnClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.pageNumberControl.markAllAsTouched();
        this.dataFormGroup.markAllAsTouched();

        if (this.pageNumberControl.valid && this.dataFormGroup.valid) {
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

    public cancelBtnClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public setData(data: AddShipWizardDialogParams, buttons: DialogWrapperData): void {
        this.service = data.service;
        this.logBookId = data.logBookId;
        this.pageNumberControl.setValue(data.pageNumber);
    }

    public stepSelected(event: StepperSelectionEvent): void {
        if (event.previouslySelectedIndex === 0 && event.selectedIndex === 1) {
            if (this.pageNumberControl.valid) {
                const cached: ShipLogBookPageEditDTO[] | undefined = this.dataCache.get(this.pageNumberControl.value);

                if (cached === undefined || cached === null) {
                    this.subscriptions.push(
                        this.service.getNewShipLogBookPages(this.pageNumberControl.value, this.logBookId).subscribe({
                            next: (data: ShipLogBookPageEditDTO[]) => {
                                this.dataCache.set(this.pageNumberControl.value, data);
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
            this.model.pageNumber = this.pageNumberControl.value;
            this.fillDataForm();
        }
        else {
            this.model = new ShipLogBookPageEditDTO({
                logBookId: data[0].logBookId,
                fillDate: data[0].fillDate,
                shipId: data[0].shipId,
                shipName: data[0].shipName,
                pageNumber: this.pageNumberControl.value
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
    }

    private buildForm(): void {
        this.pageNumberControl = new FormControl(null, [Validators.required, TLValidators.number(0)]);

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

    private closeEditShipLogBookPageDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }
}