import { Component, EventEmitter, Input, OnDestroy, OnInit, Optional, Output, Self, ViewChild } from '@angular/core';
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
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { GenerateMarksComponent } from './generate-marks/generate-marks.component';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { MarksRangeData } from '../models/marks-range.model';
import { FishingGearManipulationService } from '../services/fishing-gear-manipulation.service';
import { FishingGearUtils } from '@app/components/common-app/commercial-fishing/utils/fishing-gear.utils';
import { PrefixInputDTO } from '@app/models/generated/dtos/PrefixInputDTO';
import { TariffCodesEnum } from '@app/enums/tariff-codes.enum';

@Component({
    selector: 'edit-fishing-gear',
    templateUrl: './edit-fishing-gear.component.html'
})
export class EditFishingGearComponent extends CustomFormControl<FishingGearDTO | undefined> implements OnInit, IDialogComponent, OnDestroy {
    public fishingGearTypes: FishingGearNomenclatureDTO[] = [];
    public fishingGearTypesEnum: typeof FishingGearTypesEnum = FishingGearTypesEnum;

    @Input()
    public isInspected: boolean = false;

    @Input()
    public filterTypes: boolean = true;

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

    @Input()
    public listenToService: boolean = false;

    @Input()
    public isNewInspectedGear: boolean = false;

    @Output()
    public selectedMark = new EventEmitter<FishingGearMarkDTO>();

    public readonly markNumberValidators: ValidatorFn[] = [Validators.required, TLValidators.number(0)];

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
    private appliedTariffCodes: string[] = [];
    private isDunabe: boolean = false;

    private gearManipulationServiceSub: Subscription | undefined;

    private readonly loader: FormControlDataLoader;
    private readonly confirmationDialog: TLConfirmDialog;
    private readonly translateService: FuseTranslationLoaderService;
    private readonly generateMarksDialog: TLMatDialog<GenerateMarksComponent>;
    private readonly gearManipulationService: FishingGearManipulationService;


    public constructor(
        @Self() @Optional() ngControl: NgControl,
        commonNomenclatures: CommonNomenclatures,
        confirmationDialog: TLConfirmDialog,
        translateService: FuseTranslationLoaderService,
        generateMarksDialog: TLMatDialog<GenerateMarksComponent>,
        gearManipulationService: FishingGearManipulationService
    ) {
        super(ngControl);

        this.nomenclatures = commonNomenclatures;
        this.confirmationDialog = confirmationDialog;
        this.translateService = translateService;
        this.generateMarksDialog = generateMarksDialog;
        this.gearManipulationService = gearManipulationService;

        this.loader = new FormControlDataLoader(this.getNomenclatures.bind(this));
    }

