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
import { forkJoin } from 'rxjs';
import { CrossCheckResultResolutionsEnum } from '@app/enums/cross-check-result-resolutions.enum';

@Component({
    selector: 'cross-check-results-resolution',
    templateUrl: './cross-check-results-resolution.component.html'
})
export class CrossCheckResultsResolutionComponent implements OnInit, IDialogComponent {
    public form!: FormGroup;
    public readOnly: boolean = false;
    public readonly today: Date = new Date();

    public resolutions: NomenclatureDTO<number>[] = [];
    public resolutionTypes: NomenclatureDTO<number>[] = [];

    private readonly service: CrossChecksService;

    private crossCheckId: number | undefined;
    private model!: CrossCheckResolutionEditDTO;

    public constructor(
        service: CrossChecksService
    ) {
        this.service = service;

        this.buildform();
    }

    public async ngOnInit(): Promise<void> {
        const nomenclatures: NomenclatureDTO<number>[][] = await forkJoin(
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.CheckResolutions, this.service.getCheckResolutions.bind(this.service), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.CheckResolutionTypes, this.service.getCheckResolutionTypes.bind(this.service), false)
        ).toPromise();

        this.resolutions = nomenclatures[0];
        this.resolutionTypes = nomenclatures[1];

        if (this.crossCheckId !== undefined && this.crossCheckId !== null) {
            this.service.getCrossCheckResolution(this.crossCheckId).subscribe({
                next: (resolution: CrossCheckResolutionEditDTO) => {
                    this.model = resolution;
                    this.fillForm();
                }
            });
        }
        else {
            this.model = new CrossCheckResolutionEditDTO();
        }
    }

    public setData(data: DialogParamsModel, buttons: DialogWrapperData): void {
        this.crossCheckId = data.id;
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
            resolutionTypeControl: new FormControl(null),
            resolutionDateControl: new FormControl(null, Validators.required),
            resolutionDetailsControl: new FormControl(null, [Validators.required, Validators.maxLength(200)])
        });

        this.form.get('resolutionControl')!.valueChanges.subscribe({
            next: (value: NomenclatureDTO<number> | undefined) => {
                this.form.get('resolutionTypeControl')!.clearValidators();

                if (value !== undefined && value !== null) {
                    if (value.code !== CrossCheckResultResolutionsEnum[CrossCheckResultResolutionsEnum.Unresolved]) {
                        this.form.get('resolutionTypeControl')!.setValidators(Validators.required);
                    }
                }

                this.form.get('resolutionTypeControl')!.updateValueAndValidity({ emitEvent: false });
                this.form.get('resolutionTypeControl')!.markAsPending({ emitEvent: false });
            }
        });
    }

    private fillForm(): void {
        this.form.get('resolutionControl')!.setValue(this.resolutions.find(x => x.value === this.model.resolutionId));
        this.form.get('resolutionDateControl')!.setValue(this.model.resolutionDate);
        this.form.get('resolutionDetailsControl')!.setValue(this.model.resolutionDetails);
        this.form.get('resolutionTypeControl')!.setValue(this.resolutionTypes.find(x => x.value === this.model.resolutionTypeId));

    }

    private fillModel(): void {
        this.model.checkResultId = this.crossCheckId;
        this.model.resolutionId = this.form.get('resolutionControl')!.value.value;
        this.model.resolutionDate = this.form.get('resolutionDateControl')!.value;
        this.model.resolutionDetails = this.form.get('resolutionDetailsControl')!.value;
        this.model.resolutionTypeId = this.form.get('resolutionTypeControl')!.value?.value;
    }
}