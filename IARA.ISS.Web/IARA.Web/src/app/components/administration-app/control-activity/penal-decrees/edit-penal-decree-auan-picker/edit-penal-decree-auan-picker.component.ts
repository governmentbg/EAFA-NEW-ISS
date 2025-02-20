import { AfterViewInit, Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Observable } from 'rxjs';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { IPenalDecreesService } from '@app/interfaces/administration-app/penal-decrees.interface';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PenalDecreeEditDTO } from '@app/models/generated/dtos/PenalDecreeEditDTO';
import { PenalDecreesService } from '@app/services/administration-app/penal-decrees.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { EditPenalDecreeComponent } from '../edit-penal-decree/edit-penal-decree.component';
import { EditPenalDecreeDialogParams } from '../models/edit-penal-decree-params.model';
import { PenalDecreeTypeEnum } from '@app/enums/penal-decree-type.enum';
import { EditDecreeAgreementComponent } from '../edit-decree-agreement/edit-decree-agreement.component';
import { EditDecreeWarningComponent } from '../edit-decree-warning/edit-decree-warning.component';
import { EditDecreeResolutionComponent } from '../edit-decree-resolution/edit-decree-resolution.component';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { PermissionsService } from '@app/shared/services/permissions.service';

@Component({
    selector: 'edit-penal-decree-auan-picker',
    templateUrl: './edit-penal-decree-auan-picker.component.html'
})
export class EditPenalDecreeAuanPickerComponent implements OnInit, AfterViewInit, IDialogComponent {
    public form: FormGroup;

    public auans: NomenclatureDTO<number>[] = [];
    public types: NomenclatureDTO<number>[] = [];

    public type!: PenalDecreeTypeEnum;
    public isThirdParty: boolean = false;

    private auanId: number | undefined;
    private typeId!: number;

    public readonly canSubmitRecords: boolean;
    public readonly canCancelRecords: boolean;
    public readonly canSaveAfterHours: boolean;

    private service: IPenalDecreesService;
    private translate: FuseTranslationLoaderService;
    private penalDecreeDialog: TLMatDialog<EditPenalDecreeComponent>;
    private agreementDialog: TLMatDialog<EditDecreeAgreementComponent>;
    private warningDialog: TLMatDialog<EditDecreeWarningComponent>;
    private resolutionDialog: TLMatDialog<EditDecreeResolutionComponent>;

    public constructor(
        service: PenalDecreesService,
        translate: FuseTranslationLoaderService,
        penalDecreeDialog: TLMatDialog<EditPenalDecreeComponent>,
        agreementDialog: TLMatDialog<EditDecreeAgreementComponent>,
        warningDialog: TLMatDialog<EditDecreeWarningComponent>,
        resolutionDialog: TLMatDialog<EditDecreeResolutionComponent>,
        permissions: PermissionsService
    ) {
        this.service = service;
        this.translate = translate;
        this.penalDecreeDialog = penalDecreeDialog;
        this.agreementDialog = agreementDialog;
        this.warningDialog = warningDialog;
        this.resolutionDialog = resolutionDialog;

        this.canSubmitRecords = permissions.has(PermissionsEnum.PenalDecreesSubmitRecords);
        this.canCancelRecords = permissions.has(PermissionsEnum.PenalDecreesCancelRecords);
        this.canSaveAfterHours = permissions.has(PermissionsEnum.PenalDecreesCanSaveAfterHours);

        this.form = this.buildForm();
    }

    public ngOnInit(): void {
        this.service.getAllAuans().subscribe({
            next: (result: NomenclatureDTO<number>[]) => {
                this.auans = result;
            }
        });

        this.service.getPenalDecreeTypes().subscribe({
            next: (result: NomenclatureDTO<number>[]) => {
                this.types = result;
            }
        })
    }

