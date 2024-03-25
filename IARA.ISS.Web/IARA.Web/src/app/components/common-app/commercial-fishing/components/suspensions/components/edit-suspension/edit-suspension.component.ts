import { AfterViewInit, Component, OnInit } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { forkJoin } from 'rxjs';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { SuspensionDataDTO } from '@app/models/generated/dtos/SuspensionDataDTO';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { SuspensionTypeNomenclatureDTO } from '@app/models/generated/dtos/SuspensionTypeNomenclatureDTO';
import { SuspensionReasonNomenclatureDTO } from '@app/models/generated/dtos/SuspensionReasonNomenclatureDTO';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { DateRangeData } from '@app/shared/components/input-controls/tl-date-range/tl-date-range.component';
import { CommercialFishingSuspensionTypesEnum } from '@app/enums/commercial-fishing-suspension-types.enum';
import { ISuspensionService } from '@app/interfaces/common-app/suspension.interface';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { SuspnesionDataDialogParams } from '@app/components/common-app/commercial-fishing/models/suspnesion-data-dialog-params.model';

@Component({
    selector: 'edit-suspension',
    templateUrl: './edit-suspension.component.html'
})
export class EditSuspensionComponent implements OnInit, AfterViewInit, IDialogComponent {
    public form!: FormGroup;
    public readOnly: boolean = false;
    public isPermit!: boolean;
    public isAdd: boolean = false;
    public showRangeControl: boolean = false;
    public showEnacmentDate: boolean = false;
    public showAdditionalControls: boolean = false;
    public suspensionTypes: SuspensionTypeNomenclatureDTO[] = [];
    public allReasons: SuspensionReasonNomenclatureDTO[] = [];
    public reasons: SuspensionReasonNomenclatureDTO[] = [];
    public suspensionNumberLabel: string = '';

    private model!: SuspensionDataDTO;
    private service!: ISuspensionService;
    private translate: FuseTranslationLoaderService;

    private postOnAdd: boolean = false;
    private pageCode!: PageCodeEnum;
    private recordId: number | undefined;
    private hasSuspensionValidToExistsError: boolean = false;

    public constructor(translate: FuseTranslationLoaderService) {
        this.translate = translate;

        this.suspensionNumberLabel = this.translate.getValue('suspensions.suspension-order-number');

        this.buildForm();
    }

    public async ngOnInit(): Promise<void> {
        const nomenclatures = await forkJoin([
            NomenclatureStore.instance.getNomenclature<number>(NomenclatureTypes.CommercialFishingSuspensionTypes, this.service.getSuspensionTypes.bind(this.service), false),
            NomenclatureStore.instance.getNomenclature<number>(NomenclatureTypes.CommercialFishingSuspensionReasons, this.service.getSuspensionReasons.bind(this.service), false)
        ]).toPromise();

        this.suspensionTypes = nomenclatures[0];
        this.allReasons = nomenclatures[1];

        this.suspensionTypes = this.suspensionTypes.filter(x => x.isPermit === this.isPermit);

        if (this.model !== null && this.model !== undefined) {
            this.fillForm();
        }
    }

