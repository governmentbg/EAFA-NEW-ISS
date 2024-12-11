import { HttpErrorResponse } from '@angular/common/http';
import { Component, EventEmitter, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn } from '@angular/forms';
import { Observable, Subject } from 'rxjs';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { AddressTypesEnum } from '@app/enums/address-types.enum';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { IApplicationsService } from '@app/interfaces/administration-app/applications.interface';
import { ILegalEntitiesService } from '@app/interfaces/administration-app/legal-entities.interface';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { ApplicationContentDTO } from '@app/models/generated/dtos/ApplicationContentDTO';
import { ApplicationRegiXCheckDTO } from '@app/models/generated/dtos/ApplicationRegiXCheckDTO';
import { AuthorizedPersonDTO } from '@app/models/generated/dtos/AuthorizedPersonDTO';
import { AuthorizedPersonErrorDTO } from '@app/models/generated/dtos/AuthorizedPersonErrorDTO';
import { AuthorizedPersonRegixDataDTO } from '@app/models/generated/dtos/AuthorizedPersonRegixDataDTO';
import { LegalEntityApplicationEditDTO } from '@app/models/generated/dtos/LegalEntityApplicationEditDTO';
import { LegalEntityEditDTO } from '@app/models/generated/dtos/LegalEntityEditDTO';
import { LegalEntityRegixDataDTO } from '@app/models/generated/dtos/LegalEntityRegixDataDTO';
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
import { EditAuthorizedPersonComponent } from '../edit-authorized-person/edit-authorized-person.component';
import { EditAuthorizedPersonDialogParams } from '../models/edit-authorized-person-dialog-params.model';
import { EditAuthorizedPersonDialogResult } from '../models/edit-authorized-person-dialog-result.model';
import { IS_PUBLIC_APP } from '@app/shared/modules/application.modules';
import { LegalEntitiesPublicService } from '@app/services/public-app/legal-entities-public.service';
import { ApplicationSubmittedByDTO } from '@app/models/generated/dtos/ApplicationSubmittedByDTO';
import { FileTypeEnum } from '@app/enums/file-types.enum';
import { Notifier } from '@app/shared/directives/notifier/notifier.class';
import { PersonFullDataDTO } from '@app/models/generated/dtos/PersonFullDataDTO';
import { LegalFullDataDTO } from '@app/models/generated/dtos/LegalFullDataDTO';

@Component({
    selector: 'edit-legal-entity',
    templateUrl: './edit-legal-entity.component.html'
})
export class EditLegalEntityComponent implements OnInit, IDialogComponent {
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
    public isIdReadOnly: boolean = false;
    public refreshFileTypes: Subject<void> = new Subject<void>();
    public isPublicApp: boolean;

    public addressTypesEnum: typeof AddressTypesEnum = AddressTypesEnum;

    public notifier: Notifier = new Notifier();
    public expectedResults: LegalEntityRegixDataDTO;
    public regixChecks: ApplicationRegiXCheckDTO[] = [];

    public authorizedPeople: AuthorizedPersonDTO[] | AuthorizedPersonRegixDataDTO[] = [];
    public authorizedPeopleTouched: boolean = false;

    public legalName: string = '';

    public showEgnAndEmailDontMatchError: boolean = false;
    public egnAndEmailDontMatchErrorEgnLnc: string | undefined = undefined;
    public egnAndEmailDontMatchErrorEmail: string | undefined = undefined;

    public noPermissionUsers: string | undefined;

    public readonly pageCode: PageCodeEnum = PageCodeEnum.LE;
    public readonly icIconSize: number = CommonUtils.IC_ICON_SIZE;

    @ViewChild('authorizedPeopleDataTable')
    private authorizedPeopleTable!: TLDataTableComponent;

    private id: number | undefined;
    private applicationId!: number;
    private isApplicationHistoryMode: boolean = false;
    private dialogRightSideActions: IActionInfo[] | undefined;

    private service!: ILegalEntitiesService;
    private applicationsService: IApplicationsService | undefined;
    private editAuthorizedPersonDialog: TLMatDialog<EditAuthorizedPersonComponent>;
    private confirmDialog: TLConfirmDialog;

    private model!: LegalEntityEditDTO | LegalEntityApplicationEditDTO | LegalEntityRegixDataDTO;
    private currentUserAuthorizedPerson: AuthorizedPersonDTO | undefined;

