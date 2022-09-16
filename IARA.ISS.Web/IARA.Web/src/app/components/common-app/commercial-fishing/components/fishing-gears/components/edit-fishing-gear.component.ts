import { Component, Input, OnInit, Optional, Self, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, Validators } from '@angular/forms';
import { forkJoin, Subscription } from 'rxjs';

import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { FishingGearDTO } from '@app/models/generated/dtos/FishingGearDTO';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { FishingGearTypesEnum } from '@app/enums/fishing-gear-types.enum';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { FishingGearMarkDTO } from '@app/models/generated/dtos/FishingGearMarkDTO';
import { FishingGearPingerDTO } from '@app/models/generated/dtos/FishingGearPingerDTO';
import { FishingGearNomenclatureDTO } from '@app/models/generated/dtos/FishingGearNomenclatureDTO';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { FishingGearParameterTypesEnum } from '@app/enums/fishing-gear-parameter-types.enum';
import { EditFishingGearDialogParamsModel } from '../models/edit-fishing-gear-dialog-params.model';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { FormControlDataLoader } from '@app/shared/utils/form-control-data-loader';
import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { FishingGearMarkStatusesEnum } from '@app/enums/fishing-gear-mark-statuses.enum';

@Component({
    selector: 'edit-fishing-gear',
    templateUrl: './edit-fishing-gear.component.html'
})
export class EditFishingGearComponent extends CustomFormControl<FishingGearDTO | undefined> implements OnInit, IDialogComponent {
    public fishingGearTypes: FishingGearNomenclatureDTO[] = [];
    public fishingGearTypesEnum: typeof FishingGearTypesEnum = FishingGearTypesEnum;

    @Input()
    public isInspected: boolean = false;

    public marksForm?: FormGroup;
    public pingersForm?: FormGroup;

    public markStatuses: NomenclatureDTO<number>[] = [];
    public pingerStatuses: NomenclatureDTO<number>[] = [];

    public showPingersTable: boolean = false;
    public model!: FishingGearDTO;

    @ViewChild('marksTable')
    private marksTable!: TLDataTableComponent;

    @ViewChild('pingersTable')
    private pingersTable!: TLDataTableComponent;

    private markedStatus!: NomenclatureDTO<number>;
    private nomenclatures: CommonNomenclatures;
    private isDraft: boolean = false;
    private pageCode: PageCodeEnum | undefined;
    private readonly loader: FormControlDataLoader;

    public constructor(
        @Self() @Optional() ngControl: NgControl,
        commonNomenclatures: CommonNomenclatures
    ) {
        super(ngControl);

        this.nomenclatures = commonNomenclatures;
        this.loader = new FormControlDataLoader(this.getNomenclatures.bind(this));
    }

    public ngOnInit(): void {
        this.initCustomFormControl();

        this.loader.load(() => {
            if (this.pageCode === PageCodeEnum.PoundnetCommFishLic) {
                this.fishingGearTypes = this.fishingGearTypes.filter(x => x.type === FishingGearParameterTypesEnum.PoundNet).slice();
            }
            else if (this.pageCode === PageCodeEnum.CatchQuataSpecies) {
                this.fishingGearTypes = this.fishingGearTypes.filter(x => x.type === FishingGearParameterTypesEnum.Quota).slice();
            }

            this.fillForm();
        });
    }

