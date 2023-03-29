import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { ReportAdministrationService } from '@app/services/administration-app/report-administration.service';
import { CommonNomenclatures } from '@app/services/common-app/common-nomenclatures.service';
import { NestedTreeControl } from '@angular/cdk/tree';
import { MatTreeNestedDataSource } from '@angular/material/tree';
import { TableNodeDTO } from '@app/models/generated/dtos/TableNodeDTO';
import { ReportDTO } from '@app/models/generated/dtos/ReportDTO';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { Router } from '@angular/router';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { EditReportDefinitionComponent } from './edit-report-definition.component';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { DialogCloseCallback } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { ReportParameterDTO } from '@app/models/generated/dtos/ReportParameterDTO';
import { ReportDefinitionDialogParams } from './models/report-definition-dialog-params.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { GridRow } from '@app/shared/components/data-table/models/row.model';
import { ReportTypesEnum } from '@app/enums/reports-type.enum';
import { ExecutionReportInfoDTO } from '@app/models/generated/dtos/ExecutionReportInfoDTO';
import { ExecutionParamDTO } from '@app/models/generated/dtos/ExecutionParamDTO';
import { HttpErrorResponse } from '@angular/common/http';
import { ErrorCode, ErrorModel } from '@app/models/common/exception.model';
import { ErrorSnackbarComponent } from '@app/shared/components/error-snackbar/error-snackbar.component';
import { RequestProperties } from '@app/shared/services/request-properties';
import { MatSnackBar } from '@angular/material/snack-bar';
import { IReportService } from '@app/interfaces/administration-app/report.interface';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { TLValidators } from '@app/shared/utils/tl-validators';

@Component({
    selector: 'report-definition',
    templateUrl: './report-definition.component.html',
    styleUrls: ['./report-definition.component.scss']
})
export class ReportDefinitionComponent implements OnInit {
    public report!: ReportDTO;
    public reportGroupId: number | undefined;
    public isMenuHidden: boolean;
    public treeControl = new NestedTreeControl<TableNodeDTO>(node => node.children);
    public dataSource = new MatTreeNestedDataSource<TableNodeDTO>();
    public maxLayoutWidth: number | undefined;
    public viewMode: boolean = false;
    public auditInfo: SimpleAuditDTO | undefined;
    public auditBtn: IHeaderAuditButton | undefined;

    public iconNameControl: FormControl = new FormControl();

    public formGroup: FormGroup;

    public users: NomenclatureDTO<number>[] = [];
    public selectedUsers: NomenclatureDTO<number>[] = [];

    public roles: NomenclatureDTO<number>[] = [];
    public selectedRoles: NomenclatureDTO<number>[] = [];
    public reportGroups: NomenclatureDTO<number>[] = [];

    public reportTypes: string[];
    public reportInfo!: ExecutionReportInfoDTO;

    @ViewChild('container')
    public container!: ElementRef;

    @ViewChild('parametersTable')
    private parametersTable!: TLDataTableComponent;

    private readonly router: Router;
    private readonly translateService: FuseTranslationLoaderService;
    private readonly commonNomenclaturesService: CommonNomenclatures;
    private readonly reportService: IReportService;
    private readonly editDialog: TLMatDialog<EditReportDefinitionComponent>;
    private readonly confirmDialog: TLConfirmDialog;
    private readonly snackbar: MatSnackBar;

    private allUsers!: NomenclatureDTO<number>[];
    private allRoles!: NomenclatureDTO<number>[];

