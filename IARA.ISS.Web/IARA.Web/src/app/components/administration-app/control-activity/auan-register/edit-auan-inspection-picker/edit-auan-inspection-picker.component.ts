import { AfterViewInit, Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Observable } from 'rxjs';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';

import { IAuanRegisterService } from '@app/interfaces/administration-app/auan-register.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { AuanRegisterService } from '@app/services/administration-app/auan-register.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { AuanRegisterEditDTO } from '@app/models/generated/dtos/AuanRegisterEditDTO';
import { EditAuanDialogParams } from '../models/edit-auan-dialog-params.model';
import { EditAuanComponent } from '../edit-auan/edit-auan.component';
import { AuanInspectionDTO } from '@app/models/generated/dtos/AuanInspectionDTO';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { PermissionsService } from '@app/shared/services/permissions.service';

@Component({
    selector: 'edit-auan-inspection-picker',
    templateUrl: './edit-auan-inspection-picker.component.html'
})
export class EditAuanInspectionPickerComponent implements OnInit, AfterViewInit, IDialogComponent {
    public form: FormGroup;
    public isThirdParty: boolean = false;

    public inspectionReports: NomenclatureDTO<number>[] = [];
    public users: NomenclatureDTO<number>[] = [];
    public territoryUnits: NomenclatureDTO<number>[] = [];

    public readonly today: Date = new Date();

    private model: AuanInspectionDTO | undefined;
    private inspectionId: number | undefined;
    private canCancelAuan: boolean;

    private readonly service!: IAuanRegisterService;
    private readonly nomenclatures: CommonNomenclatures;
    private readonly translate: FuseTranslationLoaderService;
    private readonly editDialog: TLMatDialog<EditAuanComponent>;

    public constructor(
        service: AuanRegisterService,
        nomenclatures: CommonNomenclatures,
        translate: FuseTranslationLoaderService,
        editDialog: TLMatDialog<EditAuanComponent>,
        permissions: PermissionsService
    ) {
        this.service = service;
        this.nomenclatures = nomenclatures;
        this.translate = translate;
        this.editDialog = editDialog;
        this.model = new AuanInspectionDTO();

        this.canCancelAuan = permissions.has(PermissionsEnum.AuanRegisterCancel);

        this.form = this.buildForm();
    }

    public ngOnInit(): void {
        NomenclatureStore.instance.getNomenclature(
            NomenclatureTypes.TerritoryUnits, this.nomenclatures.getTerritoryUnits.bind(this.nomenclatures), false
        ).subscribe({
            next: (result: NomenclatureDTO<number>[]) => {
                this.territoryUnits = result;
            }
        });

        this.service.getAllInspectionReports().subscribe({
            next: (result: NomenclatureDTO<number>[]) => {
                this.inspectionReports = result;
            }
        });

        this.service.getInspectorUsernames().subscribe({
            next: (result: NomenclatureDTO<number>[]) => {
                this.users = result;
            }
        });
    }

    public ngAfterViewInit(): void {
        this.form.get('isThirdPartyControl')!.valueChanges.subscribe({
            next: (value: boolean) => {
                this.isThirdParty = value;
                this.form.get('inspectorControl')!.clearValidators();
                this.form.get('inspectionReportControl')!.clearValidators();

                if (value) {
                    this.form.get('inspectorControl')!.setValidators(Validators.required);
                }
                else {
                    this.form.get('inspectionReportControl')!.setValidators(Validators.required);
                }

                this.form.get('inspectorControl')!.updateValueAndValidity({ emitEvent: false });
                this.form.get('inspectionReportControl')!.updateValueAndValidity({ emitEvent: false });
                this.form.get('inspectorControl')!.markAsPending();
                this.form.get('inspectionReportControl')!.markAsPending();
            }
        });
    }

    public setData(data: unknown, wrapperData: DialogWrapperData): void {
        // nothing to do
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.form.markAllAsTouched();
        if (this.form.valid) {
            this.fillModel();
            CommonUtils.sanitizeModelStrings(this.model);

            if (!this.isThirdParty) {
                this.openEditAuanDialog().subscribe({
                    next: (auan: AuanRegisterEditDTO | undefined) => {
                        dialogClose(auan);
                    }
                });
            }
            else {
                this.service.addAuanInspection(this.model!).subscribe({
                    next: (id: number) => {
                        this.inspectionId = id;

                        this.openEditAuanDialog().subscribe({
                            next: (auan: AuanRegisterEditDTO | undefined) => {
                                dialogClose(auan);
                            }
                        });
                    }
                });
            }
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    private openEditAuanDialog(): Observable<AuanRegisterEditDTO | undefined> {
        let leftSideActions: IActionInfo[] = [];
        if (this.canCancelAuan) {
            leftSideActions = [{
                id: 'cancel-auan',
                color: 'warn',
                translateValue: 'auan-register.cancel'
            }];
        }

        const dialog = this.editDialog.openWithTwoButtons({
            title: this.translate.getValue('auan-register.add-auan-dialog-title'),
            TCtor: EditAuanComponent,
            headerAuditButton: undefined,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditDialogBtnClicked.bind(this)
            },
            componentData: new EditAuanDialogParams({
                inspectionId: this.inspectionId,
                isThirdParty: this.isThirdParty,
                isReadonly: false
            }),
            rightSideActionsCollection: [{
                id: 'save-draft',
                color: 'primary',
                translateValue: 'auan-register.save-draft',
            },
            {
                id: 'print',
                color: 'accent',
                translateValue: 'auan-register.save-print'
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
            inspectionReportControl: new FormControl(null, Validators.required),
            isThirdPartyControl: new FormControl(null),
            inspectorControl: new FormControl(null),
            territoryUnitControl: new FormControl(null),
            inspectionDateControl: new FormControl(null)
        });
    }

    private fillModel(): void {
        if (!this.isThirdParty) {
            this.inspectionId = this.form.get('inspectionReportControl')!.value?.value;
        }
        else {
            this.model!.userId = this.form.get('inspectorControl')!.value?.value;
            this.model!.territoryUnitId = this.form.get('territoryUnitControl')!.value?.value;
            this.model!.startDate = this.form.get('inspectionDateControl')!.value;
        }
    }
}