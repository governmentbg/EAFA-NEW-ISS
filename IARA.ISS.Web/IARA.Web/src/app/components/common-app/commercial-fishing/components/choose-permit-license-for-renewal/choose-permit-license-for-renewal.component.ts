import { AfterViewInit, Component, OnInit } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { StepperSelectionEvent } from '@angular/cdk/stepper';
import { SelectionType } from '@swimlane/ngx-datatable';

import { ChoosePermitLicenseForRenewalDialogParams } from './models/choose-permit-license-for-renewal-dialog-params.model';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { PermitLicenseForRenewalDTO } from '@app/models/generated/dtos/PermitLicenseForRenewalDTO';
import { ICommercialFishingService } from '@app/interfaces/common-app/commercial-fishing.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { ShipNomenclatureDTO } from '@app/models/generated/dtos/ShipNomenclatureDTO';
import { PermitNomenclatureDTO } from '@app/models/generated/dtos/PermitNomenclatureDTO';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { IS_PUBLIC_APP } from '@app/shared/modules/application.modules';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { ShipsUtils } from '@app/shared/utils/ships.utils';
import { HttpErrorResponse } from '@angular/common/http';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';

@Component({
    selector: 'choose-permit-license-for-renewal',
    templateUrl: './choose-permit-license-for-renewal.component.html'
})
export class ChoosePermitLicenseForRenewalComponent implements OnInit, AfterViewInit, IDialogComponent {
    public allPermitLicenses: PermitLicenseForRenewalDTO[] = [];
    public permitLicenses: PermitLicenseForRenewalDTO[] = [];
    public SelectionType = SelectionType;

    public chooseShipAndPermitFormGroup!: FormGroup;
    public filterControl: FormControl;

    public selectedPermitLicenses: GridRow<PermitLicenseForRenewalDTO>[] = [];
    public noPermitLicenseChosenValidation: boolean = false;
    public isPublicApp: boolean = false;
    public noShipSelected: boolean = true;

    public ships: ShipNomenclatureDTO[] = [];
    public permits: PermitNomenclatureDTO[] = [];

    private pageCode!: PageCodeEnum;
    private service!: ICommercialFishingService;
    private shipId: number | undefined;
    private permitId: number | undefined;
    private permitNumber: string | undefined;
    private invalidPermitNumberError: boolean = false;

    public constructor() {
        this.filterControl = new FormControl();

        this.isPublicApp = IS_PUBLIC_APP;

        this.chooseShipAndPermitFormGroup = new FormGroup({
            shipControl: new FormControl(null, Validators.required)
        });

        if (this.isPublicApp) {
            this.chooseShipAndPermitFormGroup.addControl('permitRegistrationNumberControl', new FormControl(null, Validators.required));
            this.chooseShipAndPermitFormGroup.setValidators(this.validPermitNumber());
            this.chooseShipAndPermitFormGroup.get('permitRegistrationNumberControl')!.valueChanges.subscribe({
                next: () => {
                    this.invalidPermitNumberError = false;
                }
            });
        }
        else {
            this.chooseShipAndPermitFormGroup.addControl('permitControl', new FormControl(null, Validators.required));
        }
    }

    public ngOnInit(): void {
        if (!this.isPublicApp) {
            this.chooseShipAndPermitFormGroup.get('shipControl')!.valueChanges.subscribe({
                next: (ship: ShipNomenclatureDTO | undefined | string) => {
                    this.clearPermitLicensesArrays();
                    if (ship !== null && ship !== undefined && ship instanceof NomenclatureDTO) {
                        this.service.getPermitNomenclatures(ship.value!, this.pageCode === PageCodeEnum.PoundnetCommFishLic).subscribe({
                            next: (values: PermitNomenclatureDTO[]) => {
                                this.permits = values;
                                this.noShipSelected = false;
                                if (this.permitId !== null && this.permitId !== undefined) {
                                    const permit: PermitNomenclatureDTO = this.permits.find(x => x.value === this.permitId)!;
                                    this.chooseShipAndPermitFormGroup.get('permitControl')!.setValue(permit);
                                }
                            }
                        });
                    }
                    else {
                        this.permits = [];
                        this.permitId = undefined;
                        this.chooseShipAndPermitFormGroup.get('permitControl')!.setValue(this.permitId);
                        this.noShipSelected = true;
                    }
                }
            });

            this.chooseShipAndPermitFormGroup.get('permitControl')!.valueChanges.subscribe({
                next: (permit: PermitNomenclatureDTO | undefined | string) => {
                    this.clearPermitLicensesArrays();

                    if (permit === null || permit === undefined || typeof (permit) === 'string') {
                        this.permitId = undefined;
                    }
                }
            });
        }
        else {
            this.chooseShipAndPermitFormGroup.get('permitRegistrationNumberControl')!.setValue(this.permitNumber);
        }

        if (this.shipId !== null && this.shipId !== undefined) {
            if (!this.isPublicApp) {
                this.chooseShipAndPermitFormGroup.get('shipControl')!.setValue(ShipsUtils.get(this.ships, this.shipId));
            }
        }
        else {
            this.noShipSelected = true;
        }
    }

