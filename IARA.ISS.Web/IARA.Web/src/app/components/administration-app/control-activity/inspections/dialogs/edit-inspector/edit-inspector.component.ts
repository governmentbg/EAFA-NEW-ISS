import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { forkJoin } from 'rxjs';

import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { InspectorTableParams } from '../../components/inspectors-table/models/inspector-table-params';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { InspectionsService } from '@app/services/administration-app/inspections.service';
import { InspectorDTO } from '@app/models/generated/dtos/InspectorDTO';
import { InspectorTableModel } from '../../models/inspector-table-model';

@Component({
    selector: 'edit-inspector',
    templateUrl: './edit-inspector.component.html',
})
export class EditInspectorComponent implements OnInit, IDialogComponent {
    public form!: FormGroup;

    public isFromRegister: boolean = true;

    public inspectors: NomenclatureDTO<number>[] = [];
    public countries: NomenclatureDTO<number>[] = [];
    public institutions: NomenclatureDTO<number>[] = [];
    public allInstitutions: NomenclatureDTO<number>[] = [];

    public inspectorExists: boolean = false;

    protected model: InspectorTableModel = new InspectorTableModel();

    private readonly nomenclatures: CommonNomenclatures;
    private readonly service: InspectionsService;

    private isEdit: boolean = false;
    private readOnly: boolean = false;
    private selectedInspector: InspectorDTO | undefined;
    private excludeIds: number[] = [];
    private unfilteredInspectors: NomenclatureDTO<number>[] = [];

    public constructor(nomenclatures: CommonNomenclatures, service: InspectionsService) {
        this.nomenclatures = nomenclatures;
        this.service = service;

        this.buildForm();
    }

