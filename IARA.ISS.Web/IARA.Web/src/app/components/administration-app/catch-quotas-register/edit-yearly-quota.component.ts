import { Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { YearlyQuotaEditDTO } from '@app/models/generated/dtos/YearlyQuotaEditDTO';
import { YearlyQuotasService } from '@app/services/administration-app/yearly-quotas.service';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { FishNomenclatureDTO } from '@app/models/generated/dtos/FishNomenclatureDTO';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { forkJoin } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { ErrorSnackbarComponent } from '@app/shared/components/error-snackbar/error-snackbar.component';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { RequestProperties } from '@app/shared/services/request-properties';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';

@Component({
    selector: 'edit-yearly-quota-component',
    templateUrl: './edit-yearly-quota.component.html',
})
export class EditYearlyQuotaComponent implements OnInit, IDialogComponent {
    public fishes: FishNomenclatureDTO[] = [];
    public ports: NomenclatureDTO<number>[] = [];

    public model!: YearlyQuotaEditDTO;
    public id?: number;

    public editForm!: FormGroup;
    public translationService: FuseTranslationLoaderService;

    public isEditing: boolean = false;
    public isReadOnly: boolean = false;
    public quotaAlreadyExistsErrors: boolean = false;

    public readonly pageCode: PageCodeEnum = PageCodeEnum.CatchQuota;

    public service: YearlyQuotasService;

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    private readonly commonNomenclatureService: CommonNomenclatures;
    private readonly snackbar: MatSnackBar;

    public constructor(
        service: YearlyQuotasService,
        translationService: FuseTranslationLoaderService,
        commonNomenclatureService: CommonNomenclatures,
        snackbar: MatSnackBar
    ) {
        this.service = service;
        this.commonNomenclatureService = commonNomenclatureService;
        this.translationService = translationService;
        this.snackbar = snackbar;

        this.buildForm();
    }

    public async ngOnInit(): Promise<void> {
        this.isEditing = this.id !== undefined && this.id !== null;

        const nomenclatures: NomenclatureDTO<number>[][] = await forkJoin(
            NomenclatureStore.instance.getNomenclature<number>(
                NomenclatureTypes.Fishes, this.commonNomenclatureService.getFishTypes.bind(this.commonNomenclatureService), false),
            NomenclatureStore.instance.getNomenclature<number>(
                NomenclatureTypes.Ports, this.commonNomenclatureService.getPorts.bind(this.commonNomenclatureService), false)
        ).toPromise();

        this.fishes = nomenclatures[0];
        this.ports = nomenclatures[1];

        if (this.id === null || this.id === undefined) {
            this.model = new YearlyQuotaEditDTO();
            this.editForm.get('quotaChangeBasisControl')!.clearValidators();
            this.editForm.get('quotaChangeBasisControl')!.updateValueAndValidity({ emitEvent: false });
        }
        else {
            this.service.get(this.id).subscribe(result => {
                this.model = result;
                this.fillForm();
            });
        }
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (this.isReadOnly) {
            dialogClose();
        }

        this.editForm.markAllAsTouched();
        this.validityCheckerGroup.validate();

        if (this.editForm.valid) {
            this.fillModel();
            this.model = CommonUtils.sanitizeModelStrings(this.model);

            if (this.isEditing) {
                this.service.edit(this.model).subscribe({
                    next: () => {
                        NomenclatureStore.instance.clearNomenclature(NomenclatureTypes.Ports);
                        NomenclatureStore.instance.clearNomenclature(NomenclatureTypes.Fishes);
                        dialogClose(this.model);
                    }
                });
            }
            else {
                this.service.add(this.model).subscribe({
                    next: (id: number) => {
                        this.quotaAlreadyExistsErrors = false;

                        this.model.id = id;

                        NomenclatureStore.instance.clearNomenclature(NomenclatureTypes.Ports);
                        NomenclatureStore.instance.clearNomenclature(NomenclatureTypes.Fishes);
                        dialogClose(this.model);
                    },
                    error: (response: HttpErrorResponse) => {
                        this.handleAddQuotaErrorResponse(response);
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

    public setData(data: DialogParamsModel, buttons: DialogWrapperData): void {
        if (data !== undefined && data !== null) {
            this.id = data.id;
            this.isReadOnly = data.isReadonly ?? false;

            if (this.isReadOnly) {
                this.editForm.disable();
            }
        }
    }

    private fillForm(): void {
        this.editForm.controls.fishesControl.setValue(this.fishes.find(x => x.value === this.model.fishId));
        this.editForm.controls.quotaSizeControl.setValue(this.model.quotaValueKg);
        this.editForm.controls.filesControl.setValue(this.model.files);
        this.editForm.controls.portsControl.setValue(this.model.unloadPorts);

        if (this.model.year !== null && this.model.year !== undefined) {
            this.editForm.get('yearControl')!.setValue(new Date(this.model.year, 0, 1));
        }
    }

    private fillModel(): void {
        this.model.year = (this.editForm.get('yearControl')!.value as Date)?.getFullYear();
        this.model.fishId = this.editForm.get('fishesControl')!.value?.value;
        this.model.quotaValueKg = this.editForm.get('quotaSizeControl')!.value;
        this.model.files = this.editForm.get('filesControl')!.value;
        this.model.unloadPorts = this.editForm.get('portsControl')!.value;
        this.model.changeBasis = this.editForm.get('quotaChangeBasisControl')!.value;
    }

    private buildForm(): void {
        this.editForm = new FormGroup({
            yearControl: new FormControl(null, Validators.required),
            fishesControl: new FormControl(null, Validators.required),
            quotaSizeControl: new FormControl(null, [Validators.required, TLValidators.number(0, undefined, 0)]),
            filesControl: new FormControl(null),
            quotaChangeBasisControl: new FormControl(null, [Validators.required, Validators.maxLength(1000)]),
            portsControl: new FormControl(null)
        });
    }

    private handleAddQuotaErrorResponse(response: HttpErrorResponse): void {
        if (response.error?.messages !== null && response.error?.messages !== undefined) {
            const messages: string[] = response.error.messages;

            if (messages.length !== 0) {
                this.snackbar.openFromComponent(ErrorSnackbarComponent, {
                    data: response.error as ErrorModel,
                    duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                    panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
                });
            }
            else {
                this.snackbar.openFromComponent(ErrorSnackbarComponent, {
                    data: new ErrorModel({ messages: [this.translationService.getValue('service.an-error-occurred-in-the-app')] }),
                    duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                    panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
                });
            }
        }

        if (response.error?.code === ErrorCode.AlreadySubmitted) {
            this.quotaAlreadyExistsErrors = true;
            this.validityCheckerGroup.validate();
        }
    }
}