import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { RecreationalFishingAssociationEditDTO } from '@app/models/generated/dtos/RecreationalFishingAssociationEditDTO';
import { RecreationalFishingAssociationService } from '@app/services/administration-app/recreational-fishing-association.service';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { RecreationalFishingAnnulAssociationComponent } from '../annul-association/recreational-fishing-annul-association.component';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { AssociationAnnulmentResult } from '../models/association-annulment-result.model';
import { AssociationEditDialogParams } from '../models/association-edit-dialog-params.model';
import { EikUtils } from '@app/shared/utils/eik.utils';
import { IRecreationalFishingAssociationService } from '@app/interfaces/common-app/recreational-fishing-association.interface';
import { LegalFullDataDTO } from '@app/models/generated/dtos/LegalFullDataDTO';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';
import { forkJoin } from 'rxjs';
import { FishingAssociationUserDTO } from '@app/models/generated/dtos/FishingAssociationUserDTO';
import { UserLegalStatusEnum } from '@app/enums/user-legal-status.enum';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { RecordChangedEventArgs } from '@app/shared/components/data-table/models/record-changed-event.model';
import { CommandTypes } from '@app/shared/components/data-table/enums/command-type.enum';

@Component({
    selector: 'recreational-fishing-edit-association',
    templateUrl: './recreational-fishing-edit-association.component.html'
})
export class RecreationalFishingEditAssociationComponent implements OnInit, AfterViewInit, IDialogComponent {
    public form!: FormGroup;
    public usersForm!: FormGroup;
    public service: IRecreationalFishingAssociationService;
    public isEditing: boolean = false;
    public readOnly: boolean = false;

    public territoryUnits!: NomenclatureDTO<number>[];
    public users!: NomenclatureDTO<number>[];
    public associationUsers: FishingAssociationUserDTO[] = [];

    public readonly pageCode: PageCodeEnum = PageCodeEnum.Assocs;

    public legalStatusEnum: typeof UserLegalStatusEnum = UserLegalStatusEnum;

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    @ViewChild('usersTable')
    private usersTable!: TLDataTableComponent;

    private model!: RecreationalFishingAssociationEditDTO;

    private nomenclatures: CommonNomenclatures;
    private translate: FuseTranslationLoaderService;
    private annulDialog: TLMatDialog<RecreationalFishingAnnulAssociationComponent>;
    private id!: number | undefined;
    private isAdding: boolean = false;

    public constructor(
        service: RecreationalFishingAssociationService,
        nomenclatures: CommonNomenclatures,
        translate: FuseTranslationLoaderService,
        annulDialog: TLMatDialog<RecreationalFishingAnnulAssociationComponent>
    ) {
        this.service = service;
        this.nomenclatures = nomenclatures;
        this.translate = translate;
        this.annulDialog = annulDialog;

        this.buildForm();
    }

    public async ngOnInit(): Promise<void> {
        this.territoryUnits = await NomenclatureStore.instance.getNomenclature<number>(
            NomenclatureTypes.TerritoryUnits, this.nomenclatures.getTerritoryUnits.bind(this.nomenclatures), false
        ).toPromise();

        const nomenclatures: NomenclatureDTO<number>[][] = await forkJoin(
            NomenclatureStore.instance.getNomenclature<number>(NomenclatureTypes.TerritoryUnits, this.nomenclatures.getTerritoryUnits.bind(this.nomenclatures), false),
            this.service.getAssociationUsersNomenclature()
        ).toPromise();

        this.territoryUnits = nomenclatures[0];
        this.users = nomenclatures[1];

        if (this.id === undefined) {
            this.model = new RecreationalFishingAssociationEditDTO();
        }
        else {
            if (this.isAdding === true) {
                this.service.getLegalForAssociation(this.id).subscribe({
                    next: (association: RecreationalFishingAssociationEditDTO) => {
                        this.model = association;
                        this.fillForm();
                    }
                });
            }
            else {
                this.service.getAssociation(this.id).subscribe({
                    next: (association: RecreationalFishingAssociationEditDTO) => {
                        this.model = association;
                        this.fillForm();

                        this.isEditing = this.model.legal?.eik !== undefined && EikUtils.isEikValid(this.model.legal.eik);
                    }
                });
            }
        }
    }

    public ngAfterViewInit(): void {
        this.usersTable.recordChanged.subscribe({
            next: () => {
                this.form.updateValueAndValidity({ onlySelf: true });
            }
        });
    }

