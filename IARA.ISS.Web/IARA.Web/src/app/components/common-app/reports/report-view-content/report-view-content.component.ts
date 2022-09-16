import { AfterViewInit, Component, Input, OnInit } from '@angular/core';
import { NestedTreeControl } from '@angular/cdk/tree';
import { MatTreeNestedDataSource } from '@angular/material/tree';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { ReportNodeDTO } from '@app/models/generated/dtos/ReportNodeDTO';
import { FormControl, FormGroup } from '@angular/forms';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { EditReportViewComponent } from './edit-report-view.component';
import { TLConfirmDialog } from '@app/shared/components/confirmation-dialog/tl-confirm-dialog';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { BaseDialogParamsModel } from '@app/models/common/base-dialog-params.model';
import { DialogCloseCallback } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { ReportGroupDTO } from '@app/models/generated/dtos/ReportGroupDTO';
import { Router } from '@angular/router';
import { IReportService } from '@app/interfaces/administration-app/report.interface';

@Component({
    selector: 'report-view-content',
    templateUrl: './report-view-content.component.html',
    styleUrls: ['./report-view-content.component.scss']
})
export class ReportViewContentComponent implements OnInit, AfterViewInit {
    @Input()
    public reportService!: IReportService;

    @Input()
    public isPublic!: boolean;

    public filterGroup: FormGroup;
    public menuGroup: FormGroup;
    public dataSource: MatTreeNestedDataSource<ReportNodeDTO>;
    public treeControl: NestedTreeControl<ReportNodeDTO>;

    public isReportClicked: boolean;
    public showActiveNodes: boolean;
    public selectedReportNodeId: number;
    public lastReportNodeId: number;
    public name!: string;

    public canAddReport!: boolean;
    public canEditReport!: boolean;
    public canDeleteReport!: boolean;
    public canRestoreReport!: boolean;
    public canReadReport!: boolean;

    private readonly router: Router;
    private readonly translateService: FuseTranslationLoaderService;
    private readonly permissionsService: PermissionsService;

    private reportNodes!: ReportNodeDTO[];
    private filteredReportNodes: ReportNodeDTO[] = [];
    private editDialog: TLMatDialog<EditReportViewComponent>;
    private confirmationDialog: TLConfirmDialog;

    public constructor(router: Router,
        translateService: FuseTranslationLoaderService,
        permissionsService: PermissionsService,
        editDialog: TLMatDialog<EditReportViewComponent>,
        confirmationDialog: TLConfirmDialog) {
        this.router = router;
        this.translateService = translateService;
        this.permissionsService = permissionsService;

        this.filterGroup = new FormGroup({
            keywordSearchControl: new FormControl()
        });

        this.menuGroup = new FormGroup({
            addGroupControl: new FormControl(),
            switchReportStateControl: new FormControl()
        });

        this.canAddReport = this.permissionsService.has(PermissionsEnum.ReportAddRecords);
        this.canEditReport = this.permissionsService.has(PermissionsEnum.ReportEditRecords);
        this.canDeleteReport = this.permissionsService.has(PermissionsEnum.ReportDeleteRecords);
        this.canRestoreReport = this.permissionsService.has(PermissionsEnum.ReportRestoreRecords);
        this.canReadReport = this.permissionsService.has(PermissionsEnum.ReportRead);

        this.dataSource = new MatTreeNestedDataSource<ReportNodeDTO>();
        this.treeControl = new NestedTreeControl<ReportNodeDTO>(node => node.children);
        this.editDialog = editDialog;
        this.confirmationDialog = confirmationDialog;

        this.showActiveNodes = true;
        this.isReportClicked = false;
        this.selectedReportNodeId = -1;
        this.lastReportNodeId = -1;
    }

    public ngOnInit(): void {
        this.refreshReportNodes();
    }

    public ngAfterViewInit(): void {
        this.filterGroup.controls.keywordSearchControl.valueChanges.subscribe({
            next: (value: string) => {
                if (value.length > 2) {
                    const valueToLower: string = value.toLowerCase();
                    this.filteredReportNodes = this.filterNodeState();
                    this.filteredReportNodes = this.filteredReportNodes.filter(group => group.name!.toLowerCase().includes(valueToLower)
                        || group.children?.some(report => report.name!.toLowerCase().includes(valueToLower)));

                    for (const filteredGroup of this.filteredReportNodes) {
                        filteredGroup.children = filteredGroup.children?.filter(report => report.name!.toLowerCase().includes(valueToLower));
                    }

                    this.dataSource.data = this.filteredReportNodes;
                }
                else {
                    this.dataSource.data = this.filterNodeState();
                }
            }
        });
    }

    public closeDialogBtnClicked(closeFn: DialogCloseCallback): void {
        closeFn();
    }

    public isGroup = (_: number, node: ReportNodeDTO): boolean => !!node.children && node.children.length >= 0;

    public isGroupActive(node: ReportNodeDTO): boolean {
        for (const group of this.dataSource.data) {
            if (group.children?.some(report => report.id === node.id && report.name === node.name)) {
                return group.isActive!;
            }
        }

        return false;
    }

    public reportClicked(reportNodeId: number, name: string): void {
        this.selectedReportNodeId = reportNodeId;

        this.isReportClicked = !this.isReportClicked;
        if (reportNodeId !== this.lastReportNodeId) {
            this.isReportClicked = true;
        }

        this.lastReportNodeId = reportNodeId;

        this.name = name;
    }

