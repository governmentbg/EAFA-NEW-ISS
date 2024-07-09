import { AfterViewInit, Component, ElementRef, HostListener, OnInit, ViewChild } from '@angular/core';
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
import { RequestProperties } from '@app/shared/services/request-properties';
import { MatSnackBar } from '@angular/material/snack-bar';
import { IReportService } from '@app/interfaces/administration-app/report.interface';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { DialogWrapperData } from '@app/shared/components/dialog-wrapper/models/dialog-action-buttons.model';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { TLValidators } from '@app/shared/utils/tl-validators';
import { GetControlErrorLabelTextCallback } from '@app/shared/components/input-controls/base-tl-control';
import { TLError } from '@app/shared/components/input-controls/models/tl-error.model';
import { EditReportParamsModel } from '@app/components/common-app/reports/models/edit-report-params.model';
import { MenuService } from '@app/shared/services/menu.service';
import { TLSnackbar } from '@app/shared/components/snackbar/tl.snackbar';

@Component({
    selector: 'report-definition',
    templateUrl: './report-definition.component.html',
    styleUrls: ['./report-definition.component.scss']
})
export class ReportDefinitionComponent implements OnInit, AfterViewInit {
    public report!: ReportDTO;
    public reportGroupId: number | undefined;
    public isMenuHidden: boolean;
    public treeControl = new NestedTreeControl<TableNodeDTO>(node => node.children);
    public dataSource = new MatTreeNestedDataSource<TableNodeDTO>();
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
    public parameters: ReportParameterDTO[] = [];

    public reportTypes: string[];
    public reportInfo!: ExecutionReportInfoDTO;
    public isTreeExpanded: boolean = true;

    public getReportCodeErrorTextMethod: GetControlErrorLabelTextCallback = this.getReportCodeErrorText.bind(this);

    public mainPanelWidthPx: number = 0;
    public containerHeightPx: number = 0;
    public mainPanelHeightPx: number = 0;

    private host: HTMLElement;
    private toolbarElement!: HTMLElement;
    private containerElement!: HTMLElement;
    private treePanelElement!: HTMLElement;

    @ViewChild('parametersTable')
    private parametersTable!: TLDataTableComponent;

    private isCopy: boolean = false;

    private readonly router: Router;
    private readonly translateService: FuseTranslationLoaderService;
    private readonly commonNomenclaturesService: CommonNomenclatures;
    private readonly reportService: IReportService;
    private readonly editDialog: TLMatDialog<EditReportDefinitionComponent>;
    private readonly confirmDialog: TLConfirmDialog;
    private readonly snackbar: TLSnackbar;
    private menuService: MenuService;

    private allUsers!: NomenclatureDTO<number>[];
    private allRoles!: NomenclatureDTO<number>[];

    public constructor(
        router: Router,
        translateService: FuseTranslationLoaderService,
        commonNomenclaturesService: CommonNomenclatures,
        reportDefinitionService: ReportAdministrationService,
        editDialog: TLMatDialog<EditReportDefinitionComponent>,
        confirmDialog: TLConfirmDialog,
        snackbar: TLSnackbar,
        menuService: MenuService,
        host: ElementRef
    ) {
        this.router = router;
        this.translateService = translateService;
        this.commonNomenclaturesService = commonNomenclaturesService;
        this.reportService = reportDefinitionService;
        this.editDialog = editDialog;
        this.confirmDialog = confirmDialog;
        this.snackbar = snackbar;
        this.menuService = menuService;

        this.host = host.nativeElement as HTMLElement;

        this.isMenuHidden = false;
        this.selectedUsers = [];
        this.selectedRoles = [];
        this.report = new ReportDTO();

        this.reportTypes = [
            ReportTypesEnum[ReportTypesEnum.SQL],
            ReportTypesEnum[ReportTypesEnum.JasperPDF],
            ReportTypesEnum[ReportTypesEnum.JasperWord]
        ];

        this.formGroup = this.buildForm();
    }

    public hasChild = (_: number, node: TableNodeDTO): boolean => !!node.children && node.children.length > 0;

