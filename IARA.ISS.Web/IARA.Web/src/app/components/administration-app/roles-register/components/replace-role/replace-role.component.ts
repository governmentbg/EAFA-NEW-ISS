import { AfterViewInit, Component, OnInit } from '@angular/core';
import { forkJoin } from 'rxjs';

import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { RolesRegisterService } from '@app/services/administration-app/roles-register.service';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { RolesService } from '@app/services/administration-app/roles.service';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';

@Component({
    selector: 'replace-role',
    templateUrl: './replace-role.component.html'
})
export class ReplaceRoleComponent implements OnInit, AfterViewInit, IDialogComponent {
    public form: FormGroup;

    public roles: NomenclatureDTO<number>[] = [];
    public users: NomenclatureDTO<number>[] = [];

    public loaded: boolean = false;

    private service: RolesRegisterService;
    private rolesService: RolesService;

    private roleId!: number;

    public constructor(service: RolesRegisterService, rolesService: RolesService) {
        this.service = service;
        this.rolesService = rolesService;

        this.form = new FormGroup({
            roleControl: new FormControl({ value: null, disabled: true }),
            optionControl: new FormControl(null, Validators.required)
        });
    }

    public ngOnInit(): void {
        forkJoin(
            this.rolesService.getAllActiveRoles(),
            this.service.getUsersWithRole(this.roleId)
        ).subscribe({
            next: ([roles, users]: [NomenclatureDTO<number>[], NomenclatureDTO<number>[]]) => {
                this.roles = roles.filter(x => x.value !== this.roleId);
                this.users = users;
                this.loaded = true;
            }
        });
    }


    public ngAfterViewInit(): void {
        this.form.get('optionControl')!.valueChanges.subscribe({
            next: (option: string) => {
                const role: FormControl = this.form.get('roleControl') as FormControl;
                if (option === 'replace') {
                    role.enable();
                    role.setValidators(Validators.required);
                }
                else {
                    role.disable();
                    role.setValidators([]);
                }
                role.markAsPending({ onlySelf: true, emitEvent: false });
            }
        });
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.form.markAllAsTouched();

        if (this.form.valid) {
            if (this.form.get('optionControl')!.value === 'replace') {
                const newRoleId: number = (this.form.get('roleControl')!.value as NomenclatureDTO<number>).value!;

                this.service.deleteAndReplaceRole(this.roleId, newRoleId).subscribe({
                    next: () => {
                        dialogClose(true);
                    }
                });
            }
            else {
                this.service.deleteRole(this.roleId).subscribe({
                    next: () => {
                        dialogClose(true);
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

    public setData(data: DialogParamsModel, buttons: DialogWrapperData): void {
        this.roleId = data.id;
    }
}