    public openEditComponent(id: number, isGroupId: boolean = false, viewMode: boolean = false): void {
        this.router.navigateByUrl('/reports/report_definition', {
            state: {
                id: id,
                viewMode: !this.canEditReport || viewMode,
                isGroupId: isGroupId
            }
        });
    }

    public switchReportNodeState(): void {
        this.showActiveNodes = !this.showActiveNodes;

        this.dataSource.data = this.filterNodeState();
    }

    public deleteReport(reportId: number): void {
        this.confirmationDialog.open({
            title: this.translateService.getValue('report-view.dialog-report-delete-title'),
            message: this.translateService.getValue('report-view.dialog-report-delete-message'),
            okBtnLabel: this.translateService.getValue('report-view.dialog-report-delete-button')
        })
            .subscribe({
                next: (result: boolean) => {
                    if (result) {
                        this.reportService.deleteReport(reportId).subscribe({
                            next: () => {
                                this.refreshReportNodes();
                            }
                        });
                    }
                }
            });
    }

    public undoDeletedReport(reportId: number): void {
        this.confirmationDialog.open({
            title: this.translateService.getValue('report-view.dialog-report-restore-title'),
            message: this.translateService.getValue('report-view.dialog-report-restore-message'),
            okBtnLabel: this.translateService.getValue('report-view.dialog-report-restore-button')
        })
            .subscribe({
                next: (result: boolean) => {
                    if (result) {
                        this.reportService.undoDeletedReport(reportId).subscribe({
                            next: () => {
                                this.refreshReportNodes();
                            }
                        })
                    }
                }
            });
    }

    public openEditDialog(groupId?: number): void {
        let title: string = this.translateService.getValue('report-view.dialog-group-edit-title');
        let auditButtons: IHeaderAuditButton | undefined;
        let data: BaseDialogParamsModel | undefined;

        //edit
        if (groupId !== undefined) {
            data = new BaseDialogParamsModel({
                id: groupId
            });

            auditButtons = {
                getAuditRecordData: this.reportService.getReportGroupsAudit.bind(this.reportService),
                id: groupId,
                tableName: 'ReportGroup',
            } as IHeaderAuditButton;
        }
        //add
        else {
            title = this.translateService.getValue('report-view.dialog-group-add-title');
        }

        const dialog = this.editDialog.openWithTwoButtons({
            TCtor: EditReportViewComponent,
            title: title,
            componentData: data,
            headerAuditButton: auditButtons,
            translteService: this.translateService,
            headerCancelButton: {
                cancelBtnClicked: this.closeDialogBtnClicked.bind(this)
            },
            viewMode: false
        }, '1200px');

        dialog.subscribe({
            next: (result: ReportGroupDTO | undefined) => {
                if (result !== undefined) {
                    this.refreshReportNodes();
                }
            }
        })
    }

    public deleteGroup(groupId: number): void {
        this.confirmationDialog.open({
            title: this.translateService.getValue('report-view.dialog-group-delete-title'),
            message: this.translateService.getValue('report-view.dialog-group-delete-message'),
            okBtnLabel: this.translateService.getValue('report-view.dialog-group-delete-button')
        })
            .subscribe({
                next: (result: boolean) => {
                    if (result) {
                        this.reportService.deleteGroup(groupId).subscribe({
                            next: () => {
                                this.refreshReportNodes();
                            }
                        });
                    }
                }
            })
    }

    public undoDeletedGroup(groupId: number): void {
        this.confirmationDialog.open({
            title: this.translateService.getValue('report-view.dialog-group-restore-title'),
            message: this.translateService.getValue('report-view.dialog-group-restore-message'),
            okBtnLabel: this.translateService.getValue('report-view.dialog-group-restore-button')
        })
            .subscribe({
                next: (result: boolean) => {
                    if (result) {
                        this.reportService.undoDeletedGroup(groupId).subscribe({
                            next: () => {
                                this.refreshReportNodes();
                            }
                        });
                    }
                }
            })
    }

    private refreshReportNodes(): void {
        this.reportService.getReportNodes().subscribe({
            next: (reportNodes: ReportNodeDTO[]) => {
                this.reportNodes = reportNodes;

                this.dataSource.data = this.filterNodeState();
            }
        });
    }

    private filterNodeState(): ReportNodeDTO[] {
        this.filteredReportNodes = this.copyReportNodes(this.reportNodes);

        if (this.showActiveNodes) {
            this.filteredReportNodes = this.filteredReportNodes.filter(group => group.isActive === true);
        }
        else {
            this.filteredReportNodes = this.filteredReportNodes.filter(group => group.isActive === false
                || (group.children?.some(x => x.isActive === false)));
        }

        for (const filteredNode of this.filteredReportNodes) {
            filteredNode.children = filteredNode.children?.filter(x => x.isActive === this.showActiveNodes);
        }

        return this.filteredReportNodes;
    }

    private copyReportNodes(reportNodes: ReportNodeDTO[]): ReportNodeDTO[] {
        const filteredReportNodes: ReportNodeDTO[] = [];

        for (let i = 0; i < reportNodes.length; i++) {
            const report = new ReportNodeDTO({
                id: reportNodes[i].id,
                name: reportNodes[i].name,
                iconName: reportNodes[i].iconName,
                isActive: reportNodes[i].isActive,
                children: []
            });

            filteredReportNodes.push(report);

            for (const child of reportNodes[i].children!) {

                const childReport = new ReportNodeDTO({
                    id: child.id,
                    name: child.name,
                    iconName: child.iconName,
                    isActive: child.isActive
                });

                report.children!.push(childReport);
            }
        }

        return filteredReportNodes;
    }
}