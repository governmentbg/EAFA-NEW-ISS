import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { FluxVmsRequestsService } from '@app/services/administration-app/flux-vms-requests.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { FluxSalesQueryRequestEditDTO } from '@app/models/generated/dtos/FluxSalesQueryRequestEditDTO';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { FluxSalesTypeEnum } from '@app/enums/flux-sales-type.enum';

@Component({
    selector: 'flux-sales-query',
    templateUrl: './flux-sales-query.component.html'
})
export class FluxSalesQueryComponent implements IDialogComponent {
    public readonly form: FormGroup;
    public readonly queryTypes: NomenclatureDTO<FluxSalesTypeEnum>[] = [];

    private model: FluxSalesQueryRequestEditDTO = new FluxSalesQueryRequestEditDTO();

    private readonly service: FluxVmsRequestsService;

    public constructor(service: FluxVmsRequestsService) {
        this.service = service;

        this.form = this.buildForm();

        for (const type in FluxSalesTypeEnum) {
            if (isNaN(Number(type))) {
                this.queryTypes.push(new NomenclatureDTO<FluxSalesTypeEnum>({
                    value: FluxSalesTypeEnum[type as keyof typeof FluxSalesTypeEnum],
                    displayName: type,
                    isActive: true
                }));
            }
        }
    }

    public setData(data: unknown, wrapperData: DialogWrapperData): void {
        // nothing to do
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.form.markAllAsTouched();

        if (this.form.valid) {
            this.fillModel();
            CommonUtils.sanitizeModelStrings(this.model);

            this.service.addSalesQueryRequest(this.model).subscribe({
                next: () => {
                    dialogClose(true);
                },
                error: () => {
                    dialogClose(false);
                }
            });
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose(false);
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose(false);
    }

    private buildForm(): FormGroup {
        return new FormGroup({
            queryTypeControl: new FormControl(undefined, Validators.required),
            dateTimeFromControl: new FormControl(undefined, Validators.required),
            dateTimeToControl: new FormControl(undefined, Validators.required),
            vesselCfrControl: new FormControl(undefined),
            salesIdControl: new FormControl(undefined),
            tripIdControl: new FormControl(undefined)
        });
    }

    private fillModel(): void {
        this.model.queryType = this.form.get('queryTypeControl')!.value?.value;
        this.model.dateTimeFrom = this.form.get('dateTimeFromControl')!.value;
        this.model.dateTimeTo = this.form.get('dateTimeToControl')!.value;
        this.model.vesselCFR = this.form.get('vesselCfrControl')!.value;
        this.model.salesID = this.form.get('salesIdControl')!.value;
        this.model.tripID = this.form.get('tripIdControl')!.value;
    }
}