    public ngOnInit(): void {
        this.initCustomFormControl();

        this.loader.load(() => {
            if (this.filterTypes) {
                if (this.pageCode === PageCodeEnum.PoundnetCommFishLic) {
                    this.fishingGearTypes = this.fishingGearTypes.filter(x => x.type === FishingGearParameterTypesEnum.PoundNet).slice();
                }
                else if (this.pageCode === PageCodeEnum.CatchQuataSpecies) {
                    this.fishingGearTypes = this.fishingGearTypes.filter(x => x.type === FishingGearParameterTypesEnum.Quota).slice();
                    this.fishingGearTypes = this.fishingGearTypes.filter(x => x.type !== FishingGearParameterTypesEnum.PoundNet).slice();
                }
                else {
                    this.fishingGearTypes = this.fishingGearTypes.filter(x => x.type !== FishingGearParameterTypesEnum.PoundNet).slice();
                }
            }

            //Показване само на тези риболовни уреди, за които е платено в удостоверението, за да не може да се добавят в заявлението за маркиране на уреди
            if (this.appliedTariffCodes.length > 0) {
                if (this.isDunabe) {
                    if (this.shouldFilterByDanubeTariffCodes()) {
                        if (!this.appliedTariffCodes.includes(TariffCodesEnum[TariffCodesEnum.a_1805_Dunav_Ship_Nets])) {
                            this.fishingGearTypes = this.fishingGearTypes.filter(x => !FishingGearUtils.paidDunabeNetFishingGears.includes(x.code!));
                        }

                        if (!this.appliedTariffCodes.includes(TariffCodesEnum[TariffCodesEnum.a_1805_Dunav_Ship_Fishing_Traps])) {
                            this.fishingGearTypes = this.fishingGearTypes.filter(x => x.code !== FishingGearTypesEnum[FishingGearTypesEnum.FPO]);
                        }
                    }
                }
                else {
                    if (this.shouldFilterByBlackSeaTariffCodes()) {
                        if (!this.appliedTariffCodes.includes(TariffCodesEnum[TariffCodesEnum.a_1805_Ship_Till10_Longliners])) {
                            this.fishingGearTypes = this.fishingGearTypes.filter(x => !FishingGearUtils.paidLonglinesFishingGears.includes(x.code!));
                        }

                        if (!this.appliedTariffCodes.includes(TariffCodesEnum[TariffCodesEnum.a_1805_Ship_Till10_Nets])) {
                            this.fishingGearTypes = this.fishingGearTypes.filter(x => !FishingGearUtils.paidNetFishingGears.includes(x.code!) && !FishingGearUtils.paidPoleAndLineFishingGears.includes(x.code!));
                        }
                    }
                }
            }

            if (!this.ngControl) {
                this.fillForm(this.model);
            }
        });

        if (this.listenToService) {
            this.gearManipulationServiceSub = this.gearManipulationService.markAdded.subscribe(this.transferMark.bind(this));
        }
    }

    public ngOnDestroy(): void {
        this.gearManipulationServiceSub?.unsubscribe();
    }

    private transferMark(mark: FishingGearMarkDTO): void {
        if (!this.marks.some(x => x.id === mark.id)) {
            const copiedMark: FishingGearMarkDTO = new FishingGearMarkDTO(mark);
            copiedMark.fullNumber = new PrefixInputDTO({
                prefix: mark.fullNumber?.prefix,
                inputValue: mark.fullNumber?.inputValue
            });

            this.marks.push(copiedMark);

            this.marks = this.marks.slice();
        }
    }

