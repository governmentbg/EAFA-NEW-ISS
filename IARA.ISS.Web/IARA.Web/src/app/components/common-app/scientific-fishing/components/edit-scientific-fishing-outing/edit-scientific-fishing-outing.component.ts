import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { IScientificFishingService } from '@app/interfaces/common-app/scientific-fishing.interface';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { ScientificFishingOutingCatchDTO } from '@app/models/generated/dtos/ScientificFishingOutingCatchDTO';
import { ScientificFishingOutingDTO } from '@app/models/generated/dtos/ScientificFishingOutingDTO';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { FishNomenclatureDTO } from '@app/models/generated/dtos/FishNomenclatureDTO';
import { EditScientificFishingOutingDialogParams } from '../../models/edit-scientific-fishing-outing-dialog-params.model';
import { GetControlErrorLabelTextCallback } from '@app/shared/components/input-controls/base-tl-control';

@Component({
    selector: 'edit-scientific-fishing-outing',
    templateUrl: './edit-scientific-fishing-outing.component.html',
    styleUrls: ['./edit-scientific-fishing-outing.component.scss']
})
export class EditScientificFishingOutingComponent implements OnInit, AfterViewInit, OnDestroy, IDialogComponent {
    public readonly currentDate: Date = new Date();

    public editOutingForm!: FormGroup;
    public outingCatchForm!: FormGroup;

    public allFishTypes: FishNomenclatureDTO[] = [];
    public fishTypes: NomenclatureDTO<number>[] = [];

    public readOnly!: boolean;

    public outingCatches: ScientificFishingOutingCatchDTO[] = [];
    public totalCatchCalculateControls: string[] = [];
    public showNoCatchesError: boolean = false;

    public readonly canRestoreRecords: boolean;

    public getTotalCountErrorTextMethod: GetControlErrorLabelTextCallback = this.getTotalCountErrorText.bind(this);

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    private service!: IScientificFishingService;
    private nomenService: CommonNomenclatures;
    private translateService!: FuseTranslationLoaderService;
    private outingId!: number;
    private saveInDialog!: boolean;
    private model!: ScientificFishingOutingDTO;

    private mutationObserver: MutationObserver;

    public constructor(
        translateService: FuseTranslationLoaderService,
        nomenService: CommonNomenclatures,
        permissions: PermissionsService
    ) {
        this.translateService = translateService;
        this.nomenService = nomenService;

        this.canRestoreRecords = permissions.has(PermissionsEnum.ScientificFishingRestoreRecords);

        this.buildForm();

        this.mutationObserver = new MutationObserver((mutations: MutationRecord[]) => {
            for (const mutation of mutations.filter(x => x.addedNodes && x.addedNodes.length > 0)) {
                for (let i = 0; i < mutation.addedNodes.length; ++i) {
                    const node: HTMLElement = mutation.addedNodes[i] as HTMLElement;

                    if (node.className.includes('mat-autocomplete-panel')) {
                        const parent: HTMLElement = node.parentElement!;
                        parent.style.width = '300px';

                        return;
                    }
                }
            }
        });
    }

    public async ngOnInit(): Promise<void> {
        this.allFishTypes = this.fishTypes = await NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.Fishes, this.nomenService.getFishTypes.bind(this.nomenService), false
        ).toPromise();