    public constructor(
        router: Router,
        translateService: FuseTranslationLoaderService,
        commonNomenclaturesService: CommonNomenclatures,
        reportDefinitionService: ReportAdministrationService,
        editDialog: TLMatDialog<EditReportDefinitionComponent>,
        confirmDialog: TLConfirmDialog,
        snackbar: MatSnackBar
    ) {
        this.router = router;
        this.translateService = translateService;
        this.commonNomenclaturesService = commonNomenclaturesService;
        this.reportService = reportDefinitionService;
        this.editDialog = editDialog;
        this.confirmDialog = confirmDialog;
        this.snackbar = snackbar;

        this.isMenuHidden = false;
        this.selectedUsers = [];
        this.selectedRoles = [];
        this.report = new ReportDTO();
        this.report.parameters = [];

        this.reportTypes = [
            ReportTypesEnum[ReportTypesEnum.SQL],
            ReportTypesEnum[ReportTypesEnum.JasperPDF],
            ReportTypesEnum[ReportTypesEnum.JasperWord]
        ];

        this.formGroup = new FormGroup({
            generalInformationGroup: new FormGroup({
                titleControl: new FormControl(null, [Validators.required, Validators.maxLength(500)]),
                codeControl: new FormControl(null, [Validators.required, Validators.maxLength(200)]),
                descriptionControl: new FormControl(null, Validators.maxLength(1000)),
                lastRunUserControl: new FormControl({ value: null, disabled: true }),
                lastRunDateTimeControl: new FormControl({ value: null, disabled: true }),
                lastRunDurationSecControl: new FormControl({ value: null, disabled: true }),
                reportTypeControl: new FormControl(null, Validators.required),
                reportGroupControl: new FormControl(null, Validators.required),
                orderNumControl: new FormControl(null, TLValidators.number(0, undefined, 0)),
                iconNameControl: this.iconNameControl
            }),
            accessManagementGroup: new FormGroup({
                selectedRolesControl: new FormControl(),
                selectedUsersControl: new FormControl()
            }),
            queryControl: new FormControl()
        });

    }

    public hasChild = (_: number, node: TableNodeDTO): boolean => !!node.children && node.children.length > 0;

    public async ngOnInit(): Promise<void> {
        this.dataSource.data = await this.reportService.getTableNodes().toPromise();

        this.reportGroups = await this.reportService.getReportGroups().toPromise();

        this.allUsers = await this.commonNomenclaturesService.getUserNames().toPromise();
        this.users = this.allUsers;

        this.allRoles = await this.reportService.getActiveRoles().toPromise();
        this.roles = this.allRoles;

        const id: number = window.history.state?.id;
        const isGroupId: boolean = window.history.state?.isGroupId;
        const viewMode: boolean = window.history.state?.viewMode;

        if (isGroupId !== undefined) {
            this.viewMode = viewMode;

            if (!isGroupId) {
                this.report = await this.reportService.getReport(id).toPromise();
                this.fillForm(this.report);

                this.reportInfo = new ExecutionReportInfoDTO({ reportId: this.report.id });
                this.reportInfo.parameters = this.getReportInfoParams();
            }
            else {
                this.reportGroupId = id;
                (this.formGroup.controls.generalInformationGroup as FormGroup).controls.reportGroupControl.setValue(this.reportGroups.find(x => x.value === this.reportGroupId));
            }

            if (viewMode) {
                this.formGroup.disable();
            }
        }
        else {
            this.router.navigateByUrl('/reports');
        }
        this.maxLayoutWidth = this.container.nativeElement.offsetWidth;
    }

    public onIconPickerSelect(icon: string): void {
        this.iconNameControl.setValue(icon);
    }

    public hideMenu(): void {
        this.isMenuHidden = !this.isMenuHidden;
    }

    public auditBtnClicked(): void {
        if (this.report.id !== undefined && this.report.id !== null) {
            this.reportService.getReportAudit(this.report.id).subscribe({
                next: (result: SimpleAuditDTO) => {
                    this.auditInfo = result;
                }
            });
        }
    }

    public detailedAuditClicked(): void {
        if (this.report.id !== undefined && this.report.id !== null) {
            this.router.navigateByUrl('/system-log', {
                state: {
                    tableId: this.report.id.toString(),
                    tableName: 'Report'
                }
            });
        }
    }

