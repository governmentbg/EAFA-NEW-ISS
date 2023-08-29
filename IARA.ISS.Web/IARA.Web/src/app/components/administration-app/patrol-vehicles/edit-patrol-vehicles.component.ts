import { AfterViewInit, Component, OnInit } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PatrolVehiclesEditDTO } from '@app/models/generated/dtos/PatrolVehiclesEditDTO';
import { PatrolVehiclesService } from '@app/services/administration-app/patrol-vehicles.service';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';
import { PatrolVehiclesTypeEnum } from '@app/enums/patrol-vehicles-type.enum';
import { GetControlErrorLabelTextCallback } from '@app/shared/components/input-controls/base-tl-control';
import { HttpErrorResponse } from '@angular/common/http';
import { ErrorCode } from '@app/models/common/exception.model';

@Component({
    selector: 'edit-patrol-vehicles',
    templateUrl: './edit-patrol-vehicles.component.html'
})
export class EditPatrolVehiclesComponent implements OnInit, AfterViewInit, IDialogComponent {
    public editVehicleForm!: FormGroup;
    public readOnly: boolean = false;
    public flagCountries: NomenclatureDTO<number>[] = [];
    public patrolVehicleTypes: NomenclatureDTO<number>[] = [];
    public vesselTypes: NomenclatureDTO<number>[] = [];
    public institutions: NomenclatureDTO<number>[] = [];
    public vehicleTypeEnum: typeof PatrolVehiclesTypeEnum = PatrolVehiclesTypeEnum;
    public vehicleType: PatrolVehiclesTypeEnum | undefined;

    public getControlErrorLabelTextMethod: GetControlErrorLabelTextCallback = this.getControlErrorLabelText.bind(this);

    public hasPatrolVehicleAlreadyExistsError: boolean = false;

    private service: PatrolVehiclesService;
    private translate: FuseTranslationLoaderService;
    private nomenclatures: CommonNomenclatures;
    private vehicleId: number | undefined;
    private model!: PatrolVehiclesEditDTO;

    public constructor(
        service: PatrolVehiclesService,
        translate: FuseTranslationLoaderService,
        nomenclatures: CommonNomenclatures
    ) {
        this.service = service;
        this.translate = translate;
        this.nomenclatures = nomenclatures;

        this.buildForm();
    }

    public async ngOnInit(): Promise<void> {
        const nomenclatures = await forkJoin(
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.PatrolVehicleTypes, this.nomenclatures.getPatrolVehicleTypes.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.VesselTypes, this.nomenclatures.getVesselTypes.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Countries, this.nomenclatures.getCountries.bind(this.nomenclatures), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Institutions, this.nomenclatures.getInstitutions.bind(this.nomenclatures), false)
        ).toPromise();

        this.patrolVehicleTypes = nomenclatures[0];
        this.vesselTypes = nomenclatures[1];
        this.flagCountries = nomenclatures[2];
        this.institutions = nomenclatures[3];

