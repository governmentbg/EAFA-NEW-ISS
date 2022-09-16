import { AfterViewInit, Component, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Moment } from 'moment';
import moment from 'moment';
import { DatePipe } from '@angular/common';

import { CatchRecordDialogParamsModel } from '../../models/catch-record-dialog-params.model';
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


@Component({
    selector: 'edit-catch-record',
    templateUrl: './edit-catch-record.component.html'
})
export class EditCatchRecordComponent implements AfterViewInit, IDialogComponent {
    public form!: FormGroup;
    public viewMode!: boolean;
    public model!: CatchRecordDTO;
    public service!: ICatchesAndSalesService;
    public waterType!: WaterTypesEnum;

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    private dateDifferencePipe: TLDateDifferencePipe;
    private translationService: FuseTranslationLoaderService;
    private datePipe: DatePipe;
    private isAdd: boolean = false;

    public constructor(dateDifferencePipe: TLDateDifferencePipe, translationService: FuseTranslationLoaderService, datePipe: DatePipe) {
        this.dateDifferencePipe = dateDifferencePipe;
        this.translationService = translationService;
        this.datePipe = datePipe;

        this.buildForm();
    }

    public ngAfterViewInit(): void {
        this.form.get('gearEntryDateTimeControl')!.valueChanges.subscribe({
            next: (entryDate: Moment | null | undefined) => {
                if (entryDate !== null && entryDate !== undefined) {
                    const exitDate: Moment | null | undefined = this.form.get('gearExitDateTimeControl')!.value;
                    if (exitDate !== null && exitDate !== undefined) {
                        const difference: DateDifference | undefined = DateUtils.getDateDifference(entryDate.toDate(), exitDate.toDate());
                        this.form.get('totalTimeControl')!.setValue(this.dateDifferencePipe.transform(difference));
                    }
                    else {
                        this.form.get('totalTimeControl')!.setValue(undefined);
                    }
                }
                else {
                    this.form.get('totalTimeControl')!.setValue(undefined);
                }
            }
        });

        this.form.get('gearExitDateTimeControl')!.valueChanges.subscribe({
            next: (exitDate: Moment | null | undefined) => {
                if (exitDate !== null && exitDate !== undefined) {
                    const entryDate: Moment | null | undefined = this.form.get('gearEntryDateTimeControl')!.value;
                    if (entryDate !== null && entryDate !== undefined) {
                        const difference: DateDifference | undefined = DateUtils.getDateDifference(entryDate.toDate(), exitDate.toDate());
                        this.form.get('totalTimeControl')!.setValue(this.dateDifferencePipe.transform(difference));
                    }
                    else {
                        this.form.get('totalTimeControl')!.setValue(undefined);
                    }
                }
                else {
                    this.form.get('totalTimeControl')!.setValue(undefined);
                }
            }
        });

        if (this.isAdd) {
            this.form.get('aquaticOrganismTypesControl')!.setValue([new CatchRecordFishDTO({ isActive: true })]);
        }
        else {
            this.fillForm();
        }
    }

    public setData(data: CatchRecordDialogParamsModel, buttons: DialogWrapperData): void {
        this.viewMode = data.viewMode;
        this.service = data.service;
        this.waterType = data.waterType;

        if (data.model === null || data.model === undefined) {
            this.isAdd = true;
            this.model = new CatchRecordDTO({ isActive: true });
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

        return undefined;
    }

    private fillForm(): void {
        let gearEntryDateTime: Moment | undefined = undefined;
        if (this.model.gearEntryTime !== null && this.model.gearEntryTime !== undefined) {
            gearEntryDateTime = moment(this.model.gearEntryTime);
        }
        this.form.get('gearEntryDateTimeControl')!.setValue(gearEntryDateTime);

        let gearExitDateTime: Moment | undefined = undefined;
        if (this.model.gearExitTime !== null && this.model.gearExitTime !== undefined) {
            gearExitDateTime = moment(this.model.gearExitTime);
        }
        this.form.get('gearExitDateTimeControl')!.setValue(gearExitDateTime);

        this.form.get('catchOperationsCountControl')!.setValue(this.model.catchOperationsCount);
        this.form.get('depthControl')!.setValue(this.model.depth);

        this.form.get('aquaticOrganismTypesControl')!.setValue(this.model.catchRecordFishes);
    }

    private buildForm(): void {
        this.form = new FormGroup({
            gearEntryDateTimeControl: new FormControl(null, Validators.required),
            gearExitDateTimeControl: new FormControl(null, Validators.required),
            totalTimeControl: new FormControl(null),
            catchOperationsCountControl: new FormControl(null, [Validators.required, TLValidators.number(0)]),
            depthControl: new FormControl(null, Validators.required),

            aquaticOrganismTypesControl: new FormControl()
        });
    }

    private fillModel(): void {
        this.model.gearEntryTime = this.form.get('gearEntryDateTimeControl')!.value;
        this.model.gearExitTime = this.form.get('gearExitDateTimeControl')!.value;
        this.model.catchOperationsCount = this.form.get('catchOperationsCountControl')!.value;
        this.model.depth = this.form.get('depthControl')!.value;

        this.model.catchRecordFishes = this.form.get('aquaticOrganismTypesControl')!.value;
    }
}