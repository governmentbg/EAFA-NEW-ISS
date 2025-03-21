import { Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { forkJoin } from 'rxjs';

import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { ILogBookPageEditExceptionsService } from '@app/interfaces/administration-app/log-book-page-edit-exceptions.interface';
import { LogBookPageEditExceptionsService } from '@app/services/administration-app/log-book-page-edit-exceptions.service';
import { LogBookPageEditExceptionEditDTO } from '@app/models/generated/dtos/LogBookPageEditExceptionEditDTO';
import { SystemUserNomenclatureDTO } from '@app/models/generated/dtos/SystemUserNomenclatureDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { NomenclatureStore } from '@app/shared/utils/nomenclatures.store';
import { NomenclatureTypes } from '@app/enums/nomenclature.types';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { IGroupedOptions } from '@app/shared/components/input-controls/tl-autocomplete/interfaces/grouped-options.interface';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { LogBookPageEditNomenclatureDTO } from '@app/models/generated/dtos/LogBookPageEditNomenclatureDTO';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';
import { EditLogBookPageExceptionParameters } from '../../models/edit-log-book-page-exception.model';

@Component({
    selector: 'edit-log-book-page-edit-exception',
    templateUrl: './edit-log-book-page-edit-exception.component.html'
})
export class EditLogBookPageEditExceptionComponent implements OnInit, IDialogComponent {
    public form!: FormGroup;
    public users: IGroupedOptions<number>[] = [];
    public logBookTypes: NomenclatureDTO<number>[] = [];
    public activeLogBooks: LogBookPageEditNomenclatureDTO[] = [];
    public logBooks: LogBookPageEditNomenclatureDTO[] = [];

    public viewMode: boolean = true;

    public readonly pageCode: PageCodeEnum = PageCodeEnum.LogBookPageException;
    public readonly service: ILogBookPageEditExceptionsService;

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    private allLogBookTypes: NomenclatureDTO<number>[] = [];
    private allActiveLogBooks: LogBookPageEditNomenclatureDTO[] = [];
    private allLogBooks: LogBookPageEditNomenclatureDTO[] = [];
    private filteredLogBooks: LogBookPageEditNomenclatureDTO[] = [];

    private model!: LogBookPageEditExceptionEditDTO;
    private id: number | undefined;
    private isCopy: boolean = false;

    private readonly commonNomenclatures: CommonNomenclatures;
    private readonly translationService: FuseTranslationLoaderService;

    public constructor(
        service: LogBookPageEditExceptionsService,
        commonNomenclatures: CommonNomenclatures,
        translationService: FuseTranslationLoaderService
    ) {
        this.service = service;
        this.commonNomenclatures = commonNomenclatures;
        this.translationService = translationService;

        this.buildForm();
    }

    public async ngOnInit(): Promise<void> {
        const nomenclatures: (SystemUserNomenclatureDTO[] | NomenclatureDTO<number>[] | LogBookPageEditNomenclatureDTO[])[] = await forkJoin([
            this.service.getAllUsersNomenclature(),
            NomenclatureStore.instance.getNomenclature(NomenclatureTypes.LogBookTypes, this.commonNomenclatures.getLogBookTypes.bind(this.commonNomenclatures), false),
            this.service.getActiveLogBooksNomenclature(this.id)
        ]).toPromise();

        this.users = [
            {
                name: this.translationService.getValue('log-book-page-edit-exceptions.internal-users-group'),
                options: (nomenclatures[0] as SystemUserNomenclatureDTO[]).filter(x => x.isInternalUser === true)
            },
            {
                name: this.translationService.getValue('log-book-page-edit-exceptions.external-users-group'),
                options: (nomenclatures[0] as SystemUserNomenclatureDTO[]).filter(x => x.isInternalUser === false)
            }
        ];

        this.allLogBookTypes = nomenclatures[1];
        this.logBookTypes = this.allLogBookTypes.slice();

        this.allLogBooks = nomenclatures[2];
        this.allActiveLogBooks = (nomenclatures[2] as LogBookPageEditNomenclatureDTO[]).filter(x => x.isFinishedOrSuspended === false);
        this.activeLogBooks = this.allActiveLogBooks.slice();
        this.filteredLogBooks = this.allLogBooks.slice();
        this.logBooks = this.allActiveLogBooks.slice();

        if (this.id !== null && this.id !== undefined) {
            this.service.getLogBookPageEditException(this.id).subscribe({
                next: (result: LogBookPageEditExceptionEditDTO) => {
                    this.model = result;

                    this.fillForm();
                }
            });
        }
    }

    public setData(data: EditLogBookPageExceptionParameters, wrapperData: DialogWrapperData): void {
        this.id = data.id;
        this.viewMode = data.viewMode;
        this.isCopy = data.isCopy;

        if (this.id === null || this.id === undefined) {
            this.model = new LogBookPageEditExceptionEditDTO();
        }

        if (this.viewMode) {
            this.form.disable();
        }
    }

    public dialogButtonClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        this.form.markAllAsTouched();
        this.validityCheckerGroup.validate();

        if (this.form.valid) {
            this.fillModel();
            this.model = CommonUtils.sanitizeModelStrings(this.model);

            if (this.id === null || this.id === undefined || this.isCopy) { // add
                this.service.addLogBookPageEditException(this.model).subscribe({
                    next: () => {
                        dialogClose(this.model);
                    }
                });
            }
            else { // edit
                this.service.editLogBookPageEditException(this.model).subscribe({
                    next: () => {
                        dialogClose(this.model);
                    }
                });
            }
        }
    }

    public cancelBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
        dialogClose();
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
            userControl: new FormControl(),
            logBookTypeControl: new FormControl(),
            logBookControl: new FormControl(),
            exceptionActiveFromControl: new FormControl(exceptionActiveFrom),
            exceptionActiveToControl: new FormControl(exceptionActiveTo),
            editPageFromControl: new FormControl(),
            editPageToControl: new FormControl(),
            showFinshedOrSuspendedControl: new FormControl(false),
            filesControl: new FormControl(null)
        });

        // validators

        this.form.get('exceptionActiveFromControl')!.setValidators([Validators.required, TLValidators.maxDate(this.form.get('exceptionActiveToControl')!)]);
        this.form.get('exceptionActiveToControl')!.setValidators([Validators.required, TLValidators.minDate(this.form.get('exceptionActiveFromControl')!)]);
        this.form.get('editPageFromControl')!.setValidators([Validators.required, TLValidators.maxDate(this.form.get('editPageToControl')!)]);
        this.form.get('editPageToControl')!.setValidators([Validators.required, TLValidators.minDate(this.form.get('editPageFromControl')!)]);

        // value changes subscribe

        this.form.get('logBookTypeControl')!.valueChanges.subscribe({
            next: (logBookType: NomenclatureDTO<number> | null | undefined | string) => {
                const showFinishedOrSuspended: boolean = this.form.get('showFinshedOrSuspendedControl')!.value;

                if (logBookType !== null && logBookType !== undefined && logBookType instanceof NomenclatureDTO) {
                    this.activeLogBooks = this.allActiveLogBooks.filter(x => x.logBookTypeCode === logBookType.code);
                    this.filteredLogBooks = this.allLogBooks.filter(x => x.logBookTypeCode === logBookType.code);

                    const selectedLogBook: NomenclatureDTO<number> | null | undefined | string = this.form.get('logBookControl')!.value;

                    if (selectedLogBook !== null && selectedLogBook !== undefined && selectedLogBook instanceof NomenclatureDTO) {
                        if (!showFinishedOrSuspended && !this.activeLogBooks.some(x => x.value === selectedLogBook.value)) {
                            this.form.get('logBookControl')!.setValue(undefined, { emitEvent: false });
                        }
                        else if (showFinishedOrSuspended && !this.filteredLogBooks.some(x => x.value === selectedLogBook.value)) {
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

                    const selectedLogBookType: NomenclatureDTO<number> | null | undefined | string = this.form.get('logBookTypeControl')!.value;

                    if (selectedLogBookType !== null && selectedLogBookType !== undefined && selectedLogBookType instanceof NomenclatureDTO) {
                        if (!this.logBookTypes.some(x => x.value === selectedLogBookType.value)) {
                            this.form.get('logBookTypeControl')!.setValue(undefined, { emitEvent: false });
                        }
                    }

                    if (this.logBookTypes.length === 1) {
                        this.form.get('logBookTypeControl')!.setValue(this.logBookTypes[0]);
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
        if (this.model.userId !== null && this.model.userId !== undefined) {
            const userGroup = this.users.find(x => (x.options as SystemUserNomenclatureDTO[]).some(y => y.value === this.model.userId))?.options;
            this.form.get('userControl')!.setValue((userGroup as SystemUserNomenclatureDTO[] | undefined)?.find(x => x.value === this.model.userId));
        }

        if (this.model.logBookTypeId !== null && this.model.logBookTypeId !== undefined) {
            this.form.get('logBookTypeControl')!.setValue(this.logBookTypes.find(x => x.value === this.model.logBookTypeId));
        }

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
    }

    private fillModel(): void {
        this.model.userId = this.form.get('userControl')!.value?.value;
        this.model.logBookTypeId = this.form.get('logBookTypeControl')!.value?.value;
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

        if (this.isCopy) {
            this.model.id = undefined;
        }
    }
}