    public ngAfterViewInit(): void {
        this.form.get('isThirdPartyControl')!.valueChanges.subscribe({
            next: (value: boolean) => {
                this.isThirdParty = value;
                this.form.get('auanControl')!.clearValidators();

                if (value === false) {
                    this.form.get('auanControl')!.setValidators(Validators.required);
                }

                this.form.get('auanControl')!.updateValueAndValidity({ emitEvent: false });
            }
        });
    }

    public setData(data: unknown, wrapperData: DialogWrapperData): void {
        // nothing to do
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.form.markAllAsTouched();

        if (this.form.valid) {
            const type: NomenclatureDTO<number> = this.form.get('typeControl')!.value;
            this.type = PenalDecreeTypeEnum[type.code as keyof typeof PenalDecreeTypeEnum];
            this.auanId = this.form.get('auanControl')!.value?.value;
            this.typeId = this.form.get('typeControl')!.value!.value;

            switch (this.type) {
                case PenalDecreeTypeEnum.PenalDecree: {
                    this.openEditPenalDecreeDialog().subscribe({
                        next: (decree: PenalDecreeEditDTO | undefined) => {
                            dialogClose(decree);
                        }
                    });
                } break;
                case PenalDecreeTypeEnum.Agreement: {
                    this.openEditAgreementDialog().subscribe({
                        next: (decree: PenalDecreeEditDTO | undefined) => {
                            dialogClose(decree);
                        }
                    });
                } break;
                case PenalDecreeTypeEnum.Warning: {
                    this.openEditWarningDialog().subscribe({
                        next: (decree: PenalDecreeEditDTO | undefined) => {
                            dialogClose(decree);
                        }
                    });
                } break;
                case PenalDecreeTypeEnum.Resolution: {
                    this.openEditResolutionDialog().subscribe({
                        next: (decree: PenalDecreeEditDTO | undefined) => {
                            dialogClose(decree);
                        }
                    });
                } break;
                default: {
                    dialogClose();
                } break;
            }
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    private openEditPenalDecreeDialog(): Observable<PenalDecreeEditDTO | undefined> {
        let leftSideActions: IActionInfo[] = [];
        if (this.canCancelRecords) {
            leftSideActions = [{
                id: 'cancel-decree',
                color: 'warn',
                translateValue: 'penal-decrees.cancel'
            }];
        }

        const dialog = this.penalDecreeDialog.openWithTwoButtons({
            title: this.translate.getValue('penal-decrees.add-penal-decree-dialog-title'),
            TCtor: EditPenalDecreeComponent,
            headerAuditButton: undefined,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditDialogBtnClicked.bind(this)
            },
            componentData: new EditPenalDecreeDialogParams({
                auanId: this.auanId,
                typeId: this.typeId,
                canSaveAfterHours: this.canSaveAfterHours,
                isReadonly: false
            }),
            saveBtn: {
                id: 'save',
                color: 'accent',
                hidden: !this.canSubmitRecords,
                translateValue: this.translate.getValue('common.save')
            },
            rightSideActionsCollection: [{
                id: 'save-draft',
                color: 'primary',
                translateValue: 'penal-decrees.save-draft',
            },
            {
                id: 'print',
                color: 'accent',
                translateValue: 'penal-decrees.save-print'
            }],
            leftSideActionsCollection: leftSideActions,
            translteService: this.translate,
            disableDialogClose: true
        }, '1400px');

        return dialog;
    }

    private openEditAgreementDialog(): Observable<PenalDecreeEditDTO | undefined> {
        let leftSideActions: IActionInfo[] = [];
        if (this.canCancelRecords) {
            leftSideActions = [{
                id: 'cancel-decree',
                color: 'warn',
                translateValue: 'penal-decrees.cancel'
            }];
        }

        const dialog = this.agreementDialog.openWithTwoButtons({
            title: this.translate.getValue('penal-decrees.add-agreement-dialog-title'),
            TCtor: EditDecreeAgreementComponent,
            headerAuditButton: undefined,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditDialogBtnClicked.bind(this)
            },
            componentData: new EditPenalDecreeDialogParams({
                auanId: this.form.get('auanControl')!.value?.value,
                typeId: this.form.get('typeControl')!.value!.value,
                canSaveAfterHours: this.canSaveAfterHours,
                isReadonly: false
            }),
            saveBtn: {
                id: 'save',
                color: 'accent',
                hidden: !this.canSubmitRecords,
                translateValue: this.translate.getValue('common.save')
            },
            rightSideActionsCollection: [{
                id: 'save-draft',
                color: 'primary',
                translateValue: 'penal-decrees.save-draft',
            },
            {
                id: 'print',
                color: 'accent',
                translateValue: 'penal-decrees.save-print'
            }],
            leftSideActionsCollection: leftSideActions,
            translteService: this.translate,
            disableDialogClose: true
        }, '1400px');

        return dialog;
    }

