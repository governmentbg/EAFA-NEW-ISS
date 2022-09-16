import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { forkJoin } from 'rxjs';

import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { InspectionsService } from '@app/services/administration-app/inspections.service';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { InspectionConstativeProtocolDTO } from '@app/models/generated/dtos/InspectionConstativeProtocolDTO';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { CPCatchTableModel } from './models/cp-catch-table.model';
import { CPFishingGearTableModel } from './models/cp-fishing-gear-table.model';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';

@Component({
    selector: 'review-old-inspection',
    templateUrl: './review-old-inspection.component.html',
})
export class ReviewOldInspectionComponent implements OnInit, IDialogComponent {
    public form!: FormGroup;

    public catches: CPCatchTableModel[] = [];
    public fishingGears: CPFishingGearTableModel[] = [];

    public fishes: NomenclatureDTO<number>[] = [];
    public fishingGearTypes: NomenclatureDTO<number>[] = [];

    private id: number | undefined;

    private readonly service: InspectionsService;
    private readonly nomenclatures: CommonNomenclatures;

    public constructor(service: InspectionsService, nomenclatures: CommonNomenclatures) {
        this.service = service;
        this.nomenclatures = nomenclatures;

        this.buildForm();
    }

    setData(data: DialogParamsModel, wrapperData: DialogWrapperData): void {
        this.id = data?.id;
    }

    dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public async ngOnInit(): Promise<void> {
        this.form.disable();

        const nomenclatureTables = await forkJoin(
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.Fishes, this.nomenclatures.getFishTypes.bind(this.nomenclatures), false
            ),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.FishingGear, this.nomenclatures.getFishingGear.bind(this.nomenclatures), false
            ),
        ).toPromise();

        this.fishes = nomenclatureTables[0];
        this.fishingGearTypes = nomenclatureTables[1];

        const model = (await this.service.get(this.id!).toPromise()) as InspectionConstativeProtocolDTO;

        this.form.get('numControl')!.setValue(model.reportNum);
        this.form.get('dateControl')!.setValue(model.startDate);
        this.form.get('locationControl')!.setValue(model.location);
        this.form.get('inspectorControl')!.setValue(model.inspectorName);
        this.form.get('witness1Control')!.setValue(model.witness1Name);
        this.form.get('witness2Control')!.setValue(model.witness2Name);
        this.form.get('objectControl')!.setValue(model.inspectedObjectName);
        this.form.get('inspectedPersonControl')!.setValue(model.inspectedPersonName);
        this.form.get('inspectorCommentControl')!.setValue(model.inspectorComment);
        this.form.get('actionsTakenControl')!.setValue(model.actionsTaken);

        this.catches = model.catches?.map(f => {
            const m = new CPCatchTableModel(f);
            m.fishName = this.fishes.find(s => s.value === f.fishId)?.displayName;
            return m;
        }) ?? [];

        this.fishingGears = model.fishingGears?.map(f => {
            const m = new CPFishingGearTableModel(f);
            m.fishingGearName = this.fishingGearTypes.find(s => s.value === f.fishingGearId)?.displayName;
            return m;
        }) ?? [];
    }

    private buildForm(): void {
        this.form = new FormGroup({
            numControl: new FormControl(null),
            dateControl: new FormControl(null),
            locationControl: new FormControl(null),
            inspectorControl: new FormControl(null),
            witness1Control: new FormControl(null),
            witness2Control: new FormControl(null),
            objectControl: new FormControl(null),
            inspectedPersonControl: new FormControl(null),
            inspectorCommentControl: new FormControl(null),
            actionsTakenControl: new FormControl(null),
        });
    }
}