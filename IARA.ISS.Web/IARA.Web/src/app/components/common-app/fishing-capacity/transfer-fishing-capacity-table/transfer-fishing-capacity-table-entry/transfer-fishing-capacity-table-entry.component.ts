import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { AddressTypesEnum } from '@app/enums/address-types.enum';
import { FishingCapacityHolderDTO } from '@app/models/generated/dtos/FishingCapacityHolderDTO';
import { FishingCapacityHolderRegixDataDTO } from '@app/models/generated/dtos/FishingCapacityHolderRegixDataDTO';
import { RegixLegalDataDTO } from '@app/models/generated/dtos/RegixLegalDataDTO';
import { RegixPersonDataDTO } from '@app/models/generated/dtos/RegixPersonDataDTO';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { ApplicationSubmittedByDTO } from '@app/models/generated/dtos/ApplicationSubmittedByDTO';
import { PersonFullDataDTO } from '@app/models/generated/dtos/PersonFullDataDTO';
import { LegalFullDataDTO } from '@app/models/generated/dtos/LegalFullDataDTO';
import { TransferFishingCapacityTableEntryParams } from '../models/transfer-fishing-capacity-table-entry-params.model';

type HolderType = 'Person' | 'Legal';

@Component({
    selector: 'transfer-fishing-capacity-table-entry',
    templateUrl: './transfer-fishing-capacity-table-entry.component.html'
})
export class TransferFishingCapacityTableEntryComponent implements OnInit, IDialogComponent {
    public form!: FormGroup;
    public readOnly: boolean = false;

    public readonly companyHeadquartersType: AddressTypesEnum = AddressTypesEnum.COMPANY_HEADQUARTERS;

    public types: NomenclatureDTO<HolderType>[] = [];

    public isEgnLncReadOnly: boolean = false;
    public showOnlyRegiXData: boolean = false;
    public showRegiXData: boolean = false;

    public expectedResults: FishingCapacityHolderRegixDataDTO;

    private remainingPower: number | undefined;
    private remainingTonnage: number | undefined;
    private submittedBy: ApplicationSubmittedByDTO | undefined;

    private model!: FishingCapacityHolderDTO | FishingCapacityHolderRegixDataDTO;

    private translate: FuseTranslationLoaderService;

    public constructor(translate: FuseTranslationLoaderService) {
        this.translate = translate;

        this.expectedResults = new FishingCapacityHolderRegixDataDTO({
            person: new RegixPersonDataDTO(),
            legal: new RegixLegalDataDTO(),
            addresses: []
        });
    }

    public ngOnInit(): void {
        this.types = [
            new NomenclatureDTO<HolderType>({
                value: 'Person',
                displayName: this.translate.getValue('fishing-capacity.holder-person'),
                isActive: true
            }),
            new NomenclatureDTO<HolderType>({
                value: 'Legal',
                displayName: this.translate.getValue('fishing-capacity.holder-legal'),
                isActive: true
            })
        ];

        this.buildForm();

        if (this.model === undefined || this.model === null) {
            if (this.showOnlyRegiXData) {
                this.model = new FishingCapacityHolderRegixDataDTO({ isActive: true });
            }
            else {
                this.model = new FishingCapacityHolderDTO({ isActive: true });
            }
        }
        else {
            if (this.readOnly) {
                this.form.disable();
            }
            this.fillForm();
        }

        if (this.showOnlyRegiXData || this.showRegiXData) {
            setTimeout(() => {
                this.form.markAllAsTouched();
            });
        }
    }

    public setData(data: TransferFishingCapacityTableEntryParams, wrapperData: DialogWrapperData): void {
        this.readOnly = data.readOnly;
        this.showOnlyRegiXData = data.showOnlyRegixData;
        this.showRegiXData = data.showRegixData;
        this.isEgnLncReadOnly = data.isEgnLncReadOnly;

        this.remainingPower = data.remainingPower;
        this.remainingTonnage = data.remainingTonnage;
        this.submittedBy = data.submittedBy;

        if (data.expectedResults !== null && data.expectedResults !== undefined) {
            this.expectedResults = data.expectedResults;
        }
        if (data.model !== null && data.model !== undefined) {
            this.model = data.model;
        }
    }

