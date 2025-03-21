﻿import { AfterViewInit, Component, OnInit } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { ShipQuotaEditDTO } from '@app/models/generated/dtos/ShipQuotaEditDTO';
import { ShipQuotasService } from '@app/services/administration-app/ship-quotas.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { TLValidators } from '@app/shared/utils/tl-validators';

@Component({
    selector: 'transfer-ship-quota-component',
    templateUrl: './transfer-ship-quota.component.html',
})
export class TransferShipQuotaComponent implements IDialogComponent, OnInit, AfterViewInit {
    public shipQuotas: NomenclatureDTO<number>[] = [];
    public allShipQuotas: NomenclatureDTO<number>[] = [];
    public quotaId: number | undefined;

    public newQuota!: ShipQuotaEditDTO;
    public oldQuota!: ShipQuotaEditDTO;

    public form!: FormGroup;
    public translationService: FuseTranslationLoaderService;
    public service: ShipQuotasService;

    private leftover: number | undefined;

    public constructor(
        service: ShipQuotasService,
        translationService: FuseTranslationLoaderService
    ) {
        this.service = service;
        this.translationService = translationService;

        this.form = this.buildForm();
    }

    public async ngOnInit(): Promise<void> {
        if (this.quotaId === undefined || this.quotaId === null) {
            this.newQuota = new ShipQuotaEditDTO();
        }
        else {
            this.allShipQuotas = await this.service.getShipQuotasForList(this.quotaId).toPromise();

            this.service.get(this.quotaId).subscribe({
                next: (result: ShipQuotaEditDTO) => {
                    this.newQuota = result;
                    this.shipQuotas = this.allShipQuotas.filter(x => x.value !== this.quotaId);
                    this.fillForm();
                }
            });
        }
    }

    public ngAfterViewInit(): void {
        this.form.get('fromShipsControl')!.valueChanges.subscribe({
            next: (entry: NomenclatureDTO<number> | undefined) => {
                if (entry !== undefined && entry !== null && typeof entry !== 'string') {
                    if (entry.value !== undefined && entry.value !== null) {
                        this.service.get(entry.value!).subscribe({
                            next: (quota: ShipQuotaEditDTO) => {
                                if (quota !== null && quota !== undefined) {
                                    this.oldQuota = quota;
                                    this.leftover = this.oldQuota.leftoverQuotaSize;
                                    this.form.get('quotaSizeControl')!.clearValidators();

                                    if (this.leftover !== undefined && this.leftover !== null) {
                                        this.form.get('leftoverQuotaSizeControl')!.setValue(this.leftover);
                                    }
                                }
                                else {
                                    this.leftover = undefined;
                                    this.form.get('leftoverQuotaSizeControl')!.setValue(undefined);
                                    this.form.get('quotaSizeControl')!.clearValidators();
                                }
                            }
                        });
                    }
                }
                else {
                    this.leftover = undefined;
                    this.form.get('leftoverQuotaSizeControl')!.setValue(undefined);
                    this.form.get('quotaSizeControl')!.clearValidators();
                }
            }
        });
    }

    public setData(data: DialogParamsModel, buttons: DialogWrapperData): void {
        if (data !== undefined && data !== null) {
            this.quotaId = data.id;
        }
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.form.markAllAsTouched();
        if (this.form.valid) {
            const transferValue = this.form.get('quotaSizeControl')!.value;
            const basis = this.form.get('quotaChangeBasisControl')!.value;

            this.service.transfer(this.newQuota.id!, this.oldQuota.id!, transferValue, basis).subscribe(result => {
                dialogClose(this.newQuota);
            });
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    private fillForm(): void {
        this.form.get('toShipsControl')!.setValue(this.allShipQuotas.find(x => x.value === this.newQuota.id));
    }

    private buildForm(): FormGroup {
        const form: FormGroup = new FormGroup({
            fromShipsControl: new FormControl(null, Validators.required),
            toShipsControl: new FormControl({ value: null, disabled: true }),
            leftoverQuotaSizeControl: new FormControl({ value: null, disabled: true }),
            quotaSizeControl: new FormControl(null, [Validators.required, TLValidators.number(0)]),
            quotaChangeBasisControl: new FormControl(null, [Validators.required, Validators.maxLength(4000)]),
        }, this.leftoverValidator());

        form.get('leftoverQuotaSizeControl')!.valueChanges.subscribe({
            next: (value: number | undefined) => {
                form.get('quotaSizeControl')!.clearValidators();

                if (value !== undefined && value !== null) {
                    form.get('quotaSizeControl')!.setValidators([Validators.required, TLValidators.number(0, undefined, 0), Validators.max(value)]);
                }

                form.get('quotaSizeControl')!.updateValueAndValidity();
            }
        });

        return form;
    }

    private leftoverValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (control === null || control === undefined) {
                return null;
            }

            if (this.form === null || this.form === undefined) {
                return null;
            }

            const leftover: number | undefined = this.form.get('leftoverQuotaSizeControl')!.value;

            if (leftover === null || leftover === undefined) {
                return null;
            }

            if (leftover <= 0) {
                return { leftoverShouldBeGreaterThanZero: true };
            }

            return null;
        }
    }
}