    public async ngOnInit(): Promise<void> {
        this.dataSource.data = await this.reportService.getTableNodes().toPromise();

        this.reportGroups = await this.reportService.getReportGroups().toPromise();

        this.allUsers = await this.commonNomenclaturesService.getUserNames().toPromise();
        this.users = this.allUsers;

        this.allRoles = await this.reportService.getActiveRoles().toPromise();
        this.roles = this.allRoles;

        // Хак за да се показва едитора, дори когато е празен
        this.formGroup.get('queryControl')!.setValue("");

        const data: EditReportParamsModel = window.history.state?.data;
        if (data !== undefined && data !== null) {
            if (!data.isAdd) {
                this.isCopy = data.isCopy;

                this.reportService.getReport(data.id!).subscribe({
                    next: (report: ReportDTO) => {
                        this.report = report;

                        this.fillForm();
                        this.reportInfo = new ExecutionReportInfoDTO({ reportId: report.id });
                        this.reportInfo.parameters = this.getReportInfoParams();
                    }
                });
            }
            else {
                this.reportGroupId = data.id;
                (this.formGroup.get('generalInformationGroup') as FormGroup).get('reportGroupControl')!.setValue(this.reportGroups.find(x => x.value === this.reportGroupId));
            }

            this.viewMode = data.viewMode;
            if (data.viewMode) {
                this.formGroup.disable();
            }
        }
        else {
            this.router.navigateByUrl('/reports');
        }
    }

    public ngAfterViewInit(): void {
        this.calculateMainPanelWidthPx();
        this.calculateContainerHeightPx();

        this.menuService.folded.subscribe({
            next: () => {
                setTimeout(() => {
                    this.calculateMainPanelWidthPx();
                });
            }
        });
    }

    @HostListener('window:resize')
    public onWindowResize(): void {
        this.calculateContainerHeightPx();
        this.calculateMainPanelWidthPx();
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
        if (this.report.id !== undefined && this.report.id !== null && !this.isCopy) {
            this.router.navigateByUrl('/system-log', {
                state: {
                    tableId: this.report.id.toString(),
                    tableName: 'Report'
                }
            });
        }
    }

    public cancelBtnClicked(): void {
        this.router.navigateByUrl('/reports');
    }

