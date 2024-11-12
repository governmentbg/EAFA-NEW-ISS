import { Component, OnInit } from '@angular/core';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { FLUXISRQueryRequestEditDTO } from '@app/models/generated/dtos/FLUXISRQueryRequestEditDTO';
import { FluxVmsRequestsService } from '@app/services/administration-app/flux-vms-requests.service';
import { FluxIsrTypesEnum } from '@app/enums/flux-isr-types.enum';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';

@Component({
    selector: 'flux-isr-query',
    templateUrl: './flux-isr-query.component.html'
})
export class FluxIsrQueryComponent implements OnInit, IDialogComponent {
    public readonly form: FormGroup;
    public readonly queryTypes: NomenclatureDTO<FluxIsrTypesEnum>[] = [];

    public flagStates: NomenclatureDTO<number>[] = [];

    public readonly isrQueryTypes: typeof FluxIsrTypesEnum = FluxIsrTypesEnum;

    private model: FLUXISRQueryRequestEditDTO = new FLUXISRQueryRequestEditDTO();

    private readonly service: FluxVmsRequestsService;

    public constructor(service: FluxVmsRequestsService) {
        this.service = service;

        this.form = this.buildForm();

        for (const type in FluxIsrTypesEnum) {
            if (isNaN(Number(type))) {
                this.queryTypes.push(new NomenclatureDTO<FluxIsrTypesEnum>({
                    value: FluxIsrTypesEnum[type as keyof typeof FluxIsrTypesEnum],
                    displayName: type,
                    isActive: true
                }));
            }
        }
    }

    public ngOnInit(): void {
        this.service.getTerritories().subscribe({
            next: (territories: NomenclatureDTO<number>[]) => {
                this.flagStates = territories;
            }
        });
    }

    public setData(data: unknown, wrapperData: DialogWrapperData): void {
        //nothing to do
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.form.markAllAsTouched();

        if (this.form.valid) {
            this.fillModel();

            this.service.addIsrQueryRequest(this.model).subscribe({
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
        const form: FormGroup = new FormGroup({
            queryTypeControl: new FormControl(undefined, Validators.required),
            dateTimeFromControl: new FormControl(undefined, Validators.required),
            dateTimeToControl: new FormControl(undefined, Validators.required),
            vesselIdsGroup: new FormGroup({
                vesselCfrControl: new FormControl(undefined),
                vesselIrcsControl: new FormControl(undefined),
                vesselUviControl: new FormControl(undefined),
                vesselExternalMarkControl: new FormControl(undefined),
                vesselFlagStateControl: new FormControl(undefined)
            }),
            vehicleIdsGroup: new FormGroup({
                tractorIdentifierControl: new FormControl(undefined),
                trailerIdentifierControl: new FormControl(undefined),
                registrationCountryControl: new FormControl(undefined)
            })
        });

        form.get('queryTypeControl')!.valueChanges.subscribe({
            next: (value: NomenclatureDTO<FluxIsrTypesEnum> | undefined) => {
                if (value) {
                    if (value.value === FluxIsrTypesEnum.VESSEL) {
                        form.get('vesselIdsGroup')!.setValidators([this.vesselIdsValidator()]); //TODO externalMark flagState validator
                        form.get('vehicleIdsGroup')!.clearValidators();
                    }
                    else if (value.value === FluxIsrTypesEnum.TRANSPORT) {
                        form.get('vehicleIdsGroup')!.setValidators([this.vehicleIdsValidator()]);
                        form.get('vesselIdsGroup')!.clearValidators();
                    }
                    else {
                        form.get('vesselIdsGroup')!.clearValidators();
                        form.get('vehicleIdsGroup')!.clearValidators();
                    }
                }
                else {
                    form.get('vesselIdsGroup')!.clearValidators();
                    form.get('vehicleIdsGroup')!.clearValidators();
                }

                form.get('vesselIdsGroup')!.updateValueAndValidity({ emitEvent: false });
                form.get('vehicleIdsGroup')!.updateValueAndValidity({ emitEvent: false });
            }
        });

        return form;
    }

    private fillModel(): void {
        this.model.queryType = this.form.get('queryTypeControl')!.value?.value;
        this.model.dateTimeFrom = this.form.get('dateTimeFromControl')!.value;
        this.model.dateTimeTo = this.form.get('dateTimeToControl')!.value;

        if (this.model.queryType === FluxIsrTypesEnum.VESSEL) {
            this.model.vesselCFR = this.form.get('vesselIdsGroup.vesselCfrControl')!.value;
            this.model.vesselIRCS = this.form.get('vesselIdsGroup.vesselIrcsControl')!.value;
            this.model.vesselUVI = this.form.get('vesselIdsGroup.vesselUviControl')!.value;
            this.model.vesselExternalMark = this.form.get('vesselIdsGroup.vesselExternalMarkControl')!.value;
            this.model.flagStateCode = this.form.get('vesselIdsGroup.vesselFlagStateControl')!.value?.value;
        }
        else if (this.model.queryType === FluxIsrTypesEnum.TRANSPORT) {
            this.model.tractorIdentifier = this.form.get('vehicleIdsGroup.tractorIdentifierControl')!.value;
            this.model.trailerIdentifier = this.form.get('vehicleIdsGroup.trailerIdentifierControl')!.value;
            this.model.registrationCountryCode = this.form.get('vehicleIdsGroup.registrationCountryControl')!.value?.value;
        }
    }

    private vesselIdsValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            const cfr: string = control.get('vesselCfrControl')!.value;
            const ircs: string = control.get('vesselIrcsControl')!.value;
            const uvi: string = control.get('vesselUviControl')!.value;
            const externalMark: string = control.get('vesselExternalMarkControl')!.value;

            if (!cfr && !ircs && !uvi && !externalMark) {
                return { 'atLeatsOneVesselId': true };
            }

            return null;
        }
    }

    private vehicleIdsValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            const tractorNum: string = control.get('tractorIdentifierControl')!.value;
            const trailerNum: string = control.get('trailerIdentifierControl')!.value;

            if (!tractorNum && !trailerNum) {
                return { 'atLeastOneVehicleId': true };
            }

            return null;
        }
    }
}