    public addOrEditParameter(parameter?: ReportParameterDTO, viewMode: boolean = false): void {
        let title: string = this.translateService.getValue('report-definition.dialog-parameter-edit-title');
        let auditButtons: IHeaderAuditButton | undefined;
        let data: ReportDefinitionDialogParams | undefined;
        const parameterId: number | undefined = parameter?.id;

        //edit
        if (parameter !== undefined) {
            if (viewMode) title = this.translateService.getValue('report-definition.dialog-parameter-view-title');

            data = new ReportDefinitionDialogParams({
                parameter: parameter,
                viewMode: viewMode
            });

            if (parameterId !== undefined) {
                auditButtons = {
                    getAuditRecordData: this.reportService.getReportParametersAudit.bind(this.reportService),
                    id: parameterId,
                    tableName: 'ReportParameter',
                } as IHeaderAuditButton;
            }
        }
        //add
        else {
            title = this.translateService.getValue('report-definition.dialog-parameter-add-title');
        }

        const dialog = this.editDialog.openWithTwoButtons({
            TCtor: EditReportDefinitionComponent,
            title: title,
            componentData: data,
            headerAuditButton: auditButtons,
            translteService: this.translateService,
            headerCancelButton: {
                cancelBtnClicked: this.closeDialogBtnClicked.bind(this)
            },
            viewMode: viewMode
        }, '1000px');

        dialog.subscribe({
            next: (inputParameter: ReportParameterDTO | undefined) => {
                //ако се натисне 'Отказ' в диалога
                if (inputParameter !== undefined && inputParameter !== null) {
                    //edit
                    if (parameter !== undefined && parameter !== null) {
                        parameter = inputParameter;
                    }
                    //add
                    else {
                        this.report.parameters!.push(inputParameter);
                    }

                    this.report.parameters = this.report.parameters?.slice();
                    this.refreshParametersTable();
                }
            }
        });
    }

    public deleteParameter(parameter: GridRow<ReportParameterDTO>): void {
        this.confirmDialog.open({
            title: this.translateService.getValue('report-definition.dialog-parameter-delete-title'),
            message: this.translateService.getValue('report-definition.dialog-parameter-delete-message'),
            okBtnLabel: this.translateService.getValue('report-definition.dialog-parameter-delete-button')
        }).subscribe({
            next: (result: boolean) => {
                if (result) {
                    this.parametersTable.softDelete(parameter);
                }
            }
        });
    }

    public undoDeletedParameter(parameter: GridRow<ReportParameterDTO>): void {
        this.confirmDialog.open({
            title: this.translateService.getValue('report-definition.dialog-parameter-restore-title'),
            message: this.translateService.getValue('report-definition.dialog-parameter-restore-message'),
            okBtnLabel: this.translateService.getValue('report-definition.dialog-parameter-restore-button')
        }).subscribe({
            next: (result: boolean) => {
                if (result) {
                    this.parametersTable.softUndoDelete(parameter);
                }
            }
        });
    }

    public closeDialogBtnClicked(closeFn: DialogCloseCallback): void {
        closeFn();
    }

    public saveBtnClicked(): void {
        this.formGroup.markAllAsTouched();
        if (this.formGroup.valid) {
            this.report = this.fillModel(this.formGroup);
            this.report = CommonUtils.sanitizeModelStrings(this.report);

            for (let i: number = 0; i < this.report.parameters!.length; i++) {
                const currentParameter: ReportParameterDTO = this.report.parameters![i];

                //ако не е активен то няма смисъл да бъде качван в базата или пращан до backend-a 
                if (!currentParameter.isActive) {
                    this.report.parameters!.splice(i, 1);
                    i--;
                }
            }

            //edit
            if (this.report.id !== undefined) {
                this.reportService.editReport(this.report).subscribe({
                    next: () => {
                        this.router.navigateByUrl('/reports');
                    },
                    error: (response: HttpErrorResponse) => {
                        this.handleSQLExceptions(response);
                    }
                });
            }
            //add
            else {
                this.reportService.addReport(this.report).subscribe({
                    next: (addedReportId: number) => {
                        this.router.navigateByUrl('/reports');
                    },
                    error: (response: HttpErrorResponse) => {
                        this.handleSQLExceptions(response);
                    }
                });
            }
        }
    }

    public cancelBtnClicked(): void {
        this.router.navigateByUrl('/reports');
    }

    public setData(data: DialogParamsModel, buttons: DialogWrapperData): void {
        this.viewMode = data.isReadonly;
        this.reportService.getReport(data.id).subscribe((result: ReportDTO) => {
            this.report = result;
            this.fillForm(this.report);
        });

        if (this.viewMode) {
            this.formGroup.disable();
        }
    }

