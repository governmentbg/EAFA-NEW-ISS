import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';

import { ICommercialFishingService } from '@app/interfaces/common-app/commercial-fishing.interface';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { IActionInfo } from '../dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '../dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '../dialog-wrapper/models/dialog-action-buttons.model';
import { OverlappingLogBooksDialogParamsModel } from './models/overlapping-log-books-dialog-params.model';
import { OverlappingLogBooksParameters } from './models/overlapping-log-books-parameters.model';
import { RangeOverlappingLogBooksDTO } from '@app/models/generated/dtos/RangeOverlappingLogBooksDTO';
import { LogBookGroupsEnum } from '@app/enums/log-book-groups.enum';


@Component({
    selector: 'overlapping-log-books',
    templateUrl: './overlapping-log-books.component.html'
})
export class OverlappingLogBooksComponent implements OnInit, IDialogComponent {
    public logBookGroupsEnum: typeof LogBookGroupsEnum = LogBookGroupsEnum;

    public rangeLogBooks: RangeOverlappingLogBooksDTO[] = [];
    public logBookStatuses: NomenclatureDTO<number>[] = [];
    public logBookGroup!: LogBookGroupsEnum;

    private service!: ICommercialFishingService;
    private nomenclaturesService: CommonNomenclatures;
    private parameters!: OverlappingLogBooksParameters[];

    public constructor(nomenclaturesService: CommonNomenclatures) {
        this.nomenclaturesService = nomenclaturesService;
    }

    public async ngOnInit(): Promise<void> {
        this.logBookStatuses = await NomenclatureStore.instance.getNomenclature<number>(
            NomenclatureTypes.LogBookStatuses, this.nomenclaturesService.getLogBookStatuses.bind(this.nomenclaturesService), false
        ).toPromise();

        this.service.getOverlappedLogBooks(this.parameters).subscribe({
            next: (results: RangeOverlappingLogBooksDTO[]) => {
                this.rangeLogBooks = results.filter(x => x.overlappingLogBooks !== null && x.overlappingLogBooks !== undefined && x.overlappingLogBooks.length > 0);
            }
        });
    }

    public setData(data: OverlappingLogBooksDialogParamsModel, buttons: DialogWrapperData): void {
        this.service = data.service;
        this.parameters = data.ranges;
        this.logBookGroup = data.logBookGroup;
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose(true);
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }
}