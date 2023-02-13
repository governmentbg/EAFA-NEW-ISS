import { AfterViewInit, Component, Input, OnInit, Self, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { forkJoin } from 'rxjs';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { AuanConfiscationActionsNomenclatureDTO } from '@app/models/generated/dtos/AuanConfiscationActionsNomenclatureDTO';
import { InspConfiscationActionGroupsEnum } from '@app/enums/insp-confiscation-action-groups.enum';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { RecordChangedEventArgs } from '@app/shared/components/data-table/models/record-changed-event.model';
import { IPenalDecreesService } from '@app/interfaces/administration-app/penal-decrees.interface';
import { PenalDecreesService } from '@app/services/administration-app/penal-decrees.service';
import { AuanConfiscatedFishDTO } from '@app/models/generated/dtos/AuanConfiscatedFishDTO';
import { ChooseLawSectionsComponent } from '../../auan-register/choose-law-sections/choose-law-sections.component';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { ChooseLawSectionDialogParams } from '../../auan-register/models/choose-law-section-dialog-params.model';
import { AuanLawSectionDTO } from '@app/models/generated/dtos/AuanLawSectionDTO';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';

@Component({
    selector: 'decree-sized-fish',
    templateUrl: './decree-sized-fish.component.html'
})
export class DecreeSizedFishComponent extends CustomFormControl<AuanConfiscatedFishDTO[]> implements OnInit, AfterViewInit {
    @Input() public viewMode!: boolean;

    @Input() public isAppliance: boolean = false;

    @Input() public isAuan: boolean = false;

    public seizedFishForm!: FormGroup;
    public seizedFish: AuanConfiscatedFishDTO[] = [];
    public translate: FuseTranslationLoaderService;

    public isDisabled: boolean = false;

    public fishTypes: NomenclatureDTO<number>[] = [];
    public confiscatedFishActions: NomenclatureDTO<number>[] = [];
    public territoryUnits: NomenclatureDTO<number>[] = [];
    public appliances: NomenclatureDTO<number>[] = [];
    public turbotSizeGroups: NomenclatureDTO<number>[] = [];
    public seizedFishFormTouched: boolean = false;

    @ViewChild('seizedFishTable')
    private seizedFishTable!: TLDataTableComponent;

    private readonly nomenclatures: CommonNomenclatures;
    private readonly service: IPenalDecreesService;
    private readonly chooseLawSectionDialog: TLMatDialog<ChooseLawSectionsComponent>;

    public constructor(
        @Self() ngControl: NgControl,
        translate: FuseTranslationLoaderService,
        nomenclatures: CommonNomenclatures,
        service: PenalDecreesService,
        chooseLawSectionDialog: TLMatDialog<ChooseLawSectionsComponent>
    ) {
        super(ngControl);

        this.translate = translate;
        this.nomenclatures = nomenclatures;
        this.service = service;
        this.chooseLawSectionDialog = chooseLawSectionDialog;
    }

    public async ngOnInit(): Promise<void> {
        this.initCustomFormControl();

        const nomenclatures: (NomenclatureDTO<number> | AuanConfiscationActionsNomenclatureDTO)[][] = await forkJoin(
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Fishes, this.nomenclatures.getFishTypes.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.TerritoryUnits, this.nomenclatures.getTerritoryUnits.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.ConfiscatedAppliances, this.service.getConfiscatedAppliances.bind(this.service), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.TurbotSizeGroups, this.service.getTurbotSizeGroups.bind(this.service), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.InspConfiscationActions, this.service.getConfiscationActions.bind(this.service), false)
        ).toPromise();

        this.fishTypes = nomenclatures[0];
        this.territoryUnits = nomenclatures[1];
        this.appliances = nomenclatures[2];
        this.turbotSizeGroups = nomenclatures[3];

        if (this.isAuan) {
            this.confiscatedFishActions = (nomenclatures[4] as AuanConfiscationActionsNomenclatureDTO[])
                .filter(x => x.actionGroup === InspConfiscationActionGroupsEnum.AUANFish);
        }
        else {
            this.confiscatedFishActions = (nomenclatures[4] as AuanConfiscationActionsNomenclatureDTO[])
                .filter(x => x.actionGroup === InspConfiscationActionGroupsEnum.PDFish || x.actionGroup === InspConfiscationActionGroupsEnum.AUANFish);
        }
    }

    public ngAfterViewInit(): void {
        this.seizedFishForm.get('applianceIdControl')!.clearValidators();
        this.seizedFishForm.get('fishTypeIdControl')!.clearValidators();
        this.seizedFishForm.get('countControl')!.clearValidators();

        if (this.isAppliance) {
            this.seizedFishForm.get('applianceIdControl')!.setValidators(Validators.required);
            this.seizedFishForm.get('countControl')!.setValidators(Validators.required);
        }
        else {
            this.seizedFishForm.get('fishTypeIdControl')!.setValidators(Validators.required);

            this.seizedFishForm.get('countControl')!.valueChanges.subscribe({
                next: (event: RecordChangedEventArgs<AuanConfiscatedFishDTO>) => {
                    if (!this.isAppliance) {
                        this.seizedFishFormTouched = true;
                        this.seizedFishForm.get('fishTypeIdControl')!.updateValueAndValidity({ onlySelf: true });
                    }
                }
            });

            this.seizedFishForm.get('weightControl')!.valueChanges.subscribe({
                next: (event: RecordChangedEventArgs<AuanConfiscatedFishDTO>) => {
                    if (!this.isAppliance) {
                        this.seizedFishFormTouched = true;
                        this.seizedFishForm.get('fishTypeIdControl')!.updateValueAndValidity({ onlySelf: true });
                    }
                }
            });
        }

        this.seizedFishForm.get('fishTypeIdControl')!.updateValueAndValidity({ emitEvent: false });
        this.seizedFishForm.get('applianceIdControl')!.updateValueAndValidity({ emitEvent: false });
        this.seizedFishForm.get('countControl')!.updateValueAndValidity({ emitEvent: false });
    }

    public writeValue(value: AuanConfiscatedFishDTO[]): void {
        if (value !== null && value !== undefined) {
            setTimeout(() => {
                this.seizedFish = value;
            });
        }
        else {
            setTimeout(() => {
                this.seizedFish = [];
            });
        }
    }

    public setDisabledState(isDisabled: boolean): void {
        this.isDisabled = isDisabled;
        if (this.isDisabled) {
            this.seizedFishForm.disable();
        }
        else {
            this.seizedFishForm.enable();
        }
    }

    public validate(control: AbstractControl): ValidationErrors | null {
        const errors: ValidationErrors = control.errors ?? {};
        return Object.keys(errors).length > 0 ? errors : null;
    }

    public seizedFishRecordChanged(event: RecordChangedEventArgs<AuanConfiscatedFishDTO>): void {
        this.seizedFish = this.seizedFishTable.rows;

        this.onChanged(this.getValue());
        this.control.updateValueAndValidity({ emitEvent: false });
    }

    public onEditRecord(row: GridRow<AuanConfiscatedFishDTO>): void {
        if (row !== undefined && row !== null) {
            this.seizedFishForm.get('lawTextControl')!.setValue(row.data.lawText);
        }
        else {
            this.seizedFishForm.get('lawTextControl')!.reset();
        }
    }

    public openLawSectionsDialog(row: GridRow<AuanConfiscatedFishDTO>): void {
        let auditButton: IHeaderAuditButton | undefined;
        const title: string = this.translate.getValue('penal-decrees.choose-law-section-dialog-title');
        const data: ChooseLawSectionDialogParams = new ChooseLawSectionDialogParams({
            id: row.data?.lawSectionId
        });

        const dialog = this.chooseLawSectionDialog.openWithTwoButtons({
            title: title,
            TCtor: ChooseLawSectionsComponent,
            headerAuditButton: auditButton,
            headerCancelButton: {
                cancelBtnClicked: this.closeDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translate,
            disableDialogClose: true,
            saveBtn: {
                id: 'save',
                color: 'accent',
                translateValue: this.translate.getValue('penal-decrees.choose')
            },
            cancelBtn: {
                id: 'cancel',
                color: 'primary',
                translateValue: this.translate.getValue('common.cancel'),
            }
        }, '1400px');

        dialog.subscribe((entry: AuanLawSectionDTO) => {
            if (entry !== undefined && entry !== null) {
                row.data.lawSectionId = entry.id;
                row.data.lawText = entry.lawText;
                this.seizedFishForm.get('lawTextControl')!.setValue(row.data.lawText);
            }
            else {
                row.data.lawSectionId = undefined;
                row.data.lawText = undefined;
                this.seizedFishForm.get('lawTextControl')!.reset();
            }

            this.onChanged(this.seizedFish);
            this.seizedFishForm.get('lawTextControl')!.updateValueAndValidity({ onlySelf: true });
        });
    }

    public getControlErrorLabelText(controlName: string, errorValue: unknown, errorCode: string): TLError | undefined {
        if (errorValue === true) {
            if (controlName === 'fishTypeIdControl') {
                if (errorCode === 'fishCountValidationError') {
                    return new TLError({ text: this.translate.getValue('penal-decrees.fish-count-validation'), type: 'error' });
                }
            }
        }
        return undefined;
    }

    protected getValue(): AuanConfiscatedFishDTO[] {
        return this.seizedFish.map(x => new AuanConfiscatedFishDTO({
            id: x.id,
            fishTypeId: x.fishTypeId,
            confiscationActionId: x.confiscationActionId,
            territoryUnitId: x.territoryUnitId,
            turbotSizeGroupId: x.turbotSizeGroupId,
            applianceId: x.applianceId,
            weight: x.weight,
            length: x.length,
            count: x.count,
            comments: x.comments,
            isActive: x.isActive ?? true,
            lawSectionId: x.lawSectionId ?? undefined,
            lawText: x.lawText ?? undefined,
        }));
    }

    protected buildForm(): AbstractControl {
        this.seizedFishForm = new FormGroup({
            fishTypeIdControl: new FormControl(null, [Validators.required, this.fishCountValidator()]),
            weightControl: new FormControl(null, TLValidators.number(0)),
            lengthControl: new FormControl(null, TLValidators.number(0)),
            countControl: new FormControl(null, [Validators.required, TLValidators.number(1)]),
            confiscationActionIdControl: new FormControl(null, Validators.required),
            applianceIdControl: new FormControl(null, Validators.required),
            turbotSizeGroupIdControl: new FormControl(null),
            commentsControl: new FormControl(null, Validators.maxLength(2000)),
            territoryUnitIdControl: new FormControl(null),
            lawTextControl: new FormControl(null, this.lawSectionValidator())
        });

        return new FormControl(null);
    }

    private fishCountValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (this.seizedFishTable !== undefined && this.seizedFishTable !== null && !this.viewMode) {
                const count: number | undefined = this.seizedFishForm.get('countControl')!.value ?? undefined;
                const weight: number | undefined = this.seizedFishForm.get('weightControl')!.value ?? undefined;

                if ((count === undefined || count === null || Number(count) === 0) && (weight === undefined || weight === null || Number(weight) === 0)) {
                    return { 'fishCountValidationError': true };
                }
            }
            return null;
        }
    }

    private lawSectionValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            const lawText: string | undefined = control.value;

            if ((lawText === undefined || lawText === null) && this.isAuan) {
                return { 'lawSectionError': true };
            }
            return null;
        }
    }

    private closeDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }
}