    public async ngOnInit(): Promise<void> {
        const nomenclatureTables = await forkJoin(
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.Countries, this.nomenclatures.getCountries.bind(this.nomenclatures), false
            ),
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.Institutions, this.nomenclatures.getInstitutions.bind(this.nomenclatures), false
            ),
            this.service.getInspectors()
        ).toPromise();

        this.countries = nomenclatureTables[0];
        this.allInstitutions = this.institutions = nomenclatureTables[1];
        this.unfilteredInspectors = nomenclatureTables[2];
        this.inspectors = nomenclatureTables[2].filter(f => !this.excludeIds.includes(f.value!));

        this.fillForm();
    }

    public setData(data: InspectorTableParams, wrapperData: DialogWrapperData): void {
        if (data.model !== undefined && data.model !== null) {
            this.model = data.model;
            this.selectedInspector = data.model;
        }

        this.excludeIds = data.excludeIds;
        this.readOnly = data.readOnly;
        this.isEdit = data.isEdit;
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (this.readOnly) {
            dialogClose(this.model);
        }
        else {
            this.form.markAllAsTouched();

            if (this.form.valid) {
                this.fillModel();
                CommonUtils.sanitizeModelStrings(this.model);

                if (this.isFromRegister) {
                    dialogClose(this.model);
                }
                else {
                    this.service.unregisteredInspectorExists(this.model).subscribe({
                        next: (exists: boolean) => {
                            if (exists) {
                                this.inspectorExists = true;
                            }
                            else {
                                dialogClose(this.model);
                            }
                        }
                    });
                }
            }
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    protected buildForm(): void {
        this.form = new FormGroup({
            inspectorRegisteredControl: new FormControl(true),
            inspectorControl: new FormControl(undefined, Validators.required),
            firstNameControl: new FormControl(undefined, Validators.maxLength(200)),
            middleNameControl: new FormControl(undefined, Validators.maxLength(200)),
            lastNameControl: new FormControl(undefined, Validators.maxLength(200)),
            cardNumControl: new FormControl(undefined),
            countryControl: new FormControl({ value: undefined, disabled: true }, Validators.required),
            institutionControl: new FormControl({ value: undefined, disabled: true }, Validators.required),
            headOfInspectionControl: new FormControl(false),
            inspectorIdentifiedControl: new FormControl(false),
        });

        this.form.get('inspectorRegisteredControl')!.valueChanges.subscribe({
            next: this.onInspectorRegisteredChanged.bind(this)
        });

        this.form.get('inspectorControl')!.valueChanges.subscribe({
            next: this.onInspectorChanged.bind(this)
        });

        this.form.valueChanges.subscribe({
            next: () => {
                this.inspectorExists = false;
            }
        });

    }

    protected fillForm(): void {
        if (this.isEdit) {
            this.isFromRegister = this.model.isNotRegistered === false;
            this.form.get('inspectorRegisteredControl')!.setValue(this.isFromRegister);

            if (!this.isFromRegister) {
                this.form.get('firstNameControl')!.setValue(this.model.firstName);
                this.form.get('middleNameControl')!.setValue(this.model.middleName);
                this.form.get('lastNameControl')!.setValue(this.model.lastName);
                this.form.get('cardNumControl')!.setValue(this.model.cardNum);
            }
            else {
                this.form.get('inspectorControl')!.setValue(this.unfilteredInspectors.find(f => f.value === this.model.inspectorId));

                if (this.model.isCurrentUser) {
                    this.form.get('inspectorRegisteredControl')!.disable();
                    this.form.get('inspectorControl')!.disable();
                }

                this.form.get('headOfInspectionControl')!.setValue(this.model.isInCharge);
            }

            this.form.get('countryControl')!.setValue(this.countries.find(f => f.value === this.model.citizenshipId));
            this.form.get('institutionControl')!.setValue(this.institutions.find(f => f.value === this.model.institutionId));

            this.form.get('inspectorIdentifiedControl')!.setValue(this.model.hasIdentifiedHimself);
        }
        else {
            this.form.get('countryControl')!.setValue(this.countries.find(f => f.code === CommonUtils.COUNTRIES_BG));
            this.form.get('institutionControl')!.setValue(this.institutions.find(f => f.code === CommonUtils.INSTITUTIONS_IARA));
        }

        if (this.readOnly) {
            this.form.disable();
        }
    }

    protected fillModel(): void {
        if (this.isFromRegister) {
            this.model = new InspectorTableModel(this.selectedInspector);

            this.model.isInCharge = this.form.get('headOfInspectionControl')!.value;
        }
        else {
            this.model = new InspectorTableModel({
                firstName: this.form.get('firstNameControl')!.value,
                middleName: this.form.get('middleNameControl')!.value,
                lastName: this.form.get('lastNameControl')!.value,
                cardNum: this.form.get('cardNumControl')!.value,
                citizenshipId: this.form.get('countryControl')!.value?.value,
                institutionId: this.form.get('institutionControl')!.value?.value,
                isInCharge: false,
            });
        }

        this.model.hasIdentifiedHimself = this.form.get('inspectorIdentifiedControl')!.value ?? false;
        this.model.institution = this.form.get('institutionControl')!.value?.displayName;
        this.model.isNotRegistered = !this.isFromRegister;
    }

    private onInspectorRegisteredChanged(value: boolean): void {
        this.isFromRegister = value;
        this.inspectorExists = false;

        if (value) {
            this.institutions = this.allInstitutions;

            this.form.get('institutionControl')!.setValue(this.institutions.find(f => f.code === CommonUtils.INSTITUTIONS_IARA));

            this.form.get('inspectorControl')!.setValidators(Validators.required);
            this.form.get('firstNameControl')!.clearValidators();
            this.form.get('middleNameControl')!.clearValidators();
            this.form.get('lastNameControl')!.clearValidators();
            this.form.get('cardNumControl')!.clearValidators();

            this.form.get('countryControl')!.disable();
            this.form.get('institutionControl')!.disable();
        }
        else {
            this.institutions = this.allInstitutions.filter(x => x.code !== CommonUtils.INSTITUTIONS_IARA);

            if (this.isEdit) {
                this.form.get('institutionControl')!.setValue(this.institutions.find(x => x.value === this.model.institutionId));
            }
            else {
                this.form.get('institutionControl')!.setValue(undefined);
            }

            this.form.get('inspectorControl')!.clearValidators();
            this.form.get('firstNameControl')!.setValidators([Validators.required, Validators.maxLength(200)]);
            this.form.get('middleNameControl')!.setValidators([Validators.required, Validators.maxLength(200)]);
            this.form.get('lastNameControl')!.setValidators([Validators.required, Validators.maxLength(200)]);
            this.form.get('cardNumControl')!.setValidators([Validators.required, Validators.maxLength(7)]);

            this.form.get('countryControl')!.enable();
            this.form.get('institutionControl')!.enable();

            this.form.get('firstNameControl')!.markAsPending();
            this.form.get('middleNameControl')!.markAsPending();
            this.form.get('lastNameControl')!.markAsPending();
            this.form.get('cardNumControl')!.markAsPending();
        }

        if (this.readOnly) {
            this.form.get('countryControl')!.disable();
            this.form.get('institutionControl')!.disable();
        }

        this.institutions = this.institutions.slice();

        this.form.get('inspectorControl')!.updateValueAndValidity({ emitEvent: false });
        this.form.get('firstNameControl')!.updateValueAndValidity({ emitEvent: false });
        this.form.get('middleNameControl')!.updateValueAndValidity({ emitEvent: false });
        this.form.get('lastNameControl')!.updateValueAndValidity({ emitEvent: false });
        this.form.get('cardNumControl')!.updateValueAndValidity({ emitEvent: false });
    }

    private async onInspectorChanged(value: NomenclatureDTO<number> | undefined): Promise<void> {
        if (value !== undefined && value !== null && value.value !== undefined && value.value !== null) {
            this.selectedInspector = await this.service.getInspector(value.value).toPromise();

            this.form.get('countryControl')!.setValue(this.countries.find(f => f.value === this.selectedInspector!.citizenshipId));
            this.form.get('institutionControl')!.setValue(this.institutions.find(f => f.value === this.selectedInspector!.institutionId));
        }
    }
}