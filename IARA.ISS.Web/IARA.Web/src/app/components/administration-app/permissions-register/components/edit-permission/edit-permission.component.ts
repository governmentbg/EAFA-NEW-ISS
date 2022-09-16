import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { forkJoin } from 'rxjs';

import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PermissionRegisterEditDTO } from '@app/models/generated/dtos/PermissionRegisterEditDTO';
import { RolePermissionRegisterDTO } from '@app/models/generated/dtos/RolePermissionRegisterDTO';
import { PermissionsRegisterService } from '@app/services/administration-app/permissions-register.service';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';

@Component({
    selector: 'edit-permission',
    templateUrl: './edit-permission.component.html'
})
export class EditPermissionComponent implements IDialogComponent, OnInit, AfterViewInit {
    public form!: FormGroup;
    public rolesForm!: FormGroup;

    public model!: PermissionRegisterEditDTO;
    public readOnly!: boolean;

    public types: NomenclatureDTO<number>[] = [];
    public groups: NomenclatureDTO<number>[] = [];
    public allRoles: NomenclatureDTO<number>[] = [];
    public roles: NomenclatureDTO<number>[] = [];

    public tableRoles: RolePermissionRegisterDTO[] = [];

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    private service: PermissionsRegisterService;
    private permissionId!: number;

    public constructor(service: PermissionsRegisterService) {
        this.service = service;

        this.buildForm();
    }

    public async ngOnInit(): Promise<void> {
        const nomenclatures: NomenclatureDTO<number>[][] = await forkJoin(
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.Roles, this.service.getAllRoles.bind(this.service), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.PermissionTypes, this.service.getPermissionTypes.bind(this.service), false),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.PermissionGroups, this.service.getPermissionGroups.bind(this.service), false)
        ).toPromise();

        this.allRoles = this.roles = nomenclatures[0];
        this.types = nomenclatures[1];
        this.groups = nomenclatures[2];

        this.service.getPermission(this.permissionId).subscribe({
            next: (permission: PermissionRegisterEditDTO) => {
                this.model = permission;
                this.fillForm();
            }
        });
    }

    public ngAfterViewInit(): void {
        this.rolesForm.get('roleIdControl')!.valueChanges.subscribe({
            next: () => {
                this.roles = [...this.allRoles];
                const currentRoleIds: number[] = this.datatable.rows.map(x => x.roleId);

                this.roles = this.roles.filter(x => !currentRoleIds.includes(x.value!));
                this.roles = this.roles.slice();
            }
        });
    }

    public saveBtnClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (this.readOnly) {
            dialogClose();
        }

        this.form.markAllAsTouched();

        if (this.form.valid) {
            this.fillModel();
            CommonUtils.sanitizeModelStrings(this.model);

            this.service.editPermission(this.model).subscribe({
                next: () => {
                    dialogClose(this.model);
                }
            });
        }
    }

    public cancelBtnClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(action: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public setData(data: DialogParamsModel, buttons: DialogWrapperData): void {
        this.readOnly = data.isReadonly;
        this.permissionId = data.id;

        if (this.readOnly) {
            this.form.disable();
        }
    }

    private buildForm(): void {
        this.form = new FormGroup({
            nameControl: new FormControl({ value: null, disabled: true }),
            groupControl: new FormControl({ value: null, disabled: true }),
            typeControl: new FormControl({ value: null, disabled: true }),
            descriptionControl: new FormControl(null, Validators.maxLength(4000))
        });

        this.rolesForm = new FormGroup({
            roleIdControl: new FormControl(null, Validators.required)
        });
    }

    private fillForm(): void {
        this.form.get('nameControl')!.setValue(this.model.name);
        this.form.get('groupControl')!.setValue(this.groups.find(x => x.value === this.model.groupId));
        this.form.get('typeControl')!.setValue(this.types.find(x => x.value === this.model.typeId));
        this.form.get('descriptionControl')!.setValue(this.model.description);

        this.tableRoles = this.model.roles ?? [];
    }

    private fillModel(): void {
        this.model.description = this.form.get('descriptionControl')!.value;

        this.model.roles = this.datatable.rows.map(x => new RolePermissionRegisterDTO({
            permissionId: this.permissionId,
            roleId: x.roleId,
            isActive: x.isActive ?? true
        }));
    }
}
