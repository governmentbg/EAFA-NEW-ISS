import { AfterViewInit, Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatRadioChange } from '@angular/material/radio';
import { AddressTypesEnum } from '@app/enums/address-types.enum';
import { RegixLegalDataDTO } from '@app/models/generated/dtos/RegixLegalDataDTO';
import { RegixPersonDataDTO } from '@app/models/generated/dtos/RegixPersonDataDTO';
import { ShipOwnerDTO } from '@app/models/generated/dtos/ShipOwnerDTO';
import { ShipOwnerRegixDataDTO } from '@app/models/generated/dtos/ShipOwnerRegixDataDTO';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { SubmittedByRolesEnum } from '@app/enums/submitted-by-roles.enum';
import { ApplicationSubmittedForDTO } from '@app/models/generated/dtos/ApplicationSubmittedForDTO';
import { EditShipOwnerDialogParams } from '../models/edit-ship-owner-dialog-params.model';
import { EditShipOwnerDialogResult } from '../models/edit-ship-owner-dialog-result.model';
import { PersonFullDataDTO } from '@app/models/generated/dtos/PersonFullDataDTO';
import { LegalFullDataDTO } from '@app/models/generated/dtos/LegalFullDataDTO';
import { Notifier } from '@app/shared/directives/notifier/notifier.class';

type OwnerType = 'Person' | 'Legal';

@Component({
    selector: 'edit-ship-owner',
    templateUrl: './edit-ship-owner.component.html'
})
export class EditShipOwnerComponent implements IDialogComponent, OnInit, AfterViewInit {
    public form!: FormGroup;
    public ownerType: OwnerType = 'Person';

    public readonly companyHeadquartersType: AddressTypesEnum = AddressTypesEnum.COMPANY_HEADQUARTERS;

    public model!: ShipOwnerDTO;
    public expectedResults: ShipOwnerRegixDataDTO;
    public isApplication: boolean = false;
    public readOnly!: boolean;
    public showOnlyRegiXData: boolean = false;
    public showRegiXData: boolean = false;
    public isThirdPartyShip: boolean = false;
    public isEditing!: boolean;

    public notifier: Notifier = new Notifier();

    private submittedFor!: ApplicationSubmittedForDTO;
    private isTouched: boolean = false;
    private isDraft: boolean = false;

    public constructor() {
        this.expectedResults = new ShipOwnerRegixDataDTO({
            addressRegistrations: [],
            regixPersonData: new RegixPersonDataDTO(),
            regixLegalData: new RegixLegalDataDTO()
        });
    }

    public ngOnInit(): void {
        setTimeout(() => {
            if (!this.readOnly && (this.showOnlyRegiXData || this.showRegiXData)) {
                this.notifier.start();
                this.notifier.onNotify.subscribe({
                    next: () => {
                        this.form.markAllAsTouched();
                        this.notifier.stop();
                    }
                });
            }
        });
    }

    public ngAfterViewInit(): void {
        setTimeout(() => {
            if (this.isTouched === false) {
                this.form.valueChanges.subscribe(() => {
                    this.isTouched = true;
                });
            }
        }, 1000);

        this.form.get('ownedShareControl')?.valueChanges.subscribe({
            next: (share: string | undefined) => {
                if (share === '100') {
                    this.form.get('isHolderControl')!.setValue(true);
                }
            }
        });
    }

