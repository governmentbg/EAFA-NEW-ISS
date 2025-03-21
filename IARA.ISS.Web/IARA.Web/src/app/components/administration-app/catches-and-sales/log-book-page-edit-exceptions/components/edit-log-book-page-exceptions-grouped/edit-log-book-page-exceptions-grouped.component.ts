import { Component, OnInit, ViewChild } from '@angular/core';
import { forkJoin } from 'rxjs';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { LogBookPageExceptionGroupedEditDTO } from '@app/models/generated/dtos/LogBookPageExceptionGroupedEditDTO';
import { EditLogBookPageExceptionsGroupedParameters } from '../../models/edit-log-book-page-exceptions-grouped.model';
import { ILogBookPageEditExceptionsService } from '@app/interfaces/administration-app/log-book-page-edit-exceptions.interface';
import { LogBookPageEditExceptionsService } from '@app/services/administration-app/log-book-page-edit-exceptions.service';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';
import { IGroupedOptions } from '@app/shared/components/input-controls/tl-autocomplete/interfaces/grouped-options.interface';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { LogBookPageEditNomenclatureDTO } from '@app/models/generated/dtos/LogBookPageEditNomenclatureDTO';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { SystemUserNomenclatureDTO } from '@app/models/generated/dtos/SystemUserNomenclatureDTO';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { CommonUtils } from '@app/shared/utils/common.utils';

const USERS_PER_PAGE: number = 10;

@Component({
    selector: 'edit-log-book-page-exceptions-grouped',
    templateUrl: './edit-log-book-page-exceptions-grouped.component.html'
})
export class EditLogBookPageExceptionsGroupedComponent implements OnInit, IDialogComponent {
    public form!: FormGroup;
    public users: IGroupedOptions<number>[] = [];
    public allUsers: SystemUserNomenclatureDTO[] = [];
    public selectedUsers: SystemUserNomenclatureDTO[] = [];
    public logBookTypes: NomenclatureDTO<number>[] = [];
    public activeLogBooks: LogBookPageEditNomenclatureDTO[] = [];
    public logBooks: LogBookPageEditNomenclatureDTO[] = [];

    public viewMode: boolean = true;

    public readonly usersPerPage: number;
    public readonly pageCode: PageCodeEnum = PageCodeEnum.LogBookPageException;
    public readonly service: ILogBookPageEditExceptionsService;

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    private allLogBookTypes: NomenclatureDTO<number>[] = [];
    private allActiveLogBooks: LogBookPageEditNomenclatureDTO[] = [];
    private allLogBooks: LogBookPageEditNomenclatureDTO[] = [];
    private filteredLogBooks: LogBookPageEditNomenclatureDTO[] = [];

    private ids: number[] | undefined;
    private isCopy: boolean = false;
    private model!: LogBookPageExceptionGroupedEditDTO;

    private readonly commonNomenclatures: CommonNomenclatures;
    private readonly translate: FuseTranslationLoaderService;

    public constructor(
        service: LogBookPageEditExceptionsService,
        commonNomenclatures: CommonNomenclatures,
        translate: FuseTranslationLoaderService
    ) {
        this.service = service;
        this.commonNomenclatures = commonNomenclatures;
        this.translate = translate;

        this.usersPerPage = USERS_PER_PAGE;

        this.buildForm();
    }

    public async ngOnInit(): Promise<void> {
        const nomenclatures: (SystemUserNomenclatureDTO[] | NomenclatureDTO<number>[] | LogBookPageEditNomenclatureDTO[])[] = await forkJoin([
            this.service.getAllUsersNomenclature(),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.LogBookTypes, this.commonNomenclatures.getLogBookTypes.bind(this.commonNomenclatures), false),
            this.service.getActiveLogBooksNomenclature(undefined)
        ]).toPromise();

        this.users = [
            {
                name: this.translate.getValue('log-book-page-edit-exceptions.internal-users-group'),
                options: (nomenclatures[0] as SystemUserNomenclatureDTO[]).filter(x => x.isInternalUser === true)
            },
            {
                name: this.translate.getValue('log-book-page-edit-exceptions.external-users-group'),
                options: (nomenclatures[0] as SystemUserNomenclatureDTO[]).filter(x => x.isInternalUser === false)
            }
        ];

        this.allUsers = nomenclatures[0];
        this.allLogBookTypes = nomenclatures[1];

