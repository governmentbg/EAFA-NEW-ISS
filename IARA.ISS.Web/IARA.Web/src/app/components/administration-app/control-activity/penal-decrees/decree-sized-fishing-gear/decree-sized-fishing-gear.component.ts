import { Component, Input, OnInit, Self, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, Validators } from '@angular/forms';
import { PenalDecreeSeizedFishingGearDTO } from '@app/models/generated/dtos/PenalDecreeSeizedFishingGearDTO';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { AuanConfiscationActionsNomenclatureDTO } from '@app/models/generated/dtos/AuanConfiscationActionsNomenclatureDTO';
import { forkJoin } from 'rxjs';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { InspConfiscationActionGroupsEnum } from '@app/enums/insp-confiscation-action-groups.enum';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { RecordChangedEventArgs } from '@app/shared/components/data-table/models/record-changed-event.model';
import { IPenalDecreesService } from '@app/interfaces/administration-app/penal-decrees.interface';
import { PenalDecreesService } from '@app/services/administration-app/penal-decrees.service';

@Component({
    selector: 'decree-sized-fishing-gear',
    templateUrl: './decree-sized-fishing-gear.component.html'
})
export class DecreeSizedFishingGearComponent extends CustomFormControl<PenalDecreeSeizedFishingGearDTO[]> implements OnInit {
    @Input() public viewMode!: boolean;

    @Input() public isAuan: boolean = false;

    public seizedFishingGearForm!: FormGroup;
    public seizedFishingGear: PenalDecreeSeizedFishingGearDTO[] = [];
    public translate: FuseTranslationLoaderService;

    public isDisabled: boolean = false;

    public fishingGear: NomenclatureDTO<number>[] = [];
    public confiscatedFishingGearActions: NomenclatureDTO<number>[] = [];
    public territoryUnits: NomenclatureDTO<number>[] = [];

    @ViewChild('seizedFishingGearTable')
    private seizedFishingGearTable!: TLDataTableComponent;

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
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.FishingGear, this.nomenclatures.getFishingGear.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.TerritoryUnits, this.nomenclatures.getTerritoryUnits.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.InspConfiscationActions, this.service.getConfiscationActions.bind(this.service), false)
        ).toPromise();

        this.fishingGear = nomenclatures[0];
        this.territoryUnits = nomenclatures[1];

        if (this.isAuan) {
            this.confiscatedFishingGearActions = (nomenclatures[2] as AuanConfiscationActionsNomenclatureDTO[]).filter(x => x.actionGroup === InspConfiscationActionGroupsEnum.AUANGear);
        }
        else {
            this.confiscatedFishingGearActions = (nomenclatures[2] as AuanConfiscationActionsNomenclatureDTO[]).filter(x => x.actionGroup === InspConfiscationActionGroupsEnum.Gear);
        }
    }

    public writeValue(value: PenalDecreeSeizedFishingGearDTO[]): void {
        if (value !== null && value !== undefined) {
            setTimeout(() => {
                this.seizedFishingGear = value;
            });
        }
        else {
            setTimeout(() => {
                this.seizedFishingGear = [];
            });
        }
    }

    public onUndoAddEditRow(row: GridRow<PenalDecreeSeizedFishingGearDTO>): void {
        this.seizedFishingGearTable.undoAddEditRow(row);
    }

    public setDisabledState(isDisabled: boolean): void {
        this.isDisabled = isDisabled;
        if (this.isDisabled) {
            this.seizedFishingGearForm.disable();
        }
        else {
            this.seizedFishingGearForm.enable();
        }
    }

    public seizedFishingGearRecordChanged(event: RecordChangedEventArgs<PenalDecreeSeizedFishingGearDTO>): void {
        this.seizedFishingGear = this.seizedFishingGearTable.rows.map(x => new PenalDecreeSeizedFishingGearDTO({
            id: x.id,
            fishingGearId: x.fishingGearId,
            confiscationActionId: x.confiscationActionId,
            storageTerritoryUnitId: x.storageTerritoryUnitId,
            count: x.count,
            length: x.length,
            netEyeSize: x.netEyeSize,
            comments: x.comments,
            isActive: x.isActive ?? true
        }));

        this.onChanged(this.seizedFishingGear);
    }

    protected getValue(): PenalDecreeSeizedFishingGearDTO[] {
        this.seizedFishingGear = this.seizedFishingGearTable.rows;
        return this.seizedFishingGear;
    }

    protected buildForm(): AbstractControl {
        this.seizedFishingGearForm = new FormGroup({
            fishingGearIdControl: new FormControl(null, Validators.required),
            countControl: new FormControl(null, [Validators.required, TLValidators.number(1, undefined, 0)]),
            lengthControl: new FormControl(null, TLValidators.number(0)),
            netEyeSizeControl: new FormControl(null, TLValidators.number(0)),
            confiscationActionIdControl: new FormControl(null, Validators.required),
            commentsControl: new FormControl(null, Validators.maxLength(2000)),
            storageTerritoryUnitIdControl: new FormControl(null)
        });

        return new FormControl(null);
    }
}