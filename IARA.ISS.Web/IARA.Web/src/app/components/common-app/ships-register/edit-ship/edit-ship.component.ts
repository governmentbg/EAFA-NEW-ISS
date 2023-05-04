import { AfterViewInit, Component, EventEmitter, Injector, Input, OnChanges, OnDestroy, OnInit, Optional, Output, Self, SimpleChange, SimpleChanges, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, NgControl, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';
import { forkJoin, Observable, Subject, Subscription } from 'rxjs';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { ShipEventTypeEnum } from '@app/enums/ship-event-type.enum';
import { ShipExportTypeEnum } from '@app/enums/ship-export-type.enum';
import { IApplicationsService } from '@app/interfaces/administration-app/applications.interface';
import { IShipsRegisterService } from '@app/interfaces/common-app/ships-register.interface';
import { ApplicationContentDTO } from '@app/models/generated/dtos/ApplicationContentDTO';
import { ApplicationRegiXCheckDTO } from '@app/models/generated/dtos/ApplicationRegiXCheckDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { RegixChecksWrapperDTO } from '@app/models/generated/dtos/RegixChecksWrapperDTO';
import { SailAreaNomenclatureDTO } from '@app/models/generated/dtos/SailAreaNomenclatureDTO';
import { ShipOwnerDTO } from '@app/models/generated/dtos/ShipOwnerDTO';
import { ShipOwnerRegixDataDTO } from '@app/models/generated/dtos/ShipOwnerRegixDataDTO';
import { ShipRegisterApplicationEditDTO } from '@app/models/generated/dtos/ShipRegisterApplicationEditDTO';
import { ShipRegisterEditDTO } from '@app/models/generated/dtos/ShipRegisterEditDTO';
import { ShipRegisterRegixDataDTO } from '@app/models/generated/dtos/ShipRegisterRegixDataDTO';
import { VesselTypeNomenclatureDTO } from '@app/models/generated/dtos/VesselTypeNomenclatureDTO';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { ApplicationDialogData, ApplicationUtils } from '@app/shared/utils/application.utils';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { EditShipOwnerComponent } from '@app/components/common-app/ships-register/edit-ship-owner/edit-ship-owner.component';
import { EditShipDialogParams } from '@app/components/common-app/ships-register/models/edit-ship-dialog-params.model';
import { EditShipOwnerDialogParams } from '@app/components/common-app/ships-register/models/edit-ship-owner-dialog-params.model';
import { EditShipOwnerDialogResult } from '@app/components/common-app/ships-register/models/edit-ship-owner-dialog-result.model';
import { DETAIL_FIELDS, ENABLE_FIELD_CONTROLS, ENABLE_OWNERS_EDIT } from '@app/components/common-app/ships-register/edit-ship/edit-ship-fields.data';
import { CancellationReasonDTO } from '@app/models/generated/dtos/CancellationReasonDTO';
import { CancellationReasonGroupEnum } from '@app/enums/cancellation-reason-group.enum';
import { ApplicationSubmittedByRegixDataDTO } from '@app/models/generated/dtos/ApplicationSubmittedByRegixDataDTO';
import { ApplicationSubmittedForRegixDataDTO } from '@app/models/generated/dtos/ApplicationSubmittedForRegixDataDTO';
import { CancellationDetailsDTO } from '@app/models/generated/dtos/CancellationDetailsDTO';
import { FormControlDataLoader } from '@app/shared/utils/form-control-data-loader';
import { FishingGearNomenclatureDTO } from '@app/models/generated/dtos/FishingGearNomenclatureDTO';
import { RegixPersonDataDTO } from '@app/models/generated/dtos/RegixPersonDataDTO';
import { RegixLegalDataDTO } from '@app/models/generated/dtos/RegixLegalDataDTO';
import { RequestProperties } from '@app/shared/services/request-properties';
import { ErrorSnackbarComponent } from '@app/shared/components/error-snackbar/error-snackbar.component';
import { ErrorModel } from '@app/models/common/exception.model';
import { ApplicationValidationErrorsEnum } from '@app/enums/application-validation-errors.enum';
import { FleetTypeNomenclatureDTO } from '@app/models/generated/dtos/FleetTypeNomenclatureDTO';
import { ApplicationSubmittedForDTO } from '@app/models/generated/dtos/ApplicationSubmittedForDTO';
import { SubmittedByRolesEnum } from '@app/enums/submitted-by-roles.enum';
import { ApplicationSubmittedByDTO } from '@app/models/generated/dtos/ApplicationSubmittedByDTO';
import { ShipRegisterUserDTO } from '@app/models/generated/dtos/ShipRegisterUserDTO';
import { CustomFormControl } from '@app/shared/utils/custom-form-control';
import { ShipRegisterUsedCertificateDTO } from '@app/models/generated/dtos/ShipRegisterUsedCertificateDTO';
import { IS_PUBLIC_APP } from '@app/shared/modules/application.modules';
import { ShipsRegisterPublicService } from '@app/services/public-app/ships-register-public.service';
import { FileTypeEnum } from '@app/enums/file-types.enum';
import { Notifier } from '@app/shared/directives/notifier/notifier.class';
import { FishingCapacityFreedActionsRegixDataDTO } from '@app/models/generated/dtos/FishingCapacityFreedActionsRegixDataDTO';
import { IFishingCapacityService } from '@app/interfaces/common-app/fishing-capacity.interface';
import { FishingCapacityPublicService } from '@app/services/public-app/fishing-capacity-public.service';
import { FishingCapacityAdministrationService } from '@app/services/administration-app/fishing-capacity-administration.service';
import { FishingCapacityFreedActionsDTO } from '@app/models/generated/dtos/FishingCapacityFreedActionsDTO';
import { FishingCapacityRemainderActionEnum } from '@app/enums/fishing-capacity-remainder-action.enum';
import { PermittedFileTypeDTO } from '@app/models/generated/dtos/PermittedFileTypeDTO';
import { DateUtils } from '@app/shared/utils/date.utils';
import { NewCertificateData } from '../../fishing-capacity/acquired-fishing-capacity/acquired-fishing-capacity.component';
import { GetControlErrorLabelTextCallback } from '@app/shared/components/input-controls/base-tl-control';

@Component({
    selector: 'edit-ship',
    templateUrl: './edit-ship.component.html'
})
export class EditShipComponent extends CustomFormControl<ShipRegisterEditDTO | number | null> implements OnInit, AfterViewInit, OnDestroy, OnChanges, IDialogComponent {
    @Input() public isThirdPartyShip: boolean = false;
    @Input() public isReadonly: boolean = false;
    @Input() public viewMode: boolean = false;
    @Input() public disableFieldsByEventType: ShipEventTypeEnum | undefined;
    @Input() public service!: IShipsRegisterService;

    @Output()
    public shipRegisterLoaded: EventEmitter<ShipRegisterEditDTO> = new EventEmitter<ShipRegisterEditDTO>();

    public readonly today: Date = new Date();

    public pageCode: PageCodeEnum = PageCodeEnum.RegVessel;

    public readonly icIconSize: number = CommonUtils.IC_ICON_SIZE;
    public readonly eventTypes: typeof ShipEventTypeEnum = ShipEventTypeEnum;

    public isApplication: boolean = false;
    public showOnlyRegiXData: boolean = false;
    public showRegiXData: boolean = false;
    public isDialog: boolean = false;
    public isEditing: boolean = false;
    public isPaid: boolean = false;
    public hasDelivery: boolean = false;
    public hasNoEDeliveryRegistrationError: boolean = false;
    public isRegisterEntry: boolean = false;
    public isAddingRegisterEntry: boolean = false;
    public hasImportCountry: boolean = false;
    public hasFishingCapacity: boolean = false;
    public hasControlCard: boolean = false;
    public hasFitnessCertificate: boolean = false;
    public willIssueCapacityCertificates: boolean = false;
    public hideBasicPaymentInfo: boolean = false;
    public dialogRightSideActions: IActionInfo[] | undefined;

    public notifier: Notifier = new Notifier();

    public fleets: FleetTypeNomenclatureDTO[] = [];
    public vesselTypes: VesselTypeNomenclatureDTO[] = [];
    public sailAreas: SailAreaNomenclatureDTO[] = [];
    public publicAidTypes: NomenclatureDTO<number>[] = [];
    public fleetSegments: NomenclatureDTO<number>[] = [];
    public ports: NomenclatureDTO<number>[] = [];
    public fishingGears: FishingGearNomenclatureDTO[] = [];
    public hullMaterials: NomenclatureDTO<number>[] = [];
    public shipAssociations: NomenclatureDTO<number>[] = [];
    public countries: NomenclatureDTO<number>[] = [];
    public exportTypes: NomenclatureDTO<ShipExportTypeEnum>[] = [];
    public cancellationReasons: NomenclatureDTO<number>[] = [];
    public destroyReasons: NomenclatureDTO<number>[] = [];
    public fuelTypes: NomenclatureDTO<number>[] = [];

    public expectedResults: ShipRegisterRegixDataDTO;
    public regixChecks: ApplicationRegiXCheckDTO[] = [];

    public shipOwners: ShipOwnerDTO[] | ShipOwnerRegixDataDTO[] = [];
    public allowOwnersEdit: boolean = false;
    public ownersTouched: boolean = false;

    public shipUsers: ShipRegisterUserDTO[] = [];
    public usedCapacityCertificates: ShipRegisterUsedCertificateDTO[] = [];

    public remainingTonnage: number = 0;
    public remainingPower: number = 0;
    public newCertificateData: NewCertificateData | undefined;

    public isPublicApp: boolean;
    public isDraft: boolean = false;
    public isOnlineApplication: boolean = false;
    public refreshFileTypes: Subject<void> = new Subject<void>();;
    public canRestoreRecords!: boolean;
    public canDeleteRecords!: boolean;

    public fishingCapacityService: IFishingCapacityService;

    public getControlErrorLabelTextMethod: GetControlErrorLabelTextCallback = this.getControlErrorLabelText.bind(this);

    private id: number | undefined;
    private applicationId: number | undefined;
    private isApplicationHistoryMode: boolean = false;
    private loadRegisterFromApplication: boolean = false;

    private currentActiveFormControls: string[] = [];

    @ViewChild('ownersTable')
    private ownersTable!: TLDataTableComponent;

    private translate: FuseTranslationLoaderService;
    private nomenclatures: CommonNomenclatures;
    private permissions: PermissionsService;
    private applicationsService: IApplicationsService | undefined;
    private confirmDialog: TLConfirmDialog;
    private editOwnerDialog: TLMatDialog<EditShipOwnerComponent>;
    private snackbar: MatSnackBar;

    private model!: ShipRegisterEditDTO | ShipRegisterApplicationEditDTO | ShipRegisterRegixDataDTO;

    private actionsSub: Subscription | undefined;

    private readonly loader: FormControlDataLoader;

    public constructor(
        @Optional() @Self() ngControl: NgControl,
        translate: FuseTranslationLoaderService,
        nomenclatures: CommonNomenclatures,
        permissions: PermissionsService,
        confirmDialog: TLConfirmDialog,
        editOwnerDialog: TLMatDialog<EditShipOwnerComponent>,
        snackbar: MatSnackBar,
        injector: Injector
    ) {
        super(ngControl);

        this.translate = translate;
        this.permissions = permissions;
        this.nomenclatures = nomenclatures;
        this.confirmDialog = confirmDialog;
        this.editOwnerDialog = editOwnerDialog;
        this.snackbar = snackbar;
        this.isPublicApp = IS_PUBLIC_APP;

        if (this.isPublicApp) {
            this.fishingCapacityService = injector.get(FishingCapacityPublicService);
        }
        else {
            this.fishingCapacityService = injector.get(FishingCapacityAdministrationService);
        }

        this.expectedResults = new ShipRegisterRegixDataDTO({
            submittedBy: new ApplicationSubmittedByRegixDataDTO({
                person: new RegixPersonDataDTO(),
                addresses: []
            }),
            submittedFor: new ApplicationSubmittedForRegixDataDTO({
                person: new RegixPersonDataDTO(),
                legal: new RegixLegalDataDTO(),
                addresses: []
            }),
            owners: [],
            remainingCapacityAction: new FishingCapacityFreedActionsRegixDataDTO()
        });

        this.loader = new FormControlDataLoader(this.getNomenclatures.bind(this));
    }

    public ngOnInit(): void {
        this.initCustomFormControl();

        if (this.form === undefined || this.form === null) {
            this.setPermissions();
        }

        this.setValidators(this.form);

        this.loader.load(() => {
            this.loadData();
        });
    }

    public ngAfterViewInit(): void {
        this.form.get('forbiddenForRSRControl')?.valueChanges.subscribe({
            next: (yes: boolean) => {
                if (yes) {
                    this.form.get('forbiddenReasonControl')!.setValidators([Validators.maxLength(500), Validators.required]);
                    this.form.get('forbiddenStartDateControl')!.setValidators([Validators.required]);
                    this.form.get('forbiddenEndDateControl')!.setValidators([Validators.required]);
                }
                else {
                    this.form.get('forbiddenReasonControl')!.clearValidators();
                    this.form.get('forbiddenStartDateControl')!.clearValidators();
                    this.form.get('forbiddenEndDateControl')!.clearValidators();
                }

                this.form.get('forbiddenReasonControl')!.markAsPending({ emitEvent: false });
                this.form.get('forbiddenStartDateControl')!.markAsPending({ emitEvent: false });
                this.form.get('forbiddenEndDateControl')!.markAsPending({ emitEvent: false });

                if (yes) {
                    this.form.get('forbiddenReasonControl')!.enable({ emitEvent: false });
                    this.form.get('forbiddenStartDateControl')!.enable({ emitEvent: false });
                    this.form.get('forbiddenEndDateControl')!.enable({ emitEvent: false });

                    if (!this.isDialog) {
                        setTimeout(() => {
                            this.setActiveFieldsClass(['forbiddenReasonControl', 'forbiddenStartDateControl', 'forbiddenEndDateControl'], true, false);
                        });
                    }
                }
                else {
                    this.form.get('forbiddenReasonControl')!.disable({ emitEvent: false });
                    this.form.get('forbiddenStartDateControl')!.disable({ emitEvent: false });
                    this.form.get('forbiddenEndDateControl')!.disable({ emitEvent: false });

                    if (!this.isDialog) {
                        setTimeout(() => {
                            this.setActiveFieldsClass(['forbiddenReasonControl', 'forbiddenStartDateControl', 'forbiddenEndDateControl'], false, false);
                        });
                    }
                }
            }
        });

        if (!this.showOnlyRegiXData) {
            if (this.isApplication) {
                this.form.get('deliveryDataControl')!.valueChanges.subscribe({
                    next: () => {
                        this.hasNoEDeliveryRegistrationError = false;
                    }
                });
            }

            this.form.get('fleetTypeControl')!.valueChanges.subscribe({
                next: (fleet: FleetTypeNomenclatureDTO | undefined) => {
                    this.hasFishingCapacity = fleet?.hasFishingCapacity === true;
                    this.hasControlCard = fleet?.hasControlCard === true;
                    this.hasFitnessCertificate = fleet?.hasFitnessCertificate === true;
                    this.refreshFileTypes.next();

                    if (this.hasFishingCapacity) {
                        if (!this.form.contains('acquiredCapacityControl')) {
                            this.form.addControl('acquiredCapacityControl', new FormControl(null));
                        }

                        this.actionsSub?.unsubscribe();
                        this.actionsSub = this.form.get('actionsControl')!.valueChanges.subscribe({
                            next: (actions: FishingCapacityFreedActionsDTO) => {
                                if (actions) {
                                    this.willIssueCapacityCertificates =
                                        actions.action !== FishingCapacityRemainderActionEnum.NoCertificate
                                        && (this.remainingTonnage > 0 || this.remainingPower > 0);
                                }
                                else {
                                    this.willIssueCapacityCertificates = false;
                                }
                            }
                        });
                    }
                    else {
                        this.form.removeControl('acquiredCapacityControl');
                    }

                    const actions: FishingCapacityFreedActionsDTO = this.form.get('actionsControl')?.value;
                    this.willIssueCapacityCertificates =
                        actions !== undefined && actions !== null
                        && actions.action !== FishingCapacityRemainderActionEnum.NoCertificate
                        && (this.remainingTonnage > 0 || this.remainingPower > 0);

                    if (this.hasControlCard) {
                        this.form.get('controlCardNumControl')!.setValidators([Validators.maxLength(20)]);
                        this.form.get('controlCardDateControl')!.setValidators(Validators.required);

                        this.form.get('controlCardNumControl')!.markAsPending({ emitEvent: false });
                        this.form.get('controlCardDateControl')!.markAsPending({ emitEvent: false });
                    }
                    else {
                        this.form.get('controlCardNumControl')!.clearValidators();
                        this.form.get('controlCardDateControl')!.clearValidators();
                    }

                    if (this.hasFitnessCertificate) {
                        this.form.get('controlCardValidityCertificateNumControl')!.setValidators([Validators.required, Validators.maxLength(50)]);
                        this.form.get('controlCardValidityCertificateDateControl')!.setValidators(Validators.required);
                        this.form.get('controlCardDateOfLastAttestationControl')!.setValidators(Validators.required);

                        this.form.get('controlCardValidityCertificateNumControl')!.markAsPending({ emitEvent: false });
                        this.form.get('controlCardValidityCertificateDateControl')!.markAsPending({ emitEvent: false });
                        this.form.get('controlCardDateOfLastAttestationControl')!.markAsPending({ emitEvent: false });
                    }
                    else {
                        this.form.get('controlCardValidityCertificateNumControl')!.clearValidators();
                        this.form.get('controlCardValidityCertificateDateControl')!.clearValidators();
                        this.form.get('controlCardDateOfLastAttestationControl')!.clearValidators();
                    }

                    this.form.get('controlCardNumControl')!.updateValueAndValidity({ emitEvent: false });
                    this.form.get('controlCardDateControl')!.updateValueAndValidity({ emitEvent: false });
                    this.form.get('controlCardValidityCertificateNumControl')!.updateValueAndValidity({ emitEvent: false });
                    this.form.get('controlCardValidityCertificateDateControl')!.updateValueAndValidity({ emitEvent: false });
                    this.form.get('controlCardDateOfLastAttestationControl')!.updateValueAndValidity({ emitEvent: false });

                    if (this.isReadonly || this.viewMode) {
                        this.form.get('controlCardNumControl')!.disable({ emitEvent: false });
                        this.form.get('controlCardDateControl')!.disable({ emitEvent: false });
                        this.form.get('controlCardValidityCertificateNumControl')!.disable({ emitEvent: false });
                        this.form.get('controlCardValidityCertificateDateControl')!.disable({ emitEvent: false });
                        this.form.get('controlCardDateOfLastAttestationControl')!.disable({ emitEvent: false });
                    }
                }
            });

            this.form.get('importCountryControl')?.valueChanges.subscribe({
                next: (country: NomenclatureDTO<number> | undefined) => {
                    if ((country === undefined || country === null) && !(this.isRegisterEntry || this.isAddingRegisterEntry)) {
                        this.form.get('cfrControl')!.setValidators([Validators.required, Validators.maxLength(20)]);
                    }
                    else {
                        this.form.get('cfrControl')!.setValidators([Validators.required, TLValidators.cfr]);
                    }
                    this.form.get('cfrControl')!.updateValueAndValidity();
                }
            });
        }
    }

    public ngOnChanges(changes: SimpleChanges): void {
        if (this.model !== undefined && this.model !== null) {
            const disableFieldsByEventType: SimpleChange | undefined = changes['disableFieldsByEventType'];

            if (disableFieldsByEventType !== undefined) {
                setTimeout(() => {
                    this.disableFieldsByEvent();
                });

                const controls: string[] = [
                    'importCountryControl', 'exportCountryControl', 'exportTypeControl', 'destructionTypeControl',
                    'cancellationReasonControl', 'cancellationOrderNumControl', 'cancellationOrderDateControl'
                ];

                const required = new Map<string, ValidatorFn | ValidatorFn[]>();

                switch (this.disableFieldsByEventType) {
                    case ShipEventTypeEnum.IMP:
                        required.set('importCountryControl', Validators.required);
                        break;
                    case ShipEventTypeEnum.EXP:
                        required.set('exportCountryControl', Validators.required);
                        required.set('exportTypeControl', Validators.required);
                        break;
                    case ShipEventTypeEnum.DES:
                        required.set('destructionTypeControl', Validators.required);
                        break;
                    case ShipEventTypeEnum.RET:
                        required.set('cancellationReasonControl', Validators.required);
                        required.set('cancellationOrderNumControl', [Validators.required, Validators.maxLength(200)]);
                        required.set('cancellationOrderDateControl', Validators.required);
                        break;
                }

                for (const [control, validators] of required) {
                    this.form.get(control)!.setValidators(validators);
                    this.form.get(control)!.markAsPending({ emitEvent: false });
                }

                for (const control of controls.filter(control => !Array.from(required.keys()).includes(control))) {
                    this.form.get(control)!.clearValidators();
                    this.form.get(control)!.markAsPending({ emitEvent: false });
                }
            }
        }
    }

    public ngOnDestroy(): void {
        this.actionsSub?.unsubscribe();
    }

    public writeValue(value: ShipRegisterEditDTO | number | null): void {
        if (value !== null && value !== undefined) {
            this.loader.load(() => {
                this.setValue(value);
            });
        }
    }

    public setData(data: EditShipDialogParams, buttons: DialogWrapperData): void {
        this.isDialog = true;
        this.id = data.id;
        this.isThirdPartyShip = data.isThirdPartyShip ?? false;
        this.applicationId = data.applicationId;
        this.applicationsService = data.applicationsService;
        this.isApplication = data.isApplication;
        this.isReadonly = data.isReadonly;
        this.isApplicationHistoryMode = data.isApplicationHistoryMode;
        this.viewMode = data.viewMode;
        this.showOnlyRegiXData = data.showOnlyRegiXData;
        this.showRegiXData = data.showRegiXData;
        this.service = data.service as IShipsRegisterService;
        this.dialogRightSideActions = buttons.rightSideActions;
        this.loadRegisterFromApplication = data.loadRegisterFromApplication;
        this.pageCode = data.pageCode ?? PageCodeEnum.RegVessel;

        this.setPermissions();
        this.buildForm();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.form.markAllAsTouched();
        this.ownersTouched = true;
        this.validityCheckerGroup!.validate();

        if (this.form.valid) {
            this.saveShip(dialogClose);
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        let applicationAction: boolean = false;

        if (this.model instanceof ShipRegisterApplicationEditDTO || this.model instanceof ShipRegisterRegixDataDTO) {
            this.model = this.getValue()!;
            CommonUtils.sanitizeModelStrings(this.model);

            applicationAction = ApplicationUtils.applicationDialogButtonClicked(new ApplicationDialogData({
                action: actionInfo,
                dialogClose: dialogClose,
                applicationId: this.applicationId,
                model: this.model,
                readOnly: this.isReadonly,
                viewMode: this.viewMode,
                editForm: this.form,
                saveFn: this.saveShip.bind(this),
                onMarkAsTouched: () => {
                    this.ownersTouched = true;
                    this.validityCheckerGroup!.validate();
                }
            }));
        }

        if (!this.isReadonly && !this.viewMode && !applicationAction) {
            this.form.markAllAsTouched();
            this.validityCheckerGroup!.validate();

            if (this.form.valid) {
                if (actionInfo.id === 'save') {
                    return this.saveBtnClicked(actionInfo, dialogClose);
                }
            }
        }
    }

    public runValidityChecker(): void {
        this.validityCheckerGroup!.validate();
    }

    public addEditShipOwner(owner?: ShipOwnerDTO | ShipOwnerRegixDataDTO, viewMode: boolean = false): void {
        const readOnly: boolean = (this.isReadonly || viewMode) && !this.allowOwnersEdit;

        let data: EditShipOwnerDialogParams | undefined;
        let headerAuditBtn: IHeaderAuditButton | undefined;
        let title: string;

        if (owner !== undefined) {
            data = new EditShipOwnerDialogParams({
                model: owner,
                isApplication: this.isApplication,
                isEgnLncReadOnly: this.isEditing,
                expectedResults: this.expectedResults.owners
                    ?.find(x => owner?.isOwnerPerson
                        ? (x.regixPersonData?.egnLnc?.identifierType === owner.regixPersonData?.egnLnc?.identifierType
                            && x.regixPersonData?.egnLnc?.egnLnc === owner.regixPersonData?.egnLnc?.egnLnc)
                        : x.regixLegalData?.eik === owner?.regixLegalData?.eik)
                    ?? new ShipOwnerRegixDataDTO(),
                readOnly: readOnly,
                isDraft: this.isDraftMode(),
                showOnlyRegiXData: this.showOnlyRegiXData,
                showRegiXData: this.showRegiXData,
                isThirdPartyShip: this.isThirdPartyShip,
                submittedFor: this.getSubmittedForAsOwner()
            });

            if (owner.id !== undefined && !this.isPublicApp) {
                headerAuditBtn = {
                    id: owner.id,
                    getAuditRecordData: this.service.getShipOwnerAudit.bind(this.service),
                    tableName: 'ShipOwner'
                };
            }

            if (readOnly) {
                title = this.translate.getValue('ships-register.view-ship-owner-dialog-title');
            }
            else {
                title = this.translate.getValue('ships-register.edit-ship-owner-dialog-title');
            }
        }
        else {
            data = new EditShipOwnerDialogParams({
                isApplication: this.isApplication,
                isDraft: this.isDraftMode(),
                showOnlyRegiXData: this.showOnlyRegiXData,
                isThirdPartyShip: this.isThirdPartyShip,
                submittedFor: this.getSubmittedForAsOwner()
            });

            title = this.translate.getValue('ships-register.add-ship-owner-dialog-title');
        }

        const dialog = this.editOwnerDialog.openWithTwoButtons({
            title: title,
            TCtor: EditShipOwnerComponent,
            headerAuditButton: headerAuditBtn,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditOwnerDialogBtnClicked.bind(this)
            },
            componentData: data,
            translteService: this.translate,
            disableDialogClose: true,
            viewMode: readOnly
        }, '1200px');

        dialog.subscribe((result: EditShipOwnerDialogResult) => {
            if (result && result.owner) {
                result.owner.hasRegixDataDiscrepancy = !this.ownerEqualsRegixOwner(result.owner);

                if (owner !== undefined) {
                    owner = result.owner;
                }
                else {
                    this.shipOwners.push(result.owner);
                }

                if (result.isTouched) {
                    ApplicationUtils.enableOrDisableRegixCheckButtons(this.form, this.dialogRightSideActions);
                }

                this.shipOwners = this.shipOwners.slice();
                this.ownersTouched = true;
                this.form.updateValueAndValidity();

                if (this.ngControl) {
                    this.onChanged(this.getValue());
                }
            }
        });
    }

    public deleteShipOwner(owner: GridRow<ShipOwnerDTO>): void {
        this.confirmDialog.open({
            title: this.translate.getValue('ships-register.delete-owner-dialog-label'),
            message: this.translate.getValue('ships-register.confirm-delete-owner-message'),
            okBtnLabel: this.translate.getValue('ships-register.delete-owner-btn-label')
        }).subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.ownersTable.softDelete(owner);
                    this.ownersTouched = true;
                    this.form.updateValueAndValidity();

                    if (this.ngControl) {
                        this.onChanged(this.getValue());
                    }
                }
            }
        });
    }

    public undoDeleteShipOwner(owner: GridRow<ShipOwnerDTO>): void {
        this.confirmDialog.open().subscribe({
            next: (ok: boolean) => {
                if (ok) {
                    this.ownersTable.softUndoDelete(owner);
                    this.ownersTouched = true;
                    this.form.updateValueAndValidity();

                    if (this.ngControl) {
                        this.onChanged(this.getValue());
                    }
                }
            }
        });
    }

    public getControlErrorLabelText(controlName: string, errorValue: unknown, errorCode: string): TLError | undefined {
        const result: TLError | undefined = CommonUtils.getControlErrorLabelTextForRegixExpectedValueValidator(controlName, errorValue, errorCode);
        if (result !== undefined) {
            return result;
        }

        if (errorCode === 'cfr') {
            if (errorValue === true) {
                return new TLError({ text: this.translate.getValue('ships-register.invalid-cfr'), type: 'error' });
            }
        }
        return undefined;
    }

    public onCfrFocusout(): void {
        if (this.isRegisterEntry || this.isAddingRegisterEntry) {
            const value: string = this.form.get('cfrControl')!.value ?? '';
            this.form.get('cfrControl')!.setValue(value.toUpperCase());
        }
    }

    public onAcquiredGrossTonnageChanged(acquiredTonnage: number | undefined): void {
        const tonnage: number = this.form.get('grossTonnageControl')!.value ?? 0;

        acquiredTonnage ??= 0;
        this.remainingTonnage = CommonUtils.round(acquiredTonnage - tonnage, 2)!;
        this.remainingTonnage = this.remainingTonnage < 0 ? 0 : this.remainingTonnage;

        const actions: FishingCapacityFreedActionsDTO = this.form.get('actionsControl')?.value;
        this.willIssueCapacityCertificates =
            actions !== undefined && actions !== null
            && actions.action !== FishingCapacityRemainderActionEnum.NoCertificate
            && (this.remainingTonnage > 0 || this.remainingPower > 0);
    }

    public onAcquiredPowerChanged(acquiredPower: number | undefined): void {
        let power: number = 0;
        const powerValue: string | undefined = this.form.get('mainEnginePowerControl')!.value;

        if (powerValue !== null && powerValue !== undefined) {
            power = Number(powerValue);
        }

        acquiredPower ??= 0;
        this.remainingPower = CommonUtils.round(acquiredPower - power, 2)!;
        this.remainingPower = this.remainingPower < 0 ? 0 : this.remainingPower;

        const actions: FishingCapacityFreedActionsDTO = this.form.get('actionsControl')?.value;
        this.willIssueCapacityCertificates =
            actions !== undefined && actions !== null
            && actions.action !== FishingCapacityRemainderActionEnum.NoCertificate
            && (this.remainingTonnage > 0 || this.remainingPower > 0);
    }

    public onNewCertificateDataChanged(newCertificateData: NewCertificateData): void {
        this.newCertificateData = newCertificateData;
    }

    public fileTypeFilterFn(options: PermittedFileTypeDTO[]): PermittedFileTypeDTO[] {
        const pdfs: FileTypeEnum[] = [FileTypeEnum.SIGNEDAPPL, FileTypeEnum.APPLICATION_PDF];
        const offlines: FileTypeEnum[] = [FileTypeEnum.PAYEDFEE, FileTypeEnum.SCANNED_FORM];

        let result: PermittedFileTypeDTO[] = options;

        if (this.isApplication || !this.isOnlineApplication) {
            result = result.filter(x => !pdfs.includes(FileTypeEnum[x.code as keyof typeof FileTypeEnum]));
        }

        if (this.isOnlineApplication) {
            result = result.filter(x => !offlines.includes(FileTypeEnum[x.code as keyof typeof FileTypeEnum]));
        }

        const fleet: FleetTypeNomenclatureDTO | undefined = this.form.get('fleetTypeControl')!.value;

        if (fleet !== undefined && fleet !== null) {
            if (!fleet.hasControlCard) {
                result = result.filter(x => FileTypeEnum.CONTROL_CARD !== FileTypeEnum[x.code as keyof typeof FileTypeEnum]);
            }

            if (!fleet.hasFitnessCertificate) {
                result = result.filter(x => FileTypeEnum.CONTROL_CARD_VALIDITY !== FileTypeEnum[x.code as keyof typeof FileTypeEnum]);
            }
        }
        else {
            const types: FileTypeEnum[] = [FileTypeEnum.CONTROL_CARD, FileTypeEnum.CONTROL_CARD_VALIDITY];
            result = result.filter(x => !types.includes(FileTypeEnum[x.code as keyof typeof FileTypeEnum]));
        }

        return result;
    }

    protected buildForm(): AbstractControl {
        const form: FormGroup = new FormGroup({});

        this.addApplicationFormControls(form);
        this.addBasicInfoFormControls(form);
        this.addRegistrationDataFormControls(form);
        this.addTechnicalDataFormControls(form);
        this.addOtherDataFormControls(form);
        this.addCancellationFormControls(form);

        return form;
    }

    protected getValue(): ShipRegisterEditDTO | ShipRegisterRegixDataDTO | ShipRegisterApplicationEditDTO | null {
        if (this.model === undefined || this.model === null) {
            return null;
        }

        const model: ShipRegisterEditDTO | ShipRegisterRegixDataDTO | ShipRegisterApplicationEditDTO = Object.create(this.model);

        this.fillModelApplicationData(model);
        this.fillModelBasicInfo(model);
        this.fillModelRegistrationData(model);
        this.fillModelTechnicalData(model);
        this.fillModelOtherData(model);
        this.fillModelCancellationData(model);

        model.owners = this.shipOwners;
        return model;
    }

    private loadData(): void {
        if (this.isThirdPartyShip) {
            this.model = new ShipRegisterEditDTO({
                isThirdPartyShip: true
            });
        }

        // извличане на исторически данни за заявление
        if (this.isApplicationHistoryMode && this.applicationId !== undefined) {
            this.form.disable({ emitEvent: false });

            if (this.applicationsService) {
                this.applicationsService.getApplicationChangeHistoryContent(this.applicationId).subscribe({
                    next: (content: ApplicationContentDTO) => {
                        const contentObject: Record<string, unknown> = JSON.parse(content.draftContent!);
                        const ship: ShipRegisterApplicationEditDTO = new ShipRegisterApplicationEditDTO(contentObject);
                        ship.files = content.files;
                        ship.applicationId = content.applicationId;

                        this.isPaid = ship.isPaid!;
                        this.hasDelivery = ship.hasDelivery!;
                        this.hideBasicPaymentInfo = this.shouldHidePaymentData();
                        this.isOnlineApplication = ship.isOnlineApplication!;
                        this.refreshFileTypes.next();

                        this.model = ship;
                        this.fillForm();
                    }
                });
            }
        }
        else if (this.applicationId !== undefined && this.id === undefined && !this.isApplication) {
            // извличане на данни за регистров запис от id на заявление
            if (this.loadRegisterFromApplication === true) {
                this.isEditing = true;

                if (this.isReadonly || this.viewMode) {
                    this.form.disable({ emitEvent: false });
                }

                this.service.getRegisterByApplicationId(this.applicationId, this.pageCode).subscribe({
                    next: (ship: unknown) => {
                        this.model = ship as ShipRegisterEditDTO;
                        this.refreshFileTypes.next();
                        this.fillForm();
                    }
                })
            }
            else {
                // извличане на данни за създаване на регистров запис от заявление
                this.isEditing = false;
                this.isAddingRegisterEntry = true;

                this.service.getApplicationDataForRegister(this.applicationId, this.pageCode).subscribe({
                    next: (ship: ShipRegisterEditDTO) => {
                        this.model = ship;
                        this.refreshFileTypes.next();
                        this.setValidators(this.form);
                        this.fillForm();
                    }
                });
            }
        }
        else {
            if (this.isReadonly || this.viewMode) {
                this.form.disable({ emitEvent: false });
            }

            if (this.isApplication && this.applicationId !== undefined) {
                // извличане на данни за RegiX сверка от служител
                this.isEditing = false;

                if (this.showOnlyRegiXData) {
                    this.service.getRegixData(this.applicationId, this.pageCode).subscribe({
                        next: (regixData: RegixChecksWrapperDTO<ShipRegisterRegixDataDTO>) => {
                            this.model = new ShipRegisterRegixDataDTO(regixData.dialogDataModel);
                            this.expectedResults = new ShipRegisterRegixDataDTO(regixData.regiXDataModel);

                            for (const owner of this.model.owners as ShipOwnerRegixDataDTO[]) {
                                owner.hasRegixDataDiscrepancy = !this.ownerEqualsRegixOwner(owner);
                            }

                            this.setValidators(this.form);
                            this.fillForm();
                        }
                    });
                }
                else {
                    // извличане на данни за заявление
                    this.isEditing = false;

                    this.service.getApplication(this.applicationId, this.showRegiXData, this.pageCode).subscribe({
                        next: (ship: ShipRegisterApplicationEditDTO) => {
                            ship.applicationId = this.applicationId;

                            this.isOnlineApplication = ship.isOnlineApplication!;
                            this.isPaid = ship.isPaid!;
                            this.hasDelivery = ship.hasDelivery!;
                            this.hideBasicPaymentInfo = this.shouldHidePaymentData();
                            this.refreshFileTypes.next();

                            if (this.showRegiXData) {
                                this.expectedResults = new ShipRegisterRegixDataDTO(ship.regiXDataModel);
                                ship.regiXDataModel = undefined;

                                for (const owner of ship.owners as ShipOwnerRegixDataDTO[]) {
                                    owner.hasRegixDataDiscrepancy = !this.ownerEqualsRegixOwner(owner);
                                }
                            }

                            if (this.isPublicApp && this.isOnlineApplication && (ship.submittedBy === undefined || ship.submittedBy === null)) {
                                const service = this.service as ShipsRegisterPublicService;
                                service.getCurrentUserAsSubmittedBy().subscribe({
                                    next: (submittedBy: ApplicationSubmittedByDTO) => {
                                        ship.submittedBy = submittedBy;
                                        this.model = ship;
                                        this.isDraft = this.isDraftMode();

                                        this.fillForm();
                                    }
                                });
                            }
                            else {
                                this.model = ship;
                                this.isDraft = this.isDraftMode();

                                this.fillForm();
                            }
                        }
                    });
                }
            }
            else if (this.id !== undefined) {
                // извличане на данни за регистров запис
                this.isEditing = true;
                this.isRegisterEntry = true;

                this.service.getShip(this.id).subscribe({
                    next: (ship: ShipRegisterEditDTO) => {
                        this.model = ship;
                        this.refreshFileTypes.next();

                        this.fillForm();

                        this.shipRegisterLoaded.emit(this.model);
                    }
                });
            }
        }
    }

    private setValue(value: ShipRegisterEditDTO | number): void {
        this.isRegisterEntry = true;

        if (typeof value === 'number') {
            if (value !== undefined) {
                this.service.getShip(value).subscribe({
                    next: (ship: ShipRegisterEditDTO) => {
                        this.model = ship;
                        this.fillForm();
                        this.shipRegisterLoaded.emit(this.model);
                    }
                });
            }
        }
        else {
            this.model = value;
            this.fillForm();
        }
    }

    private closeEditOwnerDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private setPermissions(): void {
        this.canRestoreRecords = this.isApplication
            ? this.permissions.has(PermissionsEnum.ShipsRegisterApplicationsRestoreRecords)
            : this.permissions.has(PermissionsEnum.ShipsRegisterRestoreRecords);

        this.canDeleteRecords = this.isApplication
            ? this.permissions.has(PermissionsEnum.ShipsRegisterApplicationsDeleteRecords)
            : this.permissions.has(PermissionsEnum.ShipsRegisterDeleteRecords);
    }

    private addApplicationFormControls(form: FormGroup): void {
        form.addControl('submittedByControl', new FormControl(null));
        form.addControl('submittedForControl', new FormControl(null));
    }

    private addBasicInfoFormControls(form: FormGroup): void {
        form.addControl('cfrControl', new FormControl(null));
        form.addControl('shipNameControl', new FormControl(null));
        form.addControl('vesselTypeControl', new FormControl(null));
        form.addControl('externalMarkControl', new FormControl(null));
        form.addControl('regDateControl', new FormControl(null));
        form.addControl('fleetTypeControl', new FormControl(null));
        form.addControl('ircsCallSignControl', new FormControl(null));
        form.addControl('mmsiControl', new FormControl(null));
        form.addControl('flagControl', new FormControl(null));
        form.addControl('uviControl', new FormControl(null));
        form.addControl('aisControl', new FormControl(false));
        form.addControl('ersControl', new FormControl(false));
        form.addControl('vmsControl', new FormControl(false));
        form.addControl('regNumberControl', new FormControl(null));

        form.addControl('hasERSExceptionControl', new FormControl(false));
        form.addControl('forbiddenForRSRControl', new FormControl(false));
        form.addControl('forbiddenReasonControl', new FormControl(null));
        form.addControl('forbiddenStartDateControl', new FormControl(this.today));
        form.addControl('forbiddenEndDateControl', new FormControl(DateUtils.MAX_DATE));
    }

    private addRegistrationDataFormControls(form: FormGroup): void {
        form.addControl('regLicenceNumControl', new FormControl(null));
        form.addControl('regLicenceDateControl', new FormControl(null));
        form.addControl('regLicencePublisherControl', new FormControl(null));
        form.addControl('regLicencePublishVolumeControl', new FormControl(null));
        form.addControl('regLicencePublishNumControl', new FormControl(null));
        form.addControl('regLicencePublishPageControl', new FormControl(null));

        form.addControl('exploitationStartDateControl', new FormControl(null));
        form.addControl('buildYearControl', new FormControl(null));
        form.addControl('buildPlaceControl', new FormControl(null));

        form.addControl('adminDecisionNumControl', new FormControl(null));
        form.addControl('adminDecisionDateControl', new FormControl(null));
        form.addControl('publicAidCodeControl', new FormControl(null));

        form.addControl('portControl', new FormControl(null));
        form.addControl('stayPortControl', new FormControl(null));
        form.addControl('sailAreaControl', new FormControl(null));

    }

    private addTechnicalDataFormControls(form: FormGroup): void {
        form.addControl('totalLengthControl', new FormControl(null));
        form.addControl('totalWidthControl', new FormControl(null));
        form.addControl('grossTonnageControl', new FormControl(null));
        form.addControl('netTonnageControl', new FormControl(null));
        form.addControl('otherTonnageControl', new FormControl(null));
        form.addControl('mainEnginePowerControl', new FormControl(null));
        form.addControl('auxiliaryEnginePowerControl', new FormControl(null));
        form.addControl('mainEngineNumControl', new FormControl(null));
        form.addControl('mainEngineModelControl', new FormControl(null));
        form.addControl('mainFishingGearControl', new FormControl(null));
        form.addControl('additionalFishingGearControl', new FormControl(null));
        form.addControl('boardHeightControl', new FormControl(null));
        form.addControl('draughtControl', new FormControl(null));
        form.addControl('lengthBetweenPerpendicularsControl', new FormControl(null));
        form.addControl('fuelTypeControl', new FormControl(null));
        form.addControl('hullMaterialControl', new FormControl(null));
        form.addControl('totalPassengerCapacityControl', new FormControl(null));
        form.addControl('crewCountControl', new FormControl(null));
        form.addControl('fleetSegmentControl', new FormControl(null));
    }

    private addOtherDataFormControls(form: FormGroup): void {
        form.addControl('controlCardNumControl', new FormControl(null));
        form.addControl('controlCardDateControl', new FormControl(null));
        form.addControl('controlCardValidityCertificateNumControl', new FormControl(null));
        form.addControl('controlCardValidityCertificateDateControl', new FormControl(null));
        form.addControl('controlCardDateOfLastAttestationControl', new FormControl(null));

        form.addControl('importCountryControl', new FormControl(null));

        form.addControl('foodLawLicenceNumControl', new FormControl(null));
        form.addControl('foodLawLicenceDateControl', new FormControl(null));

        form.addControl('shipAssociationControl', new FormControl(null));

        form.addControl('commentsControl', new FormControl(null));

        form.addControl('deliveryDataControl', new FormControl(null));
        form.addControl('applicationPaymentInformationControl', new FormControl());
        form.addControl('filesControl', new FormControl(null));

        form.addControl('acquiredCapacityControl', new FormControl(null));
        form.addControl('actionsControl', new FormControl(null));

        form.addControl('exportCountryControl', new FormControl(null));
        form.addControl('exportTypeControl', new FormControl(null));
    }

    private addCancellationFormControls(form: FormGroup): void {
        form.addControl('destructionTypeControl', new FormControl(null));
        form.addControl('cancellationReasonControl', new FormControl(null));
        form.addControl('cancellationOrderNumControl', new FormControl(null));
        form.addControl('cancellationOrderDateControl', new FormControl(null));
    }

    private setValidators(form: FormGroup): void {
        // owners
        if (!this.showOnlyRegiXData) {
            form.setValidators(this.validateOwners());
        }

        // basic data
        if (!this.showOnlyRegiXData) {
            form.get('externalMarkControl')!.setValidators([Validators.required, Validators.maxLength(50)]);
            form.get('regDateControl')!.setValidators(Validators.required);
            form.get('fleetTypeControl')!.setValidators(Validators.required);
            form.get('ircsCallSignControl')!.setValidators(Validators.maxLength(7));
            form.get('flagControl')!.setValidators(Validators.required);
            form.get('mmsiControl')!.setValidators([TLValidators.exactLength(9), TLValidators.number(0)]);
            form.get('uviControl')!.setValidators([TLValidators.exactLength(7), TLValidators.number(0)]);
            form.get('regNumberControl')!.setValidators([Validators.required, Validators.maxLength(14)]);
        }

        if (this.isRegisterEntry || this.isAddingRegisterEntry) {
            form.get('cfrControl')!.setValidators([Validators.required, TLValidators.cfr]);
        }
        else {
            form.get('cfrControl')!.setValidators([Validators.required, Validators.maxLength(20)]);
        }
        form.get('shipNameControl')!.setValidators([Validators.required, Validators.maxLength(500), TLValidators.expectedValueMatch(this.expectedResults.name)]);
        form.get('vesselTypeControl')!.setValidators(TLValidators.expectedValueMatch(this.expectedResults.vesselTypeName));

        // registration data
        if (!this.showOnlyRegiXData) {
            form.get('regLicenceDateControl')!.setValidators(Validators.required);
            form.get('regLicencePublisherControl')!.setValidators(Validators.maxLength(50));
            form.get('regLicencePublishNumControl')!.setValidators(Validators.maxLength(10));

            form.get('buildYearControl')!.setValidators(Validators.required);
            form.get('buildPlaceControl')!.setValidators([Validators.required, Validators.maxLength(100)]);

            form.get('adminDecisionNumControl')!.setValidators(Validators.maxLength(20));
            form.get('publicAidCodeControl')!.setValidators(Validators.required);

            form.get('portControl')!.setValidators(Validators.required);
        }

        form.get('regLicenceNumControl')!.setValidators([Validators.required, Validators.maxLength(20), TLValidators.expectedValueMatch(this.expectedResults.regLicenceNum)]);
        form.get('regLicencePublishVolumeControl')!.setValidators([Validators.maxLength(10), TLValidators.expectedValueMatch(this.expectedResults.regLicencePublishVolume)]);
        form.get('regLicencePublishPageControl')!.setValidators([Validators.maxLength(10), TLValidators.expectedValueMatch(this.expectedResults.regLicencePublishPage)]);

        // technical data
        if (!this.showOnlyRegiXData) {
            form.get('otherTonnageControl')!.setValidators(TLValidators.number(0, undefined, 2));
            form.get('mainEnginePowerControl')!.setValidators([TLValidators.number(0, undefined, 2), Validators.required]);
            form.get('auxiliaryEnginePowerControl')!.setValidators(TLValidators.number(0, undefined, 2));
            form.get('mainEngineNumControl')!.setValidators(Validators.maxLength(20));
            form.get('mainEngineModelControl')!.setValidators(Validators.maxLength(100));
            form.get('mainFishingGearControl')!.setValidators(Validators.required);
            form.get('additionalFishingGearControl')!.setValidators(Validators.required);
            form.get('hullMaterialControl')!.setValidators(Validators.required);
            form.get('fleetSegmentControl')!.setValidators(Validators.required);
            form.get('totalPassengerCapacityControl')!.setValidators([TLValidators.number(0), Validators.required]);
            form.get('crewCountControl')!.setValidators([Validators.required, Validators.maxLength(50), TLValidators.number(0)]);
        }

        form.get('totalLengthControl')!.setValidators([Validators.required, TLValidators.number(0), TLValidators.expectedValueMatch(this.expectedResults.totalLength)]);
        form.get('totalWidthControl')!.setValidators([Validators.required, TLValidators.number(0), TLValidators.expectedValueMatch(this.expectedResults.totalWidth)]);
        form.get('grossTonnageControl')!.setValidators([Validators.required, TLValidators.number(0, undefined, 2), TLValidators.expectedValueMatch(this.expectedResults.grossTonnage)]);
        form.get('netTonnageControl')!.setValidators([TLValidators.number(0, undefined, 2), TLValidators.expectedValueMatch(this.expectedResults.netTonnage)]);
        form.get('boardHeightControl')!.setValidators([Validators.required, TLValidators.number(0), TLValidators.expectedValueMatch(this.expectedResults.boardHeight)]);
        form.get('draughtControl')!.setValidators([Validators.required, TLValidators.number(0), TLValidators.expectedValueMatch(this.expectedResults.shipDraught)]);
        form.get('lengthBetweenPerpendicularsControl')!.setValidators([TLValidators.number(0), TLValidators.expectedValueMatch(this.expectedResults.lengthBetweenPerpendiculars)]);
        form.get('fuelTypeControl')!.setValidators([Validators.required, Validators.maxLength(50), TLValidators.expectedValueMatch(this.expectedResults.fuelTypeName)]);

        // other data
        if (!this.showOnlyRegiXData) {
            form.get('foodLawLicenceNumControl')!.setValidators([Validators.required, Validators.maxLength(50)]);
            form.get('foodLawLicenceDateControl')!.setValidators(Validators.required);
            form.get('commentsControl')!.setValidators(Validators.maxLength(4000));
        }
    }

    private fillForm(): void {
        this.fillFormApplicationData();
        this.fillFormBasicInfo();
        this.fillFormRegistrationData();
        this.fillFormTechnicalData();
        this.fillFormOtherData();
        this.fillCancellationFormData();

        if (this.model.owners !== undefined && this.model.owners !== null) {
            const owners: ShipOwnerDTO[] | ShipOwnerRegixDataDTO[] = this.model.owners;

            setTimeout(() => {
                this.shipOwners = owners;
                this.updateShipOwnersNameAndEgnLncEik();
                this.form.updateValueAndValidity();
            });
        }

        if (this.model instanceof ShipRegisterRegixDataDTO) {
            this.fillFormRegiX(this.model);
        }
        else if (this.model instanceof ShipRegisterEditDTO) {
            if (this.model.shipUsers !== undefined && this.model.shipUsers !== null) {
                const shipUsers: ShipRegisterUserDTO[] = this.model.shipUsers;

                setTimeout(() => {
                    this.shipUsers = shipUsers;
                });
            }

            if (this.model.usedCapacityCertificates !== undefined && this.model.usedCapacityCertificates !== null) {
                const usedCertificates: ShipRegisterUsedCertificateDTO[] = this.model.usedCapacityCertificates;

                setTimeout(() => {
                    this.usedCapacityCertificates = usedCertificates;
                });
            }
        }
        else {
            if (this.showRegiXData) {
                this.fillFormRegiX(this.model);
            }
        }
    }

    private fillFormRegiX(model: ShipRegisterApplicationEditDTO | ShipRegisterRegixDataDTO): void {
        if (model.applicationRegiXChecks !== undefined && model.applicationRegiXChecks !== null) {
            const applicationRegiXChecks: ApplicationRegiXCheckDTO[] = model.applicationRegiXChecks;

            setTimeout(() => {
                this.regixChecks = applicationRegiXChecks;
            });

            model.applicationRegiXChecks = undefined;
        }

        if (!this.viewMode) {
            this.notifier.start();
            this.notifier.onNotify.subscribe({
                next: () => {
                    this.form.markAllAsTouched();

                    if (this.showOnlyRegiXData) {
                        ApplicationUtils.enableOrDisableRegixCheckButtons(this.form, this.dialogRightSideActions);
                    }

                    this.notifier.stop();
                }
            });
        }
    }

    private fillFormApplicationData(): void {
        if (!(this.model instanceof ShipRegisterEditDTO)) {
            this.form.get('submittedByControl')!.setValue(this.model.submittedBy);
            this.form.get('submittedForControl')!.setValue(this.model.submittedFor);
        }
    }

    private fillFormBasicInfo(): void {
        if (!(this.model instanceof ShipRegisterRegixDataDTO)) {
            this.form.get('externalMarkControl')!.setValue(this.model.externalMark);
            this.form.get('regNumberControl')!.setValue(this.model.registrationNumber);
            this.form.get('regDateControl')!.setValue(this.model.registrationDate);
            this.form.get('ircsCallSignControl')!.setValue(this.model.ircsCallSign);
            this.form.get('mmsiControl')!.setValue(this.model.mmsi);
            this.form.get('uviControl')!.setValue(this.model.uvi);

            const fleetTypeId: number | undefined = this.model.fleetTypeId;
            if (fleetTypeId !== undefined && fleetTypeId !== null) {
                this.form.get('fleetTypeControl')!.setValue(this.fleets.find(x => x.value === fleetTypeId));
            }
            else {
                this.form.get('fleetTypeControl')!.setValue(undefined);
            }

            const countryFlagId: number | undefined = this.model.countryFlagId;
            if (countryFlagId !== undefined && countryFlagId !== null) {
                this.form.get('flagControl')!.setValue(this.countries.find(x => x.value === countryFlagId));
            }
            else {
                this.form.get('flagControl')!.setValue(undefined);
            }

            this.form.get('aisControl')!.setValue(this.model.hasAIS ?? false);
            this.form.get('ersControl')!.setValue(this.model.hasERS ?? false);
            this.form.get('vmsControl')!.setValue(this.model.hasVMS ?? false);
        }

        this.form.get('cfrControl')!.setValue(this.model.cfr);
        this.form.get('shipNameControl')!.setValue(this.model.name);

        const vesselTypeId: number | undefined = this.model.vesselTypeId;
        if (vesselTypeId !== undefined && vesselTypeId !== null) {
            this.form.get('vesselTypeControl')!.setValue(this.vesselTypes.find(x => x.value === vesselTypeId));
        }
        else {
            this.form.get('vesselTypeControl')!.setValue(undefined);
        }

        if (this.model instanceof ShipRegisterEditDTO) {
            this.form.get('hasERSExceptionControl')!.setValue(this.model.hasERSException);
            this.form.get('forbiddenForRSRControl')!.setValue(this.model.isForbiddenForRSR, { emitEvent: false });

            if (this.model.isForbiddenForRSR === true) {
                this.form.get('forbiddenReasonControl')!.setValue(this.model.forbiddenReason);
                this.form.get('forbiddenStartDateControl')!.setValue(this.model.forbiddenStartDate);
                this.form.get('forbiddenEndDateControl')!.setValue(this.model.forbiddenEndDate);
            }
            else {
                this.form.get('forbiddenReasonControl')?.setValue(undefined);
                this.form.get('forbiddenStartDateControl')?.setValue(this.today);
                this.form.get('forbiddenEndDateControl')?.setValue(DateUtils.MAX_DATE);
            }
        }
    }

    private fillFormRegistrationData(): void {
        if (!(this.model instanceof ShipRegisterRegixDataDTO)) {
            this.form.get('regLicenceDateControl')!.setValue(this.model.regLicenceDate);
            this.form.get('regLicencePublisherControl')!.setValue(this.model.regLicencePublisher);
            this.form.get('regLicencePublishNumControl')!.setValue(this.model.regLicencePublishNum);

            this.form.get('exploitationStartDateControl')!.setValue(this.model.exploitationStartDate);

            if (this.model.buildYear !== null && this.model.buildYear !== undefined) {
                this.form.get('buildYearControl')!.setValue(new Date(this.model.buildYear, 1));
            }
            else {
                this.form.get('buildYearControl')!.setValue(undefined);
            }

            this.form.get('buildPlaceControl')!.setValue(this.model.buildPlace);

            this.form.get('adminDecisionNumControl')!.setValue(this.model.adminDecisionNum);
            this.form.get('adminDecisionDateControl')!.setValue(this.model.adminDecisionDate);

            const publicAidId: number | undefined = this.model.publicAidTypeId;
            const portId: number | undefined = this.model.portId;
            const stayPortId: number | undefined = this.model.stayPortId;
            const sailAreaId: number | undefined = this.model.sailAreaId;
            const sailAreaName: string | undefined = this.model.sailAreaName;

            if (publicAidId !== undefined && publicAidId !== null) {
                this.form.get('publicAidCodeControl')!.setValue(this.publicAidTypes.find(x => x.value === publicAidId));
            }
            else {
                this.form.get('publicAidCodeControl')!.setValue(undefined);
            }

            if (portId !== undefined && portId !== null) {
                this.form.get('portControl')!.setValue(this.ports.find(x => x.value === portId));
            }
            else {
                this.form.get('portControl')!.setValue(undefined);
            }

            if (stayPortId !== undefined && stayPortId !== null) {
                this.form.get('stayPortControl')!.setValue(this.ports.find(x => x.value === stayPortId));
            }
            else {
                this.form.get('stayPortControl')!.setValue(undefined);
            }

            if (sailAreaId !== undefined && sailAreaId !== null) {
                NomenclatureStore.instance.refreshNomenclature(
                    NomenclatureTypes.SailAreas, this.service.getSailAreas.bind(this.service)
                ).subscribe({
                    next: (sailAreas: SailAreaNomenclatureDTO[]) => {
                        this.sailAreas = sailAreas;
                        this.form.get('sailAreaControl')!.setValue(this.sailAreas.find(x => x.value === sailAreaId));
                    }
                });
            }
            else if (sailAreaName !== undefined && sailAreaName !== null) {
                this.form.get('sailAreaControl')!.setValue(sailAreaName);
            }
            else {
                this.form.get('sailAreaControl')!.setValue(undefined);
            }
        }

        this.form.get('regLicenceNumControl')!.setValue(this.model.regLicenceNum);
        this.form.get('regLicencePublishVolumeControl')!.setValue(this.model.regLicencePublishVolume);
        this.form.get('regLicencePublishPageControl')!.setValue(this.model.regLicencePublishPage);
    }

    private fillFormTechnicalData(): void {
        if (!(this.model instanceof ShipRegisterRegixDataDTO)) {
            this.form.get('otherTonnageControl')!.setValue(this.model.otherTonnage?.toFixed(2));
            this.form.get('mainEnginePowerControl')!.setValue(this.model.mainEnginePower?.toFixed(2));
            this.form.get('auxiliaryEnginePowerControl')!.setValue(this.model.auxiliaryEnginePower?.toFixed(2));
            this.form.get('mainEngineNumControl')!.setValue(this.model.mainEngineNum);
            this.form.get('mainEngineModelControl')!.setValue(this.model.mainEngineModel);

            const mainFishingGearId: number | undefined = this.model.mainFishingGearId;
            const additionalFishingGearId: number | undefined = this.model.additionalFishingGearId;
            const hullMaterialId: number | undefined = this.model.hullMaterialId;
            const fleetSegmentId: number | undefined = this.model.fleetSegmentId;

            if (mainFishingGearId !== undefined && mainFishingGearId !== null) {
                this.form.get('mainFishingGearControl')!.setValue(this.fishingGears.find(x => x.value === mainFishingGearId));
            }
            else {
                this.form.get('mainFishingGearControl')!.setValue(undefined);
            }

            if (additionalFishingGearId !== undefined && additionalFishingGearId !== null) {
                this.form.get('additionalFishingGearControl')!.setValue(this.fishingGears.find(x => x.value === additionalFishingGearId));
            }
            else {
                this.form.get('additionalFishingGearControl')!.setValue(undefined);
            }

            if (hullMaterialId !== undefined && hullMaterialId !== null) {
                this.form.get('hullMaterialControl')!.setValue(this.hullMaterials.find(x => x.value === hullMaterialId));
            }
            else {
                this.form.get('hullMaterialControl')!.setValue(undefined);
            }

            if (fleetSegmentId !== undefined && fleetSegmentId !== null) {
                this.form.get('fleetSegmentControl')!.setValue(this.fleetSegments.find(x => x.value === fleetSegmentId));
            }
            else {
                this.form.get('fleetSegmentControl')!.setValue(undefined);
            }

            this.form.get('totalPassengerCapacityControl')!.setValue(this.model.totalPassengerCapacity);
            this.form.get('crewCountControl')!.setValue(this.model.crewCount);
        }

        this.form.get('totalLengthControl')!.setValue(this.model.totalLength);
        this.form.get('totalWidthControl')!.setValue(this.model.totalWidth);
        this.form.get('grossTonnageControl')!.setValue(this.model.grossTonnage?.toFixed(2));
        this.form.get('netTonnageControl')!.setValue(this.model.netTonnage?.toFixed(2));
        this.form.get('boardHeightControl')!.setValue(this.model.boardHeight);
        this.form.get('draughtControl')!.setValue(this.model.shipDraught);
        this.form.get('lengthBetweenPerpendicularsControl')!.setValue(this.model.lengthBetweenPerpendiculars);
        this.form.get('fuelTypeControl')!.setValue(this.fuelTypes.find(x => x.value === this.model.fuelTypeId));
    }

    private fillFormOtherData(): void {
        if (!(this.model instanceof ShipRegisterRegixDataDTO)) {
            if (this.model.hasControlCard === true) {
                this.form.get('controlCardNumControl')!.setValue(this.model.controlCardNum);
                this.form.get('controlCardDateControl')!.setValue(this.model.controlCardDate);
            }
            else {
                this.form.get('controlCardNumControl')!.setValue(undefined);
                this.form.get('controlCardDateControl')!.setValue(undefined);
            }

            if (this.model.hasValidityCertificate === true) {
                this.form.get('controlCardValidityCertificateNumControl')!.setValue(this.model.controlCardValidityCertificateNum);
                this.form.get('controlCardValidityCertificateDateControl')!.setValue(this.model.controlCardValidityCertificateDate);
                this.form.get('controlCardDateOfLastAttestationControl')!.setValue(this.model.controlCardDateOfLastAttestation);
            }
            else {
                this.form.get('controlCardValidityCertificateNumControl')!.setValue(undefined);
                this.form.get('controlCardValidityCertificateDateControl')!.setValue(undefined);
                this.form.get('controlCardDateOfLastAttestationControl')!.setValue(undefined);
            }

            const importCountryId: number | undefined = this.model.importCountryId;
            if (importCountryId !== undefined && importCountryId !== null) {
                this.hasImportCountry = true;
                this.form.get('importCountryControl')!.setValue(this.countries.find(x => x.value === importCountryId));
            }
            else {
                this.hasImportCountry = false;
                this.form.get('importCountryControl')!.setValue(undefined);
            }

            if (this.model.hasFoodLawLicence === true) {
                this.form.get('foodLawLicenceNumControl')!.setValue(this.model.foodLawLicenceNum);
                this.form.get('foodLawLicenceDateControl')!.setValue(this.model.foodLawLicenceDate);
            }
            else {
                this.form.get('foodLawLicenceNumControl')!.setValue(undefined);
                this.form.get('foodLawLicenceDateControl')!.setValue(undefined);
            }

            if (this.model instanceof ShipRegisterApplicationEditDTO) {
                this.form.get('acquiredCapacityControl')?.setValue(this.model.acquiredFishingCapacity);
                this.form.get('actionsControl')?.setValue(this.model.remainingCapacityAction);

                if (this.hasDelivery === true) {
                    this.form.get('deliveryDataControl')!.setValue(this.model.deliveryData);
                }

                if (this.isPaid === true) {
                    this.form.get('applicationPaymentInformationControl')!.setValue(this.model.paymentInformation);
                }
            }
            else {
                if (this.isAddingRegisterEntry) {
                    this.form.get('acquiredCapacityControl')?.setValue(this.model.acquiredFishingCapacity);
                    this.form.get('actionsControl')?.setValue(this.model.remainingCapacityAction);
                }

                const exportCountryId: number | undefined = this.model.exportCountryId;
                const exportType: ShipExportTypeEnum | undefined = this.model.exportType;

                if (exportCountryId !== undefined && exportCountryId !== null) {
                    this.form.get('exportCountryControl')!.setValue(this.countries.find(x => x.value === exportCountryId));
                }
                else {
                    this.form.get('exportCountryControl')!.setValue(undefined);
                }

                if (exportType !== undefined && exportType !== null) {
                    this.form.get('exportTypeControl')!.setValue(this.exportTypes.find(x => x.value === exportType));
                }
            }

            if (this.model.shipAssociationId !== undefined && this.model.shipAssociationId !== null) {
                const shipAssociationId: number | undefined = this.model.shipAssociationId;
                if (shipAssociationId !== undefined && shipAssociationId !== null) {
                    this.form.get('shipAssociationControl')!.setValue(this.shipAssociations.find(x => x.value === shipAssociationId));
                }
            }
            else {
                this.form.get('shipAssociationControl')!.setValue(undefined);
            }

            this.form.get('commentsControl')!.setValue(this.model.comments);
            this.form.get('filesControl')!.setValue(this.model.files);
        }
        else {
            this.form.get('actionsControl')!.setValue(this.model.remainingCapacityAction);
        }
    }

    private fillCancellationFormData(): void {
        if (this.model instanceof ShipRegisterEditDTO) {
            if (this.isShipDestroyed(this.model)) {
                const reasonId: number | undefined = this.model.cancellationDetails!.reasonId;
                this.form.get('destructionTypeControl')!.setValue(this.destroyReasons.find(x => x.value === reasonId));
            }
            else if (this.isShipCancelled(this.model)) {
                const reasonId: number | undefined = this.model.cancellationDetails!.reasonId;

                this.form.get('cancellationReasonControl')!.setValue(this.cancellationReasons.find(x => x.value === reasonId));
                this.form.get('cancellationOrderNumControl')!.setValue(this.model.cancellationDetails!.issueOrderNum);
                this.form.get('cancellationOrderDateControl')!.setValue(this.model.cancellationDetails!.date);
            }
            else {
                this.form.get('destructionTypeControl')!.setValue(undefined);
                this.form.get('cancellationReasonControl')!.setValue(undefined);
                this.form.get('cancellationOrderNumControl')!.setValue(undefined);
                this.form.get('cancellationOrderDateControl')!.setValue(undefined);
            }
        }
    }

    private fillModelApplicationData(model: ShipRegisterEditDTO | ShipRegisterRegixDataDTO | ShipRegisterApplicationEditDTO): void {
        if (!(model instanceof ShipRegisterEditDTO)) {
            model.submittedBy = this.form.get('submittedByControl')!.value;
            model.submittedFor = this.form.get('submittedForControl')!.value;
        }
    }

    private fillModelBasicInfo(model: ShipRegisterEditDTO | ShipRegisterRegixDataDTO | ShipRegisterApplicationEditDTO): void {
        if (!(model instanceof ShipRegisterRegixDataDTO)) {
            model.externalMark = this.form.get('externalMarkControl')!.value;
            model.registrationNumber = this.form.get('regNumberControl')!.value;
            model.registrationDate = this.form.get('regDateControl')!.value;
            model.ircsCallSign = this.form.get('ircsCallSignControl')!.value;
            model.mmsi = this.form.get('mmsiControl')!.value;
            model.uvi = this.form.get('uviControl')!.value;
            model.fleetTypeId = this.form.get('fleetTypeControl')!.value?.value;

            model.countryFlagId = this.form.get('flagControl')!.value?.value;
            model.hasAIS = this.form.get('aisControl')!.value;
            model.hasERS = this.form.get('ersControl')!.value;
            model.hasVMS = this.form.get('vmsControl')!.value;
        }

        if (model instanceof ShipRegisterEditDTO) {
            model.hasERSException = this.form.get('hasERSExceptionControl')!.value;

            model.isForbiddenForRSR = this.form.get('forbiddenForRSRControl')!.value;
            if (model.isForbiddenForRSR) {
                model.forbiddenReason = this.form.get('forbiddenReasonControl')!.value;
                model.forbiddenStartDate = this.form.get('forbiddenStartDateControl')!.value;
                model.forbiddenEndDate = this.form.get('forbiddenEndDateControl')!.value;
            }
            else {
                model.forbiddenReason = undefined;
                model.forbiddenStartDate = undefined;
                model.forbiddenEndDate = undefined;
            }
        }

        model.cfr = this.form.get('cfrControl')!.value;
        model.name = this.form.get('shipNameControl')!.value;
        model.vesselTypeId = this.form.get('vesselTypeControl')!.value?.value;
    }

    private fillModelRegistrationData(model: ShipRegisterEditDTO | ShipRegisterRegixDataDTO | ShipRegisterApplicationEditDTO): void {
        if (!(model instanceof ShipRegisterRegixDataDTO)) {
            model.regLicenceDate = this.form.get('regLicenceDateControl')!.value;
            model.regLicencePublisher = this.form.get('regLicencePublisherControl')!.value;
            model.regLicencePublishNum = this.form.get('regLicencePublishNumControl')!.value;

            model.exploitationStartDate = this.form.get('exploitationStartDateControl')!.value;
            model.buildYear = (this.form.get('buildYearControl')!.value as Date)?.getFullYear();
            model.buildPlace = this.form.get('buildPlaceControl')!.value;

            model.adminDecisionNum = this.form.get('adminDecisionNumControl')!.value;
            model.adminDecisionDate = this.form.get('adminDecisionDateControl')!.value;
            model.publicAidTypeId = this.form.get('publicAidCodeControl')!.value?.value;

            model.portId = this.form.get('portControl')!.value?.value;
            model.stayPortId = this.form.get('stayPortControl')!.value?.value;

            const sailArea: NomenclatureDTO<number> | string = this.form.get('sailAreaControl')!.value;
            if (sailArea !== undefined && sailArea !== null) {
                if (typeof sailArea === 'string') {
                    model.sailAreaId = undefined;
                    model.sailAreaName = sailArea;
                }
                else {
                    model.sailAreaId = sailArea.value;
                    model.sailAreaName = undefined;
                }
            }
            else {
                model.sailAreaId = undefined;
                model.sailAreaName = undefined;
            }
        }

        model.regLicenceNum = this.form.get('regLicenceNumControl')!.value;
        model.regLicencePublishVolume = this.form.get('regLicencePublishVolumeControl')!.value;
        model.regLicencePublishPage = this.form.get('regLicencePublishPageControl')!.value;
    }

    private fillModelTechnicalData(model: ShipRegisterEditDTO | ShipRegisterRegixDataDTO | ShipRegisterApplicationEditDTO): void {
        if (!(model instanceof ShipRegisterRegixDataDTO)) {
            model.otherTonnage = this.form.get('otherTonnageControl')!.value;
            model.mainEnginePower = this.form.get('mainEnginePowerControl')!.value;
            model.auxiliaryEnginePower = this.form.get('auxiliaryEnginePowerControl')!.value;
            model.mainEngineNum = this.form.get('mainEngineNumControl')!.value;
            model.mainEngineModel = this.form.get('mainEngineModelControl')!.value;
            model.mainFishingGearId = this.form.get('mainFishingGearControl')!.value?.value;
            model.additionalFishingGearId = this.form.get('additionalFishingGearControl')!.value?.value;
            model.hullMaterialId = this.form.get('hullMaterialControl')!.value?.value;
            model.totalPassengerCapacity = this.form.get('totalPassengerCapacityControl')!.value;
            model.crewCount = this.form.get('crewCountControl')!.value;
            model.fleetSegmentId = this.form.get('fleetSegmentControl')!.value?.value;
        }

        model.totalLength = this.form.get('totalLengthControl')!.value;
        model.totalWidth = this.form.get('totalWidthControl')!.value;
        model.grossTonnage = this.form.get('grossTonnageControl')!.value;
        model.netTonnage = this.form.get('netTonnageControl')!.value;
        model.boardHeight = this.form.get('boardHeightControl')!.value;
        model.shipDraught = this.form.get('draughtControl')!.value;
        model.lengthBetweenPerpendiculars = this.form.get('lengthBetweenPerpendicularsControl')!.value;
        model.fuelTypeId = this.form.get('fuelTypeControl')!.value?.value;
    }

    private fillModelOtherData(model: ShipRegisterEditDTO | ShipRegisterRegixDataDTO | ShipRegisterApplicationEditDTO): void {
        if (!(model instanceof ShipRegisterRegixDataDTO)) {
            if (this.hasControlCard) {
                model.hasControlCard = true;
                model.controlCardNum = this.form.get('controlCardNumControl')!.value;
                model.controlCardDate = this.form.get('controlCardDateControl')!.value;
            }
            else {
                model.hasControlCard = false;
                model.controlCardNum = undefined;
                model.controlCardDate = undefined;
            }

            if (this.hasFitnessCertificate) {
                model.hasValidityCertificate = true;
                model.controlCardValidityCertificateNum = this.form.get('controlCardValidityCertificateNumControl')!.value;
                model.controlCardValidityCertificateDate = this.form.get('controlCardValidityCertificateDateControl')!.value;
                model.controlCardDateOfLastAttestation = this.form.get('controlCardDateOfLastAttestationControl')!.value;
            }
            else {
                model.hasValidityCertificate = false;
                model.controlCardValidityCertificateNum = undefined;
                model.controlCardValidityCertificateDate = undefined;
                model.controlCardDateOfLastAttestation = undefined;
            }

            model.importCountryId = this.form.get('importCountryControl')!.value?.value;

            model.foodLawLicenceNum = this.form.get('foodLawLicenceNumControl')!.value;
            model.foodLawLicenceDate = this.form.get('foodLawLicenceDateControl')!.value;
            model.hasFoodLawLicence = model.foodLawLicenceNum !== undefined
                && model.foodLawLicenceNum !== null
                && model.foodLawLicenceDate !== undefined
                && model.foodLawLicenceDate !== null;

            model.shipAssociationId = this.form.get('shipAssociationControl')!.value?.value;

            if (model instanceof ShipRegisterApplicationEditDTO) {
                model.acquiredFishingCapacity = this.form.get('acquiredCapacityControl')?.value;
                model.remainingCapacityAction = this.form.get('actionsControl')?.value;

                if (model.remainingCapacityAction && !this.willIssueCapacityCertificates) {
                    model.remainingCapacityAction!.action = FishingCapacityRemainderActionEnum.NoCertificate;
                }

                if (this.hasDelivery === true && this.willIssueCapacityCertificates) {
                    model.deliveryData = this.form.get('deliveryDataControl')!.value;
                }
                else {
                    model.deliveryData = undefined;
                }

                if (this.isPaid === true) {
                    (this.model as ShipRegisterApplicationEditDTO).paymentInformation = this.form.get('applicationPaymentInformationControl')!.value;
                }
            }
            else {
                if (this.isAddingRegisterEntry) {
                    model.acquiredFishingCapacity = this.form.get('acquiredCapacityControl')?.value;
                    model.remainingCapacityAction = this.form.get('actionsControl')?.value;

                    if (model.remainingCapacityAction && !this.willIssueCapacityCertificates) {
                        model.remainingCapacityAction!.action = FishingCapacityRemainderActionEnum.NoCertificate;
                    }
                }

                model.exportCountryId = this.form.get('exportCountryControl')!.value?.value;
                model.exportType = this.form.get('exportTypeControl')?.value?.value;
            }

            model.comments = this.form.get('commentsControl')!.value;
            model.files = this.form.get('filesControl')!.value;
        }
        else {
            model.remainingCapacityAction = this.form.get('actionsControl')!.value;

            if (model.remainingCapacityAction && !this.willIssueCapacityCertificates) {
                model.remainingCapacityAction!.action = FishingCapacityRemainderActionEnum.NoCertificate;
            }
        }
    }

    private fillModelCancellationData(model: ShipRegisterEditDTO | ShipRegisterRegixDataDTO | ShipRegisterApplicationEditDTO): void {
        if (model instanceof ShipRegisterEditDTO) {
            if (this.isFormShipDestroyed(model)) {
                model.cancellationDetails = new CancellationDetailsDTO({
                    reasonId: this.form.get('destructionTypeControl')!.value!.value,
                    date: new Date()
                });
            }
            else if (this.isFormShipCancelled(model)) {
                model.cancellationDetails = new CancellationDetailsDTO({
                    reasonId: this.form.get('cancellationReasonControl')!.value!.value,
                    date: this.form.get('cancellationOrderDateControl')!.value,
                    issueOrderNum: this.form.get('cancellationOrderNumControl')!.value
                });
            }
        }
    }

    private getNomenclatures(): Subscription {
        type Type = FleetTypeNomenclatureDTO | NomenclatureDTO<number> | SailAreaNomenclatureDTO | FishingGearNomenclatureDTO | CancellationReasonDTO;

        const observables: Observable<Type[]>[] = [
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.VesselTypes, this.service.getVesselTypes.bind(this.service), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.FuelTypes, this.service.getFuelTypes.bind(this.service), false)
        ];

        if (!this.showOnlyRegiXData) {
            observables.push(NomenclatureStore.instance.getNomenclature(NomenclatureTypes.FleetTypes, this.service.getFleetTypes.bind(this.service), false));
            observables.push(NomenclatureStore.instance.getNomenclature(NomenclatureTypes.SailAreas, this.service.getSailAreas.bind(this.service), false));
            observables.push(NomenclatureStore.instance.getNomenclature(NomenclatureTypes.PublicAidTypes, this.service.getPublicAidTypes.bind(this.service), false));
            observables.push(NomenclatureStore.instance.getNomenclature(NomenclatureTypes.FleetSegments, this.service.getPublicAidSegments.bind(this.service), false));
            observables.push(NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Ports, this.nomenclatures.getPorts.bind(this.nomenclatures), false));
            observables.push(NomenclatureStore.instance.getNomenclature(NomenclatureTypes.FishingGear, this.nomenclatures.getFishingGear.bind(this.nomenclatures), false));
            observables.push(NomenclatureStore.instance.getNomenclature(NomenclatureTypes.HullMaterials, this.service.getHullMaterials.bind(this.service), false));
            observables.push(NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Countries, this.nomenclatures.getCountries.bind(this.nomenclatures), false));
            observables.push(NomenclatureStore.instance.getNomenclature(NomenclatureTypes.CancellationReasons, this.nomenclatures.getCancellationReasons.bind(this.nomenclatures), false));
            observables.push(NomenclatureStore.instance.getNomenclature(NomenclatureTypes.ShipAssociations, this.nomenclatures.getShipAssociations.bind(this.nomenclatures), false));

            this.exportTypes = [
                new NomenclatureDTO<ShipExportTypeEnum>({
                    value: ShipExportTypeEnum.ExportOrTransferInEU,
                    displayName: this.translate.getValue('ships-register.export-or-transfer-in-eu'),
                    isActive: true
                }),
                new NomenclatureDTO<ShipExportTypeEnum>({
                    value: ShipExportTypeEnum.ExportJointVenture,
                    displayName: this.translate.getValue('ships-register.export-joint-venture'),
                    isActive: true
                })
            ];
        }

        const subscription: Subscription = forkJoin(observables).subscribe({
            next: (nomenclatures: NomenclatureDTO<number>[][]) => {
                this.vesselTypes = nomenclatures[0];
                this.fuelTypes = nomenclatures[1];

                if (!this.showOnlyRegiXData) {
                    this.fleets = nomenclatures[2];
                    this.sailAreas = nomenclatures[3];
                    this.publicAidTypes = nomenclatures[4];
                    this.fleetSegments = nomenclatures[5];
                    this.ports = nomenclatures[6];
                    this.fishingGears = nomenclatures[7];
                    this.hullMaterials = nomenclatures[8];
                    this.countries = nomenclatures[9];

                    this.cancellationReasons = (nomenclatures[10] as CancellationReasonDTO[]).filter(x => x.group === CancellationReasonGroupEnum.ShipCancel);
                    this.destroyReasons = (nomenclatures[10] as CancellationReasonDTO[]).filter(x => x.group === CancellationReasonGroupEnum.ShipDestroy);

                    this.shipAssociations = nomenclatures[11];
                }

                this.loader.complete();
            }
        });

        return subscription;
    }

    private disableFieldsByEvent(): void {
        if (this.form !== undefined && this.form !== null) {
            if (!this.form.disabled) {
                this.form.disable({ emitEvent: false });
                this.allowOwnersEdit = false;
                this.setActiveFieldsClass(this.currentActiveFormControls, false);
            }

            if (this.disableFieldsByEventType !== undefined && this.disableFieldsByEventType !== null) {
                this.allowOwnersEdit = ENABLE_OWNERS_EDIT.get(this.disableFieldsByEventType) ?? false;

                const controls: string[] | undefined = ENABLE_FIELD_CONTROLS.get(this.disableFieldsByEventType);
                if (controls !== undefined) {
                    if (this.disableFieldsByEventType !== ShipEventTypeEnum.EDIT) {
                        controls.push(...(ENABLE_FIELD_CONTROLS.get(ShipEventTypeEnum.EDIT) ?? []));
                    }

                    const filtered: string[] = controls.filter(x => !DETAIL_FIELDS.includes(x));

                    for (const control of filtered) {
                        this.form.controls[control].enable();
                    }

                    if (this.disableFieldsByEventType === ShipEventTypeEnum.MOD) {
                        const cfr: string | undefined = this.form.get('cfrControl')!.value;
                        if (!cfr || cfr.length === 0 || TLValidators.cfr(this.form.get('cfrControl')!) !== null) {
                            this.form.get('cfrControl')!.enable();
                            controls.push('cfrControl');
                        }
                    }

                    this.setActiveFieldsClass(controls);
                }
                else {
                    throw Error(`Could not find event type: ${this.disableFieldsByEventType}`);
                }
            }
        }
    }

    private setActiveFieldsClass(controls: string[], active: boolean = true, setCurrentActiveControls: boolean = true): void {
        if (setCurrentActiveControls) {
            this.currentActiveFormControls = controls;
        }

        document.querySelectorAll('[formControlName]').forEach((element: Element) => {
            const control: string = element.getAttribute('formControlName')!;
            if (controls.includes(control)) {
                const underlines: HTMLElement[] = Array.from(element.querySelectorAll('.mat-form-field-underline')) as HTMLElement[];

                for (const underline of underlines) {
                    if (underline !== null) {
                        if (active) {
                            underline.style.bottom = '14px';
                            underline.style.height = '7px';
                            underline.style.background = 'linear-gradient(180deg, rgba(0,120,194,1) 0%, rgba(0,120,194,0) 100%)';
                        }
                        else {
                            underline.style.bottom = '';
                            underline.style.height = '';
                            underline.style.background = '';
                        }
                    }
                }
            }
        });

        if (!active && setCurrentActiveControls) {
            this.currentActiveFormControls = [];
        }
    }

    private validateOwners(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            let hasActiveOwners: boolean = false;
            let titulars: number = 0;
            let shares: number = 0;
            let validation: number = 0;

            for (const owner of (this.shipOwners as ShipOwnerDTO[]).filter(x => x.isActive)) {
                if (owner.hasValidationErrors) {
                    ++validation;
                }
                if (owner.isActive && !hasActiveOwners) {
                    hasActiveOwners = true;
                }
                if (owner.isShipHolder) {
                    ++titulars;
                }
                shares += owner.ownedShare!;
            }

            const errors: ValidationErrors = {};
            if (validation !== 0) {
                errors['ownersValidation'] = true;
            }

            if (!hasActiveOwners) {
                errors['noOwners'] = true;
            }

            if (titulars !== 1) {
                errors['onlyOneOwnerTitular'] = true;
            }

            if (shares !== 100) {
                errors['ownersOwnedShareNot100'] = true;
            }

            return Object.keys(errors).length !== 0 ? errors : null;
        };
    }

    private isShipDestroyed(model: ShipRegisterEditDTO): boolean {
        return model.cancellationDetails !== undefined && model.cancellationDetails !== null;
    }

    private isShipCancelled(model: ShipRegisterEditDTO): boolean {
        return model.cancellationDetails !== undefined && model.cancellationDetails !== null;
    }

    private isFormShipDestroyed(model: ShipRegisterEditDTO): boolean {
        const destruction = this.form.get('destructionTypeControl')!.value;
        return destruction !== undefined && destruction !== null;
    }

    private isFormShipCancelled(model: ShipRegisterEditDTO): boolean {
        const reason = this.form.get('cancellationReasonControl')!.value;
        const orderNum = this.form.get('cancellationOrderNumControl')!.value;
        const orderDate = this.form.get('cancellationOrderDateControl')!.value;

        return (reason !== undefined && reason !== null)
            || (orderNum !== undefined && orderNum !== null)
            || (orderDate !== undefined && orderDate !== null);
    }

    private saveShip(dialogClose: DialogCloseCallback, fromSaveAsDraft: boolean = false): Observable<boolean> {
        const saveOrEditDone: EventEmitter<boolean> = new EventEmitter<boolean>();

        this.saveOrEdit(fromSaveAsDraft).subscribe({
            next: (id: number | void) => {
                this.hasNoEDeliveryRegistrationError = false;

                NomenclatureStore.instance.clearNomenclature(NomenclatureTypes.Ships);

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
                if (response.error?.messages !== null && response.error?.messages !== undefined) {
                    const messages: string[] = response.error.messages;

                    if (messages.length !== 0) {
                        this.snackbar.openFromComponent(ErrorSnackbarComponent, {
                            data: response.error as ErrorModel,
                            duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                            panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
                        });
                    }
                    else {
                        this.snackbar.openFromComponent(ErrorSnackbarComponent, {
                            data: new ErrorModel({ messages: [this.translate.getValue('service.an-error-occurred-in-the-app')] }),
                            duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                            panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
                        });
                    }

                    if (messages.find(message => message === ApplicationValidationErrorsEnum[ApplicationValidationErrorsEnum.NoEDeliveryRegistration])) {
                        this.hasNoEDeliveryRegistrationError = true;
                    }
                }
            }
        });

        return saveOrEditDone.asObservable();
    }

    private saveOrEdit(fromSaveAsDraft: boolean): Observable<number | void> {
        this.model = this.getValue()!;
        CommonUtils.sanitizeModelStrings(this.model);

        let saveOrEditObservable: Observable<void | number>;

        if (this.model instanceof ShipRegisterEditDTO) {
            if (this.id !== undefined) {
                saveOrEditObservable = this.model.isThirdPartyShip
                    ? this.service.editThirdPartyShip(this.model)
                    : this.service.editShip(this.model);
            }
            else {
                saveOrEditObservable = this.model.isThirdPartyShip
                    ? this.service.addThirdPartyShip(this.model)
                    : this.service.addShip(this.model);
            }
        }
        else {
            if (this.model.id !== undefined && this.model.id !== null) {
                saveOrEditObservable = this.service.editApplication(this.model, this.pageCode, fromSaveAsDraft);
            }
            else {
                saveOrEditObservable = this.service.addApplication(this.model, this.pageCode);
            }
        }

        return saveOrEditObservable;
    }

    private ownerEqualsRegixOwner(owner: ShipOwnerRegixDataDTO): boolean {
        let regixOwner: ShipOwnerRegixDataDTO | undefined;

        if (owner.isOwnerPerson) {
            regixOwner = this.expectedResults.owners?.find(x => {
                return x.regixPersonData?.egnLnc?.identifierType === owner.regixPersonData?.egnLnc?.identifierType
                    && x.regixPersonData?.egnLnc?.egnLnc === owner.regixPersonData?.egnLnc?.egnLnc;
            });

            if (regixOwner) {
                if (owner.regixPersonData!.firstName !== regixOwner.regixPersonData!.firstName
                    || owner.regixPersonData!.middleName !== regixOwner.regixPersonData!.middleName
                    || owner.regixPersonData!.lastName !== regixOwner.regixPersonData!.lastName
                    || owner.regixPersonData!.email != regixOwner.regixPersonData!.email
                    || owner.regixPersonData!.citizenshipCountryId != regixOwner.regixPersonData!.citizenshipCountryId
                    || owner.regixPersonData!.birthDate != regixOwner.regixPersonData!.birthDate
                    || owner.regixPersonData!.phone != regixOwner.regixPersonData!.phone) {
                    return false;
                }
            }
        }
        else {
            regixOwner = this.expectedResults.owners?.find(x => {
                return x.regixLegalData?.eik === owner.regixLegalData?.eik;
            });

            if (regixOwner) {
                if (owner.regixLegalData!.name !== regixOwner.regixLegalData!.name
                    || owner.regixLegalData!.phone != regixOwner.regixLegalData!.phone
                    || owner.regixLegalData!.email != regixOwner.regixLegalData!.email) {
                    return false;
                }
            }
        }

        return true;
    }

    private getSubmittedForAsOwner(): ApplicationSubmittedForDTO | undefined {
        let result: ApplicationSubmittedForDTO | undefined = this.form.get('submittedForControl')?.value;

        if (result) {
            if (result.submittedByRole === SubmittedByRolesEnum.Personal) {
                const submittedBy: ApplicationSubmittedByDTO | undefined = this.form.get('submittedByControl')?.value;

                if (submittedBy) {
                    result = new ApplicationSubmittedForDTO({
                        submittedByRole: result.submittedByRole,
                        person: submittedBy.person,
                        addresses: submittedBy.addresses
                    });
                }
            }

            return result;
        }

        return undefined;
    }

    private isDraftMode(): boolean {
        return this.isApplication && (this.model.id === null || this.model.id === undefined);
    }

    private updateShipOwnersNameAndEgnLncEik(): void {
        for (const owner of this.shipOwners) {
            if (owner.regixPersonData !== undefined && owner.regixPersonData !== null) {
                owner.names = '';

                if (owner.regixPersonData.firstName !== undefined && owner.regixPersonData.firstName !== null) {
                    owner.names = `${owner.regixPersonData.firstName}`;
                }
                if (owner.regixPersonData.middleName !== undefined && owner.regixPersonData.middleName !== null) {
                    owner.names = `${owner.names} ${owner.regixPersonData.middleName}`;
                }
                if (owner.regixPersonData.lastName !== undefined && owner.regixPersonData.lastName !== null) {
                    owner.names = `${owner.names} ${owner.regixPersonData.lastName}`;
                }
                owner.egnLncEik = owner.regixPersonData.egnLnc?.egnLnc;
            }
            else if (owner.regixLegalData !== undefined && owner.regixLegalData !== null) {
                owner.names = owner.regixLegalData.name;
                owner.egnLncEik = owner.regixLegalData.eik;
            }
        }
    }

    private shouldHidePaymentData(): boolean {
        return (this.model as ShipRegisterApplicationEditDTO)?.paymentInformation?.paymentType === null
            || (this.model as ShipRegisterApplicationEditDTO)?.paymentInformation?.paymentType === undefined
            || (this.model as ShipRegisterApplicationEditDTO)?.paymentInformation?.paymentType === '';
    }
}