        if (this.model !== undefined && this.model !== null) {
            this.fillForm(this.model);
        }
    }

    public ngAfterViewInit(): void {
        this.outingCatchForm.get('fishTypeIdControl')!.valueChanges.subscribe({
            next: () => {
                this.fishTypes = [...this.allFishTypes];
                const fishTypeIds = new Set<number>(this.datatable.rows.map(x => x.fishTypeId));

                this.fishTypes = this.fishTypes.filter(x => !fishTypeIds.has(x.value!));
                this.fishTypes = this.fishTypes.slice();
            }
        });

        this.mutationObserver.observe(document.body, {
            childList: true,
            subtree: true,
            attributes: false,
            characterData: false
        });
    }

    public ngOnDestroy(): void {
        this.mutationObserver.disconnect();
    }

    public saveBtnClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.editOutingForm.markAllAsTouched();

        if (this.datatable.rows.length === 0) {
            this.showNoCatchesError = true;
        }

        if ((this.editOutingForm.valid && this.datatable.rows.length !== 0) || this.readOnly) {
            this.fillModel(this.editOutingForm);
            CommonUtils.sanitizeModelStrings(this.model);

            if (this.saveInDialog) {
                this.service.addOuting(this.model).subscribe((id: number) => {
                    this.model.id = id;
                    dialogClose(this.model);
                });
            }
            else {
                dialogClose(this.model);
            }
        }
    }

    public cancelBtnClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public setData(data: EditScientificFishingOutingDialogParams, buttons: DialogWrapperData): void {
        this.service = data.service;
        this.outingId = data.id;
        this.saveInDialog = data.saveInDialog;
        this.readOnly = data.isReadonly;

        if (this.outingId !== -1) {
            if (this.readOnly) {
                this.editOutingForm.disable();
            }
            this.editOutingForm.get('dateOfOutingControl')!.disable();

            if (data.model !== undefined && data.model !== null) {
                this.model = data.model;
            }
        }
        else {
            this.model = new ScientificFishingOutingDTO({
                permitId: data.permitId,
                isActive: true
            });
        }
    }

    public calculateTotal(row: ScientificFishingOutingCatchDTO): string {
        const under100: number = row.catchUnder100 !== undefined ? Number(row.catchUnder100) : 0;
        const catch100To500: number = row.catch100To500 !== undefined ? Number(row.catch100To500) : 0;
        const catch500To1000: number = row.catch500To1000 !== undefined ? Number(row.catch500To1000) : 0;
        const over1000: number = row.catchOver1000 !== undefined ? Number(row.catchOver1000) : 0;

        let result: number = 0;
        result += isNaN(under100) ? 0 : under100;
        result += isNaN(catch100To500) ? 0 : catch100To500;
        result += isNaN(catch500To1000) ? 0 : catch500To1000;
        result += isNaN(over1000) ? 0 : over1000;
        return result.toString();
    }

    public getTotalCountErrorText(controlName: string, error: unknown, errorCode: string): TLError | undefined {
        if (controlName === 'totalKeptCountControl') {
            if (errorCode === 'totalkeptgreaterthancaught') {
                return new TLError({
                    text: this.translateService.getValue('scientific-fishing.total-kept-greater-than-caught'),
                    type: 'error'
                });
            }
        }
        return undefined;
    }

    private buildForm(): void {
        this.editOutingForm = new FormGroup({
            dateOfOutingControl: new FormControl('', Validators.required),
            waterAreaControl: new FormControl('', [Validators.maxLength(4000), Validators.required])
        });

        this.outingCatchForm = new FormGroup({
            fishTypeIdControl: new FormControl(null, Validators.required),
            catchUnder100Control: new FormControl(null, [TLValidators.number(0), Validators.required]),
            catch100To500Control: new FormControl(null, [TLValidators.number(0), Validators.required]),
            catch500To1000Control: new FormControl(null, [TLValidators.number(0), Validators.required]),
            catchOver1000Control: new FormControl(null, [TLValidators.number(0), Validators.required]),
            totalCatchControl: new FormControl(0),
            totalKeptCountControl: new FormControl(null, [TLValidators.number(0), Validators.required, this.totalKeptCountValidate()])
        });

        this.totalCatchCalculateControls = ['catchUnder100Control', 'catch100To500Control', 'catch500To1000Control', 'catchOver1000Control'];

        this.outingCatchForm.get('totalCatchControl')!.valueChanges.subscribe(() => {
            this.outingCatchForm.get('totalKeptCountControl')!.updateValueAndValidity({ emitEvent: false });
        });
    }

    private totalKeptCountValidate(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            const totalCatch = this.outingCatchForm?.get('totalCatchControl')?.value;
            const totalKept = control?.value;

            if (totalCatch && totalKept) {
                const caught: number = Number(totalCatch);
                const kept: number = Number(totalKept);

                if (kept > caught) {
                    return { 'totalkeptgreaterthancaught': true };
                }
            }
            return null;
        };
    }

    private fillForm(model: ScientificFishingOutingDTO): void {
        this.editOutingForm.get('dateOfOutingControl')!.setValue(model.dateOfOuting);
        this.editOutingForm.get('waterAreaControl')!.setValue(model.waterArea);

        if (model.catches !== undefined && model.catches !== null) {
            this.outingCatches = model.catches;
        }
    }

    private fillModel(form: FormGroup): void {
        this.model.dateOfOuting = form.get('dateOfOutingControl')!.value;
        this.model.waterArea = form.get('waterAreaControl')!.value;

        this.model.catches = this.datatable.rows.map(x => new ScientificFishingOutingCatchDTO({
            id: x.id !== undefined ? Number(x.id) : undefined,
            outingId: this.outingId,
            fishTypeId: x.fishTypeId,
            catchUnder100: Number(x.catchUnder100),
            catch100To500: Number(x.catch100To500),
            catch500To1000: Number(x.catch500To1000),
            catchOver1000: Number(x.catchOver1000),
            totalKeptCount: Number(x.totalKeptCount),
            totalCatch: Number(x.totalCatch),
            isActive: x.isActive !== undefined ? Boolean(x.isActive) : true
        }));
    }
}
