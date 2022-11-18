import { Component, OnInit } from '@angular/core';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { PenalPointsAppealDTO } from '@app/models/generated/dtos/PenalPointsAppealDTO';
import { EditPenalPointsComplaintDialogParams } from '../models/edit-penal-points-complaint-dialog-params.model';
import { forkJoin } from 'rxjs';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { IPenalPointsService } from '@app/interfaces/administration-app/penal-points.interface';
import { IPenalDecreesService } from '@app/interfaces/administration-app/penal-decrees.interface';
import { PenalDecreesService } from '@app/services/administration-app/penal-decrees.service';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { DateUtils } from '@app/shared/utils/date.utils';

@Component({
    selector: 'edit-penal-points-complaint-status',
    templateUrl: './edit-penal-points-complaint-status.component.html'
})
export class EditPenalPointsComplaintStatusComponent implements OnInit, IDialogComponent {
    public form!: FormGroup;
    public viewMode: boolean = false;

    public statusTypes: NomenclatureDTO<number>[] = [];
    public courts: NomenclatureDTO<number>[] = [];

    public model!: PenalPointsAppealDTO;

    private service!: IPenalPointsService;

    private readonly decreeService: IPenalDecreesService;
    private readonly translate: FuseTranslationLoaderService;

    public constructor(
        decreeService: PenalDecreesService,
        translate: FuseTranslationLoaderService
    ) {
        this.decreeService = decreeService;
        this.translate = translate;

        this.buildForm();
    }

    public async ngOnInit(): Promise<void> {
        const nomenclatures: (NomenclatureDTO<number>)[][] = await forkJoin(
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.PenalPointsStatuses, this.service.getAllPenalPointsStatuses.bind(this.service), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Courts, this.decreeService.getCourts.bind(this.decreeService), false)
        ).toPromise();

        this.statusTypes = nomenclatures[0];
        this.courts = nomenclatures[1];

        this.fillForm();
    }

    public setData(data: EditPenalPointsComplaintDialogParams, wrapperData: DialogWrapperData): void {
        this.service = data.service as IPenalPointsService;
        this.viewMode = data.viewMode;

        if (data.model === undefined) {
            this.model = new PenalPointsAppealDTO({ isActive: true });
        }
        else {
            if (this.viewMode) {
                this.form.disable();
            }

            this.model = data.model;
            this.fillForm();
        }

    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (this.viewMode) {
            dialogClose(this.model);
        }
        else {
            this.form.markAllAsTouched();
            if (this.form.valid) {
                this.fillModel();
                CommonUtils.sanitizeModelStrings(this.model);
                dialogClose(this.model);
            }
        }
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    private buildForm(): void {
        this.form = new FormGroup({
            statusTypeControl: new FormControl(null, Validators.required),
            appealNumControl: new FormControl(null, Validators.maxLength(50)),
            appealDateControl: new FormControl(null),
            courtControl: new FormControl(null),
            caseNumControl: new FormControl(null, Validators.maxLength(50)),
            caseDateControl: new FormControl(null)
        });
    }

    private fillForm(): void {
        this.form.get('statusTypeControl')!.setValue(this.statusTypes.find(x => x.value === this.model.statusId));
        this.form.get('appealNumControl')!.setValue(this.model.appealNum);
        this.form.get('appealDateControl')!.setValue(this.model.appealDate);
        this.form.get('courtControl')!.setValue(this.courts.find(x => x.value === this.model.courtId));
        this.form.get('caseNumControl')!.setValue(this.model.decreeNum);
        this.form.get('caseDateControl')!.setValue(this.model.decreeDate);
    }

    private fillModel(): void {
        if (this.form.get('statusTypeControl')!.valid) {
            const statusType: NomenclatureDTO<number> = this.form.get('statusTypeControl')!.value!;

            this.model.statusName = statusType.displayName;
            this.model.statusId = statusType.value;
        }

        this.model.dateOfChange = new Date();

        this.model.appealNum = this.form.get('appealNumControl')!.value;
        this.model.appealDate = this.form.get('appealDateControl')!.value;
        this.model.courtId = this.form.get('courtControl')!.value?.value;
        this.model.decreeNum = this.form.get('caseNumControl')!.value;
        this.model.decreeDate = this.form.get('caseDateControl')!.value;

        if (this.model.appealDate !== undefined && this.model.appealDate !== null) {
            this.model.details = `${this.translate.getValue('penal-points.edit-complaint-date')}: ${DateUtils.ToDisplayDateString(this.model.appealDate)} `;
        }

        if (this.model.appealNum !== undefined && this.model.appealNum !== null) {
            const appealNum: string = `${this.translate.getValue('penal-points.edit-complaint-num')}: ${this.model.appealNum} `

            if (this.model.details !== undefined && this.model.details !== null) {
                this.model.details += `; ${appealNum}`;
            }
            else {
                this.model.details = appealNum;
            }
        }

        if (this.model.decreeNum !== undefined && this.model.decreeNum !== null) {
            const decreeNum: string = `${this.translate.getValue('penal-points.edit-complaint-case-num')}: ${this.model.decreeNum}`;

            if (this.model.details !== undefined && this.model.details !== null) {
                this.model.details += `; ${decreeNum}`;
            }
            else {
                this.model.details = decreeNum;
            }
        }
    }
}