    public ngAfterViewInit(): void {
        this.form.get('suspensionTypeControl')!.valueChanges.subscribe({
            next: (selectedSuspensionType: SuspensionTypeNomenclatureDTO) => {
                this.form.get('suspensionReasonControl')!.reset(null, { emitEvent: false });

                if (selectedSuspensionType !== null && selectedSuspensionType !== undefined) {
                    this.showAdditionalControls = true;
                    this.reasons = this.allReasons.filter(x => x.suspensionTypeId === selectedSuspensionType.value);

                    if (this.reasons.length === 1) {
                        this.form.get('suspensionReasonControl')!.setValue(this.reasons[0]);
                    }

                    if (selectedSuspensionType.code === CommercialFishingSuspensionTypesEnum[CommercialFishingSuspensionTypesEnum.PermanentLicense]
                        || selectedSuspensionType.code === CommercialFishingSuspensionTypesEnum[CommercialFishingSuspensionTypesEnum.PermenentWithdrawalPermit]
                        || selectedSuspensionType.code === CommercialFishingSuspensionTypesEnum[CommercialFishingSuspensionTypesEnum.OwnerRequest]
                    ) {
                        this.showRangeControl = false;
                        this.form.get('suspensionDateRangeControl')!.clearValidators();
                    }
                    else {
                        this.showRangeControl = true;
                        this.form.get('suspensionDateRangeControl')!.setValidators(Validators.required);
                    }

                    if (selectedSuspensionType.code === CommercialFishingSuspensionTypesEnum[CommercialFishingSuspensionTypesEnum.OwnerRequest]) {
                        this.showEnacmentDate = false;
                        this.suspensionNumberLabel = this.translate.getValue('suspensions.suspension-client-number');
                        this.form.get('enactmentDateControl')!.clearValidators();
                    }
                    else {
                        this.showEnacmentDate = true;
                        this.suspensionNumberLabel = this.translate.getValue('suspensions.suspension-order-number');
                        this.form.get('enactmentDateControl')!.setValidators(Validators.required);
                    }

                    this.form.get('suspensionDateRangeControl')!.markAsPending({ emitEvent: false });
                    this.form.get('suspensionDateRangeControl')!.updateValueAndValidity({ emitEvent: false });

                    this.form.get('enactmentDateControl')!.markAsPending({ emitEvent: false });
                    this.form.get('enactmentDateControl')!.updateValueAndValidity({ emitEvent: false });
                }
                else {
                    this.reasons = [];

                    this.showRangeControl = false;
                    this.showEnacmentDate = false;
                    this.showAdditionalControls = false;

                    this.form.get('suspensionDateRangeControl')!.clearValidators();
                    this.form.get('enactmentDateControl')!.clearValidators();

                    this.form.get('suspensionDateRangeControl')!.markAsPending({ emitEvent: false });
                    this.form.get('suspensionDateRangeControl')!.updateValueAndValidity({ emitEvent: false });

                    this.form.get('enactmentDateControl')!.markAsPending({ emitEvent: false });
                    this.form.get('enactmentDateControl')!.updateValueAndValidity({ emitEvent: false });
                }

                if (this.readOnly) {
                    this.form.get('suspensionDateRangeControl')!.disable();
                    this.form.get('enactmentDateControl')!.disable();
                }

                this.form.updateValueAndValidity({ emitEvent: false });
            }
        });

        this.form.get('suspensionReasonControl')!.valueChanges.subscribe({
            next: (value: SuspensionReasonNomenclatureDTO) => {
                const enactmentDate: Date | undefined = this.form.get('enactmentDateControl')!.value;
                let validFrom: Date;
                let validTo: Date | undefined = (this.form.get('suspensionDateRangeControl')!.value as DateRangeData)?.end;

                if (enactmentDate !== null && enactmentDate !== undefined) {
                    validFrom = new Date(enactmentDate);
                }
                else {
                    validFrom = new Date();
                }

                if (value !== null && value !== undefined) {
                    if (value.monthsDuration !== null && value.monthsDuration !== undefined) { // TODO при случай на окончателно прекратяване - не знам кога настъпва ???
                        validTo = new Date(validFrom);
                        validTo.setMonth(validFrom.getMonth() + value.monthsDuration);

                        this.form.get('suspensionDateRangeControl')!.setValue(new DateRangeData({ start: validFrom, end: validTo }));
                        this.form.get('suspensionDateRangeControl')!.disable();
                    }
                    else {
                        if (validFrom !== undefined && validFrom !== null && validTo !== undefined && validTo !== null) {
                            this.form.get('suspensionDateRangeControl')!.setValue(new DateRangeData({ start: validFrom, end: validTo }));
                        }

                        this.form.get('suspensionDateRangeControl')!.enable();
                    }
                }
                else {
                    if (this.form.get('suspensionDateRangeControl')!.disabled) {
                        if (validFrom !== undefined && validFrom !== null && validTo !== undefined && validTo !== null) {
                            this.form.get('suspensionDateRangeControl')!.setValue(new DateRangeData({ start: validFrom, end: validTo }));
                        }
                        else {
                            this.form.get('suspensionDateRangeControl')!.reset();
                        }

                        this.form.get('suspensionDateRangeControl')!.enable();
                    }
                }

                if (this.readOnly) {
                    this.form.get('suspensionDateRangeControl')!.disable();
                }
            }
        });

        this.form.get('enactmentDateControl')!.valueChanges.subscribe({
            next: (value: Date | undefined) => {
                this.form.get('suspensionReasonControl')!.updateValueAndValidity();
            }
        });
    }

