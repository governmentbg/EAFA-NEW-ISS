import { AfterViewInit, Component, Input, OnChanges, OnDestroy, OnInit, Self, SimpleChange, SimpleChanges } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, Validators } from '@angular/forms';

import { CommonLogBookPageDataDTO } from '@app/models/generated/dtos/CommonLogBookPageDataDTO';
import { ShipNomenclatureDTO } from '@app/models/generated/dtos/ShipNomenclatureDTO';
import { forkJoin, Observable, Subscription } from 'rxjs';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { FormControlDataLoader } from '@app/shared/utils/form-control-data-loader';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { LogBookTypesEnum } from '@app/enums/log-book-types.enum';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { ShipsUtils } from '@app/shared/utils/ships.utils';

@Component({
    selector: 'common-log-book-page-data',
    templateUrl: './common-log-book-page-data.component.html'
})
export class CommonLogBookPageDataComponent extends CustomFormControl<CommonLogBookPageDataDTO> implements OnInit, AfterViewInit, OnChanges, OnDestroy {
    @Input()
    public isReadonly: boolean = false;

    @Input()
    public logBookType!: LogBookTypesEnum;

    public readonly logBookTypesEnum: typeof LogBookTypesEnum = LogBookTypesEnum;

    public ships: ShipNomenclatureDTO[] = [];

    private model!: CommonLogBookPageDataDTO;

    private commonNomenclatures: CommonNomenclatures;
    private readonly loader!: FormControlDataLoader;
    private isImportNotByShipValueChangeSubscriber: Subscription | undefined;

    public constructor(
        @Self() ngControl: NgControl,
        commonNomenclatures: CommonNomenclatures
    ) {
        super(ngControl);

        this.commonNomenclatures = commonNomenclatures;
        this.loader = new FormControlDataLoader(this.getNomenclatures.bind(this));
    }    

    public ngOnInit(): void {
        this.initCustomFormControl();
        this.loader.load();
    }

    public ngAfterViewInit(): void {
        this.isImportNotByShipValueChangeSubscriber = this.form.get('isImportNotByShipControl')!.valueChanges.subscribe({
            next: (value: boolean | undefined) => {
                this.setFormValidators();
            }
        });
    }

    public ngOnChanges(changes: SimpleChanges): void {
        const logBookType: SimpleChange | undefined = changes['logBookType'];
        
        if (logBookType) {
            this.setFormValidators();
        }
    }

    public ngOnDestroy(): void {
        if (this.isImportNotByShipValueChangeSubscriber !== null && this.isImportNotByShipValueChangeSubscriber !== undefined) {
            this.isImportNotByShipValueChangeSubscriber.unsubscribe();
        }
    }

    public writeValue(value: CommonLogBookPageDataDTO): void {
        this.loader.load(() => {
            if (value !== null && value !== undefined) {
                this.fillForm(value);
                this.onChanged(value);
            }
            else {
                this.form.reset();
            }
        });
    }

    protected buildForm(): AbstractControl {
        return new FormGroup({
            originDeclarationNumberControl: new FormControl(),
            originDeclarationDateControl: new FormControl(),
            transportationDocumentNumberControl: new FormControl(),
            transportationDocumentDateControl: new FormControl(),
            admissionDocumentNumberControl: new FormControl(),
            admissionDocumentHandoverDateControl: new FormControl(),
            shipControl: new FormControl(),
            captainNameControl: new FormControl(),
            unloadingInformationControl: new FormControl(),

            isImportNotByShipControl: new FormControl(false),
            placeOfImportControl: new FormControl(null, Validators.maxLength(500)),

            vendorControl: new FormControl()
        });
    }

    protected getValue(): CommonLogBookPageDataDTO {
        if (this.model === null || this.model === undefined) {
            this.model = new CommonLogBookPageDataDTO();
        }

        if (this.logBookType === LogBookTypesEnum.FirstSale || this.logBookType === LogBookTypesEnum.Admission) {
            this.model.transportationDocumentNumber = this.form.get('transportationDocumentNumberControl')!.value;
            this.model.transportationDocumentDate = this.form.get('transportationDocumentDateControl')!.value;
            this.model.isImportNotByShip = false;

            if (this.logBookType === LogBookTypesEnum.FirstSale) {
                this.model.admissionDocumentNumber = this.form.get('admissionDocumentNumberControl')!.value;
                this.model.admissionHandoverDate = this.form.get('admissionDocumentHandoverDateControl')!.value;
            }

        }
        else if (this.logBookType === LogBookTypesEnum.Transportation) {
            this.model.isImportNotByShip = this.form.get('isImportNotByShipControl')!.value ?? false;
            this.model.placeOfImport = this.form.get('placeOfImportControl')!.value;
        }

        this.model.isImportNotByShip ??= false;

        this.model.originDeclarationNumber = this.form.get('originDeclarationNumberControl')!.value;
        this.model.originDeclarationDate = this.form.get('originDeclarationDateControl')!.value;
        this.model.shipId = this.form.get('shipControl')!.value?.value;
        this.model.captainName = this.form.get('captainNameControl')!.value;
        this.model.unloadingInformation = this.form.get('unloadingInformationControl')!.value;

        if (this.logBookType === LogBookTypesEnum.FirstSale) {
            this.model.vendorName = this.form.get('vendorControl')!.value;
        }

        return this.model;
    }

