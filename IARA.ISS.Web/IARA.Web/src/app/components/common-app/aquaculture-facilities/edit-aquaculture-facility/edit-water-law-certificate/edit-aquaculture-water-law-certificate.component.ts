import { Component, Input, OnInit, Optional, Self } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';

import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { IAquacultureFacilitiesService } from '@app/interfaces/common-app/aquaculture-facilities.interface';
import { AquacultureWaterLawCertificateDTO } from '@app/models/generated/dtos/AquacultureWaterLawCertificateDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { FormControlDataLoader } from '@app/shared/utils/form-control-data-loader';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { EditWaterLawCertificateDialogParams } from '../models/edit-water-law-certificate-dialog-params.model';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { DateRangeData } from '@app/shared/components/input-controls/tl-date-range/tl-date-range.component';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { DateRangeIndefiniteData } from '@app/shared/components/date-range-indefinite/date-range-indefinite.component';

@Component({
    selector: 'edit-aquaculture-water-law-certificate',
    templateUrl: './edit-aquaculture-water-law-certificate.component.html'
})
export class EditAquacultureWaterLawCertificateComponent extends CustomFormControl<AquacultureWaterLawCertificateDTO> implements OnInit, IDialogComponent {
    @Input()
    public service!: IAquacultureFacilitiesService;

    public isDialog: boolean = false;

    public waterLawCertificateTypes: NomenclatureDTO<number>[] = [];

    private id: number | undefined;
    private isActive: boolean = true;
    private model: AquacultureWaterLawCertificateDTO | undefined;
    private viewMode: boolean = false;

    private readonly loader: FormControlDataLoader;

    public constructor(@Optional() @Self() ngControl: NgControl) {
        super(ngControl);

        this.loader = new FormControlDataLoader(this.getNomenclatures.bind(this));
    }

    public ngOnInit(): void {
        this.initCustomFormControl();

        if (this.model !== undefined && this.model !== null) {
            this.writeValue(this.model);
        }

        this.loader.load();
    }

    public writeValue(value: AquacultureWaterLawCertificateDTO): void {
        if (value !== null && value !== undefined) {
            this.id = value.id;
            this.isActive = value.isActive ?? false;

            if (value.certificateTypeId !== undefined && value !== null) {
                this.loader.load(() => {
                    this.form.get('waterLawCertificateTypeControl')!.setValue(this.waterLawCertificateTypes.find(x => x.value === value!.certificateTypeId));
                });
            }

            this.form.get('waterLawCertificateNumControl')!.setValue(value.certificateNum);
            this.form.get('waterLawCertificateIssuerControl')!.setValue(value.certificateIssuer);
            this.form.get('waterLawCertificateCommentsControl')!.setValue(value.comments);
            this.form.get('waterLawCertificateValidityControl')!.setValue(new DateRangeIndefiniteData({
                range: new DateRangeData({ start: value.certificateValidFrom, end: value.certificateValidTo }),
                indefinite: value.isCertificateIndefinite ?? false
            }));
        }
    }

    public setData(data: EditWaterLawCertificateDialogParams, wrapperData: DialogWrapperData): void {
        this.isDialog = true;
        this.service = data.service;
        this.model = data.model;
        this.viewMode = data.viewMode;

        this.buildForm();

        if (this.viewMode) {
            this.form.disable();
        }
    }

    public saveBtnClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (this.viewMode) {
            dialogClose(this.model);
        }
        else {
            this.form.markAllAsTouched();
            if (this.form.valid) {
                this.model = this.getValue();
                CommonUtils.sanitizeModelStrings(this.model);
                dialogClose(this.model);
            }
        }
    }

    public cancelBtnClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    protected buildForm(): AbstractControl {
        return new FormGroup({
            waterLawCertificateTypeControl: new FormControl(null, Validators.required),
            waterLawCertificateNumControl: new FormControl(null, [Validators.required, Validators.maxLength(50)]),
            waterLawCertificateIssuerControl: new FormControl(null, [Validators.required, Validators.maxLength(200)]),
            waterLawCertificateValidityControl: new FormControl(null, Validators.required),
            waterLawCertificateCommentsControl: new FormControl(null, Validators.maxLength(1000)),
        });
    }

    protected getValue(): AquacultureWaterLawCertificateDTO {
        const result: AquacultureWaterLawCertificateDTO = new AquacultureWaterLawCertificateDTO({
            id: this.id,
            certificateTypeId: this.form.get('waterLawCertificateTypeControl')!.value?.value,
            certificateNum: this.form.get('waterLawCertificateNumControl')!.value,
            certificateIssuer: this.form.get('waterLawCertificateIssuerControl')!.value,
            comments: this.form.get('waterLawCertificateCommentsControl')!.value,
            isActive: this.isActive
        });

        const validity: DateRangeIndefiniteData | undefined = this.form.get('waterLawCertificateValidityControl')!.value;
        if (validity !== undefined && validity !== null) {
            result.isCertificateIndefinite = validity.indefinite;
            result.certificateValidFrom = validity.range?.start;
            result.certificateValidTo = validity.range?.end;
        }
        else {
            result.isCertificateIndefinite = false;
        }

        return result;
    }

    private getNomenclatures(): Subscription {
        return NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.WaterLawCertificateTypes, this.service.getWaterLawCertificateTypes.bind(this.service), false
        ).subscribe({
            next: (result: NomenclatureDTO<number>[]) => {
                this.waterLawCertificateTypes = result;

                this.loader.complete();
            }
        });
    }
}