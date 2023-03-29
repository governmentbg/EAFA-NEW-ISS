import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { forkJoin } from 'rxjs';

import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PermissionGroupDTO } from '@app/models/generated/dtos/PermissionGroupDTO';
import { RoleRegisterEditDTO } from '@app/models/generated/dtos/RoleRegisterEditDTO';
import { UserRoleRegisterDTO } from '@app/models/generated/dtos/UserRoleRegisterDTO';
import { RolesRegisterService } from '@app/services/administration-app/roles-register.service';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { IRolesRegisterService } from '@app/interfaces/administration-app/roles-register.interface';
import { DateRangeData } from '@app/shared/components/input-controls/tl-date-range/tl-date-range.component';
import { GetControlErrorLabelTextCallback } from '@app/shared/components/input-controls/base-tl-control';

type TreeStatus = 'collapsed' | 'expanded' | 'disabled';

class PermissionGroupTreeDTO extends PermissionGroupDTO {
    public treeStatus!: TreeStatus;

    public constructor(obj?: Partial<PermissionGroupDTO>) {
        super(obj);
    }
}

@Component({
    selector: 'edit-role',
    templateUrl: './edit-role.component.html'
})
export class EditRoleComponent implements OnInit, AfterViewInit, IDialogComponent {
    public form!: FormGroup;
    public permissionFilterControl: FormControl = new FormControl();
    public usersForm!: FormGroup;
    public readOnly: boolean = false;

    public users!: NomenclatureDTO<number>[];
    public tableUsers!: UserRoleRegisterDTO[];

    public permissionGroups: PermissionGroupTreeDTO[] = [];
    public allPermissionGroups: PermissionGroupTreeDTO[] = [];

    public getDatesOverlappingErrorTextMethod: GetControlErrorLabelTextCallback = this.getDatesOverlappingErrorText.bind(this);

    @ViewChild('usersTable')
    private readonly usersTable!: TLDataTableComponent;

    private readonly service: IRolesRegisterService;
    private readonly nomenclatures: CommonNomenclatures;
    private readonly translate: FuseTranslationLoaderService;

    private roleId!: number | undefined;
    private model!: RoleRegisterEditDTO;

    private readonly EXPANDED_COLOR: string = '#EDEDED';
    private readonly COLLAPSED_COLOR: string = '#FFFFFF';

    private childrenIndices: Map<string, number[]> = new Map<string, number[]>();

    public constructor(service: RolesRegisterService, nomenclatures: CommonNomenclatures, translate: FuseTranslationLoaderService) {
        this.service = service;
        this.nomenclatures = nomenclatures;
        this.translate = translate;

        this.buildForm();
    }

    public async ngOnInit(): Promise<void> {
        const nomenclatures: [NomenclatureDTO<number>[], PermissionGroupDTO[]] = await forkJoin(
            this.nomenclatures.getUserNames(),
            this.service.getPermissionGroups()
        ).toPromise();

        this.users = nomenclatures[0];
        this.allPermissionGroups = nomenclatures[1].map(group => new PermissionGroupTreeDTO(group));

        this.organizeAndInitPermissionGroups();
        this.calculateChildrenIndices();
        this.initTreeStatuses();
        this.buildPermissionsFormGroup();

        if (this.roleId === undefined) {
            this.model = new RoleRegisterEditDTO();
        }
        else {
            this.form.get('codeControl')!.disable();

            this.service.getRole(this.roleId).subscribe({
                next: (role: RoleRegisterEditDTO) => {
                    this.model = role;
                    this.fillForm();
                }
            });
        }
    }