    public addOrEditParameter(parameter?: ReportParameterDTO, viewMode: boolean = false): void {
        let title: string = this.translateService.getValue('report-definition.dialog-parameter-edit-title');
        let auditButtons: IHeaderAuditButton | undefined;
        let data: ReportDefinitionDialogParams | undefined;
        const parameterId: number | undefined = parameter?.id;

        //edit
        if (parameter !== undefined && parameter !== null) {
            if (viewMode) title = this.translateService.getValue('report-definition.dialog-parameter-view-title');

            data = new ReportDefinitionDialogParams({
                parameter: parameter,
                viewMode: viewMode
            });

            if (parameterId !== undefined && parameterId !== null) {
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
            next: (result: ReportParameterDTO | undefined) => {
                //ако се натисне 'Отказ' в диалога
                if (result !== undefined && result !== null) {
                    //edit
                    if (parameter !== undefined && parameter !== null) {
                        parameter = result;
                    }
                    //add
                    else {
                        this.parameters!.push(result);
                    }

                    this.parameters = this.parameters?.slice();
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

    public saveBtnClicked(): void {
        this.formGroup.markAllAsTouched();
        if (this.formGroup.valid) {
            this.fillModel();
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
            if (this.report.id !== undefined && this.report.id !== null && !this.isCopy) {
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
                    next: (id: number) => {
                        this.router.navigateByUrl('/reports');
                    },
                    error: (response: HttpErrorResponse) => {
                        this.handleSQLExceptions(response);
                    }
                });
            }
        }
    }

    public setData(data: DialogParamsModel, buttons: DialogWrapperData): void {
        this.viewMode = data.isReadonly;
        this.reportService.getReport(data.id).subscribe({
            next: (result: ReportDTO) => {
                this.report = result;
                this.fillForm();
            }
        });

        if (this.viewMode) {
            this.formGroup.disable();
        }
    }

    public toggleTree(): void {
        if (this.isTreeExpanded) {
            this.isTreeExpanded = false;
        }
        else {
            setTimeout(() => {
                this.isTreeExpanded = true;
            }, 180);
        }
    }

    public onTreePanelToggled(event: TransitionEvent): void {
        if (event.propertyName === 'width') {
            this.calculateMainPanelWidthPx();
        }
    }

    public getReportCodeErrorText(controlName: string, errorValue: unknown, errorCode: string): TLError | undefined {
        if (errorCode === 'reportCodeAlreadyExists') {
            if (errorValue === true) {
                return new TLError({ text: this.translateService.getValue('report-definition.report-code-already-exists-error'), type: 'error' });
            }
        }
        return undefined;
    }

    private buildForm(): FormGroup {
        return new FormGroup({
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

    private fillForm() {
        const reportName: string | undefined = this.isCopy
            ? `${this.report.name} - ${this.translateService.getValue('report-definition.copy')}`
            : this.report.name;

        (this.formGroup.get('generalInformationGroup') as FormGroup).get('titleControl')!.setValue(reportName);
        (this.formGroup.get('generalInformationGroup') as FormGroup).get('descriptionControl')!.setValue(this.report.description);
        (this.formGroup.get('generalInformationGroup') as FormGroup).get('codeControl')!.setValue(this.report.code);
        (this.formGroup.get('generalInformationGroup') as FormGroup).get('lastRunUserControl')!.setValue(this.report.lastRunUsername);
        (this.formGroup.get('generalInformationGroup') as FormGroup).get('lastRunDateTimeControl')!.setValue(this.report.lastRunDateTime);
        (this.formGroup.get('generalInformationGroup') as FormGroup).get('lastRunDurationSecControl')!.setValue(this.report.lastRunDurationSec);
        (this.formGroup.get('generalInformationGroup') as FormGroup).get('iconNameControl')!.setValue(this.report.iconName);
        (this.formGroup.get('generalInformationGroup') as FormGroup).get('reportTypeControl')!.setValue(this.report.reportType);
        (this.formGroup.get('generalInformationGroup') as FormGroup).get('orderNumControl')!.setValue(this.report.orderNum);

        this.formGroup.get('queryControl')!.setValue(this.report.reportSQL);
        (this.formGroup.get('accessManagementGroup') as FormGroup).get('selectedUsersControl')!.setValue(this.report.users);
        (this.formGroup.get('accessManagementGroup') as FormGroup).get('selectedRolesControl')!.setValue(this.report.roles);
        (this.formGroup.get('generalInformationGroup') as FormGroup).get('reportGroupControl')!.setValue(this.reportGroups.find(x => x.value === this.report.reportGroupId));

        this.parameters = this.report.parameters ?? [];
    }

    private fillModel(): void {
        this.report.name = (this.formGroup.get('generalInformationGroup') as FormGroup).get('titleControl')!.value;
        this.report.description = (this.formGroup.get('generalInformationGroup') as FormGroup).get('descriptionControl')!.value;
        this.report.code = (this.formGroup.get('generalInformationGroup') as FormGroup).get('codeControl')!.value;
        this.report.orderNum = (this.formGroup.get('generalInformationGroup') as FormGroup).get('orderNumControl')!.value;
        this.report.iconName = (this.formGroup.get('generalInformationGroup') as FormGroup).get('iconNameControl')!.value;
        this.report.reportType = (this.formGroup.get('generalInformationGroup') as FormGroup).get('reportTypeControl')!.value;

        this.report.reportSQL = this.formGroup.get('queryControl')!.value;
        this.report.roles = (this.formGroup.get('accessManagementGroup') as FormGroup).get('selectedRolesControl')!.value;
        this.report.users = (this.formGroup.get('accessManagementGroup') as FormGroup).get('selectedUsersControl')!.value;

        this.report.parameters = this.parameters;

        const groupId: number = (this.formGroup.get('generalInformationGroup') as FormGroup).get('reportGroupControl')!.value?.value;
        if (groupId !== null && groupId !== undefined) {
            this.reportGroupId = groupId;
        }

        if (this.reportGroupId !== null && this.reportGroupId !== undefined) {
            this.report.reportGroupId = this.reportGroupId;
        }

        if (this.isCopy) {
            this.report.id = undefined;
        }
    }

    private closeDialogBtnClicked(closeFn: DialogCloseCallback): void {
        closeFn();
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
                    this.snackbar.errorModel(response.error as ErrorModel, RequestProperties.DEFAULT);
                }
                else {
                    this.snackbar.error(this.translateService.getValue('service.an-error-occurred-in-the-app'), RequestProperties.DEFAULT.showExceptionDurationErr, RequestProperties.DEFAULT.showExceptionColorClassErr);
                }
            }

            if ((response.error as ErrorModel).code === ErrorCode.InvalidSqlQuery) {
                this.formGroup.get('queryControl')!.setErrors({ 'invalidSqlQuery': true });
            }

            if ((response.error as ErrorModel).code === ErrorCode.ReportCodeAlreadyExists) {
                (this.formGroup.get('generalInformationGroup') as FormGroup).get('codeControl')!.setErrors({ 'reportCodeAlreadyExists': true });
                (this.formGroup.get('generalInformationGroup') as FormGroup).get('codeControl')!.markAsTouched();
            }
        }
    }

    private calculateContainerHeightPx(): void {
        if (!this.toolbarElement) {
            this.toolbarElement = document.getElementsByTagName('toolbar').item(0) as HTMLElement;
        }

        this.containerHeightPx = window.innerHeight - this.toolbarElement.offsetHeight;
        this.mainPanelHeightPx = this.containerHeightPx;
    }

    private calculateMainPanelWidthPx(): void {
        if (!this.containerElement) {
            this.containerElement = this.host.getElementsByClassName('container').item(0) as HTMLElement;
        }

        if (!this.treePanelElement) {
            this.treePanelElement = this.host.getElementsByClassName('tree-panel').item(0) as HTMLElement;
        }

        this.mainPanelWidthPx = this.containerElement.offsetWidth - this.treePanelElement.offsetWidth;
    }
}
