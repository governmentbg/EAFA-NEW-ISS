import { AfterViewInit, Component, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { DatePipe } from '@angular/common';

import { Moment } from 'moment';
import moment from 'moment';

import { CatchRecordDialogParamsModel } from '@app/components/common-app/catches-and-sales/models/catch-record-dialog-params.model';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { CatchRecordDTO } from '@app/models/generated/dtos/CatchRecordDTO';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { ICatchesAndSalesService } from '@app/interfaces/common-app/catches-and-sales.interface';
import { DateDifference } from '@app/models/common/date-difference.model';
import { DateUtils } from '@app/shared/utils/date.utils';
import { TLDateDifferencePipe } from '@app/shared/pipes/tl-date-difference.pipe';
import { WaterTypesEnum } from '@app/enums/water-types.enum';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';
import { CatchRecordFishDTO } from '@app/models/generated/dtos/CatchRecordFishDTO';
import { ShipLogBookPageDataService } from '../ship-log-book/services/ship-log-book-page-data.service';
import { GetControlErrorLabelTextCallback } from '@app/shared/components/input-controls/base-tl-control';

@Component({
    selector: 'edit-catch-record',
    templateUrl: './edit-catch-record.component.html'
})
export class EditCatchRecordComponent implements AfterViewInit, IDialogComponent {
    public form!: FormGroup;
    public viewMode!: boolean;
    public model!: CatchRecordDTO;
    public service!: ICatchesAndSalesService;
    public shipLogBookPageDataService!: ShipLogBookPageDataService;
    public waterType!: WaterTypesEnum;
    public permitLicenseAquaticOrganismTypeIds: number[] = [];

    public tripStartDateTime: Date | undefined;
    public tripEndDateTime: Date | undefined;

    public getControlErrorLabelTextMethod: GetControlErrorLabelTextCallback = this.getControlErrorLabelText.bind(this);

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    private isAdd: boolean = false;

    private readonly dateDifferencePipe: TLDateDifferencePipe;
    private readonly translationService: FuseTranslationLoaderService;
    private readonly datePipe: DatePipe;

    public constructor(
        dateDifferencePipe: TLDateDifferencePipe,
        translationService: FuseTranslationLoaderService,
        datePipe: DatePipe
    ) {
        this.dateDifferencePipe = dateDifferencePipe;
        this.translationService = translationService;
        this.datePipe = datePipe;

        this.buildForm();
    }

    public ngAfterViewInit(): void {
        if (this.isAdd) {
            this.form.get('aquaticOrganismTypesControl')!.setValue([
                new CatchRecordFishDTO({
                    id: this.shipLogBookPageDataService.nextNewCatchRecordId,
                    isActive: true,
                    unloadedQuantityKg: 0,
                    unloadedInOtherTripQuantityKg: 0
                })
            ]);
        }
        else {
            this.fillForm();
        }

        this.form.get('gearEntryDateTimeControl')!.valueChanges.subscribe({
            next: (entryDate: Moment | null | undefined) => {
                this.onGearEntryDateTimeChanged(entryDate);
            }
        });

        this.form.get('gearExitDateTimeControl')!.valueChanges.subscribe({
            next: (exitDate: Moment | null | undefined) => {
                this.onGearExitDateTimeChanged(exitDate);
            }
        });
    }

    public setData(data: CatchRecordDialogParamsModel, buttons: DialogWrapperData): void {
        this.viewMode = data.viewMode;
        this.service = data.service;
        this.waterType = data.waterType;
        this.permitLicenseAquaticOrganismTypeIds = data.permitLicenseAquaticOrganismTypeIds;
        this.shipLogBookPageDataService = data.shipLogBookPageDataService;
        this.tripStartDateTime = data.tripStartDateTime;
        this.tripEndDateTime = data.tripEndDateTime;

        this.setGearEntryExitMinMaxValidators();

        if (data.model === null || data.model === undefined) {
            this.isAdd = true;
            this.model = new CatchRecordDTO({
                isActive: true,
                hasGearEntry: true,
                hasGearExit: true
            });
        }
        else {
            if (this.viewMode) {
                this.form.disable();
            }

            this.isAdd = false;
            this.model = data.model;
        }
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (this.viewMode) {
            dialogClose(this.model);
            return;
        }

        this.form.markAllAsTouched();
        this.validityCheckerGroup.validate();

        if (this.form.valid) {
            this.fillModel();
            CommonUtils.sanitizeModelStrings(this.model);
            dialogClose(this.model);
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public getControlErrorLabelText(controlName: string, errorValue: unknown, errorCode: string): TLError | undefined {
        if (controlName === 'gearEntryDateTimeControl') {
            if (errorCode === 'matDatetimePickerParse') {
                const message: string = `${this.translationService.getValue('validation.pattern')}: ${this.translationService.getValue('common.date-time-control-format-hint')}`;
                return new TLError({ text: message });
            }
            else if (errorCode === 'required') {
                const message: string = `${this.translationService.getValue('validation.required')} (${this.translationService.getValue('catches-and-sales.with-pattern')}: ${this.translationService.getValue('common.date-time-control-format-hint')})`;
                return new TLError({ text: message });
            }
            else if (errorCode === 'matDatetimePickerMax') {
                if (this.form.get('gearExitDateTimeControl')!.value !== null && this.form.get('gearExitDateTimeControl')!.value !== undefined) {
                    const maxDate: Date = (this.form.get('gearExitDateTimeControl')!.value as Moment).toDate();
                    const dateString: string = this.datePipe.transform(maxDate, 'dd.MM.YYYY HH:mm') ?? "";
                    return new TLError({ text: `${this.translationService.getValue('validation.max')}: ${dateString}` });
                }
            }
        }
        else if (controlName === 'gearExitDateTimeControl') {
            if (errorCode === 'matDatetimePickerParse') {
                const message: string = `${this.translationService.getValue('validation.pattern')}: ${this.translationService.getValue('common.date-time-control-format-hint')}`;
                return new TLError({ text: message });
            }
            else if (errorCode === 'required') {
                const message: string = `${this.translationService.getValue('validation.required')} (${this.translationService.getValue('catches-and-sales.with-pattern')}: ${this.translationService.getValue('common.date-time-control-format-hint')})`;
                return new TLError({ text: message });
            }
            else if (errorCode === 'matDatetimePickerMin') {
                if (this.form.get('gearEntryDateTimeControl')!.value !== null && this.form.get('gearEntryDateTimeControl')!.value !== undefined) {
                    const maxDate: Date = (this.form.get('gearEntryDateTimeControl')!.value as Moment).toDate();
                    const dateString: string = this.datePipe.transform(maxDate, 'dd.MM.YYYY HH:mm') ?? "";
                    return new TLError({ text: `${this.translationService.getValue('validation.min')}: ${dateString}` });
                }
            }
        }
        else if (controlName === 'totalTimeControl') {
            if (errorCode === 'incorrectTimeDifference') {
                const messageText: string = this.translationService.getValue('catches-and-sales.ship-page-catch-gear-entry-exit-difference-error');
                return new TLError({ text: messageText });
            }
        }

        return undefined;
    }

    private buildForm(): void {
        this.form = new FormGroup({
            gearEntryDateTimeControl: new FormControl(undefined, Validators.required),
            hasNoGearEntryControl: new FormControl(false),
            gearExitDateTimeControl: new FormControl(undefined, Validators.required),
            hasNoGearExitControl: new FormControl(false),
            totalTimeControl: new FormControl(undefined, this.totalTimeGreaterThanZeroValidator()),
            catchOperationsCountControl: new FormControl(undefined, [Validators.required, TLValidators.number(0)]),
            depthControl: new FormControl(undefined, Validators.required),

            aquaticOrganismTypesControl: new FormControl()
        });

        this.form.get('hasNoGearEntryControl')!.valueChanges.subscribe({
            next: (value: boolean) => {
                this.onHasNoGearEntryChanged(value);
            }
        });

        this.form.get('hasNoGearExitControl')!.valueChanges.subscribe({
            next: (value: boolean) => {
                this.onHasNoGearExitChanged(value);
            }
        });
    }

    private fillForm(): void {
        let gearEntryDateTime: Moment | undefined = undefined;
        if (this.model.gearEntryTime !== null && this.model.gearEntryTime !== undefined) {
            gearEntryDateTime = moment(this.model.gearEntryTime);
        }
        this.form.get('gearEntryDateTimeControl')!.setValue(gearEntryDateTime);

        const hasNoGearEntry: boolean = !(this.model.hasGearEntry ?? false);
        this.form.get('hasNoGearEntryControl')!.setValue(hasNoGearEntry);

        let gearExitDateTime: Moment | undefined = undefined;
        if (this.model.gearExitTime !== null && this.model.gearExitTime !== undefined) {
            gearExitDateTime = moment(this.model.gearExitTime);
        }
        this.form.get('gearExitDateTimeControl')!.setValue(gearExitDateTime);

        const hasNoGearExit: boolean = !(this.model.hasGearExit ?? false);
        this.form.get('hasNoGearExitControl')!.setValue(hasNoGearExit);

        this.form.get('catchOperationsCountControl')!.setValue(this.model.catchOperationsCount);
        this.form.get('depthControl')!.setValue(this.model.depth);

        this.form.get('aquaticOrganismTypesControl')!.setValue(this.model.catchRecordFishes);

        this.calculateAndChangeTotalTime();
    }

    private fillModel(): void {
        this.model.gearEntryTime = this.form.get('gearEntryDateTimeControl')!.value;
        this.model.hasGearEntry = !(this.form.get('hasNoGearEntryControl')!.value ?? false);
        this.model.gearExitTime = this.form.get('gearExitDateTimeControl')!.value;
        this.model.hasGearExit = !(this.form.get('hasNoGearExitControl')!.value ?? false);
        this.model.catchOperationsCount = this.form.get('catchOperationsCountControl')!.value;
        this.model.depth = this.form.get('depthControl')!.value;

        this.model.catchRecordFishes = this.form.get('aquaticOrganismTypesControl')!.value;
    }

    private setGearEntryExitMinMaxValidators(): void {
        if (this.tripStartDateTime !== null && this.tripStartDateTime !== undefined) {
            const gearEntyValidator: ValidatorFn | null = this.form.get('gearEntryDateTimeControl')!.validator;

            if (gearEntyValidator !== null && gearEntyValidator !== undefined) {
                this.form.get('gearEntryDateTimeControl')!.setValidators([
                    gearEntyValidator,
                    TLValidators.minDate(undefined, this.tripStartDateTime),
                    TLValidators.maxDate(this.form.get('gearExitDateTimeControl')!)
                ]);

                this.form.get('gearEntryDateTimeControl')!.markAsPending({ emitEvent: false });
            }
            else {
                this.form.get('gearEntryDateTimeControl')!.setValidators([
                    TLValidators.minDate(undefined, this.tripStartDateTime),
                    TLValidators.maxDate(this.form.get('gearExitDateTimeControl')!)
                ]);
            }
        }

        if (this.tripEndDateTime !== null && this.tripEndDateTime !== undefined) {
            const gearExitValidator: ValidatorFn | null = this.form.get('gearExitDateTimeControl')!.validator;

            if (gearExitValidator !== null && gearExitValidator !== undefined) {
                this.form.get('gearExitDateTimeControl')!.setValidators([
                    gearExitValidator,
                    TLValidators.minDate(this.form.get('gearEntryDateTimeControl')!),
                    TLValidators.maxDate(undefined, this.tripEndDateTime)
                ]);

                this.form.get('gearExitDateTimeControl')!.markAsPending({ emitEvent: false });
            }
            else {
                this.form.get('gearExitDateTimeControl')!.setValidators([
                    TLValidators.maxDate(undefined, this.tripEndDateTime),
                    TLValidators.minDate(this.form.get('gearEntryDateTimeControl')!)
                ]);
            }
        }
    }

    private onGearEntryDateTimeChanged(gearEntryDateTime: Moment | null | undefined): void {
        if (gearEntryDateTime !== null && gearEntryDateTime !== undefined) {
            const gearExitDateTime: Moment | null | undefined = this.form.get('gearExitDateTimeControl')!.value;

            if (gearExitDateTime === null || gearExitDateTime === undefined) {
                this.form.get('gearExitDateTimeControl')!.setValue(gearEntryDateTime);
            }
        }

        this.form.get('gearExitDateTimeControl')!.updateValueAndValidity({ emitEvent: false });
        this.form.get('gearExitDateTimeControl')!.markAsTouched();

        this.calculateAndChangeTotalTime();
    }

    private onGearExitDateTimeChanged(gearExitDateTime: Moment | null | undefined): void {
        if (gearExitDateTime !== null && gearExitDateTime !== undefined) {
            const gearEntryDateTime: Moment | null | undefined = this.form.get('gearEntryDateTimeControl')!.value;

            if (gearEntryDateTime === null || gearEntryDateTime === undefined) {
                this.form.get('gearEntryDateTimeControl')!.setValue(gearExitDateTime);
            }
        }

        this.form.get('gearEntryDateTimeControl')!.updateValueAndValidity({ emitEvent: false });
        this.form.get('gearEntryDateTimeControl')!.markAsTouched();

        this.calculateAndChangeTotalTime();
    }

    private calculateAndChangeTotalTime(): void {
        const difference: DateDifference | undefined = this.calculateTotalTime();
        this.form.get('totalTimeControl')!.setValue(this.dateDifferencePipe.transform(difference));
        this.form.get('totalTimeControl')!.markAsTouched();
    }

    private calculateTotalTime(): DateDifference | undefined {
        const entryDate: Moment | null | undefined = this.form.get('gearEntryDateTimeControl')!.value;
        const exitDate: Moment | null | undefined = this.form.get('gearExitDateTimeControl')!.value;

        if (entryDate !== null && entryDate !== undefined && exitDate !== null && exitDate !== undefined) {
            const difference: DateDifference | undefined = DateUtils.getDateDifference(entryDate.toDate(), exitDate.toDate());

            return difference;
        }
        else {
            return undefined;
        }
    }

    private onHasNoGearEntryChanged(value: boolean | undefined): void {
        if (value === true) {
            this.form.get('gearEntryDateTimeControl')!.setValue(undefined);
            this.form.get('gearEntryDateTimeControl')!.clearValidators();
        }
        else {
            this.form.get('gearEntryDateTimeControl')!.setValidators(Validators.required);
        }

        this.form.get('gearEntryDateTimeControl')!.markAsPending({ emitEvent: false });
        this.setGearEntryExitMinMaxValidators();
    }

    private onHasNoGearExitChanged(value: boolean | undefined): void {
        if (value === true) {
            this.form.get('gearExitDateTimeControl')!.setValue(undefined);
            this.form.get('gearExitDateTimeControl')!.clearValidators();
        }
        else {
            this.form.get('gearExitDateTimeControl')!.setValidators(Validators.required);
        }

        this.form.get('gearExitDateTimeControl')!.markAsPending({ emitEvent: false });
        this.setGearEntryExitMinMaxValidators();

        this.form.get('aquaticOrganismTypesControl')!.setValue(undefined);
    }

    private totalTimeGreaterThanZeroValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (control === null || control === undefined) {
                return null;
            }

            if (this.form === null || this.form === undefined) {
                return null;
            }

            const difference: DateDifference | undefined = this.calculateTotalTime();

            if (difference === null || difference === undefined) { // ако самият обект е невалиден
                return null;
            }

            if ((difference.days === null || difference.days === undefined)
                && (difference.hours === null || difference.hours === undefined)
                && (difference.minutes === null || difference.minutes === undefined)
            ) { // ако всички разлики са невалидни
                return { incorrectTimeDifference: true };
            }

            if (difference.days === 0 && difference.hours === 0 && difference.minutes === 0) {
                return { incorrectTimeDifference: true };
            }

            if (difference?.days !== null && difference?.days !== undefined && difference.days < 0) {
                return { incorrectTimeDifference: true };
            }

            if (difference?.hours !== null && difference?.hours !== undefined && difference.hours < 0) {
                return { incorrectTimeDifference: true };
            }

            if (difference?.minutes !== null && difference?.minutes !== undefined && difference.minutes < 0) {
                return { incorrectTimeDifference: true };
            }

            if ((difference.days === null || difference.days === undefined)
                && (difference.hours === null || difference.hours === undefined)
                && difference.minutes !== null
                && difference.minutes !== undefined
                && difference.minutes < 1) {
                return { incorrectTimeDifference: true };
            }

            return null;
        }
    }
}