    public setData(data: SuspnesionDataDialogParams, buttons: DialogWrapperData): void {
        this.readOnly = data.viewMode;
        this.isPermit = data.isPermit;
        this.service = data.service;
        this.postOnAdd = data.postOnAdd;
        this.pageCode = data.pageCode;
        this.recordId = data.recordId;

        if (data.model === null || data.model === undefined) {
            this.model = new SuspensionDataDTO({ isActive: true });
            this.isAdd = true;
        }
        else {
            this.isAdd = false;

            if (this.readOnly) {
                this.form.disable();
            }

            this.model = data.model;
            this.fillForm();
        }
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (this.readOnly) {
            dialogClose(this.model);
            return;
        }

        this.form.markAllAsTouched();

        if (this.form.valid) {
            this.fillModel();
            CommonUtils.sanitizeModelStrings(this.model);

            if (this.isAdd && this.postOnAdd) { // should save data in db
                this.service.addSuspension(this.model, this.recordId!, this.pageCode).subscribe({
                    next: () => {
                        dialogClose(this.model);
                    },
                    error: (errorResponse: HttpErrorResponse) => {
                        if ((errorResponse.error as ErrorModel)?.code === ErrorCode.PermitSuspensionValidToExists) {
                            this.hasSuspensionValidToExistsError = true;
                            this.form.updateValueAndValidity({ emitEvent: false });
                        }
                        else if ((errorResponse.error as ErrorModel)?.code === ErrorCode.PermitLicenseSuspensionValidToExists) {
                            this.hasSuspensionValidToExistsError = true;
                            this.form.updateValueAndValidity({ emitEvent: false });
                        }
                    }
                });
            }
            else { // the saving is left for the outher dialog
                dialogClose(this.model);
            }
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    private buildForm(): void {
        const date: Date = new Date();

        this.form = new FormGroup({
            suspensionTypeControl: new FormControl(undefined, Validators.required),
            suspensionReasonControl: new FormControl(undefined, Validators.required),
            suspensionDateRangeControl: new FormControl(undefined, Validators.required),
            orderNumberControl: new FormControl(undefined, Validators.required),
            enactmentDateControl: new FormControl(undefined, Validators.required)
        }, this.uniqueValidToValidator());

        this.form.get('suspensionTypeControl')!.valueChanges.subscribe({
            next: () => {
                this.hasSuspensionValidToExistsError = false;
                this.form.updateValueAndValidity({ emitEvent: false });
            }
        });

        this.form.get('suspensionDateRangeControl')!.valueChanges.subscribe({
            next: () => {
                this.hasSuspensionValidToExistsError = false;
                this.form.updateValueAndValidity({ emitEvent: false });
            }
        });
    }

    private fillForm(): void {
        this.form.get('suspensionTypeControl')!.setValue(this.suspensionTypes.find(x => x.value === this.model.suspensionTypeId)!);
        this.form.get('suspensionReasonControl')!.setValue(this.reasons.find(x => x.value === this.model.reasonId)!);
        this.form.get('suspensionDateRangeControl')!.setValue(new DateRangeData({ start: this.model.validFrom, end: this.model.validTo }));
        this.form.get('orderNumberControl')!.setValue(this.model.orderNumber);
        this.form.get('enactmentDateControl')!.setValue(this.model.enactmentDate);
    }

    private fillModel(): void {
        const selectedSuspensionType: SuspensionTypeNomenclatureDTO = this.form.get('suspensionTypeControl')!.value;
        this.model.suspensionTypeId = selectedSuspensionType.value;
        this.model.suspensionTypeName = selectedSuspensionType.displayName;

        const selectedReason: SuspensionReasonNomenclatureDTO = this.form.get('suspensionReasonControl')!.value;
        this.model.reasonId = selectedReason.value;
        this.model.reasonName = selectedReason.displayName;

        this.model.orderNumber = this.form.get('orderNumberControl')!.value;

        if (selectedSuspensionType.code === CommercialFishingSuspensionTypesEnum[CommercialFishingSuspensionTypesEnum.OwnerRequest]) {
            this.model.validFrom = undefined;
            this.model.validTo = undefined;
            this.model.enactmentDate = undefined;
        }
        else if (selectedSuspensionType.code === CommercialFishingSuspensionTypesEnum[CommercialFishingSuspensionTypesEnum.PermenentWithdrawalPermit]
            || selectedSuspensionType.code === CommercialFishingSuspensionTypesEnum[CommercialFishingSuspensionTypesEnum.PermanentLicense]
        ) {
            this.model.validFrom = undefined;
            this.model.validTo = undefined;
            this.model.enactmentDate = this.form.get('enactmentDateControl')!.value;
        }
        else if (selectedSuspensionType.code === CommercialFishingSuspensionTypesEnum[CommercialFishingSuspensionTypesEnum.TemporaryPermit]
            || selectedSuspensionType.code === CommercialFishingSuspensionTypesEnum[CommercialFishingSuspensionTypesEnum.TemporaryWithdrawalPermit]
            || selectedSuspensionType.code === CommercialFishingSuspensionTypesEnum[CommercialFishingSuspensionTypesEnum.TemporaryLicense]
            || selectedSuspensionType.code === CommercialFishingSuspensionTypesEnum[CommercialFishingSuspensionTypesEnum.PermitSuspendedLicense]
        ) {
            this.model.validFrom = (this.form.get('suspensionDateRangeControl')!.value as DateRangeData)?.start;
            this.model.validTo = (this.form.get('suspensionDateRangeControl')!.value as DateRangeData)?.end;

            const enactmentDate: Date = this.form.get('enactmentDateControl')!.value;
            if (enactmentDate !== undefined && enactmentDate !== null) {
                this.model.validFrom = enactmentDate;
                this.model.enactmentDate = enactmentDate;
            }
        }
    }

    private uniqueValidToValidator(): ValidatorFn {
        return (form: AbstractControl): ValidationErrors | null => {
            if (form === null || form === undefined) {
                return null;
            }

            if (this.hasSuspensionValidToExistsError) {
                return { 'suspensionValidToExists': true };
            }

            return null;
        }
    }
}