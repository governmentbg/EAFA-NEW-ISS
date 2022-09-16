import { AfterViewInit, Component, Input, OnInit, Optional, Self } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { AddressTypesEnum } from '@app/enums/address-types.enum';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { UsageDocumentTypesEnum } from '@app/enums/usage-document-types.enum';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { UsageDocumentDTO } from '@app/models/generated/dtos/UsageDocumentDTO';
import { UsageDocumentRegixDataDTO } from '@app/models/generated/dtos/UsageDocumentRegixDataDTO';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { FormControlDataLoader } from '@app/shared/utils/form-control-data-loader';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { UsageDocumentDialogParams } from './models/usage-document-dialog-params.model';
import { ValidityCheckerDirective } from '@app/shared/directives/validity-checker/validity-checker.directive';
import { NotifyingCustomFormControl } from '@app/shared/utils/notifying-custom-form-control';
import { Notifier } from '@app/shared/directives/notifier/notifier.class';
import { NotifierDirective } from '@app/shared/directives/notifier/notifier.directive';
import { DateRangeData } from '../input-controls/tl-date-range/tl-date-range.component';
import { DateRangeIndefiniteData } from '../date-range-indefinite/date-range-indefinite.component';

type LessorType = 'Person' | 'Legal';

@Component({
    selector: 'usage-document',
    templateUrl: './usage-document.component.html'
})
export class UsageDocumentComponent
    extends NotifyingCustomFormControl<UsageDocumentDTO | UsageDocumentRegixDataDTO> implements OnInit, AfterViewInit, IDialogComponent {
    @Input()
    public title: string;

    @Input()
    public tooltipResourceName: string = 'usage-document.mat-card-helper';

    @Input()
    public isIdReadOnly: boolean = false;

    @Input()
    public showOnlyRegiXData: boolean = false;

    @Input()
    public expectedResults: UsageDocumentRegixDataDTO | undefined;

    public readonly companyHeadquartersType: AddressTypesEnum = AddressTypesEnum.COMPANY_HEADQUARTERS;
    public readonly usageDocumentTypes: typeof UsageDocumentTypesEnum = UsageDocumentTypesEnum;

    public notifierGroup: Notifier = new Notifier();
    public type: UsageDocumentTypesEnum | undefined;
    public isDialog: boolean = false;

    public documentTypes: NomenclatureDTO<number>[] = [];
    public documentLessorTypes: NomenclatureDTO<LessorType>[] = [];

    private id: number | undefined;
    private model: UsageDocumentDTO | undefined;
    private isActive: boolean = true;
    private viewMode: boolean = false;
    private translate: FuseTranslationLoaderService;
    private nomenclatures: CommonNomenclatures;

    private readonly loader: FormControlDataLoader;

    public constructor(
        @Optional() @Self() ngControl: NgControl,
        translate: FuseTranslationLoaderService,
        nomenclatures: CommonNomenclatures,
        @Optional() @Self() validityChecker: ValidityCheckerDirective,
        @Optional() @Self() notifier: NotifierDirective
    ) {
        super(ngControl, true, validityChecker, notifier);
        this.translate = translate;
        this.nomenclatures = nomenclatures;
        this.title = this.translate.getValue('usage-document.usage-document');

        this.documentLessorTypes = [
            new NomenclatureDTO<LessorType>({
                value: 'Person',
                displayName: this.translate.getValue('usage-document.lessor-person'),
                isActive: true
            }),
            new NomenclatureDTO<LessorType>({
                value: 'Legal',
                displayName: this.translate.getValue('usage-document.lessor-legal'),
                isActive: true
            })
        ];

        this.buildForm();
        this.registerOnChanged();

        this.form.get('isLessorPersonControl')!.setValue(this.documentLessorTypes.find(x => x.value === 'Person'));

        this.loader = new FormControlDataLoader(this.getNomenclatures.bind(this));
    }

    public ngOnInit(): void {
        this.initNotifyingCustomFormControl(this.notifierGroup, () => { this.notify(); });
        this.setValidators();
        this.loader.load();
    }

    public ngAfterViewInit(): void {
        if (this.model !== undefined && this.model !== null) {
            this.writeValue(this.model);
        }
    }

    public writeValue(value: UsageDocumentDTO | UsageDocumentRegixDataDTO): void {
        setTimeout(() => {
            if (value !== null && value !== undefined) {
                this.id = value.id;
                this.isActive = value.isActive ?? false;

                this.loader.load(() => {
                    const documentTypeId: number | undefined = value.documentTypeId;

                    if (documentTypeId !== undefined && documentTypeId !== null) {
                        const documentType: NomenclatureDTO<number> = this.documentTypes.find(x => x.value === documentTypeId)!;

                        this.form.get('typeControl')!.setValue(documentType);
                        this.type = UsageDocumentTypesEnum[documentType.code as keyof typeof UsageDocumentTypesEnum];;
                    }

                    switch (this.type) {
                        case UsageDocumentTypesEnum.Concession:
                            this.form.get('concessionerControl')!.setValue(value.lessorLegal);
                            this.form.get('concessionerAddressesControl')!.setValue(value.lessorAddresses);
                            break;
                        case UsageDocumentTypesEnum.Lease:
                            if (value.isLessorPerson !== undefined && value.isLessorPerson !== null) {
                                if (value.isLessorPerson) {
                                    this.form.get('isLessorPersonControl')!.setValue(this.documentLessorTypes.find(x => x.value === 'Person'));
                                    this.form.get('personControl')!.setValue(value.lessorPerson);
                                    this.form.get('personAddressesControl')!.setValue(value.lessorAddresses);
                                }
                                else {
                                    this.form.get('isLessorPersonControl')!.setValue(this.documentLessorTypes.find(x => x.value === 'Legal'));
                                    this.form.get('legalControl')!.setValue(value.lessorLegal);
                                    this.form.get('legalAddressesControl')!.setValue(value.lessorAddresses);
                                }
                            }
                            break;
                    }

                    if (value instanceof UsageDocumentDTO) {
                        this.form.get('numControl')!.setValue(value.documentNum);
                        this.form.get('commentsControl')!.setValue(value.comments);

                        this.form.get('validityControl')!.setValue(new DateRangeIndefiniteData({
                            range: new DateRangeData({ start: value.documentValidFrom, end: value.documentValidTo }),
                            indefinite: value.isDocumentIndefinite ?? false
                        }));
                    }
                });
            }
        });
    }

    public setData(data: UsageDocumentDialogParams, wrapperData: DialogWrapperData): void {
        this.isDialog = true;
        this.isIdReadOnly = data.isIdReadOnly;
        this.showOnlyRegiXData = data.showOnlyRegiXData;
        this.model = data.model;
        this.viewMode = data.viewMode;

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

    public dialogButtonClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public cancelBtnClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    protected buildForm(): AbstractControl {
        return new FormGroup({
            typeControl: new FormControl(null),
            numControl: new FormControl(null),
            validityControl: new FormControl(null),
            isLessorPersonControl: new FormControl(),
            personControl: new FormControl(),
            personAddressesControl: new FormControl(),
            legalControl: new FormControl(),
            legalAddressesControl: new FormControl(),
            concessionerControl: new FormControl(),
            concessionerAddressesControl: new FormControl(),
            commentsControl: new FormControl(null)
        });
    }

    protected getValue(): UsageDocumentDTO | UsageDocumentRegixDataDTO {
        const result: UsageDocumentDTO | UsageDocumentRegixDataDTO = this.showOnlyRegiXData
            ? new UsageDocumentRegixDataDTO()
            : new UsageDocumentDTO();

        result.id = this.id;
        result.documentTypeId = this.form.get('typeControl')!.value?.value;
        result.isLessorPerson = undefined;
        result.isActive = this.isActive;

        switch (this.type) {
            case UsageDocumentTypesEnum.Concession:
                result.isLessorPerson = false;
                result.lessorLegal = this.form.get('concessionerControl')!.value;
                result.lessorAddresses = this.form.get('concessionerAddressesControl')!.value;
                break;
            case UsageDocumentTypesEnum.Lease:
                result.isLessorPerson = this.form.get('isLessorPersonControl')!.value?.value === 'Person';

                if (result.isLessorPerson !== undefined && result.isLessorPerson !== null) {
                    if (result.isLessorPerson) {
                        result.lessorPerson = this.form.get('personControl')!.value;
                        result.lessorAddresses = this.form.get('personAddressesControl')!.value;
                    }
                    else {
                        result.lessorLegal = this.form.get('legalControl')!.value;
                        result.lessorAddresses = this.form.get('legalAddressesControl')!.value;
                    }
                }
                break;
        }

        if (result instanceof UsageDocumentDTO) {
            result.documentNum = this.form.get('numControl')!.value;
            result.comments = this.form.get('commentsControl')!.value;

            const validity: DateRangeIndefiniteData | undefined = this.form.get('validityControl')!.value;
            if (validity !== undefined && validity !== null) {
                result.isDocumentIndefinite = validity.indefinite;
                result.documentValidFrom = validity.range?.start;
                result.documentValidTo = validity.range?.end;
            }
            else {
                result.isDocumentIndefinite = false;
            }
        }

        return result;
    }

    private setValidators(): void {
        if (!this.showOnlyRegiXData) {
            this.form.get('typeControl')!.setValidators(Validators.required);
            this.form.get('numControl')!.setValidators([Validators.required, Validators.maxLength(50)]);
            this.form.get('validityControl')!.setValidators(Validators.required);
            this.form.get('commentsControl')!.setValidators(Validators.maxLength(1000));
        }
    }

    private registerOnChanged(): void {
        this.form.get('isLessorPersonControl')!.valueChanges.subscribe({
            next: (type: NomenclatureDTO<LessorType> | undefined) => {
                if (type !== undefined) {
                    if (type.value === 'Person') {
                        this.form.get('legalControl')!.setErrors(null);
                        this.form.get('legalAddressesControl')!.setErrors(null);
                    }
                    else {
                        this.form.get('personControl')!.setErrors(null);
                        this.form.get('personAddressesControl')!.setErrors(null);
                    }
                }
                else {
                    this.form.get('personControl')!.setErrors(null);
                    this.form.get('personAddressesControl')!.setErrors(null);
                    this.form.get('legalControl')!.setErrors(null);
                    this.form.get('legalAddressesControl')!.setErrors(null);
                }
            }
        });

        this.form.get('typeControl')!.valueChanges.subscribe({
            next: (type: NomenclatureDTO<number> | undefined) => {
                if (type !== undefined && type !== null) {
                    this.type = UsageDocumentTypesEnum[type.code! as keyof typeof UsageDocumentTypesEnum];
                }
                else {
                    this.type = undefined;
                }

                switch (this.type) {
                    case UsageDocumentTypesEnum.Concession:
                        this.form.get('isLessorPersonControl')!.setErrors(null);
                        this.form.get('personControl')!.setErrors(null);
                        this.form.get('personAddressesControl')!.setErrors(null);
                        this.form.get('legalControl')!.setErrors(null);
                        this.form.get('legalAddressesControl')!.setErrors(null);
                        break;
                    case UsageDocumentTypesEnum.Lease:
                        this.form.get('concessionerControl')!.setErrors(null);
                        this.form.get('concessionerAddressesControl')!.setErrors(null);
                        break;
                    case UsageDocumentTypesEnum.NotaryDeed:
                    case UsageDocumentTypesEnum.Other:
                        this.form.get('isLessorPersonControl')!.setErrors(null);
                        this.form.get('personControl')!.setErrors(null);
                        this.form.get('personAddressesControl')!.setErrors(null);
                        this.form.get('legalControl')!.setErrors(null);
                        this.form.get('legalAddressesControl')!.setErrors(null);
                        this.form.get('concessionerControl')!.setErrors(null);
                        this.form.get('concessionerAddressesControl')!.setErrors(null);
                        break;
                }

                if (this.type !== undefined) {
                    this.form.updateValueAndValidity({ emitEvent: false });
                }
            }
        });
    }

    private getNomenclatures(): Subscription {
        return NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.UsageDocumentTypes, this.nomenclatures.getUsageDocumentTypes.bind(this.nomenclatures), false
        ).subscribe({
            next: (result: NomenclatureDTO<number>[]) => {
                this.documentTypes = result;

                this.loader.complete();
            }
        });
    }
}