        if (this.vehicleId === undefined) {
            this.model = new PatrolVehiclesEditDTO();
        }
        else {
            this.service.getPatrolVehicle(this.vehicleId).subscribe({
                next: (vehicle: PatrolVehiclesEditDTO) => {
                    this.model = vehicle;
                    this.fillForm();
                }
            });
        }
    }

    public ngAfterViewInit(): void {
        this.editVehicleForm.get('patrolVehicleTypeIdControl')!.valueChanges.subscribe({
            next: (type: NomenclatureDTO<number> | undefined) => {
                this.editVehicleForm.get('registerNumControl')!.clearValidators();
                this.editVehicleForm.get('cfrControl')!.clearValidators();

                if (type !== undefined && type !== null) {
                    this.vehicleType = PatrolVehiclesTypeEnum[type.code as keyof typeof PatrolVehiclesTypeEnum];

                    if (this.vehicleType === PatrolVehiclesTypeEnum.Ship || this.vehicleType === PatrolVehiclesTypeEnum.Boat) {
                        this.editVehicleForm.get('cfrControl')!.setValidators([Validators.required, TLValidators.cfr]);
                    }
                    else {
                        this.editVehicleForm.get('externalMarkControl')!.reset();
                        this.editVehicleForm.get('ircsCallSignControl')!.reset();
                        this.editVehicleForm.get('vesselTypeIdControl')!.reset();
                        this.editVehicleForm.get('uviControl')!.reset();
                        this.editVehicleForm.get('mmsiControl')!.reset();

                        this.editVehicleForm.get('registerNumControl')!.setValidators(Validators.required);
                    }
                }
                else {
                    this.vehicleType = undefined;
                }

                this.editVehicleForm.get('registerNumControl')!.markAsPending({ emitEvent: false });
                this.editVehicleForm.get('registerNumControl')!.updateValueAndValidity({ emitEvent: false });
                this.editVehicleForm.get('cfrControl')!.markAsPending({ emitEvent: false });
                this.editVehicleForm.get('cfrControl')!.updateValueAndValidity({ emitEvent: false });
            }
        });
    }

    public setData(data: DialogParamsModel, wrapperData: DialogWrapperData): void {
        this.vehicleId = data?.id;
        this.readOnly = data?.isReadonly ?? false;
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (this.readOnly) {
            dialogClose();
        }

        this.editVehicleForm.markAllAsTouched();
        if (this.editVehicleForm.valid) {
            this.fillModel();

            if (this.vehicleId !== undefined) {
                this.service.editPatrolVehicle(this.model).subscribe({
                    next: () => {
                        dialogClose(this.model);
                    },
                    error: (err: HttpErrorResponse) => {
                        this.handlePatrolVehicleExistsException(err);
                    }
                });
            }
            else {
                this.service.addPatrolVehicle(this.model).subscribe({
                    next: (id: number) => {
                        this.model.id = id;
                        dialogClose(this.model);
                    },
                    error: (err: HttpErrorResponse) => {
                        this.handlePatrolVehicleExistsException(err);
                    }
                });
            }
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public getControlErrorLabelText(controlName: string, errorValue: unknown, errorCode: string): TLError | undefined {
        if (errorCode === 'cfr') {
            if (errorValue === true) {
                return new TLError({ text: this.translate.getValue('patrol-vehicles.invalid-cfr'), type: 'error' });
            }
        }
        return undefined;
    }

    private buildForm(): void {
        this.editVehicleForm = new FormGroup({
            nameControl: new FormControl(null, [Validators.required, Validators.maxLength(500)]),
            institutionIdControl: new FormControl(null, Validators.required),
            patrolVehicleTypeIdControl: new FormControl(null, Validators.required),
            externalMarkControl: new FormControl(null, Validators.maxLength(50)),
            cfrControl: new FormControl(null, TLValidators.cfr),
            registerNumControl: new FormControl(null),
            uviControl: new FormControl(null, [TLValidators.exactLength(7), TLValidators.number(0)]),
            ircsCallSignControl: new FormControl(null, Validators.maxLength(7)),
            mmsiControl: new FormControl(null, [TLValidators.exactLength(9), TLValidators.number(0)]),
            vesselTypeIdControl: new FormControl(),
            flagCountryIdControl: new FormControl()
        }, this.uniquePatrolVehicleValidator());

        this.editVehicleForm.valueChanges.subscribe({
            next: () => {
                this.hasPatrolVehicleAlreadyExistsError = false;
                this.editVehicleForm.updateValueAndValidity({ emitEvent: false });
            }
        });
    }

    private fillForm(): void {
        this.editVehicleForm.get('nameControl')!.setValue(this.model.name);

        if (this.model.institutionId !== null && this.model.institutionId !== undefined) {
            const institution = NomenclatureStore.instance.getNomenclatureItem(NomenclatureTypes.Institutions, this.model.institutionId);
            this.editVehicleForm.controls.institutionIdControl.setValue(institution);
        }

        const patrolVehicleType = NomenclatureStore.instance.getNomenclatureItem(NomenclatureTypes.PatrolVehicleTypes, this.model.patrolVehicleTypeId!);
        this.editVehicleForm.controls.patrolVehicleTypeIdControl.setValue(patrolVehicleType);

        if (this.vehicleType === PatrolVehiclesTypeEnum.Ship || this.vehicleType === PatrolVehiclesTypeEnum.Boat) {
            this.editVehicleForm.get('externalMarkControl')!.setValue(this.model.externalMark);
            this.editVehicleForm.get('cfrControl')!.setValue(this.model.cfr);
            this.editVehicleForm.get('uviControl')!.setValue(this.model.uvi);
            this.editVehicleForm.get('ircsCallSignControl')!.setValue(this.model.ircscallSign);
            this.editVehicleForm.get('mmsiControl')!.setValue(this.model.mmsi);
        }
        else {
            this.editVehicleForm.get('registerNumControl')!.setValue(this.model.cfr);
        }

        if (this.model.vesselTypeId !== null && this.model.vesselTypeId !== undefined) {
            const vesselTypeType = NomenclatureStore.instance.getNomenclatureItem(NomenclatureTypes.VesselTypes, this.model.vesselTypeId!);
            this.editVehicleForm.controls.vesselTypeIdControl.setValue(vesselTypeType);
        }

        if (this.model.flagCountryId !== null && this.model.flagCountryId !== undefined) {
            const flagCountry = NomenclatureStore.instance.getNomenclatureItem(NomenclatureTypes.Countries, this.model.flagCountryId!);
            this.editVehicleForm.controls.flagCountryIdControl.setValue(flagCountry);
        }

        if (this.readOnly) {
            this.editVehicleForm.disable();
        }
    }

    private fillModel(): void {
        this.model.name = this.editVehicleForm.get('nameControl')!.value;
        this.model.institutionId = NomenclatureStore.getValue(this.editVehicleForm.controls.institutionIdControl.value);
        this.model.patrolVehicleTypeId = NomenclatureStore.getValue(this.editVehicleForm.controls.patrolVehicleTypeIdControl.value);
        this.model.externalMark = this.editVehicleForm.get('externalMarkControl')!.value;
        this.model.ircscallSign = this.editVehicleForm.get('ircsCallSignControl')!.value;
        this.model.uvi = this.editVehicleForm.get('uviControl')!.value;
        this.model.mmsi = this.editVehicleForm.get('mmsiControl')!.value;
        this.model.vesselTypeId = NomenclatureStore.getValue(this.editVehicleForm.controls.vesselTypeIdControl.value);
        this.model.flagCountryId = NomenclatureStore.getValue(this.editVehicleForm.controls.flagCountryIdControl.value);

        if (this.vehicleType === PatrolVehiclesTypeEnum.Ship || this.vehicleType === PatrolVehiclesTypeEnum.Boat) {
            this.model.cfr = this.editVehicleForm.get('cfrControl')!.value;
        }
        else {
            this.model.cfr = this.editVehicleForm.get('registerNumControl')!.value;
        }
    }

    private handlePatrolVehicleExistsException(response: HttpErrorResponse): void {
        if (response.error?.code === ErrorCode.PatrolVehicleAlreadyExists) {
            this.hasPatrolVehicleAlreadyExistsError = true;
            this.editVehicleForm.updateValueAndValidity({ emitEvent: false });
        }
    }

    private uniquePatrolVehicleValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (control === null || control === undefined) {
                return null;
            }

            if (this.hasPatrolVehicleAlreadyExistsError) {
                return { 'patrolVehicleAlreadyExists': true };
            }

            return null;
        }
    }
}