    private openEditWarningDialog(): Observable<PenalDecreeEditDTO | undefined> {
        let leftSideActions: IActionInfo[] = [];
        if (this.canCancelRecords) {
            leftSideActions = [{
                id: 'cancel-decree',
                color: 'warn',
                translateValue: 'penal-decrees.cancel'
            }];
        }

        const dialog = this.warningDialog.openWithTwoButtons({
            title: this.translate.getValue('penal-decrees.add-warning-dialog-title'),
            TCtor: EditDecreeWarningComponent,
            headerAuditButton: undefined,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditDialogBtnClicked.bind(this)
            },
            componentData: new EditPenalDecreeDialogParams({
                auanId: this.form.get('auanControl')!.value?.value,
                typeId: this.form.get('typeControl')!.value!.value,
                canSaveAfterHours: this.canSaveAfterHours,
                isReadonly: false
            }),
            saveBtn: {
                id: 'save',
                color: 'accent',
                hidden: !this.canSubmitRecords,
                translateValue: this.translate.getValue('common.save')
            },
            rightSideActionsCollection: [{
                id: 'save-draft',
                color: 'primary',
                translateValue: 'penal-decrees.save-draft',
            },
            {
                id: 'print',
                color: 'accent',
                translateValue: 'penal-decrees.save-print'
            }],
            leftSideActionsCollection: leftSideActions,
            translteService: this.translate,
            disableDialogClose: true
        }, '1400px');

        return dialog;
    }

    private openEditResolutionDialog(): Observable<PenalDecreeEditDTO | undefined> {
        let leftSideActions: IActionInfo[] = [];
        if (this.canCancelRecords) {
            leftSideActions = [{
                id: 'cancel-decree',
                color: 'warn',
                translateValue: 'penal-decrees.cancel'
            }];
        }

        const dialog = this.resolutionDialog.openWithTwoButtons({
            title: this.translate.getValue('penal-decrees.add-resolution-dialog-title'),
            TCtor: EditDecreeResolutionComponent,
            headerAuditButton: undefined,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditDialogBtnClicked.bind(this)
            },
            componentData: new EditPenalDecreeDialogParams({
                auanId: this.form.get('auanControl')!.value?.value,
                typeId: this.form.get('typeControl')!.value!.value,
                canSaveAfterHours: this.canSaveAfterHours,
                isReadonly: false
            }),
            saveBtn: {
                id: 'save',
                color: 'accent',
                hidden: !this.canSubmitRecords,
                translateValue: this.translate.getValue('common.save')
            },
            rightSideActionsCollection: [{
                id: 'save-draft',
                color: 'primary',
                translateValue: 'penal-decrees.save-draft',
            },
            {
                id: 'print',
                color: 'accent',
                translateValue: 'penal-decrees.save-print'
            }],
            leftSideActionsCollection: leftSideActions,
            translteService: this.translate,
            disableDialogClose: true
        }, '1400px');

        return dialog;
    }

    private closeEditDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private buildForm(): FormGroup {
        return new FormGroup({
            typeControl: new FormControl(null, Validators.required),
            auanControl: new FormControl(null, Validators.required),
            isThirdPartyControl: new FormControl(null)
        });
    }
}