    public ngAfterViewInit(): void {
        this.usersForm.valueChanges.subscribe({
            next: () => {
                this.usersForm.get('userIdControl')!.updateValueAndValidity({ emitEvent: false });
                this.usersForm.get('userIdControlHidden')!.updateValueAndValidity({ emitEvent: false });
            }
        });

        this.permissionFilterControl.valueChanges.subscribe({
            next: (value: string) => {
                if (value !== undefined && value !== null && value.length > 0) {
                    value = value.toLowerCase();

                    this.permissionGroups = this.allPermissionGroups.filter((group: PermissionGroupDTO) => {
                        if (group.parentGroup?.toLowerCase().includes(value)) {
                            return true;
                        }

                        if (group.name?.toLowerCase().includes(value)) {
                            return true;
                        }

                        if (group.otherPermissions?.some(x => x.description?.toLowerCase().includes(value))) {
                            return true;
                        }
                        return false;
                    });
                }
                else {
                    this.permissionGroups = this.allPermissionGroups;
                }

                this.setBackgroundColorToRows();
            }
        });
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (this.readOnly) {
            dialogClose();
        }

        this.form.markAllAsTouched();
        if (this.form.valid) {
            this.fillModel();
            CommonUtils.sanitizeModelStrings(this.model);

            if (this.roleId !== undefined) {
                this.service.editRole(this.model).subscribe({
                    next: () => {
                        dialogClose(this.model);
                    }
                });
            }
            else {
                this.service.addRole(this.model).subscribe({
                    next: (id: number) => {
                        this.model.id = id;
                        dialogClose(this.model);
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

    public setData(data: DialogParamsModel | undefined, buttons: DialogWrapperData): void {
        if (data !== undefined && data !== null) {
            this.roleId = data.id;
            this.readOnly = data.isReadonly ?? false;
        }
    }

    public onTreeAction(event: { row: PermissionGroupTreeDTO, rowIndex: number }): void {
        event.row.treeStatus = event.row.treeStatus === 'collapsed' ? 'expanded' : 'collapsed';

        this.setBackgroundColorToRows();
        this.permissionGroups = [...this.permissionGroups];
    }

    public getDatesOverlappingErrorText(controlName: string, error: unknown, errorCode: string): TLError | undefined {
        if (controlName === 'userIdControl') {
            if (errorCode === 'datesOverlap') {
                return new TLError({
                    text: this.translate.getValue('roles-register.dates-overlap-with-other-record'),
                    type: 'error'
                });
            }
        }
        return undefined;
    }

    private buildForm(): void {
        this.form = new FormGroup({
            codeControl: new FormControl(null, [Validators.required, Validators.maxLength(50)]),
            nameControl: new FormControl(null, [Validators.required, Validators.maxLength(500)]),
            validityDateRangeControl: new FormControl(null, Validators.required),
            hasInternalAccessControl: new FormControl(false),
            hasPublicAccessControl: new FormControl(false),
            descriptionControl: new FormControl(null, Validators.maxLength(4000))
        });

        this.usersForm = new FormGroup({
            userIdControl: new FormControl(null, [Validators.required, this.datesOverlappingValidator()]),
            accessValidFromControl: new FormControl(null, Validators.required),
            accessValidToControl: new FormControl(null, Validators.required)
        });
    }

    private datesOverlappingValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            const max = (lhs: Date, rhs: Date) => lhs > rhs ? lhs : rhs;
            const min = (lhs: Date, rhs: Date) => lhs < rhs ? lhs : rhs;

            if (this.usersTable) {
                const userId: number | undefined = this.usersForm.get('userIdControl')!.value;
                const validFrom: Date | undefined = this.usersForm.get('accessValidFromControl')!.value;
                const validTo: Date | undefined = this.usersForm.get('accessValidToControl')!.value;

                if (userId !== undefined && validFrom !== undefined && validTo !== undefined) {
                    const otherEntries: UserRoleRegisterDTO[] = (this.usersTable.rows as UserRoleRegisterDTO[]).filter(
                        x => x.userId === userId && x.isActive !== false && x.accessValidFrom !== validFrom && x.accessValidTo !== validTo
                    );

                    for (const otherEntry of otherEntries) {
                        if (max(validFrom, otherEntry.accessValidFrom!) < min(validTo, otherEntry.accessValidTo!)) {
                            return { 'datesOverlap': true };
                        }
                    }
                }
            }
            return null;
        };
    }

    private fillForm(): void {
        this.form.get('codeControl')!.setValue(this.model.code);
        this.form.get('nameControl')!.setValue(this.model.name);
        this.form.get('validityDateRangeControl')!.setValue(new DateRangeData({ start: this.model.validFrom, end: this.model.validTo }));
        this.form.get('hasInternalAccessControl')!.setValue(this.model.hasInternalAccess);
        this.form.get('hasPublicAccessControl')!.setValue(this.model.hasPublicAccess);
        this.form.get('descriptionControl')!.setValue(this.model.description);

        this.tableUsers = this.model.users ?? [];

        if (this.model.permissionIds) {
            for (const permissionId of this.model.permissionIds) {
                const control: FormControl = this.getPermissionControlById(permissionId);
                control.setValue(true);
            }
        }

        if (this.readOnly) {
            this.form.disable();
        }
    }

    private fillModel(): void {
        this.model.code = this.form.get('codeControl')!.value;
        this.model.name = this.form.get('nameControl')!.value;
        this.model.validFrom = (this.form.get('validityDateRangeControl')!.value as DateRangeData)?.start;
        this.model.validTo = (this.form.get('validityDateRangeControl')!.value as DateRangeData)?.end;
        this.model.hasInternalAccess = this.form.get('hasInternalAccessControl')!.value;
        this.model.hasPublicAccess = this.form.get('hasPublicAccessControl')!.value;
        this.model.description = this.form.get('descriptionControl')!.value;

        this.model.users = this.usersTable.rows
            .filter(x => x.userId !== undefined && x.accessValidFrom !== undefined && x.accessValidTo !== undefined)
            .map(x => new UserRoleRegisterDTO({
                id: x.id,
                userId: x.userId,
                roleId: this.roleId,
                accessValidFrom: x.accessValidFrom,
                accessValidTo: x.accessValidTo,
                isActive: x.isActive ?? true
            }));

        this.model.permissionIds = [];

        const controls: { [key: string]: AbstractControl } = (this.form.get('permissionsGroup') as FormGroup).controls;
        for (const key of Object.keys(controls)) {
            if (this.form.get('permissionsGroup')!.get(key)!.value === true) {
                this.model.permissionIds.push(this.getPermissionIdByName(key));
            }
        }
    }

    private organizeAndInitPermissionGroups(): void {
        const parentGroups: PermissionGroupTreeDTO[] = this.allPermissionGroups.filter(x => x.parentGroup === undefined || x.parentGroup === null);
        const childGroups: PermissionGroupTreeDTO[] = this.allPermissionGroups.filter(x => !parentGroups.some(y => y.id === x.id));

        this.allPermissionGroups = [];
        for (const parent of parentGroups) {
            this.allPermissionGroups.push(parent);

            const children: PermissionGroupTreeDTO[] = childGroups.filter(x => x.parentGroup === parent.name);
            for (const child of children) {
                this.allPermissionGroups.push(child);
            }
        }

        this.permissionGroups = [...this.allPermissionGroups];
    }

    private initTreeStatuses(): void {
        for (const group of this.permissionGroups) {
            const hasChildren: boolean = this.permissionGroups.findIndex(x => x.parentGroup === group.name) !== -1;
            group.treeStatus = hasChildren ? 'collapsed' : 'disabled';
        }
    }

    private calculateChildrenIndices(): void {
        this.childrenIndices.clear();

        const parentGroups: string[] = this.permissionGroups.filter(x => x.parentGroup === undefined || x.parentGroup === null).map(x => x.name!);
        for (const parent of parentGroups) {
            this.childrenIndices.set(parent, []);
        }

        for (let i = 0; i < this.permissionGroups.length; ++i) {
            const parentGroup: string | undefined = this.permissionGroups[i].parentGroup;

            if (parentGroup !== undefined && parentGroup !== null && parentGroup.length > 0) {
                if (this.childrenIndices.has(parentGroup)) {
                    const invisibleRows: number = this.invisibleChildrenBeforeGroupCount(parentGroup);
                    this.childrenIndices.set(parentGroup, [...this.childrenIndices.get(parentGroup)!, i - invisibleRows]);
                }
            }
        }
    }

    private invisibleChildrenBeforeGroupCount(parentGroup: string): number {
        let count: number = 0;

        for (const group of this.permissionGroups) {
            if (group.name === parentGroup) {
                break;
            }

            if (group.treeStatus === 'collapsed') {
                count += this.permissionGroups.filter(x => x.parentGroup === group.name).length;
            }
        }
        return count;
    }

    private setBackgroundColorToRows(): void {
        this.calculateChildrenIndices();
        this.applyBackgroundColorToVisibleRows();
    }

    private applyBackgroundColorToVisibleRows(): void {
        const interval: number = setInterval(() => {
            const visibleRows: number = this.getVisibleRowsCount();
            let rows: HTMLElement[] = this.getTableRows();

            while (rows.length !== visibleRows) {
                rows = this.getTableRows();
            }

            // reset colors
            for (const row of rows) {
                row.style.backgroundColor = this.COLLAPSED_COLOR;
            }

            // reapply colors to expanded rows
            const expandedGroups: PermissionGroupTreeDTO[] = this.permissionGroups.filter(x => x.treeStatus === 'expanded');
            for (const group of expandedGroups) {
                const indices: number[] = this.childrenIndices.get(group.name!) ?? [];

                for (let i = 0; i < indices.length; ++i) {
                    rows[indices[i]].style.backgroundColor = this.EXPANDED_COLOR;
                }
            }
            clearInterval(interval);
        });
    }

    private getVisibleRowsCount(): number {
        let count: number = 0;

        for (const group of this.permissionGroups) {
            if (group.parentGroup !== undefined && group.parentGroup !== null) {
                const parent: PermissionGroupTreeDTO | undefined = this.permissionGroups.find(x => x.name === group.parentGroup);

                if (parent === undefined || parent.treeStatus === 'expanded') {
                    ++count;
                }
            }
            else {
                ++count;
            }
        }

        return count;
    }

    private buildPermissionsFormGroup(): void {
        const formGroup: FormGroup = new FormGroup({});
        for (const group of this.permissionGroups) {
            if (group.readAllPermission) {
                formGroup.addControl(`${group.readAllPermission.displayName}`, new FormControl());
            }

            if (group.readPermission) {
                formGroup.addControl(`${group.readPermission.displayName}`, new FormControl());
            }

            if (group.addPermission) {
                formGroup.addControl(`${group.addPermission.displayName}`, new FormControl());
            }

            if (group.editPermission) {
                formGroup.addControl(`${group.editPermission.displayName}`, new FormControl());
            }

            if (group.deletePermission) {
                formGroup.addControl(`${group.deletePermission.displayName}`, new FormControl());
            }

            if (group.restorePermission) {
                formGroup.addControl(`${group.restorePermission.displayName}`, new FormControl());
            }

            if (group.otherPermissions) {
                for (const perm of group.otherPermissions!) {
                    formGroup.addControl(`${perm.displayName}`, new FormControl());
                }
            }
        }

        this.form.addControl('permissionsGroup', formGroup);
    }

    private getPermissionControlById(id: number): FormControl {
        let permissionName: string | undefined;

        for (const group of this.permissionGroups) {
            if (group.readAllPermission?.value === Number(id)) {
                permissionName = group.readAllPermission.displayName!;
            }
            else if (group.readPermission?.value === Number(id)) {
                permissionName = group.readPermission.displayName!;
            }
            else if (group.addPermission?.value === Number(id)) {
                permissionName = group.addPermission.displayName!;
            }
            else if (group.editPermission?.value === Number(id)) {
                permissionName = group.editPermission.displayName!;
            }
            else if (group.deletePermission?.value === Number(id)) {
                permissionName = group.deletePermission.displayName!;
            }
            else if (group.restorePermission?.value === Number(id)) {
                permissionName = group.restorePermission.displayName!;
            }
            else if (group.otherPermissions) {
                for (const perm of group.otherPermissions) {
                    if (perm.value === Number(id)) {
                        permissionName = perm.displayName!;
                        break;
                    }
                }
            }
        }

        if (permissionName !== undefined) {
            return this.form.get('permissionsGroup')!.get(permissionName) as FormControl;
        }

        throw new Error(`Could not find permission with id ${id}`);
    }

    private getPermissionIdByName(name: string): number {
        for (const group of this.allPermissionGroups) {
            if (group.readAllPermission?.displayName === name) {
                return group.readAllPermission.value!;
            }

            if (group.readPermission?.displayName === name) {
                return group.readPermission.value!;
            }

            if (group.addPermission?.displayName === name) {
                return group.addPermission.value!;
            }

            if (group.editPermission?.displayName === name) {
                return group.editPermission.value!;
            }

            if (group.deletePermission?.displayName === name) {
                return group.deletePermission.value!;
            }

            if (group.restorePermission?.displayName === name) {
                return group.restorePermission.value!;
            }
            else if (group.otherPermissions) {
                for (const perm of group.otherPermissions) {
                    if (perm.displayName === name) {
                        return perm.value!;
                    }
                }
            }
        }
        throw new Error(`Could not find permission with name ${name}`);
    }

    private getTableRows(): HTMLElement[] {
        const table: HTMLElement = document.getElementById('role-permissions-table')!;
        const rows: HTMLElement[] = Array.from(table.getElementsByTagName('datatable-body-row')) as HTMLElement[];

        return rows;
    }
}