    public setData(data: EditFishingGearDialogParamsModel, buttons: DialogWrapperData): void {
        this.isDisabled = data.readOnly;
        this.isDraft = data.isDraft;
        this.pageCode = data.pageCode;

        if (data.model === null || data.model === undefined) {
            this.model = new FishingGearDTO({ isActive: true });
        }
        else {
            if (this.isDisabled) {
                this.form.disable();
            }
            this.model = data.model;
        }
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (this.isDisabled) {
            dialogClose(this.model);
        }
        else if (this.isDraft) {
            this.fillModel();
            CommonUtils.sanitizeModelStrings(this.model);
            dialogClose(this.model);
        }
        else {
            this.form.markAllAsTouched();
            // TODO validate pingers here
            if (this.form.valid) {
                this.fillModel();
                CommonUtils.sanitizeModelStrings(this.model);
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

    public writeValue(value: FishingGearDTO | undefined): void {
        if (value === null || value === undefined) {
            this.model = new FishingGearDTO({ isActive: true });
        }
        else {
            this.model = value;
        }

        this.loader.load(() => {
            this.fillForm();
        });
    }

    public onEditMark(row: GridRow<FishingGearMarkDTO> | undefined): void {
        if (this.isInspected && (!(row !== null && row !== undefined) || row.data.statusId === this.markedStatus.value)) {
            this.marksForm!.get('statusIdControl')!.setValue(this.markedStatus.value);
            this.marksForm!.get('statusIdControlHidden')!.disable();
        }
        else {
            if (row === null || row === undefined) {
                const newMarkStatus: NomenclatureDTO<number> | undefined = this.markStatuses.find(x => x.code === FishingGearMarkStatusesEnum[FishingGearMarkStatusesEnum.NEW] && x.isActive);
                if (newMarkStatus !== null && newMarkStatus !== undefined) {
                    setTimeout(() => {
                        this.marksForm!.get('statusIdControl')!.setValue(newMarkStatus.value);
                        this.marksForm!.get('statusIdControlHidden')!.disable();
                    });
                }
            }
            else {
                this.marksForm!.get('statusIdControlHidden')!.enable();
            }
        }
    }

    public onEditedMark(): void {
        if (this.isInspected) {
            this.onChanged(this.getValue());
        }
    }

    public onEditedPinger(): void {
        if (this.isInspected) {
            this.onChanged(this.getValue());
        }
    }

    protected getValue(): FishingGearDTO | undefined {
        if (this.form.invalid) {
            if (this.model.typeId === null || this.model.typeId === undefined) {
                return undefined;
            }
            else {
                return this.model;
            }
        }

        this.fillModel();
        return this.model;
    }

    protected buildForm(): AbstractControl {
        const form = new FormGroup({
            typeControl: new FormControl(undefined, Validators.required),
            countControl: new FormControl(undefined),
            netEyeSizeControl: new FormControl(undefined, Validators.required),
            descriptionControl: new FormControl(undefined),
            hooksContControl: new FormControl(undefined),
            lengthControl: new FormControl(undefined),
            heightControl: new FormControl(undefined),
            towelLengthControl: new FormControl(undefined),
            houseLengthControl: new FormControl(undefined),
            houseWidthControl: new FormControl(undefined),
            hasPingersControl: new FormControl(undefined),
            cordThicknessControl: new FormControl(undefined),
        });

        this.marksForm = new FormGroup({
            numberControl: new FormControl(undefined, Validators.required),
            statusIdControl: new FormControl(undefined, Validators.required)
        });

        this.pingersForm = new FormGroup({
            numberControl: new FormControl(undefined, Validators.required),
            statusIdControl: new FormControl(undefined, Validators.required),
            modelControl: new FormControl(undefined, Validators.maxLength(500)),
            brandControl: new FormControl(undefined, Validators.maxLength(500)),
        });

        form.get('hasPingersControl')!.valueChanges.subscribe({
            next: (value: boolean) => {
                this.showPingersTable = value;
            }
        });

        form.get('typeControl')!.valueChanges.subscribe({
            next: (value: NomenclatureDTO<number> | string | undefined) => {
                if (value instanceof NomenclatureDTO) {
                    if (value.code !== FishingGearTypesEnum[FishingGearTypesEnum.DLN]) {
                        this.form.get('countControl')!.setValidators(Validators.required);
                        this.form.get('countControl')!.markAsPending();
                    }
                    else {
                        this.form.get('countControl')!.reset();
                        this.form.get('countControl')!.setValidators(null);
                        this.form.get('countControl')!.updateValueAndValidity();
                    }

                    if (this.pageCode === PageCodeEnum.CatchQuataSpecies) {
                        this.setQuotaGearLength();
                    }

                    if (this.isDisabled) {
                        this.form.get('countControl')!.disable();
                    }
                }
            }
        });

        return form;
    }

    private fillForm(): void {
        if (this.model.typeId !== null && this.model.typeId !== undefined) {
            const type: NomenclatureDTO<number> = this.fishingGearTypes.find(x => x.value === this.model.typeId)!;
            this.form.get('typeControl')!.setValue(type, { emitEvent: false });

            if (type instanceof NomenclatureDTO) {
                if (type.code !== FishingGearTypesEnum[FishingGearTypesEnum.DLN]) {
                    this.form.get('countControl')!.setValidators(Validators.required);
                    this.form.get('countControl')!.markAsPending();
                }
                else {
                    this.form.get('countControl')!.reset();
                    this.form.get('countControl')!.setValidators(null);
                    this.form.get('countControl')!.updateValueAndValidity();
                }

                if (this.pageCode === PageCodeEnum.CatchQuataSpecies) {
                    this.setQuotaGearLength();
                }

                if (this.isDisabled) {
                    this.form.get('countControl')!.disable();
                }
            }

            if (type.code !== FishingGearTypesEnum[FishingGearTypesEnum.DLN]) {
                this.form.get('countControl')!.setValue(this.model.count);
                this.form.get('hooksContControl')!.setValue(this.model.hookCount);
                this.form.get('lengthControl')!.setValue(this.model.length);
                this.form.get('heightControl')!.setValue(this.model.height);
                this.form.get('cordThicknessControl')!.setValue(this.model.cordThickness);
            }
            else {
                this.form.get('towelLengthControl')!.setValue(this.model.towelLength);
                this.form.get('houseLengthControl')!.setValue(this.model.houseLength);
                this.form.get('houseWidthControl')!.setValue(this.model.houseWidth);
            }
        }

        this.form.get('netEyeSizeControl')!.setValue(this.model.netEyeSize);
        this.form.get('descriptionControl')!.setValue(this.model.description);
        this.form.get('hasPingersControl')!.setValue(this.model.hasPingers);
    }

    private fillModel(): void {
        const type: NomenclatureDTO<number> = this.form.get('typeControl')!.value;

        //if (this.ngControl !== null && this.ngControl !== undefined) {
        //    if (type === null || type === undefined) {
        //        return;
        //    }
        //}

        this.model.typeId = type.value;
        this.model.type = type.displayName;

        this.model.netEyeSize = this.form.get('netEyeSizeControl')!.value;
        this.model.description = this.form.get('descriptionControl')!.value;
        this.model.hasPingers = this.form.get('hasPingersControl')!.value;

        if (type.code !== FishingGearTypesEnum[FishingGearTypesEnum.DLN]) {
            this.model.count = this.form.get('countControl')!.value ?? 0;
            this.model.hookCount = this.form.get('hooksContControl')!.value;
            this.model.length = this.form.get('lengthControl')!.value;
            this.model.height = this.form.get('heightControl')!.value;
            this.model.cordThickness = this.form.get('cordThicknessControl')!.value;
        }
        else {
            this.model.towelLength = this.form.get('towelLengthControl')!.value;
            this.model.houseLength = this.form.get('houseLengthControl')!.value;
            this.model.houseWidth = this.form.get('houseWidthControl')!.value;
        }

        if (this.marksTable !== null && this.marksTable !== undefined && (!this.isInspected || this.marksTable.rows.length > 0)) {
            this.model.marks = this.marksTable.rows.map((row: FishingGearMarkDTO) => {
                return new FishingGearMarkDTO({
                    id: row.id,
                    number: row.number,
                    statusId: row.statusId,
                    selectedStatus: FishingGearMarkStatusesEnum[this.markStatuses.find(x => x.value === row.statusId)?.code as keyof typeof FishingGearMarkStatusesEnum],
                    isActive: row.isActive ?? true
                });
            });
        }

        this.model.marksNumbers = '';
        if (this.model.marks !== null && this.model.marks !== undefined && this.model.marks.length > 0) {
            for (const mark of this.model.marks) {
                if (mark.number !== null && mark.number !== undefined) {
                    this.model.marksNumbers = this.model.marksNumbers?.concat(`${mark.number};`) ?? '';
                }
            }
        }

        if (this.form.get('hasPingersControl')!.value === true) {
            if (this.pingersTable !== null && this.pingersTable !== undefined) {
                this.model.pingers = this.pingersTable.rows.map((row: FishingGearPingerDTO) => {
                    return new FishingGearPingerDTO({
                        id: row.id,
                        number: row.number,
                        statusId: row.statusId,
                        isActive: row.isActive ?? true,
                        model: row.model,
                        brand: row.brand,
                    });
                });
            }
        }
    }

    private setQuotaGearLength(): void {
        if (this.model.id === null || this.model.id === undefined) {
            this.form.get('netEyeSizeControl')!.setValue(400);
        }

        this.form.get('netEyeSizeControl')!.disable();
    }

    private getNomenclatures(): Subscription {
        const subscription: Subscription = forkJoin(
            NomenclatureStore.instance.getNomenclature<number>(
                NomenclatureTypes.FishingGear, this.nomenclatures.getFishingGear.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature<number>(
                NomenclatureTypes.FishingGearMarkStatuses, this.nomenclatures.getFishingGearMarkStatuses.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature<number>(
                NomenclatureTypes.FishingGearPingerStatuses, this.nomenclatures.getFishingGearPingerStatuses.bind(this.nomenclatures), false)
        ).subscribe((nomenclatures: NomenclatureDTO<number>[][]) => {
            this.fishingGearTypes = nomenclatures[0];
            this.markedStatus = nomenclatures[1].find(f => f.code === FishingGearMarkStatusesEnum[FishingGearMarkStatusesEnum.MARKED])!;
            this.markStatuses = nomenclatures[1];
            this.pingerStatuses = nomenclatures[2];

            this.loader.complete();
        });

        return subscription;
    }
}