    private fillForm(model: CommonLogBookPageDataDTO): void {
        if (this.logBookType === LogBookTypesEnum.FirstSale || this.logBookType === LogBookTypesEnum.Admission) {
            this.form.get('transportationDocumentNumberControl')!.setValue(model.transportationDocumentNumber);
            this.form.get('transportationDocumentDateControl')!.setValue(model.transportationDocumentDate);
            
            if (this.logBookType === LogBookTypesEnum.FirstSale) {
                this.form.get('admissionDocumentNumberControl')!.setValue(model.admissionDocumentNumber);
                this.form.get('admissionDocumentHandoverDateControl')!.setValue(model.admissionHandoverDate);
            }
        }
        
        this.form.get('isImportNotByShipControl')!.setValue(model.isImportNotByShip ?? false);
        this.form.get('placeOfImportControl')!.setValue(model.placeOfImport);

        this.form.get('originDeclarationNumberControl')!.setValue(model.originDeclarationNumber);
        this.form.get('originDeclarationDateControl')!.setValue(model.originDeclarationDate);

        if (model.shipId !== null && model.shipId !== undefined) {
            this.form.get('shipControl')!.setValue(ShipsUtils.get(this.ships, model.shipId!));
            this.form.get('captainNameControl')!.setValue(model.captainName);
            this.form.get('unloadingInformationControl')!.setValue(model.unloadingInformation);
        }

        if (this.logBookType === LogBookTypesEnum.FirstSale) {
            this.form.get('vendorControl')!.setValue(model.vendorName);
        }
    }

    private setFormValidators(): void {
        const isImportByShip: boolean = this.form.get('isImportNotByShipControl')!.value ?? false;
        
        if (isImportByShip === true) {
            this.form.get('placeOfImportControl')!.setValidators([Validators.maxLength(500), Validators.required]);
            this.form.get('placeOfImportControl')!.markAsPending();

            this.form.get('shipControl')!.clearValidators();
            this.form.get('shipControl')!.reset();
            this.form.get('captainNameControl')!.clearValidators();
            this.form.get('captainNameControl')!.reset();
            this.form.get('unloadingInformationControl')!.clearValidators();
            this.form.get('unloadingInformationControl')!.reset();
            this.form.get('vendorControl')!.clearValidators();
            this.form.get('vendorControl')!.reset();
        }
        else {
            this.form.get('placeOfImportControl')!.clearValidators();
            this.form.get('placeOfImportControl')!.reset();

            if (this.logBookType === LogBookTypesEnum.FirstSale
                || this.logBookType === LogBookTypesEnum.Admission
                || this.logBookType === LogBookTypesEnum.Transportation
            ) {
                this.form.get('shipControl')!.setValidators(Validators.required);
                this.form.get('shipControl')!.markAsPending();
                this.form.get('captainNameControl')!.setValidators(Validators.required);
                this.form.get('captainNameControl')!.markAsPending();
                this.form.get('unloadingInformationControl')!.setValidators(Validators.required);
                this.form.get('unloadingInformationControl')!.markAsPending();

                if (this.logBookType === LogBookTypesEnum.FirstSale) {
                    this.form.get('vendorControl')!.setValidators(Validators.required);
                }
            }
            else {
                this.form.get('shipControl')!.clearValidators();
                this.form.get('shipControl')!.reset();
                this.form.get('captainNameControl')!.clearValidators();
                this.form.get('captainNameControl')!.reset();
                this.form.get('unloadingInformationControl')!.clearValidators();
                this.form.get('unloadingInformationControl')!.reset();
                this.form.get('vendorControl')!.clearValidators();
                this.form.get('vendorControl')!.reset();
                this.form.get('placeOfImportControl')!.clearValidators();
                this.form.get('placeOfImportControl')!.reset();
            }
        }
    }

    private getNomenclatures(): Subscription {
        const observables: Observable<ShipNomenclatureDTO[]>[] = [];

        observables.push(NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.Ships, this.commonNomenclatures.getShips.bind(this.commonNomenclatures), false
        ));

        const subscription: Subscription = forkJoin(observables).subscribe({
            next: (nomenclatures: ShipNomenclatureDTO[][]) => {
                this.ships = nomenclatures[0];
                this.loader.complete();
            }
        });

        return subscription;
    }

}