        this.allLogBooks = nomenclatures[2];
        this.allActiveLogBooks = (nomenclatures[2] as LogBookPageEditNomenclatureDTO[]).filter(x => x.isFinishedOrSuspended === false);

        this.logBookTypes = this.allLogBookTypes.slice();
        this.activeLogBooks = this.allActiveLogBooks.slice();
        this.filteredLogBooks = this.allLogBooks.slice();
        this.logBooks = this.allActiveLogBooks.slice();

        if (this.ids !== undefined && this.ids !== null && this.ids.length > 0) {
            this.service.getLogBookPagesExceptionsGrouped(this.ids).subscribe({
                next: (model: LogBookPageExceptionGroupedEditDTO) => {
                    this.model = model;
                    this.fillForm();
                }
            })
        }
    }

    public setData(data: EditLogBookPageExceptionsGroupedParameters, wrapperData: DialogWrapperData): void {
        this.viewMode = data.viewMode;
        this.isCopy = data.isCopy;

        if (data.model !== undefined && data.model !== null) {
            this.ids = data.model.logBookPageExceptionIds;
        }
        else {
            this.model = new LogBookPageExceptionGroupedEditDTO();
        }

        if (this.viewMode) {
            this.form.disable();
        }
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.form.markAllAsTouched();
        this.validityCheckerGroup.validate();

        if (this.form.valid) {
            this.fillModel();
            this.model = CommonUtils.sanitizeModelStrings(this.model);

            if (this.ids === null || this.ids === undefined || this.ids.length === 0 || this.isCopy) { // add
                this.service.addLogBookPageExceptionsGrouped(this.model).subscribe({
                    next: () => {
                        dialogClose(this.model);
                    }
                });
            }
            else { // edit
                this.service.editLogBookPageExceptionsGrouped(this.model).subscribe({
                    next: () => {
                        dialogClose(this.model);
                    }
                });
            }
        }
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public removeUser(row: SystemUserNomenclatureDTO): void {
        this.selectedUsers = this.selectedUsers.filter(x => x.value !== row.value).slice();
        this.filterUsersNomenclature();
    }

    private buildForm(): void {
        const exceptionActiveFrom: Date = new Date();
        exceptionActiveFrom.setHours(0);
        exceptionActiveFrom.setMinutes(0);
        exceptionActiveFrom.setSeconds(0);
        exceptionActiveFrom.setMilliseconds(0);

        const exceptionActiveTo: Date = new Date();
        exceptionActiveTo.setHours(23);
        exceptionActiveTo.setMinutes(59);
        exceptionActiveTo.setSeconds(59);
        exceptionActiveTo.setMilliseconds(0);

        this.form = new FormGroup({
            usersControl: new FormControl(),
            logBookTypesControl: new FormControl(),
            logBookControl: new FormControl(),
            exceptionActiveFromControl: new FormControl(exceptionActiveFrom),
            exceptionActiveToControl: new FormControl(exceptionActiveTo),
            editPageFromControl: new FormControl(),
            editPageToControl: new FormControl(),
            showFinshedOrSuspendedControl: new FormControl(false),
            filesControl: new FormControl(null)
        });

        this.form.get('exceptionActiveFromControl')!.setValidators([Validators.required, TLValidators.maxDate(this.form.get('exceptionActiveToControl')!)]);
        this.form.get('exceptionActiveToControl')!.setValidators([Validators.required, TLValidators.minDate(this.form.get('exceptionActiveFromControl')!)]);
        this.form.get('editPageFromControl')!.setValidators([Validators.required, TLValidators.maxDate(this.form.get('editPageToControl')!)]);
        this.form.get('editPageToControl')!.setValidators([Validators.required, TLValidators.minDate(this.form.get('editPageFromControl')!)]);

        this.form.get('logBookTypesControl')!.valueChanges.subscribe({
            next: (logBookTypes: NomenclatureDTO<number>[] | null | undefined | string) => {
                const showFinishedOrSuspended: boolean = this.form.get('showFinshedOrSuspendedControl')!.value;

                if (logBookTypes !== null && logBookTypes !== undefined && logBookTypes && Array.isArray(logBookTypes) && logBookTypes.length > 0) {
                    const logBookTypeCodes: string[] = logBookTypes.map(x => x.code!);

                    this.activeLogBooks = this.allActiveLogBooks.filter(x => logBookTypeCodes.includes(x.logBookTypeCode!));
                    this.filteredLogBooks = this.allLogBooks.filter(x => logBookTypeCodes.includes(x.logBookTypeCode!));

                    const selectedLogBook: NomenclatureDTO<number> | null | undefined | string = this.form.get('logBookControl')!.value;

                    if (selectedLogBook !== null && selectedLogBook !== undefined && selectedLogBook instanceof NomenclatureDTO) {
                        if (!showFinishedOrSuspended && !this.activeLogBooks.some(x => x.value === selectedLogBook.value)) {
                            this.form.get('logBookControl')!.setValue(undefined, { emitEvent: false });
                        }
                        else if (showFinishedOrSuspended && !this.filteredLogBooks.some(x => x.value === selectedLogBook.value)) {
                            this.form.get('logBookControl')!.setValue(undefined, { emitEvent: false });
                        }
                        else if (logBookTypes.length > 1) {
                            this.form.get('logBookControl')!.setValue(undefined, { emitEvent: false });
                        }
                    }
                }
                else {
                    this.activeLogBooks = this.allActiveLogBooks.slice();
                    this.filteredLogBooks = this.allLogBooks.slice();
                }

                if (showFinishedOrSuspended) {
                    this.logBooks = this.filteredLogBooks.slice();
                }
                else {
                    this.logBooks = this.activeLogBooks.slice();
                }
            }
        });

        this.form.get('logBookControl')!.valueChanges.subscribe({
            next: (logBook: LogBookPageEditNomenclatureDTO | null | undefined | string) => {
                if (logBook !== null && logBook !== undefined && logBook instanceof NomenclatureDTO) {
                    this.logBookTypes = this.allLogBookTypes.filter(x => x.code === logBook.logBookTypeCode);

                    const selectedLogBookTypes: NomenclatureDTO<number>[] | null | undefined | string = this.form.get('logBookTypesControl')!.value;

                    if (selectedLogBookTypes !== null && selectedLogBookTypes !== undefined && Array.isArray(selectedLogBookTypes) && selectedLogBookTypes.length > 0) {
                        const logBookTypeIds: number[] = selectedLogBookTypes.map(x => x.value!);

                        if (!this.logBookTypes.some(x => logBookTypeIds.includes(x.value!))) {
                            this.form.get('logBookTypesControl')!.setValue(undefined, { emitEvent: false });
                        }
                    }

                    if (this.logBookTypes.length === 1) {
                        this.form.get('logBookTypesControl')!.setValue(this.logBookTypes);
                    }
                }
                else {
                    this.logBookTypes = this.allLogBookTypes.slice();
                }
            }
        });

        this.form.get('exceptionActiveFromControl')!.valueChanges.subscribe({
            next: () => {
                this.form.get('exceptionActiveToControl')!.updateValueAndValidity({ emitEvent: false });
            }
        });

        this.form.get('exceptionActiveToControl')!.valueChanges.subscribe({
            next: () => {
                this.form.get('exceptionActiveFromControl')!.updateValueAndValidity({ emitEvent: false });
            }
        });

        this.form.get('editPageFromControl')!.valueChanges.subscribe({
            next: () => {
                this.form.get('editPageToControl')!.updateValueAndValidity({ emitEvent: false });
            }
        });

        this.form.get('editPageToControl')!.valueChanges.subscribe({
            next: () => {
                this.form.get('editPageFromControl')!.updateValueAndValidity({ emitEvent: false });
            }
        });

        this.form.get('usersControl')!.valueChanges.subscribe({
            next: (value: SystemUserNomenclatureDTO | string | undefined) => {
                if (value !== undefined && value !== null && typeof value !== 'string') {
                    this.updateSelectedUsers(value);
                }
            }
        });

        this.form.get('showFinshedOrSuspendedControl')!.valueChanges.subscribe({
            next: (value: boolean) => {
                if (value) {
                    this.logBooks = this.filteredLogBooks;
                }
                else {
                    this.logBooks = this.activeLogBooks;
                }

                this.logBooks = this.logBooks.slice();

                const selectedLogBook: NomenclatureDTO<number> | null | undefined | string = this.form.get('logBookControl')!.value;

                if (selectedLogBook !== null && selectedLogBook !== undefined && selectedLogBook instanceof NomenclatureDTO) {
                    if (!this.logBooks.some(x => x.value === selectedLogBook.value)) {
                        this.form.get('logBookControl')!.setValue(undefined, { emitEvent: false });
                    }
                }
            }
        });
    }

    private fillForm(): void {
        const logBookTypes: NomenclatureDTO<number>[] = this.logBookTypes.filter(x => this.model.logBookTypeIds?.includes(x.value!)) ?? [];
        this.form.get('logBookTypesControl')!.setValue(logBookTypes);

        if (this.model.logBookId !== null && this.model.logBookId !== undefined) {
            const logBook: LogBookPageEditNomenclatureDTO | undefined = this.allLogBooks.find(x => x.value === this.model.logBookId);

            if (logBook !== undefined && logBook !== null) {
                this.form.get('showFinshedOrSuspendedControl')!.setValue(logBook.isFinishedOrSuspended);
            }

            this.form.get('logBookControl')!.setValue(this.logBooks.find(x => x.value === this.model.logBookId));
        }

        this.form.get('exceptionActiveFromControl')!.setValue(this.model.exceptionActiveFrom);
        this.form.get('exceptionActiveToControl')!.setValue(this.model.exceptionActiveTo);
        this.form.get('editPageFromControl')!.setValue(this.model.editPageFrom);
        this.form.get('editPageToControl')!.setValue(this.model.editPageTo);
        this.form.get('filesControl')!.setValue(this.model.files);

        if (this.model.userIds !== undefined && this.model.userIds !== null && this.model.userIds.length > 0) {
            this.updateSelectedUsers(this.allUsers.filter(
                (x: SystemUserNomenclatureDTO) => {
                    return x.value !== null && x.value !== undefined && this.model.userIds!.includes(x.value!);
                }
            ));
        }
    }

    private fillModel(): void {
        this.model.logBookId = this.form.get('logBookControl')!.value?.value;

        this.model.exceptionActiveFrom = this.form.get('exceptionActiveFromControl')!.value;
        this.model.exceptionActiveTo = this.form.get('exceptionActiveToControl')!.value;

        this.model.files = this.form.get('filesControl')!.value;

        this.model.editPageFrom = this.form.get('editPageFromControl')!.value;
        this.model.editPageFrom!.setHours(0);
        this.model.editPageFrom!.setMinutes(0);
        this.model.editPageFrom!.setSeconds(0);
        this.model.editPageFrom!.setMilliseconds(0);

        this.model.editPageTo = this.form.get('editPageToControl')!.value;
        this.model.editPageTo!.setHours(23);
        this.model.editPageTo!.setMinutes(59);
        this.model.editPageTo!.setSeconds(59);
        this.model.editPageTo!.setMilliseconds(0);

        this.model.userIds = [];
        if (this.selectedUsers !== undefined && this.selectedUsers !== null && this.selectedUsers.length > 0) {
            for (const el of this.selectedUsers) {
                this.model.userIds.push(el.value!);
            }
        }

        this.model.logBookTypeIds = [];
        const logBookTypes: NomenclatureDTO<number>[] | null | undefined | string = this.form.get('logBookTypesControl')!.value;
        if (logBookTypes !== undefined && logBookTypes !== null && logBookTypes.length > 0) {
            for (const el of logBookTypes as NomenclatureDTO<number>[]) {
                this.model.logBookTypeIds.push(el.value!);
            }
        }

        if (this.isCopy) {
            this.model.logBookPageExceptionIds = [];
        }
        else {
            this.model.logBookPageExceptionIds = this.ids ?? [];
        }
    }

    private updateSelectedUsers(users: SystemUserNomenclatureDTO[] | SystemUserNomenclatureDTO | undefined): void {
        if (Array.isArray(users)) {
            this.selectedUsers = this.selectedUsers.concat(users);
        }
        else if (users instanceof SystemUserNomenclatureDTO) {
            this.selectedUsers.push(users);
        }

        setTimeout(() => {
            this.selectedUsers = this.selectedUsers.slice();
        });

        this.filterUsersNomenclature();
        this.form.get('usersControl')!.setValue(undefined);
    }

    private filterUsersNomenclature(): void {
        const users: IGroupedOptions<number>[] = [
            {
                name: this.translate.getValue('log-book-page-edit-exceptions.internal-users-group'),
                options: (this.allUsers as SystemUserNomenclatureDTO[]).filter(x => x.isInternalUser === true && !this.selectedUsers.includes(x))
            },
            {
                name: this.translate.getValue('log-book-page-edit-exceptions.external-users-group'),
                options: (this.allUsers as SystemUserNomenclatureDTO[]).filter(x => x.isInternalUser === false && !this.selectedUsers.includes(x))
            }
        ];

        this.users = users.slice();
    }
}