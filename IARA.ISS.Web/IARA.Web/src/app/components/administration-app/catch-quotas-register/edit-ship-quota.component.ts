import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { ShipNomenclatureDTO } from '@app/models/generated/dtos/ShipNomenclatureDTO';
import { ShipQuotaEditDTO } from '@app/models/generated/dtos/ShipQuotaEditDTO';
import { ShipQuotasService } from '@app/services/administration-app/ship-quotas.service';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { TLSnackbar } from '@app/shared/components/snackbar/tl.snackbar';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { ShipsUtils } from '@app/shared/utils/ships.utils';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { forkJoin } from 'rxjs';

@Component({
    selector: 'edit-ship-quota-component',
    templateUrl: './edit-ship-quota.component.html',
})
export class EditShipQuotaComponent implements IDialogComponent, OnInit {
    public fishQuotas: NomenclatureDTO<number>[] = [];
    public ships: ShipNomenclatureDTO[] = [];

    public model!: ShipQuotaEditDTO;
    public shipQuotaId: number | undefined;

    public editForm!: FormGroup;
    public translationService: FuseTranslationLoaderService;

    public isEditing: boolean = false;
    public isReadOnly: boolean = false;
    public shipQuotaAlreadyExistsErrors: boolean = false;

    public service: ShipQuotasService;
    private readonly commonNomenclatureService: CommonNomenclatures;
    private readonly snackbar: TLSnackbar;

    public constructor(
        service: ShipQuotasService,
        translationService: FuseTranslationLoaderService,
        commonNomenclatureService: CommonNomenclatures,
        snackbar: TLSnackbar
    ) {
        this.service = service;
        this.commonNomenclatureService = commonNomenclatureService;
        this.translationService = translationService;
        this.snackbar = snackbar;

        this.buildForm();
    }

    public async ngOnInit(): Promise<void> {
        this.isEditing = this.shipQuotaId !== undefined && this.shipQuotaId !== null;

        const nomenclatures: NomenclatureDTO<number>[][] = await forkJoin(
            this.commonNomenclatureService.getShips(),
            this.service.getYearlyQuotasForList()
        ).toPromise();

        this.ships = nomenclatures[0];
        this.fishQuotas = nomenclatures[1];

        this.editForm.get('quotaChangeBasisControl')!.clearValidators();

        if (this.shipQuotaId === undefined || this.shipQuotaId === null) {
            this.model = new ShipQuotaEditDTO();
        }
        else {
            this.editForm.controls.shipsControl.disable();
            this.editForm.controls.fishQuotasControl.disable();

            this.editForm.get('quotaChangeBasisControl')!.setValidators([Validators.required, Validators.maxLength(4000)]);
            this.editForm.get('quotaChangeBasisControl')!.updateValueAndValidity({ emitEvent: false });

            this.service.get(this.shipQuotaId).subscribe(result => {
                this.model = result;
                this.fillForm();
            });
        }
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.editForm.markAllAsTouched();

        if (this.editForm.valid) {
            this.fillModel();

            if (this.isEditing) {
                this.service.edit(this.model).subscribe({
                    next: () => {
                        NomenclatureStore.instance.clearNomenclature(NomenclatureTypes.Ships);
                        NomenclatureStore.instance.clearNomenclature(NomenclatureTypes.Fishes);
                        dialogClose(this.model);
                    }
                });
            }
            else {
                this.service.add(this.model).subscribe({
                    next: (id: number) => {
                        this.shipQuotaAlreadyExistsErrors = false;

                        this.model.id = id;

                        NomenclatureStore.instance.clearNomenclature(NomenclatureTypes.Ships);
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
            this.shipQuotaId = data.id;
            this.isReadOnly = data.isReadonly ?? false;
        }
    }

    private fillForm(): void {
        this.editForm.get('shipsControl')!.setValue(ShipsUtils.get(this.ships, this.model.shipId!));
        this.editForm.get('fishQuotasControl')!.setValue(this.fishQuotas.find(x => x.value === this.model.quotaId));
        this.editForm.get('quotaSizeControl')!.setValue(this.model.shipQuotaSize);

        if (this.isReadOnly) {
            this.editForm.disable();
        }
    }

    private fillModel(): void {
        if (this.isEditing) {
            this.model.changeBasis = this.editForm.get('quotaChangeBasisControl')!.value;
        }
        else {
            this.model.shipId = this.editForm.get('shipsControl')!.value?.value;
            this.model.quotaId = this.editForm.get('fishQuotasControl')!.value?.value;
        }

        this.model.shipQuotaSize = this.editForm.get('quotaSizeControl')!.value;
    }

    private buildForm(): void {
        this.editForm = new FormGroup({
            shipsControl: new FormControl(null, Validators.required),
            fishQuotasControl: new FormControl(null, Validators.required),
            quotaSizeControl: new FormControl(null, [Validators.required, TLValidators.number(0)]),
            quotaChangeBasisControl: new FormControl(null),
        });
    }

    private handleAddQuotaErrorResponse(response: HttpErrorResponse): void {
        if (response.error?.messages !== null && response.error?.messages !== undefined) {
            const messages: string[] = response.error.messages;

            if (messages.length !== 0) {
                this.snackbar.errorModel(response.error as ErrorModel);
            }
            else {
                this.snackbar.error(this.translationService.getValue('service.an-error-occurred-in-the-app'));
            }
        }

        if (response.error?.code === ErrorCode.AlreadySubmitted) {
            this.shipQuotaAlreadyExistsErrors = true;
        }
    }
}