    public setData(data: EditFishingGearDialogParamsModel, buttons: DialogWrapperData): void {
        this.isDisabled = data.readOnly;
        this.pageCode = data.pageCode;
        this.appliedTariffCodes = data.appliedTariffCodes;
        this.isDunabe = data.isDunabe;

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
                this.fillModel(false);
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

    public onMoveMarkToInspected(mark: FishingGearMarkDTO): void {
        this.selectedMark.emit(mark);
    }

    public writeValue(value: FishingGearDTO | undefined): void {
        if (value === null || value === undefined) {
            this.model = new FishingGearDTO({ isActive: true });
        }
        else {
            this.model = value;
        }

        this.loader.load(() => {
            this.fillForm(this.model);
        });
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
        let status: NomenclatureDTO<number>;

        if (this.isInspected) {
            status = this.markStatuses.find(x => x.code === FishingGearMarkStatusesEnum[FishingGearMarkStatusesEnum.MARKED])!;
        }
        else {
            status = this.markStatuses.find(x => x.code === FishingGearMarkStatusesEnum[FishingGearMarkStatusesEnum.NEW])!
        }

        for (let num = start; num <= end; num++) {
            if (this.marks === null || this.marks === undefined) {
                this.marks = [];
            }

            const mark: FishingGearMarkDTO = new FishingGearMarkDTO({
                selectedStatus: this.isInspected ? FishingGearMarkStatusesEnum.MARKED : FishingGearMarkStatusesEnum.NEW,
                createdOn: new Date(),
                statusId: status.value,
                fullNumber: new PrefixInputDTO({
                    prefix: undefined,
                    inputValue: num.toString()
                }),
                isActive: true
            });

            this.marks.push(mark);

            if (this.isInspected) {
                this.selectedMark.emit(mark);
            }
        }

        this.marks = this.marks.slice();
        this.marksForm.updateValueAndValidity({ emitEvent: false });
    }

    public onMarkActiveRecordChanged(row: GridRow<FishingGearMarkDTO> | undefined): void {
        this.onEditMark(row);
    }

    public onMarkRecordChanged(row: any): void {
        this.onEditedMark(row);
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

        return this.fillModel(!this.isNewInspectedGear);
    }

    protected buildForm(): AbstractControl {
        const form = new FormGroup({
            typeControl: new FormControl(undefined, Validators.required),
            countControl: new FormControl(undefined, TLValidators.number(0, undefined, 0)),
            netEyeSizeControl: new FormControl(undefined, TLValidators.number(0)),
            descriptionControl: new FormControl(undefined, Validators.maxLength(4000)),
            hooksCountControl: new FormControl(undefined, TLValidators.number(0, undefined, 0)),
            lengthControl: new FormControl(undefined, TLValidators.number(0)),
            heightControl: new FormControl(undefined, TLValidators.number(0)),
            towelLengthControl: new FormControl(undefined, TLValidators.number(0, undefined, 0)),
            houseLengthControl: new FormControl(undefined, TLValidators.number(0, undefined, 0)),
            houseWidthControl: new FormControl(undefined, TLValidators.number(0, undefined, 0)),
            hasPingersControl: new FormControl(),
            cordThicknessControl: new FormControl(undefined, TLValidators.number(0)),
            lineCountControl: new FormControl(undefined, TLValidators.number(0, undefined, 0)),
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
            fullNumberControl: new FormControl(undefined, [Validators.required, TLValidators.number(0, undefined, 0)]),
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
            next: (value: FishingGearNomenclatureDTO | string | undefined) => {
                if (value instanceof NomenclatureDTO) {
                    if (value.code === FishingGearTypesEnum[FishingGearTypesEnum.DLN]) {
                        this.selectedGearTypeIsPoundNet = true;
                    }
                    else {
                        this.selectedGearTypeIsPoundNet = false;
                    }

                    this.setCountAndQuotaGearLength(value);
                    this.setFieldsValidators(value.code);

                    if (value.hasHooks) {
                        this.form.get('hooksCountControl')!.setValidators([Validators.required, TLValidators.number(0, undefined, 0)]);
                        this.form.get('hooksCountControl')!.markAsPending();
                    }
                    else {
                        this.form.get('hooksCountControl')!.setValidators(TLValidators.number(0, undefined, 0));
                        this.form.get('hooksCountControl')!.markAsPending();
                    }
                }
                else {
                    this.selectedGearTypeIsPoundNet = false;
                    this.setFieldsValidators(undefined);

                    this.form.get('hooksCountControl')!.setValidators(TLValidators.number(0, undefined, 0));
                    this.form.get('hooksCountControl')!.markAsPending();
                }

                if (this.isDisabled) {
                    this.form.get('hooksCountControl')!.disable({ emitEvent: false });
                }
            }
        });

        return form;
    }

    private fillForm(model: FishingGearDTO): void {
        if (model.typeId !== null && model.typeId !== undefined) {
            const type: NomenclatureDTO<number> = this.fishingGearTypes.find(x => x.value === model.typeId)!;
            this.form.get('typeControl')!.setValue(type);

            if (type instanceof NomenclatureDTO) {
                this.setCountAndQuotaGearLength(type);
            }

            if (type?.code !== FishingGearTypesEnum[FishingGearTypesEnum.DLN]) {
                this.form.get('countControl')!.setValue(model.count);
                this.form.get('hooksCountControl')!.setValue(model.hookCount);
                this.form.get('lengthControl')!.setValue(model.length);
                this.form.get('heightControl')!.setValue(model.height);
                this.form.get('cordThicknessControl')!.setValue(model.cordThickness);
                this.form.get('lineCountControl')!.setValue(model.lineCount);
                this.form.get('netNominalLengthControl')!.setValue(model.netNominalLength);
                this.form.get('netsInFleetCountControl')!.setValue(model.netsInFleetCount);
                this.form.get('trawlModelControl')!.setValue(model.trawlModel);
            }
            else {
                this.form.get('towelLengthControl')!.setValue(model.towelLength);
                this.form.get('houseLengthControl')!.setValue(model.houseLength);
                this.form.get('houseWidthControl')!.setValue(model.houseWidth);
            }
        }

        this.form.get('netEyeSizeControl')!.setValue(model.netEyeSize);
        this.form.get('descriptionControl')!.setValue(model.description);
        this.form.get('hasPingersControl')!.setValue(model.hasPingers);

        this.marks = this.copyMarks(model.marks?.slice() ?? []);
        this.pingers = this.copyPingers(model.pingers?.slice() ?? []);
    }

    private fillModel(returnNewObject: boolean): FishingGearDTO {
        let result: FishingGearDTO;

        if (returnNewObject) {
            result = new FishingGearDTO();
        }
        else {
            result = this.model;
        }

        result.id = this.model.id;
        result.netEyeSize = this.form.get('netEyeSizeControl')!.value;
        result.description = this.form.get('descriptionControl')!.value;
        result.hasPingers = this.form.get('hasPingersControl')!.value;
        result.permitId = this.model.permitId;
        result.isActive = this.model.isActive;

        const type: NomenclatureDTO<number> = this.form.get('typeControl')!.value;

        result.typeId = type?.value;
        result.type = type?.displayName;


        if (type?.code !== FishingGearTypesEnum[FishingGearTypesEnum.DLN]) {
            result.count = this.form.get('countControl')!.value ?? 0;
            result.hookCount = this.form.get('hooksCountControl')!.value;
            result.length = this.form.get('lengthControl')!.value;
            result.height = this.form.get('heightControl')!.value;
            result.cordThickness = this.form.get('cordThicknessControl')!.value;
            result.lineCount = this.form.get('lineCountControl')!.value;
            result.netNominalLength = this.form.get('netNominalLengthControl')!.value;
            result.netsInFleetCount = this.form.get('netsInFleetCountControl')!.value;
            result.trawlModel = this.form.get('trawlModelControl')!.value;
        }
        else {
            result.towelLength = this.form.get('towelLengthControl')!.value;
            result.houseLength = this.form.get('houseLengthControl')!.value;
            result.houseWidth = this.form.get('houseWidthControl')!.value;
        }

        if (this.marksTable !== null && this.marksTable !== undefined && (!this.isInspected || this.marksTable.rows.length > 0)) {
            result.marks = this.getMarksFromTable();
        }

        result.marksNumbers = '';
        if (result.marks !== null && result.marks !== undefined && result.marks.length > 0) {
            for (const mark of result.marks.filter(x => x.isActive)) {
                if (mark.fullNumber?.inputValue !== null && mark.fullNumber?.inputValue !== undefined) {
                    result.marksNumbers = result.marksNumbers?.concat(`${mark.fullNumber.prefix ?? ''}${mark.fullNumber.inputValue};`) ?? '';
                }
            }
        }

        if (this.form.get('hasPingersControl')!.value === true) {
            if (this.pingersTable !== null && this.pingersTable !== undefined) {
                result.pingers = this.getPingersFromTable();
            }
        }

        return result;
    }

    private isFormValid(): boolean {
        return this.form.valid
            && this.marksForm?.errors?.duplicatedMark == undefined
            && this.pingersForm?.errors?.duplicatedPinger == undefined;
    }

    private onEditMark(row: GridRow<FishingGearMarkDTO> | undefined): void {
        if (this.isInspected && (!(row !== null && row !== undefined) || row.data.statusId === this.markedStatus.value)) {
            setTimeout(() => {
                this.marksForm!.get('statusIdControl')!.setValue(this.markedStatus.value);
                this.marksForm!.get('statusIdControlHidden')!.disable();
            });
        }
        else {
            if (row === null || row === undefined) {
                const newMarkStatus: NomenclatureDTO<number> | undefined = this.markStatuses.find(x => x.code === FishingGearMarkStatusesEnum[FishingGearMarkStatusesEnum.NEW] && x.isActive);
                if (newMarkStatus !== null && newMarkStatus !== undefined) {
                    setTimeout(() => {
                        this.marksForm!.get('statusIdControl')!.setValue(newMarkStatus.value);
                    });
                }
            }
            else {
                this.marksForm!.get('statusIdControlHidden')!.enable();
            }
        }
    }

    private onEditedMark(row: any): void {
        this.marks = this.getMarksFromTable();

        if (this.isInspected) {
            this.onChanged(this.getValue());
        }
        else {
            this.marksForm!.updateValueAndValidity({ emitEvent: false });
            this.pingersForm!.updateValueAndValidity({ emitEvent: false });
        }
    }

    private getMarksFromTable(): FishingGearMarkDTO[] {
        if (this.marksTable !== null && this.marksTable !== undefined && this.marksTable.rows !== null && this.marksTable.rows !== undefined) {
            return this.marksTable.rows.map((row: FishingGearMarkDTO) => {
                return new FishingGearMarkDTO({
                    id: row.id,
                    fullNumber: new PrefixInputDTO(row.fullNumber),
                    statusId: row.statusId,
                    selectedStatus: FishingGearMarkStatusesEnum[this.markStatuses.find(x => x.value === row.statusId)!.code as keyof typeof FishingGearMarkStatusesEnum],
                    createdOn: row.createdOn ?? new Date(),
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
            this.form.get('countControl')!.setValidators([Validators.required, TLValidators.number(0, undefined, 0)]);
            this.form.get('countControl')!.markAsPending();
        }
        else {
            this.form.get('countControl')!.reset();
            this.form.get('countControl')!.setValidators(TLValidators.number(0, undefined, 0));
            this.form.get('countControl')!.markAsPending();
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

    private setFieldsValidators(fishingGearCode: string | undefined): void {
        if (fishingGearCode !== null && fishingGearCode !== undefined) {
            if (FishingGearUtils.fishingGearCodesWithRequiredMeshSize.some(x => x === fishingGearCode)) { // трябва да има задължителен размер на око
                this.form.get('netEyeSizeControl')!.setValidators([Validators.required, TLValidators.number(0)]);
                this.form.get('netEyeSizeControl')!.markAsPending();
            }
            else {
                this.form.get('netEyeSizeControl')!.setValidators(TLValidators.number(0));
                this.form.get('netEyeSizeControl')!.markAsPending();
            }

            if (FishingGearUtils.fishingGearCodesWithRequiredGearDimension.some(x => x === fishingGearCode)) { // трябва да има задължително Length/Width
                this.form.get('lengthControl')!.setValidators([Validators.required, TLValidators.number(0)]);
                this.form.get('lengthControl')!.markAsPending();
            }
            else {
                this.form.get('lengthControl')!.setValidators(TLValidators.number(0));
                this.form.get('lengthControl')!.markAsPending();
            }

            if (FishingGearUtils.fishingGearCodesWithRequiredHeight.some(x => x === fishingGearCode)) { // трябва да има задължително Height
                this.form.get('heightControl')!.setValidators([Validators.required, TLValidators.number(0)]);
                this.form.get('heightControl')!.markAsPending();
            }
            else {
                this.form.get('heightControl')!.setValidators(TLValidators.number(0));
                this.form.get('heightControl')!.markAsPending();
            }

            if (FishingGearUtils.fishingGearCodesWithRequiredNetLength.some(x => x === fishingGearCode)) { // трябва да има задължително Nominal Net Length
                this.form.get('netNominalLengthControl')!.setValidators([Validators.required, TLValidators.number(0)]);
                this.form.get('netNominalLengthControl')!.markAsPending();
            }
            else {
                this.form.get('netNominalLengthControl')!.setValidators(TLValidators.number(0));
                this.form.get('netNominalLengthControl')!.markAsPending();
            }

            //if (FishingGearUtils.fishingGearCodesWithRequiredNumberDimension.some(x => x === fishingGearCode)) { // трябва да има задължително брой (number dimension)
            //    this.form.get('hooksCountControl')!.setValidators([Validators.required, TLValidators.number(0)]); // TODO make this count for everything ??? with helper message?
            //    this.form.get('hooksCountControl')!.markAsPending();
            //}
            //else {
            //    this.form.get('hooksCountControl')!.setValidators(TLValidators.number(0));
            //    this.form.get('hooksCountControl')!.markAsPending();
            //}

            if (FishingGearUtils.fishingGearCodesWithRequiredNumberOfLines.some(x => x === fishingGearCode)) { // трябва да има задължително Line Count
                this.form.get('lineCountControl')!.setValidators([Validators.required, TLValidators.number(0, undefined, 0)]);
                this.form.get('lineCountControl')!.markAsPending();
            }
            else {
                this.form.get('lineCountControl')!.setValidators(TLValidators.number(0, undefined, 0));
                this.form.get('lineCountControl')!.markAsPending();
            }

            if (FishingGearUtils.fishingGearCodesWithRequiredNumberOfNets.some(x => x === fishingGearCode)) { // трябва да има задължително Nets In Fleet
                this.form.get('netsInFleetCountControl')!.setValidators([Validators.required, TLValidators.number(0)]);
                this.form.get('netsInFleetCountControl')!.markAsPending();
            }
            else {
                this.form.get('netsInFleetCountControl')!.setValidators(TLValidators.number(0));
                this.form.get('netsInFleetCountControl')!.markAsPending();
            }

            if (FishingGearUtils.fishingGearCodesWithRequiredTrawlModel.some(x => x === fishingGearCode)) { // трябва да има задължително Trawl Model
                this.form.get('trawlModelControl')!.setValidators([Validators.required, Validators.maxLength(5000)]);
                this.form.get('trawlModelControl')!.markAsPending();
            }
            else {
                this.form.get('trawlModelControl')!.setValidators(Validators.maxLength(5000));
                this.form.get('trawlModelControl')!.markAsPending();
            }

            if (FishingGearUtils.fishingGearCodesWithOptionalGearDimension.some(x => x === fishingGearCode)) { // трябва да има Lenght (perimeter of opening) or Trawl Model
                if (this.showPingersTable) {
                    this.form.setValidators([this.atLeastOnePingerValidator(), this.gearWithOptionalGearDimension()]);
                }
                else {
                    this.form.setValidators(this.gearWithOptionalGearDimension());
                }
            }
            else {
                if (this.showPingersTable) {
                    this.form.setValidators(this.atLeastOnePingerValidator());
                }
                else {
                    this.form.clearValidators();
                }
            }
        }
        else {
            this.form.get('netEyeSizeControl')!.clearValidators();
            this.form.get('netEyeSizeControl')!.markAsPending();

            this.form.get('lengthControl')!.setValidators(TLValidators.number(0));
            this.form.get('lengthControl')!.markAsPending();

            this.form.get('heightControl')!.setValidators(TLValidators.number(0));
            this.form.get('heightControl')!.markAsPending();

            this.form.get('netNominalLengthControl')!.setValidators(TLValidators.number(0));
            this.form.get('netNominalLengthControl')!.markAsPending();

            //    this.form.get('hooksCountControl')!.setValidators(TLValidators.number(0));
            //    this.form.get('hooksCountControl')!.markAsPending();

            this.form.get('lineCountControl')!.setValidators(TLValidators.number(0, undefined, 0));
            this.form.get('lineCountControl')!.markAsPending();

            this.form.get('netsInFleetCountControl')!.setValidators(TLValidators.number(0));
            this.form.get('netsInFleetCountControl')!.markAsPending();

            this.form.get('trawlModelControl')!.setValidators(Validators.maxLength(5000));
            this.form.get('trawlModelControl')!.markAsPending();
        }

        if (this.isDisabled) {
            this.form.disable({ emitEvent: false });
        }
    }

    private uniqueMarkNumberValidator(): ValidatorFn {
        return (form: AbstractControl): ValidationErrors | null => {
            if (form === null || form === undefined) {
                return null;
            }

            const markNum: PrefixInputDTO | undefined = form.get('fullNumberControl')!.value;
            const marks: FishingGearMarkDTO[] = this.getMarksFromTable();

            if (markNum !== null
                && markNum !== undefined
                && marks !== null
                && marks !== undefined
                && marks!.filter(x => x.isActive && x.fullNumber?.prefix == markNum.prefix && x.fullNumber?.inputValue === markNum.inputValue).length > 1
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

    private gearWithOptionalGearDimension(): ValidatorFn {
        return (form: AbstractControl): ValidationErrors | null => {
            if (form === null || form === undefined) {
                return null;
            }

            const gearTypeCode: string | undefined = form.get('typeControl')!.value?.code;

            if (FishingGearUtils.fishingGearCodesWithOptionalGearDimension.some(x => x === gearTypeCode)) {
                let lengthOrWidthValue: string | undefined = form.get('lengthControl')!.value;

                if (lengthOrWidthValue === '' || lengthOrWidthValue === null) {
                    lengthOrWidthValue = undefined;
                }

                const lengthOrWidth: number | undefined = Number(lengthOrWidthValue);
                const trawlModel: string | undefined = form.get('trawlModelControl')!.value;

                if (CommonUtils.isNumberNullOrNaN(lengthOrWidth) && CommonUtils.isNullOrEmpty(trawlModel)) {
                    return { 'lengthOrTrawlModelIsRequired': true };
                }
                else {
                    return null;
                }
            }

            return null;
        }
    }

    private copyMarks(marks: FishingGearMarkDTO[]): FishingGearMarkDTO[] {
        const copiedMarks: FishingGearMarkDTO[] = [];

        for (const mark of marks) {
            const markObject: FishingGearMarkDTO = new FishingGearMarkDTO(mark);
            markObject.fullNumber = new PrefixInputDTO(mark.fullNumber);

            copiedMarks.push(markObject);
        }

        return copiedMarks;
    }

    private copyPingers(pingers: FishingGearPingerDTO[]): FishingGearPingerDTO[] {
        const copiedPingers: FishingGearPingerDTO[] = [];

        for (const pinger of pingers) {
            const pingerObject: object = JSON.parse(JSON.stringify(pinger));
            copiedPingers.push(new FishingGearPingerDTO(pingerObject));
        }

        return copiedPingers;
    }

    //Тарифи, при които не се доплаща за добавяне на уреди
    private shouldFilterByBlackSeaTariffCodes(): boolean {
        return !this.appliedTariffCodes.includes(TariffCodesEnum[TariffCodesEnum.a_1805_ShipTill10_Fishing_Gears])
            && !this.appliedTariffCodes.includes(TariffCodesEnum[TariffCodesEnum.a_1805_Ship_Between_10_And_25])
            && !this.appliedTariffCodes.includes(TariffCodesEnum[TariffCodesEnum.a_1805_Ship_Between_25_And_40])
            && !this.appliedTariffCodes.includes(TariffCodesEnum[TariffCodesEnum.a_1805_Ship_Between_Over40])
            && !this.appliedTariffCodes.includes(TariffCodesEnum[TariffCodesEnum.a_977_1]);
    }

    private shouldFilterByDanubeTariffCodes(): boolean {
        return !this.appliedTariffCodes.includes(TariffCodesEnum[TariffCodesEnum.a_1805_Dunav_Ship_Nets_And_Fishing_Traps])
            && !this.appliedTariffCodes.includes(TariffCodesEnum[TariffCodesEnum.a_977_1]);
    }
}