    constructor(
        translate: FuseTranslationLoaderService,
        confirmDialog: TLConfirmDialog,
        editAuthorizedPersonDialog: TLMatDialog<EditAuthorizedPersonComponent>
    ) {
        this.translate = translate;
        this.confirmDialog = confirmDialog;
        this.editAuthorizedPersonDialog = editAuthorizedPersonDialog;

        this.isPublicApp = IS_PUBLIC_APP;

        this.expectedResults = new LegalEntityRegixDataDTO({
            requester: new RegixPersonDataDTO(),
            requesterAddresses: [],
            legal: new RegixLegalDataDTO(),
            addresses: [],
            authorizedPeople: []
        });
    }

    public async ngOnInit(): Promise<void> {
        // извличане на исторически данни за заявление
        if (this.isApplicationHistoryMode && this.applicationId !== undefined) {
            this.form.disable();

            if (this.applicationsService) {
                this.applicationsService.getApplicationChangeHistoryContent(this.applicationId).subscribe({
                    next: (content: ApplicationContentDTO) => {
                        const contentObject: Record<string, unknown> = JSON.parse(content.draftContent!);
                        const legal: LegalEntityApplicationEditDTO = new LegalEntityApplicationEditDTO(contentObject);
                        legal.files = content.files;
                        legal.applicationId = content.applicationId;

                        this.isOnlineApplication = legal.isOnlineApplication!;
                        this.refreshFileTypes.next();

                        this.model = legal;
                        this.fillForm();

                        if (!this.isPublicApp && content.latestRegiXChecks !== undefined && content.latestRegiXChecks !== null && content.latestRegiXChecks.length > 0) {
                            this.showRegiXData = true;

                            setTimeout(() => {
                                this.regixChecks = content.latestRegiXChecks!;
                            }, 100);
                        }
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
                        this.model = legal as LegalEntityEditDTO;
                        this.isOnlineApplication = (legal as LegalEntityEditDTO).isOnlineApplication!;
                        this.refreshFileTypes.next();
                        this.fillForm();
                    }
                });
            }
            // извличане на данни за създаване на регистров запис от заявление
            else {
                this.isEditing = false;

                this.service.getApplicationDataForRegister(this.applicationId).subscribe({
                    next: (legal: LegalEntityEditDTO) => {
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
                        next: (regixData: RegixChecksWrapperDTO<LegalEntityRegixDataDTO>) => {
                            this.model = new LegalEntityRegixDataDTO(regixData.dialogDataModel);
                            this.expectedResults = new LegalEntityRegixDataDTO(regixData.regiXDataModel);

                            for (const person of this.model.authorizedPeople as AuthorizedPersonRegixDataDTO[]) {
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
                        next: (legal: LegalEntityApplicationEditDTO) => {
                            legal.applicationId = this.applicationId!;
                            this.isOnlineApplication = legal.isOnlineApplication!;
                            this.refreshFileTypes.next();

                            if (this.showRegiXData) {
                                this.expectedResults = new LegalEntityRegixDataDTO(legal.regiXDataModel);
                                legal.regiXDataModel = undefined;
                            }

                            if (this.isPublicApp && this.isOnlineApplication && (legal.requester === undefined || legal.requester === null)) {
                                const service = this.service as LegalEntitiesPublicService;
                                service.getCurrentUserAsSubmittedBy().subscribe({
                                    next: (requester: ApplicationSubmittedByDTO) => {
                                        legal.requester = requester.person;
                                        legal.requesterAddresses = requester.addresses;
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

                this.service.getLegalEntity(this.id).subscribe({
                    next: (legal: LegalEntityEditDTO) => {
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
        this.service = data.service as ILegalEntitiesService;
        this.dialogRightSideActions = buttons.rightSideActions;
        this.loadRegisterFromApplication = data.loadRegisterFromApplication;

        this.buildForm();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.form.markAllAsTouched();
        this.authorizedPeopleTouched = true;

        if (this.form.valid && this.showEgnAndEmailDontMatchError === false) {
            this.saveLegal(dialogClose);
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        let applicationAction: boolean = false;

        if (this.model instanceof LegalEntityApplicationEditDTO || this.model instanceof LegalEntityRegixDataDTO) {
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
                    this.authorizedPeopleTouched = true;
                    this.buildNoPermissionsUsers();
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

    public addEditAuthorizedPerson(person?: AuthorizedPersonDTO | AuthorizedPersonRegixDataDTO, viewMode: boolean = false): void {
        let data: EditAuthorizedPersonDialogParams | undefined;
        let headerAuditBtn: IHeaderAuditButton | undefined;
        let title!: string;

        if (person !== undefined) {
            data = new EditAuthorizedPersonDialogParams({
                model: person,
                isEgnLncReadOnly: this.isEditing,
                expectedResults: this.expectedResults.authorizedPeople?.find(x => x.id === person?.id) ?? new AuthorizedPersonRegixDataDTO(),
                readOnly: this.isReadonly || viewMode,
                showOnlyRegiXData: this.showOnlyRegixData
            });

            if (!IS_PUBLIC_APP && person.id !== undefined && person.id !== null) {
                headerAuditBtn = {
                    id: person.id,
                    getAuditRecordData: this.service.getAuthorizedPersonSimpleAudit.bind(this.service),
                    tableName: 'Person'
                };
            }

            if (this.isReadonly || viewMode) {
                title = this.translate.getValue('legal-entities-page.view-authorized-person-dialog-title');
            }
            else {
                title = this.translate.getValue('legal-entities-page.edit-authorized-person-dialog-title');
            }
        }
        else {
            data = new EditAuthorizedPersonDialogParams({
                showOnlyRegiXData: this.showOnlyRegixData
            });
            title = this.translate.getValue('legal-entities-page.add-authorized-person-dialog-title');
        }

        const dialog = this.editAuthorizedPersonDialog.openWithTwoButtons({
            title: title,
            TCtor: EditAuthorizedPersonComponent,
            headerAuditButton: headerAuditBtn,
            headerCancelButton: {
                cancelBtnClicked: this.closeDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translate,
            disableDialogClose: true,
            viewMode: this.isReadonly || viewMode
        }, '1300px');

        dialog.subscribe((result: EditAuthorizedPersonDialogResult) => {
            if (result && result.authorizedPerson) {
                this.setAuthorizedPersonTableFields(result.authorizedPerson);

                if (person !== undefined) {
                    person = result.authorizedPerson;
                }
                else {
                    this.authorizedPeople.push(result.authorizedPerson);
                }

                if (result.isTouched) {
                    ApplicationUtils.enableOrDisableRegixCheckButtons(this.form, this.dialogRightSideActions);
                }

                this.authorizedPeople = this.authorizedPeople.slice();
                this.authorizedPeopleTouched = true;
                this.form.updateValueAndValidity({ onlySelf: true });
                this.buildNoPermissionsUsers();

                this.showEgnAndEmailDontMatchError = false;
                this.egnAndEmailDontMatchErrorEgnLnc = undefined;
                this.egnAndEmailDontMatchErrorEmail = undefined;
            }
        });
    }

    public deleteAuthorizedPerson(person: GridRow<AuthorizedPersonDTO | AuthorizedPersonRegixDataDTO>): void {
        this.confirmDialog.open({
            title: this.translate.getValue('legal-entities-page.delete-authorized-person'),
            message: this.translate.getValue('legal-entities-page.confirm-delete-authorized-person-message'),
            okBtnLabel: this.translate.getValue('legal-entities-page.delete')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.authorizedPeopleTable.softDelete(person);
                    this.authorizedPeopleTouched = true;
                    this.form.updateValueAndValidity({ onlySelf: true });
                    this.buildNoPermissionsUsers();

                    this.showEgnAndEmailDontMatchError = false;
                    this.egnAndEmailDontMatchErrorEgnLnc = undefined;
                    this.egnAndEmailDontMatchErrorEmail = undefined;
                }
            }
        });
    }

    public restoreAuthorizedPerson(person: GridRow<AuthorizedPersonDTO | AuthorizedPersonRegixDataDTO>): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    //TODO
                    this.authorizedPeopleTable.softUndoDelete(person);
                    this.authorizedPeopleTouched = true;
                    this.form.updateValueAndValidity({ onlySelf: true });

                    const authorizedPerson: AuthorizedPersonDTO  = (this.authorizedPeople.find(x => x === person) as AuthorizedPersonDTO);

                    if (authorizedPerson && authorizedPerson.roles) {
                        for (const role of authorizedPerson.roles) {
                            role.isActive = true;
                        }

                        authorizedPerson.roles = authorizedPerson.roles.slice();
                    }

                    this.buildNoPermissionsUsers();

                    this.showEgnAndEmailDontMatchError = false;
                    this.egnAndEmailDontMatchErrorEgnLnc = undefined;
                    this.egnAndEmailDontMatchErrorEmail = undefined;
                }
            }
        });
    }

    public addCurrentUserAsAuthorizedPerson(): void {
        if (this.currentUserAuthorizedPerson === undefined || this.currentUserAuthorizedPerson === null) {
            this.service.getCurrentUserAsAuthorizedPerson().subscribe({
                next: (person: AuthorizedPersonDTO) => {
                    this.currentUserAuthorizedPerson = person;
                    this.setAuthorizedPersonTableFields(this.currentUserAuthorizedPerson);

                    this.authorizedPeople.unshift(this.currentUserAuthorizedPerson);
                    this.authorizedPeople = this.authorizedPeople.slice();

                    this.authorizedPeopleTouched = true;
                    this.form.updateValueAndValidity({ onlySelf: true });
                    this.buildNoPermissionsUsers();
                }
            });
        }
        else {
            const found: AuthorizedPersonDTO | undefined = this.authorizedPeople.find(x => x.id === this.currentUserAuthorizedPerson!.id);
            if (found !== undefined && found !== null) {
                found.isActive = true;
            }
            else {
                this.authorizedPeople.unshift(this.currentUserAuthorizedPerson);
            }
            this.authorizedPeople = this.authorizedPeople.slice();

            this.authorizedPeopleTouched = true;
            this.form.updateValueAndValidity({ onlySelf: true });
            this.buildNoPermissionsUsers();
        }
    }

    public fileTypeFilterFn(options: PermittedFileTypeDTO[]): PermittedFileTypeDTO[] {
        const pdfs: FileTypeEnum[] = [FileTypeEnum.SIGNEDAPPL, FileTypeEnum.APPLICATION_PDF];
        const offlines: FileTypeEnum[] = [FileTypeEnum.PAYEDFEE, FileTypeEnum.SCANNED_FORM];

        let result: PermittedFileTypeDTO[] = options;

        if (!this.isOnlineApplication) {
            result = result.filter(x => !pdfs.includes(FileTypeEnum[x.code as keyof typeof FileTypeEnum]));
        }

        if (this.isOnlineApplication) {
            if (this.isApplication && !this.isReadonly) {
                result = result.filter(x => !pdfs.includes(FileTypeEnum[x.code as keyof typeof FileTypeEnum]));
            }

            result = result.filter(x => !offlines.includes(FileTypeEnum[x.code as keyof typeof FileTypeEnum]));
        }

        return result;
    }

    public downloadedPersonData(person: PersonFullDataDTO): void {
        this.form.get('requesterControl')!.setValue(person.person);
        this.form.get('requesterAddressesControl')!.setValue(person.addresses);
    }

    public downloadedLegalData(legal: LegalFullDataDTO): void {
        this.form.get('legalControl')!.setValue(legal.legal);
        this.form.get('addressesControl')!.setValue(legal.addresses);
    }

    private buildForm(): void {
        this.form = new FormGroup({
            legalControl: new FormControl(),
            addressesControl: new FormControl(),
            filesControl: new FormControl()
        });

        if (this.isApplication) {
            this.form.addControl('requesterControl', new FormControl());
            this.form.addControl('requesterAddressesControl', new FormControl());
        }

        if (!this.showOnlyRegixData) {
            this.form.setValidators([this.authorizedPeopleValidator(), this.peopleWithoutRolesValidator()]);
        }

        this.form.get('legalControl')!.valueChanges.subscribe({
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
        if (this.model instanceof LegalEntityRegixDataDTO || this.model instanceof LegalEntityApplicationEditDTO) {
            this.form.get('requesterControl')!.setValue(this.model.requester);
            this.form.get('requesterAddressesControl')!.setValue(this.model.requesterAddresses);
        }

        this.form.get('legalControl')!.setValue(this.model.legal);
        this.form.get('addressesControl')!.setValue(this.model.addresses);
    
        const eik: string | undefined = this.model.legal?.eik;
        this.isIdReadOnly = CommonUtils.hasDigitsOnly(eik);

        if (this.model instanceof LegalEntityRegixDataDTO) {
            this.fillFormRegiX();
        }
        else {
            this.form.get('filesControl')!.setValue(this.model.files);

            if (this.showRegiXData) {
                this.fillFormRegiX();
            }
        }

        if (this.model.authorizedPeople !== undefined && this.model.authorizedPeople !== null) {
            const people = this.model.authorizedPeople;
            setTimeout(() => {
                this.authorizedPeople = people;

                for (const person of this.authorizedPeople) {
                    this.setAuthorizedPersonTableFields(person);
                }
            });
        }
    }

    private fillFormRegiX(): void {
        if (this.model instanceof LegalEntityApplicationEditDTO || this.model instanceof LegalEntityRegixDataDTO) {
            const applicationRegixChecks: ApplicationRegiXCheckDTO[] | undefined = this.model.applicationRegiXChecks;

            setTimeout(() => {
                if (applicationRegixChecks !== undefined && applicationRegixChecks !== null) {
                    this.regixChecks = applicationRegixChecks;
                }
            });

            this.model.applicationRegiXChecks = undefined;
        }

        if (!this.viewMode) {
            this.notifier.start();
            this.notifier.onNotify.subscribe({
                next: () => {
                    this.form.markAllAsTouched();
                    this.authorizedPeopleTouched = true;

                    if (this.showOnlyRegixData) {
                        ApplicationUtils.enableOrDisableRegixCheckButtons(this.form, this.dialogRightSideActions);
                    }

                    this.notifier.stop();
                }
            });
        }
    }

    private fillModel(): void {
        if (this.model instanceof LegalEntityRegixDataDTO || this.model instanceof LegalEntityApplicationEditDTO) {
            this.model.requester = this.form.get('requesterControl')!.value;
            this.model.requesterAddresses = this.form.get('requesterAddressesControl')!.value;
        }

        this.model.legal = this.form.get('legalControl')!.value;
        this.model.addresses = this.form.get('addressesControl')!.value;

        if (!(this.model instanceof LegalEntityRegixDataDTO)) {
            this.model.files = this.form.get('filesControl')!.value;
        }

        this.model.authorizedPeople = this.authorizedPeople;
    }

    private setAuthorizedPersonTableFields(person: AuthorizedPersonDTO): void {
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

        person.rolesAll = '';
        if (person.roles) {
            person.rolesAll = person.roles.filter(x => x.isActive).map(x => x.name).join(', ');
        }
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
                const errors: AuthorizedPersonErrorDTO = response.error;

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

        if (this.model instanceof LegalEntityEditDTO) {
            if (this.id !== undefined) {
                saveOrEditObservable = this.service.editLegalEntity(this.model);
            }
            else {
                saveOrEditObservable = this.service.addLegalEntity(this.model);
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

    private personEqualsRegixPerson(person: AuthorizedPersonRegixDataDTO): boolean {
        const regixPerson: AuthorizedPersonRegixDataDTO | undefined = this.expectedResults.authorizedPeople?.find(x => x.id === person.id);

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

    private authorizedPeopleValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            let atLeastOneActive: boolean = false;
            for (const person of this.authorizedPeople) {
                if (person.isActive === true) {
                    atLeastOneActive = true;
                    break;
                }
            }

            if (!atLeastOneActive) {
                return { 'atLeastOneAuthorizedPersonNeeded': true };
            }
            return null;
        };
    }

    private peopleWithoutRolesValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            const people: AuthorizedPersonDTO[] = this.authorizedPeople as AuthorizedPersonDTO[];

            for (const person of people) {
                if (!person.roles || person.roles.length === 0 || (person.isActive !== false && !person.roles.some(x => x.isActive ?? true))) {
                    return { 'userWithoutPermissions': true };
                }
            }
            return null;
        };
    }

    private buildNoPermissionsUsers(): void {
        if (this.authorizedPeople.length > 0 && this.authorizedPeople[0] instanceof AuthorizedPersonDTO) {
            this.noPermissionUsers = (this.authorizedPeople as AuthorizedPersonDTO[])
                .filter(x => x.isActive && x.roles?.some(x => x.isActive) !== true)
                .map(x => `${x.person!.firstName} ${x.person!.lastName}`)
                .join(', ');
        }
        else {
            this.noPermissionUsers = undefined;
        }
    }
}