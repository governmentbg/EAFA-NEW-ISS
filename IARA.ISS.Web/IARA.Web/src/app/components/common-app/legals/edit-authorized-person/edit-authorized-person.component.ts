import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { AuthorizedPersonDTO } from '@app/models/generated/dtos/AuthorizedPersonDTO';
import { AuthorizedPersonRegixDataDTO } from '@app/models/generated/dtos/AuthorizedPersonRegixDataDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { RegixPersonDataDTO } from '@app/models/generated/dtos/RegixPersonDataDTO';
import { RoleDTO } from '@app/models/generated/dtos/RoleDTO';
import { RolesService } from '@app/services/administration-app/roles.service';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { CommandTypes } from '@app/shared/components/data-table/enums/command-type.enum';
import { RecordChangedEventArgs } from '@app/shared/components/data-table/models/record-changed-event.model';
import { EditAuthorizedPersonDialogParams } from '../models/edit-authorized-person-dialog-params.model';
import { EditAuthorizedPersonDialogResult } from '../models/edit-authorized-person-dialog-result.model';
import { PersonFullDataDTO } from '@app/models/generated/dtos/PersonFullDataDTO';
import { GetControlErrorLabelTextCallback } from '@app/shared/components/input-controls/base-tl-control';

@Component({
    selector: 'edit-authorized-person',
    templateUrl: './edit-authorized-person.component.html'
})
export class EditAuthorizedPersonComponent implements OnInit, AfterViewInit, IDialogComponent {
    public readonly currentDate: Date = new Date();

    public form!: FormGroup;
    public rolesForm!: FormGroup;

    public model!: AuthorizedPersonDTO;
    public expectedResults: AuthorizedPersonRegixDataDTO;
    public readOnly!: boolean;
    public showOnlyRegixData: boolean = false;
    public isEditing!: boolean;

    public roles: NomenclatureDTO<number>[] = [];
    public allRoles: NomenclatureDTO<number>[] = [];
    public rolesTouched: boolean = false;

    public personRoles: RoleDTO[] = [];

    public getDatesOverlappingErrorTextMethod: GetControlErrorLabelTextCallback = this.getDatesOverlappingErrorText.bind(this);

    private isTouched: boolean = false;

    @ViewChild('roleDataTable')
    private rolesTable!: TLDataTableComponent;

    private rolesService: RolesService;
    private translate: FuseTranslationLoaderService;

    public constructor(rolesService: RolesService, translate: FuseTranslationLoaderService) {
        this.rolesService = rolesService;
        this.translate = translate;

        this.expectedResults = new AuthorizedPersonRegixDataDTO({
            person: new RegixPersonDataDTO()
        });

        this.buildForm();
    }

    public async ngOnInit(): Promise<void> {
        if (!this.showOnlyRegixData) {
            this.allRoles = await this.rolesService.getPublicActiveRoles().toPromise();
        }

        this.roles = this.allRoles.slice();
    }

    public ngAfterViewInit(): void {
        setTimeout(() => {
            if (this.isTouched === false) {
                this.form.valueChanges.subscribe(() => {
                    this.isTouched = true;
                });
            }
        });

        if (!this.showOnlyRegixData) {
            this.rolesForm.valueChanges.subscribe({
                next: () => {
                    const max = (lhs: Date, rhs: Date) => lhs > rhs ? lhs : rhs;
                    const min = (lhs: Date, rhs: Date) => lhs < rhs ? lhs : rhs;

                    const roleId: number = this.rolesForm.get('idControl')!.value;
                    const validFrom: Date = this.rolesForm.get('accessValidFromControl')!.value;
                    const validTo: Date = this.rolesForm.get('accessValidToControl')!.value;

                    if (validFrom && validTo) {
                        const otherEntries: RoleDTO[] = (this.rolesTable.rows as RoleDTO[]).filter(
                            x => x.id === roleId
                                && x.isActive
                                && x.accessValidFrom !== validFrom
                                && x.accessValidTo !== validTo
                        );

                        for (const otherEntry of otherEntries) {
                            if (max(validFrom, otherEntry.accessValidFrom!) <= min(validTo, otherEntry.accessValidTo!)) {
                                this.rolesForm.get('idControl')?.setErrors({ 'datesOverlap': true });
                                break;
                            }
                        }
                    }
                }
            });

            this.rolesForm!.get('idControl')!.valueChanges.subscribe({
                next: () => {
                    this.roles = [...this.allRoles];
                    const currentRoleIds: number[] = this.rolesTable.rows.map(x => x.id);

                    this.roles = this.roles.filter(x => !currentRoleIds.includes(x.value!));
                    this.roles = this.roles.slice();
                }
            });

            this.rolesTable?.recordChanged.subscribe({
                next: (event: RecordChangedEventArgs<RoleDTO>) => {
                    this.rolesTouched = true;

                    if (event.Command !== CommandTypes.Edit) {
                        this.form.updateValueAndValidity({ onlySelf: true });
                    }

                    if (event.Command === CommandTypes.Add || event.Command === CommandTypes.Edit) {
                        if (event.Record.accessValidTo === null || event.Record.accessValidTo === undefined) {
                            event.Record.accessValidTo = new Date(9999, 0, 1);
                        }
                    }
                }
            });
        }
    }

