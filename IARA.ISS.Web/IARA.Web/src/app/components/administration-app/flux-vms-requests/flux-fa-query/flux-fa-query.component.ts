import { Component } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';

import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { FluxVmsRequestsService } from '@app/services/administration-app/flux-vms-requests.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { FluxFAQueryRequestEditDTO } from '@app/models/generated/dtos/FluxFAQueryRequestEditDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { FluxFAQueryTypesEnum } from '@app/enums/flux-fa-query-types.enum';

@Component({
    selector: 'flux-fa-query',
    templateUrl: './flux-fa-query.component.html'
})
export class FluxFAQueryComponent implements IDialogComponent {
    public readonly form: FormGroup;
    public readonly queryTypes: NomenclatureDTO<FluxFAQueryTypesEnum>[] = [];

    public readonly faQueryTypes: typeof FluxFAQueryTypesEnum = FluxFAQueryTypesEnum;

    private model: FluxFAQueryRequestEditDTO = new FluxFAQueryRequestEditDTO();

    private readonly service: FluxVmsRequestsService;

    public constructor(service: FluxVmsRequestsService) {
        this.service = service;

        this.form = this.buildForm();

        for (const type in FluxFAQueryTypesEnum) {
            if (isNaN(Number(type))) {
                this.queryTypes.push(new NomenclatureDTO<FluxFAQueryTypesEnum>({
                    value: FluxFAQueryTypesEnum[type as keyof typeof FluxFAQueryTypesEnum],
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

            this.service.addFAQueryRequest(this.model).subscribe({
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
        const form = new FormGroup({
            queryTypeControl: new FormControl(undefined, [Validators.required]),
            dateTimeFromControl: new FormControl(undefined),
            dateTimeToControl: new FormControl(undefined),
            vesselIdsGroup: new FormGroup({
                vesselCfrControl: new FormControl(undefined),
                vesselIrcsControl: new FormControl(undefined),
            }),
            tripIdentifierControl: new FormControl(undefined),
            consolidatedControl: new FormControl(undefined, [Validators.required])
        });

        form.get('queryTypeControl')!.valueChanges.subscribe({
            next: (value: NomenclatureDTO<FluxFAQueryTypesEnum> | undefined) => {
                if (value) {
                    if (value.value === FluxFAQueryTypesEnum.VESSEL) {
                        form.get('dateTimeFromControl')!.setValidators([Validators.required]);
                        form.get('dateTimeToControl')!.setValidators([Validators.required]);
                        form.get('vesselIdsGroup')!.setValidators([this.vesselIdsValidator()]);
                        form.get('tripIdentifierControl')!.clearValidators();
                    }
                    else if (value.value === FluxFAQueryTypesEnum.TRIP) {
                        form.get('tripIdentifierControl')!.setValidators([Validators.required]);
                        form.get('dateTimeFromControl')!.clearValidators();
                        form.get('dateTimeToControl')!.clearValidators();
                        form.get('vesselIdsGroup')!.clearValidators();
                    }
                }
                else {
                    form.get('tripIdentifierControl')!.clearValidators();
                    form.get('dateTimeFromControl')!.clearValidators();
                    form.get('dateTimeToControl')!.clearValidators();
                    form.get('vesselIdsGroup')!.clearValidators();
                }

                form.get('tripIdentifierControl')!.markAsPending();
                form.get('dateTimeFromControl')!.markAsPending();
                form.get('dateTimeToControl')!.markAsPending();

                form.get('tripIdentifierControl')!.updateValueAndValidity();
                form.get('dateTimeFromControl')!.updateValueAndValidity();
                form.get('dateTimeToControl')!.updateValueAndValidity();
                form.get('vesselIdsGroup')!.updateValueAndValidity();
            }
        });

        return form;
    }

    private fillModel(): void {
        this.model.queryType = this.form.get('queryTypeControl')!.value.value;

        if (this.model.queryType === FluxFAQueryTypesEnum.VESSEL) {
            this.model.dateTimeFrom = this.form.get('dateTimeFromControl')!.value;
            this.model.dateTimeTo = this.form.get('dateTimeToControl')!.value;
            this.model.vesselCFR = this.form.get('vesselIdsGroup.vesselCfrControl')!.value;
            this.model.vesselIRCS = this.form.get('vesselIdsGroup.vesselIrcsControl')!.value;
        }
        else if (this.model.queryType === FluxFAQueryTypesEnum.TRIP) {
            this.model.tripIdentifier = this.form.get('tripIdentifierControl')!.value;
        }

        this.model.consolidated = this.form.get('consolidatedControl')!.value;
    }

    private vesselIdsValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            const cfr: string = control.get('vesselCfrControl')!.value;
            const ircs: string = control.get('vesselIrcsControl')!.value;

            if (!cfr && !ircs) {
                return { atLeastOneVesselIdRequired: true };
            }

            return null;
        };
    }
}