    public saveBtnClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (action.id === 'save') {
            if (this.readOnly) {
                dialogClose();
            }

            this.form.markAllAsTouched();
            this.validityCheckerGroup.validate();
            if (this.form.valid) {
                this.addOrEdit(dialogClose);
            }
        }
    }

    public cancelBtnClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (action.id === 'cancel') {
            dialogClose();
        }
    }

    public dialogButtonClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.form.markAllAsTouched();
        this.validityCheckerGroup.validate();

        if (action.id === 'annul') {
            this.openAnnulDialog(dialogClose, true);
        }

        if (this.form.valid || this.form.disabled) {
            if (action.id === 'activate') {
                this.openAnnulDialog(dialogClose, false);
            }
        }
    }

    public setData(data: AssociationEditDialogParams | undefined, buttons: DialogWrapperData): void {
        this.id = data?.id;
        this.isAdding = data?.adding ?? false;
        this.readOnly = data?.readonly ?? false;
    }

    public downloadedLegalData(legal: LegalFullDataDTO): void {
        this.form.get('legalRegixDataControl')!.setValue(legal.legal);
        this.form.get('legalAddressesControl')!.setValue(legal.addresses);
    }

    public allowUser(id: number): void {
        const user: FishingAssociationUserDTO | undefined = this.associationUsers.find(x => x.userId === id);

        if (user !== undefined && user !== null) {
            user.status = UserLegalStatusEnum.Approved;
            this.associationUsers = this.associationUsers.slice();
        }
    }

    public denyUser(id: number): void {
        const user: FishingAssociationUserDTO | undefined = this.associationUsers.find(x => x.userId === id);

        if (user !== undefined && user !== null) {
            user.status = UserLegalStatusEnum.Blocked;
            this.associationUsers = this.associationUsers.slice();
        }
    }

    public userLegalsChanged(recordChangedEvent: RecordChangedEventArgs<FishingAssociationUserDTO>): void {
        if (recordChangedEvent.Command === CommandTypes.Add) {
            if (recordChangedEvent.Record.status === null
                || recordChangedEvent.Record.status === undefined) {
                recordChangedEvent.Record.status = UserLegalStatusEnum.Approved;
            }

            this.form.updateValueAndValidity({ onlySelf: true });
        }
    }

    private buildForm(): void {
        this.form = new FormGroup({
            territoryUnitControl: new FormControl(null, Validators.required),
            legalRegixDataControl: new FormControl(),
            legalAddressesControl: new FormControl(),
            filesControl: new FormControl()
        });

        this.usersForm = new FormGroup({
            userIdControl: new FormControl(null, Validators.required)
        });
    }

    private fillForm(): void {
        this.form.get('territoryUnitControl')!.setValue(this.territoryUnits.find(x => x.value === this.model.territoryUnitId));
        this.form.get('legalRegixDataControl')!.setValue(this.model.legal);
        this.form.get('legalAddressesControl')!.setValue(this.model.legalAddresses);
        this.form.get('filesControl')!.setValue(this.model.files);

        if (this.readOnly) {
            this.form.disable();
        }

        if (this.isAdding) {
            this.form.get('legalRegixDataControl')!.disable();
            this.form.get('legalAddressesControl')!.disable();
        }

        setTimeout(() => {
            this.associationUsers = this.model.users ?? [];
        });
    }

    private fillModel(): void {
        this.model.territoryUnitId = this.form.get('territoryUnitControl')!.value.value;
        this.model.files = this.form.get('filesControl')!.value;
        this.model.isAdding = this.isAdding;
        this.model.users = this.getAssociationUsersFromTable();

        if (this.isAdding) {
            this.model.legalId = this.id;
        }
        else {
            this.model.legal = this.form.get('legalRegixDataControl')!.value;
            this.model.legalAddresses = this.form.get('legalAddressesControl')!.value;
        }

        CommonUtils.sanitizeModelStrings(this.model);
    }

    private addOrEdit(dialogClose: DialogCloseCallback, isAnnul: boolean = false): void {
        if (!isAnnul) {
            this.fillModel();
        }

        if (this.id !== undefined && !this.isAdding) {
            this.service.editAssociation(this.model).subscribe({
                next: () => {
                    dialogClose(this.model);
                }
            });
        }
        else {
            this.service.addAssociation(this.model).subscribe({
                next: (id: number) => {
                    this.model.id = id;
                    dialogClose(this.model);
                }
            });
        }
    }

    private openAnnulDialog(dialogClose: DialogCloseCallback, annuling: boolean): void {
        const title: string = annuling
            ? this.translate.getValue('recreational-fishing.annul-association-dialog-title')
            : this.translate.getValue('recreational-fishing.activate-association-dialog-title');

        const dialog = this.annulDialog.openWithTwoButtons({
            title: title,
            TCtor: RecreationalFishingAnnulAssociationComponent,
            headerCancelButton: {
                cancelBtnClicked: this.closeAnnulDialogBtnClicked.bind(this)
            },
            translteService: this.translate,
            componentData: new AssociationAnnulmentResult(!annuling, this.model.cancellationDate!, this.model.cancellationReason!)
        }, '1200px');

        dialog.subscribe((result: AssociationAnnulmentResult | undefined) => {
            if (result !== undefined && result !== null) {
                this.model.isCanceled = result.canceled;
                this.model.cancellationDate = result.canceled ? result.date : undefined;
                this.model.cancellationReason = result.canceled ? result.reason : undefined;
                this.model.isAdding = this.isAdding;

                this.addOrEdit(dialogClose, true);
            }
        });
    }

    private getAssociationUsersFromTable(): FishingAssociationUserDTO[] {
        const rows = this.usersTable.rows as FishingAssociationUserDTO[];

        const users: FishingAssociationUserDTO[] = rows.map(x => new FishingAssociationUserDTO({
            userId: x.userId,
            status: x.status,
            isActive: x.isActive ?? true
        }));

        const result: FishingAssociationUserDTO[] = [];

        for (const user of users) {
            if (result.findIndex(x => x.userId === user.userId) === -1) {
                const original = users.filter(x => x.userId === user.userId);

                if (original.length === 1) {
                    result.push(user);
                }
                else {
                    result.push(original.find(x => x.isActive === true)!);
                }
            }
        }

        return result;
    }

    private closeAnnulDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }
}