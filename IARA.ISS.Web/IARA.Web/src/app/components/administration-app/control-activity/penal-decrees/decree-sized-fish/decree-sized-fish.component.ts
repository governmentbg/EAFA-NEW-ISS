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
import { CommandTypes } from '@app/shared/components/data-table/enums/command-type.enum';

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

    public constructor(
        @Self() ngControl: NgControl,
        translate: FuseTranslationLoaderService,
        nomenclatures: CommonNomenclatures,
        service: PenalDecreesService
    ) {
        super(ngControl);

        this.translate = translate;
        this.nomenclatures = nomenclatures;
        this.service = service;
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

                        if (event.Command !== CommandTypes.Edit) {
                            this.seizedFishForm.updateValueAndValidity({ onlySelf: true });
                        }
                    }
                }
            });

            this.seizedFishForm.get('weightControl')!.valueChanges.subscribe({
                next: (event: RecordChangedEventArgs<AuanConfiscatedFishDTO>) => {
                    if (!this.isAppliance) {
                        this.seizedFishFormTouched = true;

                        if (event.Command !== CommandTypes.Edit) {
                            this.seizedFishForm.updateValueAndValidity({ onlySelf: true });
                        }
                    }
                }
            });
        }

        this.seizedFishForm.get('applianceIdControl')!.updateValueAndValidity({ emitEvent: false });
        this.seizedFishForm.get('fishTypeIdControl')!.updateValueAndValidity({ emitEvent: false });
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

    public seizedFishRecordChanged(event: RecordChangedEventArgs<AuanConfiscatedFishDTO>): void {
        this.seizedFish = this.seizedFishTable.rows.map(x => new AuanConfiscatedFishDTO({
            id: x.id,
            fishTypeId: x.fishTypeId,
            confiscationActionId: x.confiscationActionId,
            territoryUnitId: x.territoryUnitId,
            turbotSizeGroupId: x.turbotSizeGroupId,
            applianceId: x.applianceId,
            weight: x.weight,
            count: x.count,
            comments: x.comments,
            isActive: x.isActive ?? true
        }));

        this.onChanged(this.seizedFish);
    }

    protected getValue(): AuanConfiscatedFishDTO[] {
        this.seizedFish = this.seizedFishTable.rows;
        return this.seizedFish;
    }

    protected buildForm(): AbstractControl {
        this.seizedFishForm = new FormGroup({
            fishTypeIdControl: new FormControl(null, Validators.required),
            weightControl: new FormControl(null, TLValidators.number(0)),
            countControl: new FormControl(null, [Validators.required, TLValidators.number(1)]),
            confiscationActionIdControl: new FormControl(null, Validators.required),
            applianceIdControl: new FormControl(null, Validators.required),
            turbotSizeGroupIdControl: new FormControl(null),
            commentsControl: new FormControl(null, Validators.maxLength(2000)),
            territoryUnitIdControl: new FormControl(null)
        }, this.fishCountValidator());

        return new FormControl(null);
    }

    private fishCountValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (this.seizedFishTable !== undefined && this.seizedFishTable !== null) {
                const count: number | undefined = this.seizedFishForm.get('countControl')!.value;
                const weight: number | undefined = this.seizedFishForm.get('weightControl')!.value;

                if ((count === undefined || count === null) && (weight === undefined || weight === null)) {
                    return { 'fishCountValidationError': true };
                }
            }
            return null;
        }
    }
}