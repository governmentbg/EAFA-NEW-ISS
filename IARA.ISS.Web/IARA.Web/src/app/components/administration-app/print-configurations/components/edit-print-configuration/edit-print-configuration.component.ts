import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { forkJoin, Subscription } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';

import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { FormControlDataLoader } from '@app/shared/utils/form-control-data-loader';
import { PrintUserNomenclatureDTO } from '@app/models/generated/dtos/PrintUserNomenclatureDTO';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { PrintConfigurationsService } from '@app/services/administration-app/print-configurations.service';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { PrintConfigurationEditDTO } from '@app/models/generated/dtos/PrintConfigurationEditDTO';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';

@Component({
    selector: 'edit-print-configuration',
    templateUrl: './edit-print-configuration.component.html'
})
export class EditPrintConfigurationComponent implements OnInit, IDialogComponent {
    public form!: FormGroup;

    public applicationTypes: NomenclatureDTO<number>[] = [];
    public territoryUnits: NomenclatureDTO<number>[] = [];
    public signUsers: NomenclatureDTO<number>[] = [];
    public substituteUsers: NomenclatureDTO<number>[] = [];

    private id: number | undefined;
    private viewMode: boolean = false;
    private model!: PrintConfigurationEditDTO;

    private hasPrintConfigurationExistsError: boolean = false;

    private readonly nomenclatureLoader: FormControlDataLoader;
    private readonly service: PrintConfigurationsService;
    private readonly nomenclaturesService: CommonNomenclatures;

    public constructor(service: PrintConfigurationsService, nomenclaturesService: CommonNomenclatures) {
        this.nomenclatureLoader = new FormControlDataLoader(this.getNomenclatures.bind(this));
        this.service = service;
        this.nomenclaturesService = nomenclaturesService;

        this.buildForm();
    }

    public ngOnInit(): void {
        this.nomenclatureLoader.load();

        if (this.id !== null && this.id !== undefined) { // edit
            this.service.getPrintConfiguration(this.id).subscribe({
                next: (result: PrintConfigurationEditDTO) => {
                    this.model = result;
                    this.fillForm();
                }
            });
        }
        else { // add
            this.model = new PrintConfigurationEditDTO();
        }
    }

    public setData(data: DialogParamsModel, wrapperData: DialogWrapperData): void {
        this.id = data.id;
        this.viewMode = data.viewMode;

        if (this.viewMode) {
            this.form.disable();
        }
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.form.markAllAsTouched();

        if (this.form.valid) {
            this.model = this.fillModel();
            this.model = CommonUtils.sanitizeModelStrings(this.model);

            if (this.id !== null && this.id !== undefined) { // edit
                this.service.editPrintConfiguration(this.model).subscribe({
                    next: () => {
                        dialogClose(this.model);
                    },
                    error: (httpErrorResponse: HttpErrorResponse) => {
                        this.handleHttpErrorResponse(httpErrorResponse);
                    }
                });
            }
            else { // add
                this.service.addPrintConfiguratoin(this.model).subscribe({
                    next: (id: number) => {
                        this.model.id = id;
                        dialogClose(this.model);
                    },
                    error: (httpErrorResponse: HttpErrorResponse) => {
                        this.handleHttpErrorResponse(httpErrorResponse);
                    }
                });
            }
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    private getNomenclatures(): Subscription {
        const subscription: Subscription = forkJoin([
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.AllUsers, this.service.getUsersNomenclature.bind(this.service), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.TerritoryUnits, this.nomenclaturesService.getTerritoryUnits.bind(this.nomenclaturesService), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.ApplicationTypes, this.service.getApplicationTypes.bind(this.service), false)
        ]).subscribe({
            next: (nomenclatures: (NomenclatureDTO<number>[] | PrintUserNomenclatureDTO[])[]) => {
                this.signUsers = nomenclatures[0].slice();
                this.substituteUsers = nomenclatures[0].slice();
                this.territoryUnits = nomenclatures[1];
                this.applicationTypes = nomenclatures[2];

                this.nomenclatureLoader.complete();
            }
        });


        return subscription;
    }

    private buildForm(): void {
        this.form = new FormGroup({
            applicationTypeControl: new FormControl(),
            territoryUnitControl: new FormControl(),
            signUserControl: new FormControl(undefined, Validators.required),
            substituteUserControl: new FormControl(undefined),
            substituteReasonControl: new FormControl(undefined, Validators.maxLength(1000))
        }, this.uniquePrintConfigurationValidator());

        this.form.get('applicationTypeControl')!.valueChanges.subscribe({
            next: () => {
                this.hasPrintConfigurationExistsError = false;
                this.form.updateValueAndValidity({ emitEvent: false });
            }
        });

        this.form.get('territoryUnitControl')!.valueChanges.subscribe({
            next: () => {
                this.hasPrintConfigurationExistsError = false;
                this.form.updateValueAndValidity({ emitEvent: false });
            }
        });

        this.form.get('substituteUserControl')!.valueChanges.subscribe({
            next: (user: string | PrintUserNomenclatureDTO | null | undefined) => {
                this.setSubstituteValidators(user);
            }
        });
    }

    private fillForm(): void {
        this.nomenclatureLoader.load(() => {
            this.form.get('applicationTypeControl')!.setValue(this.applicationTypes.find(x => x.value === this.model.applicationTypeId));
            this.form.get('territoryUnitControl')!.setValue(this.territoryUnits.find(x => x.value === this.model.territoryUnitId));
            this.form.get('signUserControl')!.setValue(this.signUsers.find(x => x.value === this.model.signUserId));
            this.form.get('substituteUserControl')!.setValue(this.substituteUsers.find(x => x.value === this.model.substituteUserId));
            this.form.get('substituteReasonControl')!.setValue(this.model.substituteReason);
        });
    }

    private fillModel(): PrintConfigurationEditDTO {
        this.model.applicationTypeId = this.form.get('applicationTypeControl')!.value?.value;
        this.model.territoryUnitId = this.form.get('territoryUnitControl')!.value?.value;
        this.model.signUserId = this.form.get('signUserControl')!.value?.value;
        this.model.substituteUserId = this.form.get('substituteUserControl')!.value?.value;
        this.model.substituteReason = this.form.get('substituteReasonControl')!.value;

        return this.model;
    }

    private handleHttpErrorResponse(httpErrorResponse: HttpErrorResponse): void {
        if ((httpErrorResponse.error as ErrorModel)?.code === ErrorCode.PrintConfigurationAlreadyExists) { // двойката тип заявление + ТЗ вече съществува
            this.hasPrintConfigurationExistsError = true;
            this.form.updateValueAndValidity({ emitEvent: false });
        }
    }

    private uniquePrintConfigurationValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (control === null || control === undefined) {
                return null;
            }

            if (this.hasPrintConfigurationExistsError) {
                return { 'printConfigurationNotUnique': true };
            }

            return null;
        }
    }

    private setSubstituteValidators(user: string | PrintUserNomenclatureDTO | null | undefined): void {
        if (user instanceof NomenclatureDTO) {
            this.form.get('substituteReasonControl')!.setValidators([Validators.required, Validators.maxLength(1000)]);
        }
        else {
            this.form.get('substituteReasonControl')!.setValidators([Validators.maxLength(1000)]);
        }

        this.form.get('substituteReasonControl')!.markAsPending();
        this.form.get('substituteReasonControl')!.updateValueAndValidity({ emitEvent: false });
    }
}