    public setData(data: EditAuthorizedPersonDialogParams, buttons: DialogWrapperData): void {
        this.readOnly = data.readOnly;
        this.showOnlyRegixData = data.showOnlyRegiXData;
        this.isEditing = data.isEgnLncReadOnly;

        if (data.expectedResults) {
            this.expectedResults = data.expectedResults;
        }

        if (data.model === undefined) {
            if (this.showOnlyRegixData) {
                this.model = new AuthorizedPersonRegixDataDTO({ isActive: true });
            }
            else {
                this.model = new AuthorizedPersonDTO({ isActive: true });
            }
        }
        else {
            if (this.readOnly) {
                this.form.disable();
            }

            this.model = data.model;
            this.fillForm();
        }

        if (this.showOnlyRegixData) {
            this.form.markAllAsTouched();
            this.rolesTouched = true;
        }
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        if (this.readOnly) {
            dialogClose(new EditAuthorizedPersonDialogResult(this.model, this.isTouched));
        }
        else {
            this.form.markAllAsTouched();
            this.rolesTouched = true;

            if (this.form.valid || this.showOnlyRegixData) {
                this.fillModel();
                CommonUtils.sanitizeModelStrings(this.model);
                dialogClose(new EditAuthorizedPersonDialogResult(this.model, this.isTouched));
            }
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public getDatesOverlappingErrorText(controlName: string, error: unknown, errorCode: string): TLError | undefined {
        if (controlName === 'idControl') {
            if (errorCode === 'datesOverlap') {
                return new TLError({
                    text: this.translate.getValue('legal-entities-page.dates-overlap-with-other-record'),
                    type: 'error'
                });
            }
        }
        return undefined;
    }

    public downloadedPersonData(person: PersonFullDataDTO): void {
        this.form.get('personControl')!.setValue(person.person);
    }

    private buildForm(): void {
        this.form = new FormGroup({
            personControl: new FormControl()
        }, this.rolesValidator());

        this.rolesForm = new FormGroup({
            idControl: new FormControl(null, Validators.required),
            accessValidFromControl: new FormControl(null, Validators.required),
            accessValidToControl: new FormControl(null)
        });
    }

    private fillForm(): void {
        this.form.get('personControl')!.setValue(this.model.person);

        if (!this.showOnlyRegixData) {
            if (this.model.roles !== undefined && this.model.roles !== null) {
                const roles = this.model.roles;
                setTimeout(() => {
                    this.personRoles = roles;
                });
            }
        }
    }

    private fillModel(): void {
        this.model.person = this.form.get('personControl')!.value;

        if (!this.showOnlyRegixData) {
            if (this.rolesForm.valid && this.rolesTable.rows.length !== 0) {
                this.model.roles = (this.rolesTable.rows as RoleDTO[]).map(x => new RoleDTO({
                    id: x.id,
                    name: this.allRoles.find(y => y.value === x.id)?.displayName,
                    accessValidFrom: x.accessValidFrom,
                    accessValidTo: x.accessValidTo,
                    userRoleId: x.userRoleId,
                    isActive: x.isActive ?? true
                }));
            }
        }
    }

    private rolesValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (this.rolesTable !== null && this.rolesTable !== undefined) {
                const roles: RoleDTO[] = (this.rolesTable.rows as RoleDTO[]) ?? [];
                if (roles.filter(x => x.isActive ?? true).length === 0) {
                    return { 'atLeastOneRoleNeeded': true };
                }
            }
            return null;
        };
    }
}