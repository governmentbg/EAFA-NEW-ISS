import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { MaximumFishingCapacityEditDTO } from '@app/models/generated/dtos/MaximumFishingCapacityEditDTO';
import { FishingCapacityAdministrationService } from '@app/services/administration-app/fishing-capacity-administration.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { LatestMaximumCapacityDTO } from '@app/models/generated/dtos/LatestMaximumCapacityDTO';

@Component({
    selector: 'edit-maximum-fishing-capacity',
    templateUrl: './edit-maximum-fishing-capacity.component.html'
})
export class EditMaximumFishingCapacityComponent implements OnInit, IDialogComponent {
    public minRegulationDate: Date | undefined;
    public maxRegulationDate: Date | undefined;

    public form!: FormGroup;

    private service: FishingCapacityAdministrationService;
    private id: number | undefined;
    private model!: MaximumFishingCapacityEditDTO;
    private readOnly: boolean = false;

    public constructor(service: FishingCapacityAdministrationService) {
        this.service = service;

        this.buildForm();
    }

    public ngOnInit(): void {
        this.service.getLatestMaximumCapacities().subscribe({
            next: (result: LatestMaximumCapacityDTO) => {
                if (result !== undefined && result !== null) {
                    if (this.id === undefined) {
                        this.minRegulationDate = new Date(result.date!);
                        this.minRegulationDate.setDate(this.minRegulationDate.getDate() + 1);

                        this.maxRegulationDate = new Date();
                    }
                    else {
                        if (this.id !== result.id!) {
                            this.form.get('dateControl')!.disable();
                        }
                        else {
                            if (result.prevDate !== undefined && result.prevDate !== null) {
                                this.minRegulationDate = new Date(result.prevDate);
                                this.minRegulationDate.setDate(this.minRegulationDate.getDate() + 1);
                            }
                            this.maxRegulationDate = new Date();
                        }
                    }
                }
                else {
                    this.maxRegulationDate = new Date();
                }
            }
        }).add(() => {
            if (this.id === undefined) {
                this.model = new MaximumFishingCapacityEditDTO();
            }
            else {
                this.service.getMaximumCapacity(this.id).subscribe({
                    next: (capacity: MaximumFishingCapacityEditDTO) => {
                        this.model = capacity;
                        this.fillForm();
                    }
                });
            }
        });
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.form.markAllAsTouched();
        if (this.form.valid) {
            this.fillModel();
            CommonUtils.sanitizeModelStrings(this.model);

            if (this.id !== undefined) {
                this.service.editMaximumCapacity(this.model).subscribe({
                    next: () => {
                        dialogClose(this.model);
                    }
                });
            }
            else {
                this.service.addMaximumCapacity(this.model).subscribe({
                    next: (id: number) => {
                        this.model.id = id;
                        dialogClose(this.model);
                    }
                });
            }
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public setData(data: DialogParamsModel, wrapperData: DialogWrapperData): void {
        this.id = data?.id;
        this.readOnly = data?.isReadonly ?? false;
    }

    private buildForm(): void {
        this.form = new FormGroup({
            dateControl: new FormControl(null, Validators.required),
            regulationControl: new FormControl(null, [Validators.required, Validators.maxLength(500)]),
            tonnageControl: new FormControl(null, [Validators.required, TLValidators.number(0, undefined, 2)]),
            powerControl: new FormControl(null, [Validators.required, TLValidators.number(0, undefined, 2)])
        });
    }

    private fillForm(): void {
        this.form.get('dateControl')!.setValue(this.model.date);
        this.form.get('regulationControl')!.setValue(this.model.regulation);
        this.form.get('tonnageControl')!.setValue(this.model.grossTonnage!.toFixed(2));
        this.form.get('powerControl')!.setValue(this.model.power!.toFixed(2));

        if (this.readOnly) {
            this.form.disable();
        }
    }

    private fillModel(): void {
        this.model.date = this.form.get('dateControl')!.value;
        this.model.regulation = this.form.get('regulationControl')!.value;
        this.model.grossTonnage = this.form.get('tonnageControl')!.value;
        this.model.power = this.form.get('powerControl')!.value;
    }
}