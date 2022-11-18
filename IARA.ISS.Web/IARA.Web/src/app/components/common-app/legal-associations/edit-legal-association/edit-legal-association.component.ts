import { HttpErrorResponse } from '@angular/common/http';
import { Component, EventEmitter, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { Observable, Subject } from 'rxjs';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { AddressTypesEnum } from '@app/enums/address-types.enum';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { IApplicationsService } from '@app/interfaces/administration-app/applications.interface';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { ApplicationContentDTO } from '@app/models/generated/dtos/ApplicationContentDTO';
import { ApplicationRegiXCheckDTO } from '@app/models/generated/dtos/ApplicationRegiXCheckDTO';
import { PermittedFileTypeDTO } from '@app/models/generated/dtos/PermittedFileTypeDTO';
import { RegixChecksWrapperDTO } from '@app/models/generated/dtos/RegixChecksWrapperDTO';
import { RegixLegalDataDTO } from '@app/models/generated/dtos/RegixLegalDataDTO';
import { RegixPersonDataDTO } from '@app/models/generated/dtos/RegixPersonDataDTO';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { ApplicationDialogData, ApplicationUtils } from '@app/shared/utils/application.utils';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { IS_PUBLIC_APP } from '@app/shared/modules/application.modules';
import { ApplicationSubmittedByDTO } from '@app/models/generated/dtos/ApplicationSubmittedByDTO';
import { FileTypeEnum } from '@app/enums/file-types.enum';
import { Notifier } from '@app/shared/directives/notifier/notifier.class';
import { FishingAssociationRegixDataDTO } from '@app/models/generated/dtos/FishingAssociationRegixDataDTO';
import { FishingAssociationPersonDTO } from '@app/models/generated/dtos/FishingAssociationPersonDTO';
import { EditLegalAssociationPersonComponent } from '../edit-legal-association-person/edit-legal-association-person.component';
import { FishingAssociationEditDTO } from '@app/models/generated/dtos/FishingAssociationEditDTO';
import { FishingAssociationApplicationEditDTO } from '@app/models/generated/dtos/FishingAssociationApplicationEditDTO';
import { EditLegalAssociationPersonDialogParams } from '../models/edit-legal-association-person-dialog-params.model';
import { EditLegalAssociationPersonResult } from '../models/edit-legal-association-person-result.model';
import { FishingAssociationPersonErrorDTO } from '@app/models/generated/dtos/FishingAssociationPersonErrorDTO';
import { IRecreationalFishingAssociationService } from '@app/interfaces/common-app/recreational-fishing-association.interface';
import { RecreationalFishingAssociationPublicService } from '@app/services/public-app/recreational-fishing-association-public.service';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { EgnLncDTO } from '@app/models/generated/dtos/EgnLncDTO';

@Component({
    selector: 'edit-legal-association',
    templateUrl: './edit-legal-association.component.html'
})
export class EditLegalAssociationComponent implements OnInit, IDialogComponent {
    public form!: FormGroup;

    public translate: FuseTranslationLoaderService;

    public isApplication!: boolean;
    public showOnlyRegixData: boolean = false;
    public showRegiXData: boolean = false;
    public isReadonly: boolean = false;
    public viewMode: boolean = false;
    public isEditing: boolean = false;
    public loadRegisterFromApplication: boolean = false;
    public isOnlineApplication: boolean = false;
    public isRegister: boolean = false;
    public refreshFileTypes: Subject<void> = new Subject<void>();
    public isPublicApp: boolean;

    public territoryUnits: NomenclatureDTO<number>[] = [];

    public addressTypesEnum: typeof AddressTypesEnum = AddressTypesEnum;

    public notifier: Notifier = new Notifier();
    public expectedResults: FishingAssociationRegixDataDTO;
    public regixChecks: ApplicationRegiXCheckDTO[] = [];

    public persons: FishingAssociationPersonDTO[] = [];
    public personsTouched: boolean = false;

    public legalName: string = '';

    public showEgnAndEmailDontMatchError: boolean = false;
    public emailNotEnteredFor: string | undefined = undefined;
    public egnAndEmailDontMatchErrorEgnLnc: string | undefined = undefined;
    public egnAndEmailDontMatchErrorEmail: string | undefined = undefined;

    public readonly pageCode: PageCodeEnum = PageCodeEnum.Assocs;
    public readonly icIconSize: number = CommonUtils.IC_ICON_SIZE;

    @ViewChild('personsDataTable')
    private personsDataTable!: TLDataTableComponent;

    private id: number | undefined;
    private applicationId!: number;
    private isApplicationHistoryMode: boolean = false;
    private dialogRightSideActions: IActionInfo[] | undefined;

    private associationRole!: string;

    private service!: IRecreationalFishingAssociationService;
    private applicationsService: IApplicationsService | undefined;
    private editPersonDialog: TLMatDialog<EditLegalAssociationPersonComponent>;
    private confirmDialog: TLConfirmDialog;
    private nomenclatures: CommonNomenclatures;

    private model!: FishingAssociationEditDTO | FishingAssociationApplicationEditDTO | FishingAssociationRegixDataDTO;
    private currentUserPerson: FishingAssociationPersonDTO | undefined;

    constructor(
        translate: FuseTranslationLoaderService,
        confirmDialog: TLConfirmDialog,
        nomenclatures: CommonNomenclatures,
        editPersonDialog: TLMatDialog<EditLegalAssociationPersonComponent>
    ) {
        this.translate = translate;
        this.confirmDialog = confirmDialog;
        this.nomenclatures = nomenclatures;
        this.editPersonDialog = editPersonDialog;

        this.isPublicApp = IS_PUBLIC_APP;

        this.expectedResults = new FishingAssociationRegixDataDTO({
            submittedBy: new RegixPersonDataDTO(),
            submittedByAddresses: [],
            submittedFor: new RegixLegalDataDTO(),
            submittedForAddresses: [],
            persons: []
        });
    }

    public async ngOnInit(): Promise<void> {
        const nomenclatures: [NomenclatureDTO<number>[], string] = await Promise.all([
            NomenclatureStore.instance.getNomenclature(
                NomenclatureTypes.TerritoryUnits, this.nomenclatures.getTerritoryUnits.bind(this.nomenclatures), false
            ).toPromise(),
            this.service.getAssociationRoleName().toPromise()
        ]);

        this.territoryUnits = nomenclatures[0];
        this.associationRole = nomenclatures[1];

        // извличане на исторически данни за заявление
        if (this.isApplicationHistoryMode && this.applicationId !== undefined) {
            this.form.disable();

            if (this.applicationsService) {
                this.applicationsService.getApplicationChangeHistoryContent(this.applicationId).subscribe({
                    next: (content: ApplicationContentDTO) => {
                        const contentObject: Record<string, unknown> = JSON.parse(content.draftContent!);
                        const legal: FishingAssociationApplicationEditDTO = new FishingAssociationApplicationEditDTO(contentObject);
                        legal.files = content.files;
                        legal.applicationId = content.applicationId;

                        this.isOnlineApplication = legal.isOnlineApplication!;
                        this.refreshFileTypes.next();

                        this.model = legal;
                        this.fillForm();
                    }
                });
            }
        }
        else if (this.applicationId !== undefined && this.id === undefined && !this.isApplication) {
            // извличане на данни за регистър по id на заявление
            if (this.loadRegisterFromApplication === true) {
                if (this.isReadonly || this.viewMode) {
                    this.form.disable();
                }
                this.isEditing = true;

                this.service.getRegisterByApplicationId(this.applicationId, this.pageCode).subscribe({
                    next: (legal: unknown) => {
                        this.model = legal as FishingAssociationEditDTO;
                        this.isOnlineApplication = (legal as FishingAssociationEditDTO).isOnlineApplication!;
                        this.refreshFileTypes.next();
                        this.fillForm();
                    }
                });
            }
            // извличане на данни за създаване на регистров запис от заявление
            else {
                this.isEditing = false;
                this.isRegister = true;

                this.form.addControl('territoryUnitControl', new FormControl(undefined, Validators.required));

                this.service.getApplicationDataForRegister(this.applicationId).subscribe({
                    next: (legal: FishingAssociationEditDTO) => {
                        this.model = legal;
                        this.isOnlineApplication = legal.isOnlineApplication!;
                        this.refreshFileTypes.next();
                        this.fillForm();
                    }
                });
            }
        }
        else {
            if (this.isReadonly || this.viewMode) {
                this.form.disable();
            }

            if (this.isApplication && this.applicationId !== undefined) {
                // извличане на данни за RegiX сверка от служител
                this.isEditing = false;

                if (this.showOnlyRegixData) {
                    this.service.getRegixData(this.applicationId).subscribe({
                        next: (regixData: RegixChecksWrapperDTO<FishingAssociationRegixDataDTO>) => {
                            this.model = new FishingAssociationRegixDataDTO(regixData.dialogDataModel);
                            this.expectedResults = new FishingAssociationRegixDataDTO(regixData.regiXDataModel);

                            for (const person of this.model.persons as FishingAssociationPersonDTO[]) {
                                person.hasRegixDataDiscrepancy = !this.personEqualsRegixPerson(person);
                            }

                            this.fillForm();
                        }
                    });
                }
                else {
                    // извличане на данни за заявление
                    this.isEditing = false;

                    this.service.getApplication(this.applicationId, this.showRegiXData).subscribe({
                        next: (legal: FishingAssociationApplicationEditDTO) => {
                            legal.applicationId = this.applicationId!;
                            this.isOnlineApplication = legal.isOnlineApplication!;
                            this.refreshFileTypes.next();

                            if (this.showRegiXData) {
                                this.expectedResults = new FishingAssociationRegixDataDTO(legal.regiXDataModel);
                                legal.regiXDataModel = undefined;
                            }

                            if (this.isPublicApp && this.isOnlineApplication && (legal.submittedBy === undefined || legal.submittedBy === null)) {
                                const service = this.service as RecreationalFishingAssociationPublicService;
                                service.getCurrentUserAsSubmittedBy().subscribe({
                                    next: (requester: ApplicationSubmittedByDTO) => {
                                        legal.submittedBy = requester.person;
                                        legal.submittedByAddresses = requester.addresses;
                                        this.model = legal;
                                        this.fillForm();
                                    }
                                });
                            }
                            else {
                                this.model = legal;
                                this.fillForm();
                            }
                        }
                    });
                }
            }
            else if (this.id !== undefined) {
                // извличане на данни за регистров запис
                this.isEditing = true;
                this.isRegister = true;

                this.form.addControl('territoryUnitControl', new FormControl(undefined, Validators.required));

                this.service.getFishingAssociation(this.id).subscribe({
                    next: (legal: FishingAssociationEditDTO) => {
                        this.model = legal;
                        this.isOnlineApplication = legal.isOnlineApplication!;
                        this.refreshFileTypes.next();
                        this.fillForm();
                    }
                });
            }
        }
    }

    public setData(data: DialogParamsModel, buttons: DialogWrapperData): void {
        this.id = data.id;
        this.applicationId = data.applicationId!;
        this.applicationsService = data.applicationsService;
        this.isApplication = data.isApplication;
        this.isReadonly = data.isReadonly;
        this.isApplicationHistoryMode = data.isApplicationHistoryMode;
        this.viewMode = data.viewMode;
        this.showOnlyRegixData = data.showOnlyRegiXData;
        this.showRegiXData = data.showRegiXData;
        this.service = data.service as IRecreationalFishingAssociationService;
        this.dialogRightSideActions = buttons.rightSideActions;
        this.loadRegisterFromApplication = data.loadRegisterFromApplication;

        this.buildForm();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.form.markAllAsTouched();
        this.personsTouched = true;

        if (this.form.valid && this.showEgnAndEmailDontMatchError === false) {
            this.saveLegal(dialogClose);
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        let applicationAction: boolean = false;

        if (this.model instanceof FishingAssociationApplicationEditDTO || this.model instanceof FishingAssociationRegixDataDTO) {
            this.fillModel();
            CommonUtils.sanitizeModelStrings(this.model);

            applicationAction = ApplicationUtils.applicationDialogButtonClicked(new ApplicationDialogData({
                action: actionInfo,
                dialogClose: dialogClose,
                applicationId: this.applicationId,
                model: this.model,
                readOnly: this.isReadonly,
                viewMode: this.viewMode,
                editForm: this.form,
                saveFn: this.saveLegal.bind(this),
                onMarkAsTouched: () => {
                    this.personsTouched = true;
                }
            }));
        }

        if (!this.isReadonly && !this.viewMode && !applicationAction) {
            switch (actionInfo.id) {
                case 'save':
                    return this.saveBtnClicked(actionInfo, dialogClose);
            }
        }
    }

    public addEditPerson(person?: FishingAssociationPersonDTO, viewMode: boolean = false): void {
        let data: EditLegalAssociationPersonDialogParams | undefined;
        let headerAuditBtn: IHeaderAuditButton | undefined;
        let title!: string;

        if (person !== undefined) {
            data = new EditLegalAssociationPersonDialogParams({
                model: person,
                isEgnLncReadOnly: this.isEditing,
                expectedResults: this.expectedResults.persons?.find(x => x.id === person?.id) ?? new FishingAssociationPersonDTO(),
                readOnly: this.isReadonly || viewMode,
                showOnlyRegiXData: this.showOnlyRegixData
            });

            if (!IS_PUBLIC_APP && person.id !== undefined && person.id !== null) {
                headerAuditBtn = {
                    id: person.id,
                    getAuditRecordData: this.service.getFishingAssociationPersonSimpleAudit.bind(this.service),
                    tableName: 'Person'
                };
            }

            if (this.isReadonly || viewMode) {
                title = this.translate.getValue('legal-association.view-person-dialog-title');
            }
            else {
                title = this.translate.getValue('legal-association.edit-person-dialog-title');
            }
        }
        else {
            data = new EditLegalAssociationPersonDialogParams({
                showOnlyRegiXData: this.showOnlyRegixData
            });
            title = this.translate.getValue('legal-association.add-person-dialog-title');
        }

        const dialog = this.editPersonDialog.openWithTwoButtons({
            title: title,
            TCtor: EditLegalAssociationPersonComponent,
            headerAuditButton: headerAuditBtn,
            headerCancelButton: {
                cancelBtnClicked: this.closeDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translate,
            disableDialogClose: true,
            viewMode: this.isReadonly || viewMode
        }, '1300px');

        dialog.subscribe((result: EditLegalAssociationPersonResult) => {
            if (result && result.person) {
                this.setPersonTableFields(result.person);

                if (person !== undefined) {
                    person = result.person;
                }
                else {
                    this.persons.push(result.person);
                }

                if (result.isTouched) {
                    ApplicationUtils.enableOrDisableRegixCheckButtons(this.form, this.dialogRightSideActions);
                }

                this.persons = this.persons.slice();
                this.personsTouched = true;
                this.form.updateValueAndValidity({ onlySelf: true });

                this.showEgnAndEmailDontMatchError = false;
                this.egnAndEmailDontMatchErrorEgnLnc = undefined;
                this.egnAndEmailDontMatchErrorEmail = undefined;
            }
        });
    }

    public deletePerson(person: GridRow<FishingAssociationPersonDTO>): void {
        this.confirmDialog.open({
            title: this.translate.getValue('legal-association.delete-person'),
            message: this.translate.getValue('legal-association.confirm-delete-person-message'),
            okBtnLabel: this.translate.getValue('legal-association.delete')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.personsDataTable.softDelete(person);
                    this.personsTouched = true;
                    this.form.updateValueAndValidity({ onlySelf: true });

                    this.showEgnAndEmailDontMatchError = false;
                    this.egnAndEmailDontMatchErrorEgnLnc = undefined;
                    this.egnAndEmailDontMatchErrorEmail = undefined;
                }
            }
        });
    }

    public restorePerson(person: GridRow<FishingAssociationPersonDTO>): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.personsDataTable.softUndoDelete(person);
                    this.personsTouched = true;
                    this.form.updateValueAndValidity({ onlySelf: true });

                    this.showEgnAndEmailDontMatchError = false;
                    this.egnAndEmailDontMatchErrorEgnLnc = undefined;
                    this.egnAndEmailDontMatchErrorEmail = undefined;
                }
            }
        });
    }

    public addCurrentUserAsPerson(): void {
        if (this.currentUserPerson === undefined || this.currentUserPerson === null) {
            (this.service as RecreationalFishingAssociationPublicService).getCurrentUserAsFishingAssociationPerson().subscribe({
                next: (person: FishingAssociationPersonDTO) => {
                    this.currentUserPerson = person;
                    this.setPersonTableFields(this.currentUserPerson);

                    if (!this.personExists(person.person!.egnLnc!)) {
                        this.persons.unshift(this.currentUserPerson);
                        this.persons = this.persons.slice();
                    }

                    this.personsTouched = true;
                    this.form.updateValueAndValidity({ onlySelf: true });
                }
            });
        }
        else {
            const found: FishingAssociationPersonDTO | undefined = this.persons.find(x => x.id === this.currentUserPerson!.id);
            if (found !== undefined && found !== null) {
                found.isActive = true;
            }
            else {
                this.persons.unshift(this.currentUserPerson);
            }
            this.persons = this.persons.slice();

            this.personsTouched = true;
            this.form.updateValueAndValidity({ onlySelf: true });
        }
    }

    public fileTypeFilterFn(options: PermittedFileTypeDTO[]): PermittedFileTypeDTO[] {
        const pdfs: FileTypeEnum[] = [FileTypeEnum.SIGNEDAPPL, FileTypeEnum.APPLICATION_PDF];

        let result: PermittedFileTypeDTO[] = options;

        if (this.isApplication || !this.isOnlineApplication) {
            result = result.filter(x => !pdfs.includes(FileTypeEnum[x.code as keyof typeof FileTypeEnum]));
        }

        return result;
    }

    private buildForm(): void {
        this.form = new FormGroup({
            submittedForControl: new FormControl(),
            submittedForAddressesControl: new FormControl(),
            filesControl: new FormControl()
        });

        if (this.isApplication) {
            this.form.addControl('submittedByControl', new FormControl());
            this.form.addControl('submittedByAddressesControl', new FormControl());
        }

        if (!this.showOnlyRegixData) {
            this.form.setValidators([this.personsValidator()]);
        }

        this.form.get('submittedForControl')!.valueChanges.subscribe({
            next: (legal: RegixLegalDataDTO) => {
                if (legal !== undefined && legal !== null) {
                    if (legal.name !== this.legalName) {
                        this.legalName = legal.name ?? '';
                    }
                }
            }
        });
    }

    private fillForm(): void {
        if (this.model instanceof FishingAssociationApplicationEditDTO) {
            this.form.get('submittedByControl')!.setValue(this.model.submittedBy);
            this.form.get('submittedByAddressesControl')!.setValue(this.model.submittedByAddresses);
            this.form.get('submittedForControl')!.setValue(this.model.submittedFor);
            this.form.get('submittedForAddressesControl')!.setValue(this.model.submittedForAddresses);

            this.form.get('filesControl')!.setValue(this.model.files);

            if (this.showRegiXData) {
                this.fillFormRegiX(this.model);
            }
        }
        else if (this.model instanceof FishingAssociationRegixDataDTO) {
            this.form.get('submittedByControl')!.setValue(this.model.submittedBy);
            this.form.get('submittedByAddressesControl')!.setValue(this.model.submittedByAddresses);
            this.form.get('submittedForControl')!.setValue(this.model.submittedFor);
            this.form.get('submittedForAddressesControl')!.setValue(this.model.submittedForAddresses);

            this.fillFormRegiX(this.model);
        }
        else if (this.model instanceof FishingAssociationEditDTO) {
            this.form.get('submittedForControl')!.setValue(this.model.submittedFor);
            this.form.get('submittedForAddressesControl')!.setValue(this.model.submittedForAddresses);

            const territoryUnitId: number | undefined = this.model.territoryUnitId;
            this.form.get('territoryUnitControl')!.setValue(this.territoryUnits.find(x => x.value === territoryUnitId));

            this.form.get('filesControl')!.setValue(this.model.files);
        }

        if (this.model.persons !== undefined && this.model.persons !== null) {
            const people = this.model.persons;
            setTimeout(() => {
                this.persons = people;

                for (const person of this.persons) {
                    this.setPersonTableFields(person);
                }
            });
        }
    }

    private fillFormRegiX(model: FishingAssociationRegixDataDTO): void {
        if (model.applicationRegiXChecks !== undefined && model.applicationRegiXChecks !== null) {
            const applicationRegixChecks: ApplicationRegiXCheckDTO[] = model.applicationRegiXChecks;

            setTimeout(() => {
                this.regixChecks = applicationRegixChecks;
            });
        }

        if (!this.viewMode) {
            this.notifier.start();
            this.notifier.onNotify.subscribe({
                next: () => {
                    this.form.markAllAsTouched();
                    this.personsTouched = true;

                    if (this.showOnlyRegixData) {
                        ApplicationUtils.enableOrDisableRegixCheckButtons(this.form, this.dialogRightSideActions);
                    }
                    this.notifier.stop();
                }
            });
        }
    }

    private fillModel(): void {
        if (this.model instanceof FishingAssociationRegixDataDTO || this.model instanceof FishingAssociationApplicationEditDTO) {
            this.model.submittedBy = this.form.get('submittedByControl')!.value;
            this.model.submittedByAddresses = this.form.get('submittedByAddressesControl')!.value;
        }

        this.model.submittedFor = this.form.get('submittedForControl')!.value;
        this.model.submittedForAddresses = this.form.get('submittedForAddressesControl')!.value;

        if (!(this.model instanceof FishingAssociationRegixDataDTO)) {
            this.model.files = this.form.get('filesControl')!.value;
        }

        if (this.model instanceof FishingAssociationEditDTO) {
            this.model.territoryUnitId = this.form.get('territoryUnitControl')!.value!.value;
        }

        this.model.persons = this.persons;
    }

    private setPersonTableFields(person: FishingAssociationPersonDTO): void {
        if (person.person !== undefined && person.person !== null) {
            person.fullName = '';
            if (person.person.firstName && person.person.firstName.length > 0) {
                person.fullName = `${person.person.firstName}`;
            }
            if (person.person.middleName && person.person.middleName.length > 0) {
                person.fullName = `${person.fullName} ${person.person.middleName}`;
            }
            if (person.person.lastName && person.person.lastName.length > 0) {
                person.fullName = `${person.fullName} ${person.person.lastName}`;
            }
            person.fullName = person.fullName.trim();

            person.email = person.person.email;
        }

        person.role = this.associationRole;
    }

    private saveLegal(dialogClose: DialogCloseCallback, fromSaveAsDraft: boolean = false): Observable<boolean> {
        const saveOrEditDone: EventEmitter<boolean> = new EventEmitter<boolean>();

        this.saveOrEdit(fromSaveAsDraft).subscribe({
            next: (id: number | void) => {
                if (typeof id === 'number' && id !== undefined) {
                    this.model.id = id;
                    dialogClose(this.model);
                }
                else {
                    dialogClose(this.model);
                }

                saveOrEditDone.emit(true);
                saveOrEditDone.complete();
            },
            error: (response: HttpErrorResponse) => {
                const errors: FishingAssociationPersonErrorDTO = response.error;

                if (errors?.egnAndEmailDontMatch === true) {
                    this.showEgnAndEmailDontMatchError = true;
                }

                if (errors?.egnLnc !== undefined && errors?.egnLnc !== null) {
                    this.egnAndEmailDontMatchErrorEgnLnc = errors?.egnLnc;
                }

                if (errors?.egnLnc !== undefined && errors?.egnLnc !== null) {
                    this.egnAndEmailDontMatchErrorEmail = errors?.email;
                }
            }
        });

        return saveOrEditDone.asObservable();
    }

    private saveOrEdit(fromSaveAsDraft: boolean): Observable<number | void> {
        this.fillModel();
        CommonUtils.sanitizeModelStrings(this.model);
        let saveOrEditObservable: Observable<void | number>;

        if (this.model instanceof FishingAssociationEditDTO) {
            if (this.id !== undefined) {
                saveOrEditObservable = this.service.editFishingAssociation(this.model);
            }
            else {
                saveOrEditObservable = this.service.addFishingAssociation(this.model);
            }
        }
        else {
            if (this.model.id !== undefined && this.model.id !== null) {
                saveOrEditObservable = this.service.editApplication(this.model, this.pageCode, fromSaveAsDraft);
            }
            else {
                saveOrEditObservable = this.service.addApplication(this.model);
            }
        }

        return saveOrEditObservable;
    }

    private personEqualsRegixPerson(person: FishingAssociationPersonDTO): boolean {
        const regixPerson: FishingAssociationPersonDTO | undefined = this.expectedResults.persons?.find(x => x.id === person.id);

        if (regixPerson !== undefined) {
            if (person.person !== undefined) {
                if (!CommonUtils.objectsEqual(person.person, regixPerson.person)) {
                    return false;
                }
            }
        }
        return true;
    }

    private closeDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private personExists(egnLnc: EgnLncDTO): boolean {
        return this.persons.findIndex(x => x.person?.egnLnc?.egnLnc === egnLnc.egnLnc
            && x.person?.egnLnc?.identifierType === egnLnc.identifierType) !== -1;
    }

    private personsValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            let atLeastOneActive: boolean = false;
            for (const person of this.persons) {
                if (person.isActive === true) {
                    atLeastOneActive = true;
                    break;
                }
            }

            if (this.persons.length > 0) {
                const noEmailPerson: FishingAssociationPersonDTO | undefined = this.persons.find(x => !x.email || x.email === '');
                if (noEmailPerson) {
                    this.emailNotEnteredFor = noEmailPerson.fullName;
                    return { 'emailNotEntered': true }
                }
                else {
                    this.emailNotEnteredFor = undefined;
                }
            }
            else {
                this.emailNotEnteredFor = undefined;
            }

            if (!atLeastOneActive) {
                return { 'atLeastOnePersonNeeded': true };
            }
            return null;
        };
    }
}