    private fillForm(report: ReportDTO) {
        (this.formGroup.controls.generalInformationGroup as FormGroup).controls.titleControl.setValue(report.name);
        (this.formGroup.controls.generalInformationGroup as FormGroup).controls.descriptionControl.setValue(report.description);
        (this.formGroup.controls.generalInformationGroup as FormGroup).controls.codeControl.setValue(report.code);
        (this.formGroup.controls.generalInformationGroup as FormGroup).controls.lastRunUserControl.setValue(report.lastRunUsername);
        (this.formGroup.controls.generalInformationGroup as FormGroup).controls.lastRunDateTimeControl.setValue(report.lastRunDateTime);
        (this.formGroup.controls.generalInformationGroup as FormGroup).controls.lastRunDurationSecControl.setValue(report.lastRunDurationSec);
        (this.formGroup.controls.generalInformationGroup as FormGroup).controls.iconNameControl.setValue(report.iconName);
        (this.formGroup.controls.generalInformationGroup as FormGroup).controls.reportTypeControl.setValue(report.reportType);
        (this.formGroup.controls.generalInformationGroup as FormGroup).controls.orderNumControl.setValue(report.orderNum);

        this.formGroup.controls.queryControl.setValue(report.reportSQL);
        (this.formGroup.controls.accessManagementGroup as FormGroup).controls.selectedUsersControl.setValue(report.users);
        (this.formGroup.controls.accessManagementGroup as FormGroup).controls.selectedRolesControl.setValue(report.roles);
        (this.formGroup.controls.generalInformationGroup as FormGroup).controls.reportGroupControl.setValue(this.reportGroups.find(x => x.value === report.reportGroupId));
    }

    private fillModel(formGroup: FormGroup): ReportDTO {
        this.report.name = (formGroup.controls.generalInformationGroup as FormGroup).controls.titleControl.value;
        this.report.description = (formGroup.controls.generalInformationGroup as FormGroup).controls.descriptionControl.value;
        this.report.code = (formGroup.controls.generalInformationGroup as FormGroup).controls.codeControl.value;
        this.report.orderNum = (formGroup.controls.generalInformationGroup as FormGroup).controls.orderNumControl.value;
        this.report.reportSQL = formGroup.controls.queryControl.value;

        this.report.iconName = (formGroup.controls.generalInformationGroup as FormGroup).controls.iconNameControl.value;

        this.report.reportType = (formGroup.controls.generalInformationGroup as FormGroup).controls.reportTypeControl.value;

        this.report.roles = (this.formGroup.controls.accessManagementGroup as FormGroup).get('selectedRolesControl')!.value;
        this.report.users = (this.formGroup.controls.accessManagementGroup as FormGroup).get('selectedUsersControl')!.value;

        const groupId: number = (formGroup.controls.generalInformationGroup as FormGroup).get('reportGroupControl')!.value?.value;
        if (groupId !== null && groupId !== undefined) {
            this.reportGroupId = groupId;
        }

        if (this.reportGroupId !== null && this.reportGroupId !== undefined) {
            this.report.reportGroupId = this.reportGroupId;
        }

        return this.report;
    }

    private refreshParametersTable(): void {
        this.report.parameters = this.report.parameters!.slice();
    }

    private getReportInfoParams(): ExecutionParamDTO[] {
        const parameterValues: ExecutionParamDTO[] = [];
        if (this.report?.parameters !== null && this.report?.parameters !== undefined) {
            for (const parameter of this.report!.parameters) {
                parameterValues.push(new ExecutionParamDTO({
                    name: parameter.code,
                    value: parameter.defaultValue,
                    type: parameter.dataType
                }));
            }
        }

        return parameterValues;
    }

    private handleSQLExceptions(response: HttpErrorResponse): void {
        if (response !== null && response !== undefined && response.error !== null && response.error !== undefined) {
            if (response.error.messages !== null && response.error.messages !== undefined) {
                const messages: string[] = response.error.messages;
                if (messages.length !== 0) {
                    this.snackbar.openFromComponent(ErrorSnackbarComponent, {
                        data: response.error as ErrorModel,
                        duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                        panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
                    });
                }
                else {
                    this.snackbar.openFromComponent(ErrorSnackbarComponent, {
                        data: new ErrorModel({ messages: [this.translateService.getValue('service.an-error-occurred-in-the-app')] }),
                        duration: RequestProperties.DEFAULT.showExceptionDurationErr,
                        panelClass: RequestProperties.DEFAULT.showExceptionColorClassErr
                    });
                }
            }
            if ((response.error as ErrorModel).code === ErrorCode.InvalidSqlQuery) {
                this.formGroup.get('queryControl')!.setErrors({ 'invalidSqlQuery': true });
            }
        }
    }
}