import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { CrossChecksService } from '@app/services/administration-app/cross-checks.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { CrossCheckResolutionEditDTO } from '@app/models/generated/dtos/CrossCheckResolutionEditDTO';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';

@Component({
    selector: 'cross-check-results-resolution',
    templateUrl: './cross-check-results-resolution.component.html'
})
export class CrossCheckResultsResolutionComponent implements OnInit, IDialogComponent {
    public form!: FormGroup;
    public readOnly: boolean = false;
    public readonly today: Date = new Date();

    public resolutions: NomenclatureDTO<number>[];

    private readonly service: CrossChecksService;

    private crossCheckId: number | undefined;
    private model!: CrossCheckResolutionEditDTO;

    public constructor(
        service: CrossChecksService
    ) {
        this.service = service;

        this.resolutions = [];

        this.buildform();
    }

    public async ngOnInit(): Promise<void> {
        this.model = new CrossCheckResolutionEditDTO();

        this.resolutions = await NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.CheckResolutionTypes, this.service.getCheckResolutionTypes.bind(this.service), false
        ).toPromise();
    }

    public setData(data: DialogParamsModel, buttons: DialogWrapperData): void {
        this.crossCheckId = data?.id;
        this.service.getCrossCheckResolution(this.crossCheckId).subscribe({
            next: (resolution: CrossCheckResolutionEditDTO) => {
                this.model = resolution;
                this.fillForm();
            }
        });
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.form.markAllAsTouched();
        if (this.form.valid) {
            this.fillModel();
            CommonUtils.sanitizeModelStrings(this.model);
            this.service.editCrossCheckResolution(this.model).subscribe({
                next: () => {
                    dialogClose(this.model);
                }
            });
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    private buildform(): void {
        this.form = new FormGroup({
            resolutionControl: new FormControl(null, Validators.required),
            resolutionDateControl: new FormControl(null, Validators.required),
            resolutionDetailsControl: new FormControl(null, [Validators.required, Validators.maxLength(200)])
        });
    }

    private fillForm(): void {
        this.form.get('resolutionControl')!.setValue(this.resolutions.find(x => x.value === this.model.resolutionId));
        this.form.get('resolutionDateControl')!.setValue(this.model.resolutionDate);
        this.form.get('resolutionDetailsControl')!.setValue(this.model.resolutionDetails);

    }

    private fillModel(): void {
        this.model.checkResultId = this.crossCheckId;
        this.model.resolutionId = this.form.get('resolutionControl')!.value.value;
        this.model.resolutionDate = this.form.get('resolutionDateControl')!.value;
        this.model.resolutionDetails = this.form.get('resolutionDetailsControl')!.value;
    }
}