    public ngAfterViewInit(): void {
        this.filterControl.valueChanges.subscribe({
            next: (value: string) => {
                if (value?.length > 0) {
                    value = value.toLowerCase();
                    const dateValue: Date | undefined = new Date(value);

                    this.permitLicenses = this.allPermitLicenses.filter((permitLicense: PermitLicenseForRenewalDTO) => {
                        if (permitLicense.registrationNumber!.toLowerCase().includes(value)) {
                            return true;
                        }

                        if (permitLicense.holderNames!.toLowerCase().includes(value)) {
                            return true;
                        }

                        if (permitLicense.captain!.toLowerCase().includes(value)) {
                            return true;
                        }

                        if (dateValue?.getTime() !== null && dateValue?.getTime() !== undefined) {
                            if (permitLicense.validFrom!.getTime() <= dateValue.getTime()
                                && permitLicense.validTo!.getTime() >= dateValue.getTime()
                            ) {
                                return true;
                            }
                        }

                        if (permitLicense.fishingGears?.toLowerCase().includes(value)) {
                            return true;
                        }

                        if (permitLicense.auqticOrganisms?.toLowerCase().includes(value)) {
                            return true;
                        }

                        return false;
                    });
                }
                else {
                    this.permitLicenses = this.allPermitLicenses;
                }

                if (this.permitLicenses.find(x => x.id === this.selectedPermitLicenses[0]?.data.id) === undefined) {
                    this.selectedPermitLicenses = [];
                }
                else { // doesn't update the UI so the selected row is not present TODO
                    const selectedApplicationRef = this.selectedPermitLicenses[0];
                    this.selectedPermitLicenses = [];
                    const applicationToSelect = this.permitLicenses.find(x => x.id === selectedApplicationRef?.data.id);
                    if (applicationToSelect !== undefined) {
                        this.selectedPermitLicenses = [new GridRow(applicationToSelect, false, false)];
                        this.selectedPermitLicenses = this.selectedPermitLicenses.slice();
                    }
                }
            }
        });
    }

    public setData(data: ChoosePermitLicenseForRenewalDialogParams, buttons: DialogWrapperData): void {
        this.service = data.service;
        this.shipId = data.shipId;
        this.permitId = data.permitId;
        this.permitNumber = data.permitNumber;
        this.pageCode = data.pageCode;
        this.ships = data.ships;
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public checkedRow(row: GridRow<PermitLicenseForRenewalDTO>): void {
        const element: PermitLicenseForRenewalDTO | undefined = this.permitLicenses?.find(x => x.id === row.data.id);
        if (element !== undefined) {
            element.isChecked = !element.isChecked;

            if (element.isChecked) {
                this.noPermitLicenseChosenValidation = false;
                const elementsToUpdate: PermitLicenseForRenewalDTO[] = this.permitLicenses!.filter(x => x.id !== row.data.id);
                for (const el of elementsToUpdate) {
                    el.isChecked = false;
                }
                this.selectedPermitLicenses = [row];
            }
            else {
                this.noPermitLicenseChosenValidation = true;
                if (this.permitLicenses !== null && this.permitLicenses !== undefined) {
                    for (const el of this.permitLicenses) {
                        el.isChecked = false;
                    }
                }
                this.selectedPermitLicenses = [];
            }

            this.permitLicenses = this.permitLicenses!.slice();
        }
    }

    public getRowClass = (row: GridRow<PermitLicenseForRenewalDTO>): Record<string, boolean> => {
        return {
            'row-selected': row.data.isChecked === true,
            'row-not-selected': row.data.isChecked === false
        };
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (!CommonUtils.isNullOrEmpty(this.selectedPermitLicenses) && !this.noPermitLicenseChosenValidation) {
            this.noPermitLicenseChosenValidation = false;
            dialogClose((this.selectedPermitLicenses as GridRow<PermitLicenseForRenewalDTO>[])[0]?.data?.id);
        }
        else {
            this.noPermitLicenseChosenValidation = true;
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public selectedStepChanged(stepperSelectionEvent: StepperSelectionEvent): void {
        if (stepperSelectionEvent.previouslySelectedIndex === 0 && stepperSelectionEvent.selectedIndex === 1) {
            this.shipId = this.chooseShipAndPermitFormGroup.get('shipControl')!.value!.value;

            if (this.isPublicApp) {
                this.permitNumber = (this.chooseShipAndPermitFormGroup.get('permitRegistrationNumberControl')! as PermitNomenclatureDTO).registrationNumber;
            }
            else {
                this.permitId = this.chooseShipAndPermitFormGroup.get('permitControl')!.value!.value;
            }

            this.getPermitLicensesForRenewal();
        }
    }

    private getPermitLicensesForRenewal(): void {
        if (this.isPublicApp) {
            this.service.getPermitLicensesForRenewal(undefined, this.permitNumber!, this.pageCode).subscribe({
                next: (results: PermitLicenseForRenewalDTO[]) => {
                    setTimeout(() => {
                        this.allPermitLicenses = results;
                        this.permitLicenses = [...this.allPermitLicenses];
                    });
                },
                error: (errorResponse: HttpErrorResponse) => {
                    if ((errorResponse.error as ErrorModel)?.code === ErrorCode.InvalidPermitNumber) {
                        this.invalidPermitNumberError = true;
                        this.chooseShipAndPermitFormGroup.updateValueAndValidity({ emitEvent: false });
                    }
                }
            });
        }
        else {
            this.service.getPermitLicensesForRenewal(this.permitId!, undefined, this.pageCode).subscribe({
                next: (results: PermitLicenseForRenewalDTO[]) => {
                    setTimeout(() => {
                        this.allPermitLicenses = results;
                        this.permitLicenses = [...this.allPermitLicenses];
                    });
                }
            });
        }
    }

    private clearPermitLicensesArrays(): void {
        this.allPermitLicenses = [];
        this.permitLicenses = [];
        this.selectedPermitLicenses = [];
    }

    private validPermitNumber(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (this.invalidPermitNumberError) {
                return { 'invalidPermitNumber': true };
            }
            else {
                return null;
            }
        }
    }
}