    public setData(data: EditShipOwnerDialogParams, buttons: DialogWrapperData): void {
        this.readOnly = data.readOnly;
        this.isApplication = data.isApplication;
        this.showOnlyRegiXData = data.showOnlyRegiXData;
        this.showRegiXData = data.showRegiXData;
        this.isDraft = data.isDraft;
        this.isEditing = data.isEgnLncReadOnly;
        this.isThirdPartyShip = data.isThirdPartyShip;
        this.submittedFor = data.submittedFor;

        this.buildForm();

        if (data.expectedResults) {
            this.expectedResults = data.expectedResults;
        }

        if (data.model === undefined) {
            if (this.showOnlyRegiXData) {
                this.model = new ShipOwnerRegixDataDTO({ isActive: true });
            }
            else {
                this.model = new ShipOwnerDTO({ isActive: true });
            }
        }
        else {
            if (this.readOnly) {
                this.form.disable();
            }

            this.model = data.model;
            this.fillForm();
        }
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (this.readOnly) {
            dialogClose(new EditShipOwnerDialogResult(this.model, this.isTouched));
        }
        else if (this.isDraft) {
            this.fillModel();
            CommonUtils.sanitizeModelStrings(this.model);
            dialogClose(new EditShipOwnerDialogResult(this.model, this.isTouched));
        }
        else {
            this.form.markAllAsTouched();
            if (this.form.valid || this.showOnlyRegiXData) {
                this.fillModel();
                CommonUtils.sanitizeModelStrings(this.model);
                dialogClose(new EditShipOwnerDialogResult(this.model, this.isTouched));
            }
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public onPersonLegalChange(change: MatRadioChange | OwnerType): void {
        this.ownerType = change instanceof MatRadioChange ? change.value : change;

        if (this.ownerType === 'Person') {
            this.form.get('personControl')!.setValidators(Validators.required);
            this.form.get('personAddressesControl')!.setValidators(Validators.required);
            this.form.get('legalControl')!.clearValidators();
            this.form.get('personAddressesControl')!.clearValidators();
        }
        else {
            this.form.get('legalControl')!.setValidators(Validators.required);
            this.form.get('legalAddressesControl')!.setValidators(Validators.required);
            this.form.get('personControl')!.clearValidators();
            this.form.get('personAddressesControl')!.clearValidators();
        }

        this.form.get('legalControl')!.updateValueAndValidity({ emitEvent: false });
        this.form.get('legalAddressesControl')!.updateValueAndValidity({ emitEvent: false });
        this.form.get('personControl')!.updateValueAndValidity({ emitEvent: false });
        this.form.get('personAddressesControl')!.updateValueAndValidity({ emitEvent: false });
    }

    public getOwnerFromSubmittedFor(): void {
        if (this.submittedFor.submittedByRole! & SubmittedByRolesEnum.PersonalRole) {
            this.onPersonLegalChange('Person');
            this.form.get('personControl')!.setValue(this.submittedFor.person);
            this.form.get('personAddressesControl')!.setValue(this.submittedFor.addresses);
        }
        else if (this.submittedFor.submittedByRole! & SubmittedByRolesEnum.LegalRole) {
            this.onPersonLegalChange('Legal');
            this.form.get('legalControl')!.setValue(this.submittedFor.legal);
            this.form.get('legalAddressesControl')!.setValue(this.submittedFor.addresses);
        }
    }

    public downloadedPersonData(person: PersonFullDataDTO): void {
        this.form.get('personControl')!.setValue(person.person);
        this.form.get('personAddressesControl')!.setValue(person.addresses);
    }

    public downloadedLegalData(legal: LegalFullDataDTO): void {
        this.form.get('legalControl')!.setValue(legal.legal);
        this.form.get('legalAddressesControl')!.setValue(legal.addresses);
    }

    private buildForm(): void {
        this.form = new FormGroup({
            personControl: new FormControl(null),
            legalControl: new FormControl(null),
            personAddressesControl: new FormControl(null),
            legalAddressesControl: new FormControl(null)
        });

        if (!this.showOnlyRegiXData) {
            this.form.addControl('ownedShareControl', new FormControl(null, [TLValidators.number(0, 100), Validators.required]));
            this.form.addControl('isHolderControl', new FormControl(false));
        }
    }

    private fillForm(): void {
        if (!this.showOnlyRegiXData) {
            this.form.get('ownedShareControl')!.setValue(this.model.ownedShare);
            this.form.get('isHolderControl')!.setValue(this.model.isShipHolder);
        }

        if (this.model.isOwnerPerson === true) {
            this.ownerType = 'Person';
            this.form.get('personControl')!.setValue(this.model.regixPersonData);
            this.form.get('personAddressesControl')!.setValue(this.model.addressRegistrations);
        }
        else {
            this.ownerType = 'Legal';
            this.form.get('legalControl')!.setValue(this.model.regixLegalData);
            this.form.get('legalAddressesControl')!.setValue(this.model.addressRegistrations);
        }

    }

    private fillModel(): void {
        if (!this.showOnlyRegiXData) {
            this.model.ownedShare = this.form.get('ownedShareControl')!.value;
            this.model.isShipHolder = this.form.get('isHolderControl')!.value;
        }

        if (this.ownerType === 'Person') {
            this.model.isOwnerPerson = true;
            this.model.regixLegalData = undefined;
            this.model.regixPersonData = this.form.get('personControl')!.value;
            this.model.addressRegistrations = this.form.get('personAddressesControl')!.value;
        }
        else {
            this.model.isOwnerPerson = false;
            this.model.regixPersonData = undefined;
            this.model.regixLegalData = this.form.get('legalControl')!.value;
            this.model.addressRegistrations = this.form.get('legalAddressesControl')!.value;
        }

        this.model.hasValidationErrors = !this.form.valid;

        // for visualisation in the table
        if (this.ownerType === 'Person') {
            this.model.names = '';

            if (this.model.regixPersonData!.firstName !== undefined && this.model.regixPersonData!.firstName !== null) {
                this.model.names = `${this.model.regixPersonData!.firstName}`;
            }
            if (this.model.regixPersonData!.middleName !== undefined && this.model.regixPersonData!.middleName !== null) {
                this.model.names = `${this.model.names} ${this.model.regixPersonData!.middleName}`;
            }
            if (this.model.regixPersonData!.lastName !== undefined && this.model.regixPersonData!.lastName !== null) {
                this.model.names = `${this.model.names} ${this.model.regixPersonData!.lastName}`;
            }

            this.model.egnLncEik = this.model.regixPersonData!.egnLnc?.egnLnc;
        }
        else {
            this.model.names = this.model.regixLegalData!.name;
            this.model.egnLncEik = this.model.regixLegalData!.eik;
        }
    }
}