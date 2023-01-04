import { Component, OnInit } from '@angular/core';
import { forkJoin } from 'rxjs';

import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { PreviousTripsCatchRecordsDialogParams } from './models/previous-trips-catch-records-dialog-params.model';
import { ICatchesAndSalesService } from '@app/interfaces/common-app/catches-and-sales.interface';
import { OnBoardCatchRecordFishDTO } from '@app/models/generated/dtos/OnBoardCatchRecordFishDTO';
import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { FishNomenclatureDTO } from '@app/models/generated/dtos/FishNomenclatureDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { CatchZoneNomenclatureDTO } from '@app/models/generated/dtos/CatchZoneNomenclatureDTO';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';

@Component({
    selector: 'previous-trips-catch-records',
    templateUrl: './previous-trips-catch-records.component.html'
})
export class PreviousTripsCatchRecordsComponent implements OnInit, IDialogComponent {
    public catchRecordFishes: OnBoardCatchRecordFishDTO[] = [];

    public aquaticOrganisms: FishNomenclatureDTO[] = [];
    public catchTypes: NomenclatureDTO<number>[] = [];
    public catchSizes: NomenclatureDTO<number>[] = [];
    public catchQuadrants: CatchZoneNomenclatureDTO[] = [];

    private service!: ICatchesAndSalesService;
    private commonNomenclaturesService: CommonNomenclatures;
    private shipId!: number;

    public constructor(commonNomenclaturesService: CommonNomenclatures) {
        this.commonNomenclaturesService = commonNomenclaturesService;
    }

    public async ngOnInit(): Promise<void> {
        const nomenclatures: (NomenclatureDTO<number> | FishNomenclatureDTO | CatchZoneNomenclatureDTO)[][] = await forkJoin([
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Fishes, this.commonNomenclaturesService.getFishTypes.bind(this.commonNomenclaturesService)),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.AquacultureCatchTypes, this.service.getCatchTypes.bind(this.service)),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.FishSizes, this.service.getFishSizes.bind(this.service)),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.CatchZones, this.commonNomenclaturesService.getCatchZones.bind(this.commonNomenclaturesService))
        ]).toPromise();

        this.aquaticOrganisms = nomenclatures[0];
        this.catchTypes = nomenclatures[1];
        this.catchSizes = nomenclatures[2];
        this.catchQuadrants = nomenclatures[3];

        this.service.getPreviousTripOnBoardCatchRecords(this.shipId).subscribe({
            next: (results: OnBoardCatchRecordFishDTO[]) => {
                this.catchRecordFishes = results;
            }
        });
    }

    public setData(data: PreviousTripsCatchRecordsDialogParams, wrapperData: DialogWrapperData): void {
        this.service = data.service;
        this.shipId = data.shipId;
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        const selectedCatchRecordFishes: OnBoardCatchRecordFishDTO[] = this.catchRecordFishes.filter(x => x.isChecked && x.isActive);
        dialogClose(selectedCatchRecordFishes);
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public checkedRow(row: GridRow<OnBoardCatchRecordFishDTO>): void {
        const element: OnBoardCatchRecordFishDTO | undefined = this.catchRecordFishes!.find(x => x.id === row.data.id);
        if (element !== null && element !== undefined) {
            element.isChecked = !element.isChecked;
        }
    }

}