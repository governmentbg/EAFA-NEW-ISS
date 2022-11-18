import { Component, Input, OnChanges, OnInit, Self, SimpleChange, SimpleChanges } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, Validators } from '@angular/forms';
import { DatePipe } from '@angular/common';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { FishingCapacityRemainderActionEnum } from '@app/enums/fishing-capacity-remainder-action.enum';
import { FishingCapacityFreedActionsDTO } from '@app/models/generated/dtos/FishingCapacityFreedActionsDTO';
import { FishingCapacityFreedActionsRegixDataDTO } from '@app/models/generated/dtos/FishingCapacityFreedActionsRegixDataDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { IFishingCapacityService } from '@app/interfaces/common-app/fishing-capacity.interface';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { ApplicationSubmittedByDTO } from '@app/models/generated/dtos/ApplicationSubmittedByDTO';
import { NewCertificateData } from '../acquired-fishing-capacity/acquired-fishing-capacity.component';

type FreedActionsType = FishingCapacityFreedActionsDTO | FishingCapacityFreedActionsRegixDataDTO;

@Component({
    selector: 'fishing-capacity-freed-actions',
    templateUrl: './fishing-capacity-freed-actions.component.html'
})
export class FishingCapacityFreedActionsComponent extends CustomFormControl<FreedActionsType> implements OnInit, OnChanges {
    @Input()
    public showOnlyRegiXData: boolean = false;

    @Input()
    public isDraft: boolean = false;

    @Input()
    public isEditing: boolean = false;

    @Input()
    public service!: IFishingCapacityService;

    @Input()
    public maxGrossTonnage: number | undefined;

    @Input()
    public maxPower: number | undefined;

    @Input()
    public submittedBy: ApplicationSubmittedByDTO | undefined;

    @Input()
    public expectedResults!: FishingCapacityFreedActionsRegixDataDTO;

    @Input()
    public newCertificateData: NewCertificateData | undefined;

    public newCertificateText: string | undefined;

    public readonly remainderActions: typeof FishingCapacityRemainderActionEnum = FishingCapacityRemainderActionEnum;

    public actions: NomenclatureDTO<FishingCapacityRemainderActionEnum>[] = [];

    private translate: FuseTranslationLoaderService;
    private datePipe: DatePipe;

    public constructor(@Self() ngControl: NgControl, translate: FuseTranslationLoaderService, datePipe: DatePipe) {
        super(ngControl);
        this.translate = translate;
        this.datePipe = datePipe;
        
        this.actions = [
            new NomenclatureDTO<FishingCapacityRemainderActionEnum>({
                value: FishingCapacityRemainderActionEnum.Certificate,
                displayName: this.translate.getValue('fishing-capacity.capacity-remainder-action-certificate'),
                isActive: true
            }),
            new NomenclatureDTO<FishingCapacityRemainderActionEnum>({
                value: FishingCapacityRemainderActionEnum.NoCertificate,
                displayName: this.translate.getValue('fishing-capacity.capacity-remainder-action-no-certificate'),
                isActive: true
            }),
            new NomenclatureDTO<FishingCapacityRemainderActionEnum>({
                value: FishingCapacityRemainderActionEnum.Transfer,
                displayName: this.translate.getValue('fishing-capacity.capacity-remainder-action-transfer'),
                isActive: true
            })
        ];

        this.form.get('actionControl')!.valueChanges.subscribe({
            next: (action: NomenclatureDTO<FishingCapacityRemainderActionEnum>) => {
                if (action?.value === FishingCapacityRemainderActionEnum.Transfer) {
                    setTimeout(() => {
                        this.form.get('holdersControl')!.updateValueAndValidity();
                    });
                }
                else {
                    this.form.get('holdersControl')!.setErrors(null);
                    this.form.get('holdersControl')!.clearValidators();
                    this.form.get('holdersControl')!.updateValueAndValidity();
                }
                this.form.updateValueAndValidity({ emitEvent: false });
            }
        });

        this.onMarkAsTouched.subscribe({
            next: () => {
                this.form.get('holdersControl')!.updateValueAndValidity();
                this.form.updateValueAndValidity({ emitEvent: false });
            }
        });
    }

    public ngOnInit(): void {
        this.initCustomFormControl();

        if (this.newCertificateData !== undefined && this.newCertificateData !== null) {
            this.buildNewCertificateText();
        }
    }

    public ngOnChanges(changes: SimpleChanges): void {
        const newCertificateData: SimpleChange | undefined = changes['newCertificateData'];

        if (newCertificateData !== undefined && newCertificateData !== null) {
            this.buildNewCertificateText();
        }
    }

    public writeValue(value: FreedActionsType): void {
        if (value !== null && value !== undefined) {
            this.form.get('actionControl')!.setValue(this.actions.find(x => x.value === value.action));
            this.form.get('holdersControl')!.setValue(value.holders);
        }
        else {
            this.form.get('actionControl')!.setValue(undefined);
            this.form.get('holdersControl')!.setValue([]);
        }
    }

    protected buildForm(): AbstractControl {
        return new FormGroup({
            actionControl: new FormControl(null, Validators.required),
            holdersControl: new FormControl(null)
        });
    }

    protected getValue(): FreedActionsType {
        const result: FreedActionsType = this.showOnlyRegiXData
            ? new FishingCapacityFreedActionsRegixDataDTO()
            : new FishingCapacityFreedActionsDTO();

        result.action = this.form.get('actionControl')!.value?.value ?? undefined;

        if (result.action === FishingCapacityRemainderActionEnum.Transfer) {
            result.holders = this.form.get('holdersControl')!.value ?? [];
        }
        else {
            result.holders = [];
        }

        return result;
    }

    private buildNewCertificateText(): void {
        if (this.newCertificateData !== undefined && this.newCertificateData !== null) {
            const tonnage: string = Number(this.newCertificateData.tonnage).toFixed(2);
            const power: string = Number(this.newCertificateData.power).toFixed(2);

            this.newCertificateText = `${this.translate.getValue('fishing-capacity.new-certificate-will-be-issued-with')} ${tonnage} `
                + `${this.translate.getValue('fishing-capacity.new-certificate-tonnage')} ${this.translate.getValue('fishing-capacity.new-certificate-and')} `
                + `${power} ${this.translate.getValue('fishing-capacity.new-certificate-power')} `
                + `${this.translate.getValue('fishing-capacity.new-certificate-with-validity')} `;

            if (this.newCertificateData!.validThreeYears) {
                this.newCertificateText += `${this.translate.getValue('fishing-capacity.new-certificate-for-three-years-after-application-processing')}`;
            }
            else {
                this.newCertificateText += `${this.datePipe.transform(this.newCertificateData!.validTo, 'dd.MM.yyyy')}`
            }
        }
        else {
            this.newCertificateData = undefined;
        }
    }
}