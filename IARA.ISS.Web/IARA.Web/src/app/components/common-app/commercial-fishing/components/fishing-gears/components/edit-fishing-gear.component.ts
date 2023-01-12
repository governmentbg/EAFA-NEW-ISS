import { Component, EventEmitter, Input, OnInit, Optional, Output, Self, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
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
import { FishingGearPingerStatusesEnum } from '@app/enums/fishing-gear-pinger-statuses.enum';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { RangeInputData } from '@app/shared/components/input-controls/tl-range-input/range-input.component';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { GenerateMarksComponent } from './generate-marks/generate-marks.component';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { MarksRangeData } from '../models/marks-range.model';

const MARK_NUMBERS_RANGE_WARNING_DIFFERENCE = 100;

@Component({
    selector: 'edit-fishing-gear',
    templateUrl: './edit-fishing-gear.component.html'
})
export class EditFishingGearComponent extends CustomFormControl<FishingGearDTO | undefined> implements OnInit, IDialogComponent {
    public fishingGearTypes: FishingGearNomenclatureDTO[] = [];
    public fishingGearTypesEnum: typeof FishingGearTypesEnum = FishingGearTypesEnum;

    @Input()
    public isInspected: boolean = false;

    @Input()
    public hasMarks: boolean = true;

    @Input()
    public hasPingers: boolean = true;

    @Input()
    public canSelectMarks: boolean = false;

    @Input()
    public marksPerPage: number = 5;

    @Input()
    public pingersPerPage: number = 5;

    @Output()
    public selectedMark = new EventEmitter<FishingGearMarkDTO>();

    public marksForm!: FormGroup;
    public pingersForm!: FormGroup;

    public markStatuses: NomenclatureDTO<number>[] = [];
    public pingerStatuses: NomenclatureDTO<number>[] = [];

    public marks: FishingGearMarkDTO[] = [];
    public pingers: FishingGearPingerDTO[] = [];

    public showPingersTable: boolean = false;
    public selectedGearTypeIsPoundNet: boolean = false;

    @ViewChild('marksTable')
    private marksTable!: TLDataTableComponent;

    @ViewChild('pingersTable')
    private pingersTable!: TLDataTableComponent;

    private markedStatus!: NomenclatureDTO<number>;
    private nomenclatures: CommonNomenclatures;
    private pageCode: PageCodeEnum | undefined;
    private model!: FishingGearDTO;

    private readonly loader: FormControlDataLoader;
    private readonly confirmationDialog: TLConfirmDialog;
    private readonly translateService: FuseTranslationLoaderService;
    private readonly generateMarksDialog: TLMatDialog<GenerateMarksComponent>;

    public constructor(
        @Self() @Optional() ngControl: NgControl,
        commonNomenclatures: CommonNomenclatures,
        confirmationDialog: TLConfirmDialog,
        translateService: FuseTranslationLoaderService,
        generateMarksDialog: TLMatDialog<GenerateMarksComponent>
    ) {
        super(ngControl);

        this.nomenclatures = commonNomenclatures;
        this.confirmationDialog = confirmationDialog;
        this.translateService = translateService;
        this.generateMarksDialog = generateMarksDialog;

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
            dialogClose();
        }
        else {
            this.form.markAllAsTouched();
            this.form.updateValueAndValidity({ emitEvent: false });

            this.marksForm.updateValueAndValidity({ emitEvent: false });
            this.pingersForm.updateValueAndValidity({ emitEvent: false });

            if (this.isFormValid()) {
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

    public openGenerateMarksDialog(): void {
        this.generateMarksDialog.openWithTwoButtons({
            TCtor: GenerateMarksComponent,
            translteService: this.translateService,
            title: this.translateService.getValue('fishing-gears.generate-marks-dialog-title'),
            headerCancelButton: { cancelBtnClicked: (closeFn: HeaderCloseFunction) => { closeFn(); } },
            saveBtn: {
                color: 'accent',
                id: 'save',
                translateValue: 'fishing-gears.generate-mark-numbers-btn-label'
            }
        }, '600px').subscribe({
            next: (range: MarksRangeData | undefined) => {
                if (range !== null && range !== undefined) {
                    this.addMarksByRange(range.start, range.end);
                }
            }
        });
    }

    private addMarksByRange(start: number, end: number): void {
        const status: NomenclatureDTO<number> = this.markStatuses.find(x => x.code === FishingGearMarkStatusesEnum[FishingGearMarkStatusesEnum.NEW])!;

        for (let num = start; num <= end; num++) {
            if (this.marks === null || this.marks === undefined) {
                this.marks = [];
            }

            this.marks.push(new FishingGearMarkDTO({
                selectedStatus: FishingGearMarkStatusesEnum.NEW,
                statusId: status.value,
                number: num.toString(),
                isActive: true
            }));
        }

        this.marks = this.marks.slice();
        this.marksForm.updateValueAndValidity({ emitEvent: false });
    }

    public onEditedMark(): void {
        if (this.isInspected) {
            this.onChanged(this.getValue());
        }
        else {
            this.marksForm!.updateValueAndValidity({ emitEvent: false });
            this.pingersForm!.updateValueAndValidity({ emitEvent: false });
        }
    }

    public onEditedPinger(): void {
        if (this.isInspected) {
            this.onChanged(this.getValue());
        }
        else {
            this.form.updateValueAndValidity({ emitEvent: false });
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
            countControl: new FormControl(undefined, TLValidators.number(0)),
            netEyeSizeControl: new FormControl(undefined, [Validators.required, TLValidators.number(0)]),
            descriptionControl: new FormControl(undefined, Validators.maxLength(4000)),
            hooksContControl: new FormControl(undefined, TLValidators.number(0)),
            lengthControl: new FormControl(undefined, TLValidators.number(0)),
            heightControl: new FormControl(undefined, TLValidators.number(0)),
            towelLengthControl: new FormControl(undefined, TLValidators.number(0)),
            houseLengthControl: new FormControl(undefined, TLValidators.number(0)),
            houseWidthControl: new FormControl(undefined, TLValidators.number(0)),
            hasPingersControl: new FormControl(),
            cordThicknessControl: new FormControl(undefined, TLValidators.number(0)),
            lineCountControl: new FormControl(undefined, TLValidators.number(0)),
            netNominalLengthControl: new FormControl(undefined, TLValidators.number(0)),
            netsInFleetCountControl: new FormControl(undefined, TLValidators.number(0)),
            trawlModelControl: new FormControl(undefined, Validators.maxLength(500))
        });

        this.pingersForm = new FormGroup({
            numberControl: new FormControl(undefined, Validators.required),
            statusIdControl: new FormControl(undefined, Validators.required),
            modelControl: new FormControl(undefined, Validators.maxLength(500)),
            brandControl: new FormControl(undefined, Validators.maxLength(500)),
        });

        this.marksForm = new FormGroup({
            numberControl: new FormControl(undefined, [Validators.required, TLValidators.number(0, undefined, 0)]),
            statusIdControl: new FormControl(undefined, Validators.required)
        });

        if (!this.isInspected) {
            this.marksForm.setValidators(this.uniqueMarkNumberValidator());
            this.pingersForm.setValidators(this.uniquePingerNumberValidator());
        }

        form.get('hasPingersControl')!.valueChanges.subscribe({
            next: (value: boolean) => {
                this.showPingersTable = value;

                if (this.isInspected) {
                    if (this.showPingersTable) {
                        this.form.setValidators(this.atLeastOnePingerValidator());
                    }
                    else {
                        this.form.clearValidators();
                    }

                    this.form.updateValueAndValidity({ emitEvent: false, onlySelf: true });
                }
            }
        });

        form.get('typeControl')!.valueChanges.subscribe({
            next: (value: NomenclatureDTO<number> | string | undefined) => {
                if (value instanceof NomenclatureDTO) {
                    this.setCountAndQuotaGearLength(value);
                }
                else {
                    this.selectedGearTypeIsPoundNet = false;
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
                this.setCountAndQuotaGearLength(type);
            }

            if (type.code !== FishingGearTypesEnum[FishingGearTypesEnum.DLN]) {
                this.form.get('countControl')!.setValue(this.model.count);
                this.form.get('hooksContControl')!.setValue(this.model.hookCount);
                this.form.get('lengthControl')!.setValue(this.model.length);
                this.form.get('heightControl')!.setValue(this.model.height);
                this.form.get('cordThicknessControl')!.setValue(this.model.cordThickness);
                this.form.get('lineCountControl')!.setValue(this.model.lineCount);
                this.form.get('netNominalLengthControl')!.setValue(this.model.netNominalLength);
                this.form.get('netsInFleetCountControl')!.setValue(this.model.netsInFleetCount);
                this.form.get('trawlModelControl')!.setValue(this.model.trawlModel);
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

        this.marks = this.model.marks?.slice() ?? [];
        this.pingers = this.model.pingers?.slice() ?? [];
    }

    private fillModel(): void {
        const type: NomenclatureDTO<number> = this.form.get('typeControl')!.value;

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
            this.model.lineCount = this.form.get('lineCountControl')!.value;
            this.model.netNominalLength = this.form.get('netNominalLengthControl')!.value;
            this.model.netsInFleetCount = this.form.get('netsInFleetCountControl')!.value;
            this.model.trawlModel = this.form.get('trawlModelControl')!.value;
        }
        else {
            this.model.towelLength = this.form.get('towelLengthControl')!.value;
            this.model.houseLength = this.form.get('houseLengthControl')!.value;
            this.model.houseWidth = this.form.get('houseWidthControl')!.value;
        }

        if (this.marksTable !== null && this.marksTable !== undefined && (!this.isInspected || this.marksTable.rows.length > 0)) {
            this.model.marks = this.getMarksFromTable();
        }

        this.model.marksNumbers = '';
        if (this.model.marks !== null && this.model.marks !== undefined && this.model.marks.length > 0) {
            for (const mark of this.model.marks.filter(x => x.isActive)) {
                if (mark.number !== null && mark.number !== undefined) {
                    this.model.marksNumbers = this.model.marksNumbers?.concat(`${mark.number};`) ?? '';
                }
            }
        }

        if (this.form.get('hasPingersControl')!.value === true) {
            if (this.pingersTable !== null && this.pingersTable !== undefined) {
                this.model.pingers = this.getPingersFromTable();
            }
        }
    }

    private isFormValid(): boolean {
        return this.form.valid
            && this.marksForm?.errors?.duplicatedMark == undefined
            && this.pingersForm?.errors?.duplicatedPinger == undefined;
    }

    private getMarksFromTable(): FishingGearMarkDTO[] {
        if (this.marksTable !== null && this.marksTable !== undefined && this.marksTable.rows !== null && this.marksTable.rows !== undefined) {
            return this.marksTable.rows.map((row: FishingGearMarkDTO) => {
                return new FishingGearMarkDTO({
                    id: row.id,
                    number: row.number,
                    statusId: row.statusId,
                    selectedStatus: FishingGearMarkStatusesEnum[this.markStatuses.find(x => x.value === row.statusId)!.code as keyof typeof FishingGearMarkStatusesEnum],
                    isActive: row.isActive ?? true
                });
            });
        }
        else {
            return new Array<FishingGearMarkDTO>();
        }
    }

    private getPingersFromTable(): FishingGearPingerDTO[] {
        if (this.pingersTable !== null && this.pingersTable !== undefined && this.pingersTable.rows !== null && this.pingersTable !== undefined) {
            return this.pingersTable.rows.map((row: FishingGearPingerDTO) => {
                return new FishingGearPingerDTO({
                    id: row.id,
                    number: row.number,
                    statusId: row.statusId,
                    selectedStatus: FishingGearPingerStatusesEnum[this.pingerStatuses.find(x => x.value === row.statusId)!.code as keyof typeof FishingGearPingerStatusesEnum],
                    isActive: row.isActive ?? true,
                    model: row.model,
                    brand: row.brand,
                });
            });
        }
        else {
            return new Array<FishingGearPingerDTO>();
        }
    }

    private setCountAndQuotaGearLength(gearType: NomenclatureDTO<number>): void {
        if (gearType.code !== FishingGearTypesEnum[FishingGearTypesEnum.DLN]) {
            this.form.get('countControl')!.setValidators([Validators.required, TLValidators.number(0)]);
            this.form.get('countControl')!.markAsPending();
        }
        else {
            this.form.get('countControl')!.reset();
            this.form.get('countControl')!.setValidators(TLValidators.number(0));
            this.form.get('countControl')!.updateValueAndValidity();
        }

        if (this.pageCode === PageCodeEnum.CatchQuataSpecies) {
            this.setQuotaGearLength();
        }

        if (this.isDisabled) {
            this.form.get('countControl')!.disable();
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

    private uniqueMarkNumberValidator(): ValidatorFn {
        return (form: AbstractControl): ValidationErrors | null => {
            if (form === null || form === undefined) {
                return null;
            }

            const markNum: string = form.get('numberControl')!.value;
            const marks: FishingGearMarkDTO[] = this.getMarksFromTable();

            if (markNum !== null
                && markNum !== undefined
                && marks !== null
                && marks !== undefined
                && marks!.filter(x => x.isActive && x.number === markNum).length > 1
            ) {
                return { 'duplicatedMark': true };
            }

            return null;
        }
    }

    private uniquePingerNumberValidator(): ValidatorFn {
        return (form: AbstractControl): ValidationErrors | null => {
            if (form === null || form === undefined) {
                return null;
            }

            const pingerNum: string = form.get('numberControl')!.value;
            const pingers: FishingGearPingerDTO[] = this.getPingersFromTable();

            if (pingerNum !== null
                && pingerNum !== undefined
                && pingers !== null
                && pingers !== undefined
                && pingers.filter(x => x.isActive && x.number === pingerNum).length > 1
            ) {
                return { 'duplicatedPinger': true };
            }

            return null;
        }
    }

    private atLeastOnePingerValidator(): ValidatorFn {
        return (form: AbstractControl): ValidationErrors | null => {
            if (form === null || form === undefined) {
                return null;
            }

            if (this.pingersTable === null || this.pingersTable === undefined) {
                return null;
            }

            if (this.pingersTable.rows !== null && this.pingersTable.rows !== undefined && this.pingersTable.rows.length > 0) {
                return null;
            }
            else {
                return { 'noPingers': true };
            }
        }
    }
}