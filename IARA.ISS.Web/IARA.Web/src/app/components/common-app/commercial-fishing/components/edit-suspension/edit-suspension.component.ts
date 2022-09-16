import { AfterViewInit, Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { forkJoin } from 'rxjs';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { SuspensionDataDTO } from '@app/models/generated/dtos/SuspensionDataDTO';
import { SuspnesionDataDialogParams } from '../../models/suspnesion-data-dialog-params.model';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { SuspensionTypeNomenclatureDTO } from '@app/models/generated/dtos/SuspensionTypeNomenclatureDTO';
import { SuspensionReasonNomenclatureDTO } from '@app/models/generated/dtos/SuspensionReasonNomenclatureDTO';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { ICommercialFishingService } from '@app/interfaces/common-app/commercial-fishing.interface';
import { DateRangeData } from '@app/shared/components/input-controls/tl-date-range/tl-date-range.component';
import { CommercialFishingSuspensionTypesEnum } from '@app/enums/commercial-fishing-suspension-types.enum';


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
    public suspensionTypes: SuspensionTypeNomenclatureDTO[] = [];
    public allReasons: SuspensionReasonNomenclatureDTO[] = [];
    public reasons: SuspensionReasonNomenclatureDTO[] = [];

    private model!: SuspensionDataDTO;
    private service!: ICommercialFishingService;

    public constructor(translate: FuseTranslationLoaderService) {
        this.buildForm();
    }

    public async ngOnInit(): Promise<void> {
        const nomenclatures = await forkJoin(
            NomenclatureStore.instance.getNomenclature<number>(NomenclatureTypes.CommercialFishingSuspensionTypes, this.service.getSuspensionTypes.bind(this.service), false),
            NomenclatureStore.instance.getNomenclature<number>(NomenclatureTypes.CommercialFishingSuspensionReasons, this.service.getSuspensionReasons.bind(this.service), false)
        ).toPromise();

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
                    this.reasons = this.allReasons.filter(x => x.suspensionTypeId === selectedSuspensionType.value);

                    if (this.reasons.length === 1) {
                        this.form.get('suspensionReasonControl')!.setValue(this.reasons[0]);
                    }

                    if (selectedSuspensionType.code === CommercialFishingSuspensionTypesEnum[CommercialFishingSuspensionTypesEnum.PermanentLicense]
                        || selectedSuspensionType.code === CommercialFishingSuspensionTypesEnum[CommercialFishingSuspensionTypesEnum.PermenentWithdrawalPermit]) {
                        this.showRangeControl = false;
                        this.form.get('suspensionDateRangeControl')!.clearValidators();
                    }
                    else {
                        this.showRangeControl = true;
                        this.form.get('suspensionDateRangeControl')!.setValidators(Validators.required);
                    }

                    this.form.get('suspensionDateRangeControl')!.updateValueAndValidity();
                }
                else {
                    this.reasons = [];
                    this.showRangeControl = false;
                    this.form.get('suspensionDateRangeControl')!.clearValidators();
                    this.form.get('suspensionDateRangeControl')!.updateValueAndValidity();
                }
            }
        });

        this.form.get('suspensionReasonControl')!.valueChanges.subscribe({
            next: (value: SuspensionReasonNomenclatureDTO) => {
                if (value !== null && value !== undefined) {
                    if (value.monthsDuration !== null && value.monthsDuration !== undefined) { // TODO при случай на окончателно прекратяване - не знам кога настъпва ???
                        const now: Date = new Date();
                        const validTo = new Date();
                        validTo.setMonth(now.getMonth() + value.monthsDuration);
                        this.form.get('suspensionDateRangeControl')!.setValue(new DateRangeData({ start: now, end: validTo }));
                        this.form.get('suspensionDateRangeControl')!.disable();
                    }
                    else {
                        this.form.get('suspensionDateRangeControl')!.enable();
                    }
                }
                else {
                    if (this.form.get('suspensionDateRangeControl')!.disabled) {
                        this.form.get('suspensionDateRangeControl')!.reset();
                        this.form.get('suspensionDateRangeControl')!.enable();
                    }
                }
            }
        });
    }

    public setData(data: SuspnesionDataDialogParams, buttons: DialogWrapperData): void {
        this.readOnly = data.viewMode;
        this.isPermit = data.isPermit;
        this.service = data.service;

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
            dialogClose(this.model);
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

        this.model.validFrom = (this.form.get('suspensionDateRangeControl')!.value as DateRangeData)?.start;
        this.model.validTo = (this.form.get('suspensionDateRangeControl')!.value as DateRangeData)?.end;
        this.model.orderNumber = this.form.get('orderNumberControl')!.value;
        this.model.enactmentDate = this.form.get('enactmentDateControl')!.value;
    }
}