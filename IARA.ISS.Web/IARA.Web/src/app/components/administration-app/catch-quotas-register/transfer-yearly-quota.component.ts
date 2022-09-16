import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { ShipQuotaEditDTO } from '@app/models/generated/dtos/ShipQuotaEditDTO';
import { YearlyQuotaEditDTO } from '@app/models/generated/dtos/YearlyQuotaEditDTO';
import { YearlyQuotasService } from '@app/services/administration-app/yearly-quotas.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { TLValidators } from '@app/shared/utils/tl-validators';

@Component({
    selector: 'transfer-yearly-quota-component',
    templateUrl: './transfer-yearly-quota.component.html',
})
export class TransferYearlyQuotaComponent implements IDialogComponent, OnInit {
    public newQuotaModel: YearlyQuotaEditDTO;
    public oldQuotaModel: YearlyQuotaEditDTO;
    public maxTransferQuota: number = 0;
    public quotaId: number | undefined;

    public form!: FormGroup;
    public translationService: FuseTranslationLoaderService;

    public service: YearlyQuotasService;

    public quotas: NomenclatureDTO<number>[] = [];

    public constructor(
        service: YearlyQuotasService,
        translationService: FuseTranslationLoaderService
    ) {
        this.service = service;
        this.translationService = translationService;

        this.newQuotaModel = new ShipQuotaEditDTO();
        this.oldQuotaModel = new ShipQuotaEditDTO();

        this.buildForm();
    }

    public ngOnInit(): void {
        this.service.getYearlyQuotasForList().subscribe({
            next: (result: NomenclatureDTO<number>[]) => {
                this.quotas = result;
            }
        });

        if (this.quotaId !== undefined && this.quotaId !== null) {
            this.service.get(this.quotaId).subscribe(newQuota => {
                this.newQuotaModel = newQuota;

                this.service.getLastYearsQuota(this.quotaId!).subscribe(oldQuota => {
                    if (oldQuota !== null && oldQuota !== undefined) {
                        this.oldQuotaModel = oldQuota;

                        if (this.oldQuotaModel.leftoverValueKg !== null && this.oldQuotaModel.leftoverValueKg !== undefined) {
                            this.maxTransferQuota = this.oldQuotaModel.leftoverValueKg;
                        }

                        this.form.get('transferQuotaSizeControl')!.setValidators([Validators.required, TLValidators.number(0, this.maxTransferQuota)]);
                        this.fillForm();
                    }
                });
            });
        }
    }

    public setData(data: DialogParamsModel, buttons: DialogWrapperData): void {
        if (data !== undefined && data.id !== undefined && data.id !== 0) {
            this.quotaId = data.id;
        }
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.form.markAllAsTouched();
        if (this.form.valid) {
            const transferValue = this.form.get('transferQuotaSizeControl')!.value;
            const basis = this.form.get('transferBasisControl')!.value;

            this.service.transfer(this.newQuotaModel.id!, this.oldQuotaModel.id!, transferValue, basis).subscribe(result => {
                dialogClose([this.newQuotaModel, this.oldQuotaModel]);
            });
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    private buildForm(): void {
        this.form = new FormGroup({
            currentQuotaControl: new FormControl({ value: null, disabled: true }),
            oldQuotaControl: new FormControl({ value: null, disabled: true }),
            leftoverQuotaSizeControl: new FormControl({ value: null, disabled: true }),
            transferQuotaSizeControl: new FormControl(null, [Validators.required, TLValidators.number(0)]),
            transferBasisControl: new FormControl(null, Validators.required),
        });
    }

    private fillForm(): void {
        this.form.get('currentQuotaControl')!.setValue(this.quotas.find(x => x.value === this.newQuotaModel.id));
        this.form.get('oldQuotaControl')!.setValue(this.quotas.find(x => x.value === this.oldQuotaModel.id));
        this.form.get('leftoverQuotaSizeControl')!.setValue(this.oldQuotaModel.leftoverValueKg);
    }
}