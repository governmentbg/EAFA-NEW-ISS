import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { forkJoin } from 'rxjs';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { DialogCloseCallback, IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { CrossCheckEditDTO } from '@app/models/generated/dtos/CrossCheckEditDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { CrossChecksService } from '@app/services/administration-app/cross-checks.service';
import { IActionInfo } from '@app/shared/components/dialog-wrapper/interfaces/action-info.interface';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { CrossChecksAutoExecFrequencyEnum } from '@app/enums/cross-checks-auto-exec-frequency.enum';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { ValidityCheckerGroupDirective } from '@app/shared/directives/validity-checker/validity-checker-group.directive';
import { MatSnackBar } from '@angular/material/snack-bar';
import { RequestProperties } from '@app/shared/services/request-properties';

@Component({
    selector: 'edit-cross-check',
    templateUrl: './edit-cross-check.component.html'
})
export class EditCrossCheckComponent implements OnInit, IDialogComponent, AfterViewInit {
    public form!: FormGroup;
    public readOnly: boolean = false;
    public hasError: boolean = true;

    public autoExecFrequencyCodes: NomenclatureDTO<CrossChecksAutoExecFrequencyEnum>[];
    public levels: NomenclatureDTO<number>[];
    public groupsNames: NomenclatureDTO<number>[] = [];

    public users: NomenclatureDTO<number>[] = [];
    public roles: NomenclatureDTO<number>[] = [];

    private readonly commonNomenclaturesService: CommonNomenclatures;
    private readonly service: CrossChecksService;
    private readonly translate: FuseTranslationLoaderService;

    private crossCheckId: number | undefined;
    private model!: CrossCheckEditDTO;
    private snackbar: MatSnackBar;

    @ViewChild(ValidityCheckerGroupDirective)
    private validityCheckerGroup!: ValidityCheckerGroupDirective;

    public constructor(
        service: CrossChecksService,
        commonNomenclaturesService: CommonNomenclatures,
        translate: FuseTranslationLoaderService,
        snackbar: MatSnackBar
    ) {
        this.service = service;
        this.commonNomenclaturesService = commonNomenclaturesService;
        this.translate = translate;
        this.snackbar = snackbar;

        this.levels = [];

        for (let i = 1; i < 6; ++i) {
            this.levels.push(new NomenclatureDTO<number>({
                value: i,
                displayName: `${i}`,
                isActive: true
            }));
        }

        this.autoExecFrequencyCodes = [
            new NomenclatureDTO<CrossChecksAutoExecFrequencyEnum>({
                value: CrossChecksAutoExecFrequencyEnum.Manual,
                displayName: translate.getValue('cross-check.manual'),
                isActive: true
            }),
            new NomenclatureDTO<CrossChecksAutoExecFrequencyEnum>({
                value: CrossChecksAutoExecFrequencyEnum.Daily,
                displayName: translate.getValue('cross-check.daily'),
                isActive: true
            }),
            new NomenclatureDTO<CrossChecksAutoExecFrequencyEnum>({
                value: CrossChecksAutoExecFrequencyEnum.Weekly,
                displayName: translate.getValue('cross-check.weekly'),
                isActive: true
            }),
            new NomenclatureDTO<CrossChecksAutoExecFrequencyEnum>({
                value: CrossChecksAutoExecFrequencyEnum.Monthly,
                displayName: translate.getValue('cross-check.monthly'),
                isActive: true
            }),
            new NomenclatureDTO<CrossChecksAutoExecFrequencyEnum>({
                value: CrossChecksAutoExecFrequencyEnum.Repeating,
                displayName: translate.getValue('cross-check.repeating'),
                isActive: true
            })
        ];

        this.buildform();
    }

    public async ngOnInit(): Promise<void> {
        const nomenclatures: NomenclatureDTO<number>[][] = await forkJoin(
            this.service.getAllReportGroups(),
            this.commonNomenclaturesService.getUserNames(),
            this.service.getActiveRoles()
        ).toPromise();

        this.groupsNames = nomenclatures[0];
        this.users = nomenclatures[1];
        this.roles = nomenclatures[2];

        if (this.crossCheckId === undefined) {
            this.model = new CrossCheckEditDTO();
        }
        else {
            this.service.getCrossCheck(this.crossCheckId).subscribe({
                next: (crossCheck: CrossCheckEditDTO) => {
                    this.model = crossCheck;
                    this.fillForm();
                }
            });
        }
    }

    public ngAfterViewInit(): void {
        this.form.get('queryControl')?.valueChanges.subscribe((value: string) => {
            if (value === "" || value === null) {
                this.hasError = true;
            } else {
                this.hasError = false;
            }
        });
    }

    public saveBtnClicked(actionInfo: IActionInfo, dialogClose: DialogCloseCallback): void {
         if (this.readOnly) {
             dialogClose();
         }

         this.form.markAllAsTouched();
         this.validityCheckerGroup.validate();

         if (this.form.valid && !this.hasError) {
             this.fillModel();
             CommonUtils.sanitizeModelStrings(this.model);

             if (this.crossCheckId !== undefined) {
                 this.service.editCrossCheck(this.model).subscribe({
                     next: () => {
                         dialogClose(this.model);
                     }
                 });
             }
             else {
                 this.service.addCrossCheck(this.model).subscribe({
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
        if (actionInfo.id === 'execute-cross-check') {
            if (this.crossCheckId !== undefined) {
                this.service.executeCrossCheck(this.crossCheckId).subscribe(result => {
                     if (result !== null && result !== undefined) {
                         const message: string = this.translate.getValue('cross-check.execute-cross-check-success-message');
                         this.snackbar.open(message, undefined, {
                             duration: RequestProperties.DEFAULT.showExceptionDurationSucc,
                             panelClass: RequestProperties.DEFAULT.showExceptionColorClassSucc
                         });
                     } 
                });
            }
        }
    }

    public setData(data: DialogParamsModel, buttons: DialogWrapperData): void {
        this.crossCheckId = data?.id;
        this.readOnly = data?.isReadonly ?? false;

        if (this.readOnly) {
            this.form.disable();
        }
    }

    private buildform(): void {
        this.form = new FormGroup({
            nameControl: new FormControl(null, [Validators.required, Validators.maxLength(500)]),
            codeControl: new FormControl(null, [Validators.required, Validators.maxLength(200)]),
            levelControl: new FormControl(null, Validators.required),
            reportGroupControl: new FormControl(null, Validators.required),
            executionDataSourceControl: new FormControl(null, Validators.maxLength(500)),
            executionDataFieldsControl: new FormControl(null, Validators.maxLength(1000)),
            dataSourceCheckControl: new FormControl(null, Validators.maxLength(500)),
            dataFieldsCheckControl: new FormControl(null, Validators.maxLength(1000)),
            purposeControl: new FormControl(null, Validators.maxLength(1000)),
            hasAutomaticExecutionControl: new FormControl(null, Validators.required),
            checkTableNameControl: new FormControl(null, [Validators.required, Validators.maxLength(255)]),
            rolesControl: new FormControl(),
            usersControl: new FormControl(),
            queryControl: new FormControl()
        });


    }

    private fillForm(): void {
        this.form.get('nameControl')!.setValue(this.model.name);
        this.form.get('codeControl')!.setValue(this.model.code);
        this.form.get('levelControl')!.setValue(this.levels.find(x => x.value === this.model.errorLevel));
        this.form.get('reportGroupControl')!.setValue(this.groupsNames.find(x => x.value === this.model.reportGroupId));
        this.form.get('executionDataSourceControl')!.setValue(this.model.dataSource);
        this.form.get('executionDataFieldsControl')!.setValue(this.model.dataSourceColumns);
        this.form.get('dataSourceCheckControl')!.setValue(this.model.checkSource);
        this.form.get('dataFieldsCheckControl')!.setValue(this.model.checkSourceColumns);
        this.form.get('purposeControl')!.setValue(this.model.purpose);
        this.form.get('hasAutomaticExecutionControl')!.setValue(this.autoExecFrequencyCodes.find(x => x.value === this.model.autoExecFrequency));
        this.form.get('checkTableNameControl')!.setValue(this.model.checkTableName);
        this.form.get('rolesControl')!.setValue(this.model.roles);
        this.form.get('usersControl')!.setValue(this.model.users);
        this.form.get('queryControl')!.setValue(this.model.reportSQL);
    }

    private fillModel(): void {
        this.model.name = this.form.get('nameControl')!.value;
        this.model.code = this.form.get('codeControl')!.value;
        this.model.errorLevel = this.form.get('levelControl')!.value.value;
        this.model.dataSource = this.form.get('executionDataSourceControl')!.value;
        this.model.dataSourceColumns = this.form.get('executionDataFieldsControl')!.value;
        this.model.checkSource = this.form.get('dataSourceCheckControl')!.value;
        this.model.checkSourceColumns = this.form.get('dataFieldsCheckControl')!.value;
        this.model.purpose = this.form.get('purposeControl')!.value;
        this.model.autoExecFrequency = this.form.get('hasAutomaticExecutionControl')!.value.value;
        this.model.checkTableName = this.form.get('checkTableNameControl')!.value;
        this.model.reportSQL = 'Report sql';
        this.model.users = this.form.get('usersControl')!.value;
        this.model.roles = this.form.get('rolesControl')!.value;
        this.model.reportSQL = this.form.get('queryControl')!.value;

        const reportGroup: string | NomenclatureDTO<number> = this.form.get('reportGroupControl')!.value;
        if (typeof reportGroup === 'string') {
            this.model.reportGroupId = undefined;
            this.model.isNewReportGroup = true;
            this.model.reportGroupName = reportGroup;
        }
        else {
            this.model.isNewReportGroup = false;
            this.model.reportGroupId = reportGroup.value;
        }
    }
}