    public saveBtnClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (this.readOnly) {
            dialogClose(this.model);
        }
        else {
            this.form.markAllAsTouched();
            if (this.form.valid || this.showOnlyRegiXData) {
                this.fillModel();
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

    public copySubmittedBy(): void {
        if (this.submittedBy) {
            this.form.get('typeControl')!.setValue(this.types[0]);
            this.form.get('regixDataControl')!.setValue(this.submittedBy.person);
            this.form.get('addressControl')!.setValue(this.submittedBy.addresses);
        }
    }

    public transferRemainingCapacity(): void {
        if (this.remainingTonnage !== undefined && this.remainingTonnage !== null) {
            this.form.get('tonnageControl')!.setValue(this.remainingTonnage.toFixed(2));
        }

        if (this.remainingPower !== undefined && this.remainingPower !== null) {
            this.form.get('powerControl')!.setValue(this.remainingPower.toFixed(2));
        }
    }

    public downloadedPersonData(person: PersonFullDataDTO): void {
        this.form.get('regixDataControl')!.setValue(person.person);
        this.form.get('addressControl')!.setValue(person.addresses);
    }

    public downloadedLegalData(legal: LegalFullDataDTO): void {
        this.form.get('regixDataControl')!.setValue(legal.legal);

        if (legal.addresses && legal.addresses.length !== 0) {
            this.form.get('addressControl')!.setValue(legal.addresses.find(x => x.addressType === this.companyHeadquartersType));
        }
    }

    private buildForm(): void {
        this.form = new FormGroup({
            typeControl: new FormControl(this.types[0]),
            regixDataControl: new FormControl(null),
            addressControl: new FormControl(null)
        });

        if (!this.showOnlyRegiXData) {
            this.form.addControl('tonnageControl', new FormControl(null, [Validators.required, TLValidators.number(0, undefined, 2)]));
            this.form.addControl('powerControl', new FormControl(null, [Validators.required, TLValidators.number(0, undefined, 2)]));
        }
    }

    private fillForm(): void {
        if (this.model.isHolderPerson === true) {
            this.form.get('typeControl')!.setValue(this.types.find(x => x.value === 'Person'));
            this.form.get('regixDataControl')!.setValue(this.model.person);
        }
        else {
            this.form.get('typeControl')!.setValue(this.types.find(x => x.value === 'Legal'));
            this.form.get('regixDataControl')!.setValue(this.model.legal);
        }

        this.form.get('addressControl')!.setValue(this.model.addresses);

        if (!this.showOnlyRegiXData && this.model instanceof FishingCapacityHolderDTO) {
            this.form.get('tonnageControl')!.setValue(this.model.transferredTonnage?.toFixed(2));
            this.form.get('powerControl')!.setValue(this.model.transferredPower?.toFixed(2));
        }
    }

    private fillModel(): void {
        if (this.form.get('typeControl')!.value?.value === 'Person') {
            this.model.isHolderPerson = true;
            this.model.legal = undefined;
            this.model.person = this.form.get('regixDataControl')!.value;
        }
        else {
            this.model.isHolderPerson = false;
            this.model.person = undefined;
            this.model.legal = this.form.get('regixDataControl')!.value;
        }

        this.model.addresses = this.form.get('addressControl')!.value;

        if (!this.showOnlyRegiXData && this.model instanceof FishingCapacityHolderDTO) {
            this.model.transferredTonnage = this.form.get('tonnageControl')!.value;
            this.model.transferredPower = this.form.get('powerControl')!.value;
        }

        // for visualisation in the table
        if (this.model.isHolderPerson) {
            this.model.name = '';
            if (this.model.person!.firstName !== undefined && this.model.person!.firstName !== null) {
                this.model.name = `${this.model.person!.firstName}`;
            }
            if (this.model.person!.middleName !== undefined && this.model.person!.middleName !== null) {
                this.model.name = `${this.model.name} ${this.model.person!.middleName}`;
            }
            if (this.model.person!.lastName !== undefined && this.model.person!.lastName !== null) {
                this.model.name = `${this.model.name} ${this.model.person!.lastName}`;
            }

            this.model.egnEik = this.model.person!.egnLnc?.egnLnc;
        }
        else {
            this.model.name = this.model.legal!.name;
            this.model.egnEik = this.model.legal!.eik;
        }
    }
}