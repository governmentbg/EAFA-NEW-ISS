import { AfterViewInit, Component, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';

import { IFishingCapacityService } from '@app/interfaces/common-app/fishing-capacity.interface';
import { ShipFishingCapacityDTO } from '@app/models/generated/dtos/ShipFishingCapacityDTO';
import { FishingCapacityFilters } from '@app/models/generated/filters/FishingCapacityFilters';
import { FishingCapacityAdministrationService } from '@app/services/administration-app/fishing-capacity-administration.service';
import { FilterEventArgs } from '@app/shared/components/data-table/models/filter-event-args.model';
import { TLDataTableComponent } from '@app/shared/components/data-table/tl-data-table.component';
import { SearchPanelComponent } from '@app/shared/components/search-panel/search-panel.component';
import { DataTableManager } from '@app/shared/utils/data-table.manager';
import { RangeInputData } from '@app/shared/components/input-controls/tl-range-input/range-input.component';
import { ShipFishingCapacityHistoryDTO } from '@app/models/generated/dtos/ShipFishingCapacityHistoryDTO';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { DialogParamsModel } from '@app/models/common/dialog-params.model';
import { TLMatDialog } from '@app/shared/components/dialog-wrapper/tl-mat-dialog';
import { IDialogComponent } from '@app/shared/components/dialog-wrapper/interfaces/dialog-content.interface';
import { EditShipComponent } from '../../../common-app/ships-register/edit-ship/edit-ship.component';
import { ShipDeregistrationComponent } from '@app/components/common-app/ships-register/ship-deregistration/ship-deregistration.component';
import { IncreaseFishingCapacityComponent } from '@app/components/common-app/fishing-capacity/increase-fishing-capacity/increase-fishing-capacity.component';
import { ReduceFishingCapacityComponent } from '@app/components/common-app/fishing-capacity/reduce-fishing-capacity/reduce-fishing-capacity.component';
import { IShipsRegisterService } from '@app/interfaces/common-app/ships-register.interface';
import { ShipsRegisterAdministrationService } from '@app/services/administration-app/ships-register-administration.service';
import { IHeaderAuditButton } from '@app/shared/components/dialog-wrapper/interfaces/header-audit-button.interface';
import { HeaderCloseFunction } from '@app/shared/components/dialog-wrapper/interfaces/header-cancel-button.interface';
import { IApplicationsService } from '@app/interfaces/administration-app/applications.interface';
import { ApplicationsAdministrationService } from '@app/services/administration-app/applications-administration.service';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { Router } from '@angular/router';

@Component({
    selector: 'fishing-capacity-register',
    templateUrl: './fishing-capacity-register.component.html',
    styleUrls: ['./fishing-capacity-register.component.scss']
})
export class FishingCapacityRegisterComponent implements AfterViewInit {
    public translate: FuseTranslationLoaderService;
    public form!: FormGroup;

    public readonly canViewShipApplications: boolean;
    public readonly canViewCapacityApplications: boolean;

    public readonly pageCodes: typeof PageCodeEnum = PageCodeEnum;

    @ViewChild(TLDataTableComponent)
    private datatable!: TLDataTableComponent;

    @ViewChild(SearchPanelComponent)
    private searchpanel!: SearchPanelComponent;

    private grid!: DataTableManager<ShipFishingCapacityDTO, FishingCapacityFilters>;

    private readonly service: IFishingCapacityService;
    private readonly shipsService: IShipsRegisterService;
    private readonly applicationsService: IApplicationsService;

    private readonly regVesselDialog: TLMatDialog<EditShipComponent>;
    private readonly deregVesselDialog: TLMatDialog<ShipDeregistrationComponent>;
    private readonly increaseFishCapDialog: TLMatDialog<IncreaseFishingCapacityComponent>;
    private readonly reduceFishCapDialog: TLMatDialog<ReduceFishingCapacityComponent>;

    private readonly router: Router;

    public constructor(
        service: FishingCapacityAdministrationService,
        shipsService: ShipsRegisterAdministrationService,
        applicationsService: ApplicationsAdministrationService,
        translate: FuseTranslationLoaderService,
        regVesselDialog: TLMatDialog<EditShipComponent>,
        deregVesselDialog: TLMatDialog<ShipDeregistrationComponent>,
        increaseFishCapDialog: TLMatDialog<IncreaseFishingCapacityComponent>,
        reduceFishCapDialog: TLMatDialog<ReduceFishingCapacityComponent>,
        permissions: PermissionsService,
        router: Router
    ) {
        this.service = service;
        this.shipsService = shipsService;
        this.applicationsService = applicationsService;
        this.translate = translate;

        this.regVesselDialog = regVesselDialog;
        this.deregVesselDialog = deregVesselDialog;
        this.increaseFishCapDialog = increaseFishCapDialog;
        this.reduceFishCapDialog = reduceFishCapDialog;
        this.router = router;

        this.canViewShipApplications = permissions.has(PermissionsEnum.ShipsRegisterApplicationsRead) || permissions.has(PermissionsEnum.ShipsRegisterApplicationReadAll);
        this.canViewCapacityApplications = permissions.has(PermissionsEnum.FishingCapacityApplicationsRead) || permissions.has(PermissionsEnum.FishingCapacityApplicationsReadAll);

        this.buildForm();
    }

    public ngAfterViewInit(): void {
        this.grid = new DataTableManager<ShipFishingCapacityDTO, FishingCapacityFilters>({
            tlDataTable: this.datatable,
            searchPanel: this.searchpanel,
            requestServiceMethod: this.service.getAllShipCapacities.bind(this.service),
            filtersMapper: this.mapFilters.bind(this)
        });

        this.grid.refreshData();
    }

    public openHistoryApplication(history: ShipFishingCapacityHistoryDTO): void {
        let title: string;
        let editDialog: TLMatDialog<IDialogComponent>;
        let editDialogTCtor: new (...args: any[]) => IDialogComponent;
        let service: IFishingCapacityService | IShipsRegisterService;
        let auditButton: IHeaderAuditButton;

        if (history.pageCode === PageCodeEnum.RegVessel) {
            title = this.translate.getValue('ships-register.view-ship-application-dialog-title');
            editDialog = this.regVesselDialog;
            editDialogTCtor = EditShipComponent;
            service = this.shipsService;
            auditButton = {
                id: history.shipId!,
                getAuditRecordData: this.shipsService.getSimpleAudit.bind(this.shipsService),
                tableName: 'RShips.ShipRegister'
            };
        }
        else if (history.pageCode === PageCodeEnum.DeregShip) {
            title = this.translate.getValue('ships-register.view-deregistration-dialog-title');
            editDialog = this.deregVesselDialog;
            editDialogTCtor = ShipDeregistrationComponent;
            service = this.shipsService;
            auditButton = {
                id: history.shipId!,
                getAuditRecordData: this.shipsService.getSimpleAudit.bind(this.shipsService),
                tableName: 'RShips.ShipRegister'
            };
        }
        else if (history.pageCode === PageCodeEnum.IncreaseFishCap) {
            title = this.translate.getValue('fishing-capacity.view-increase-capacity-application-dialog-title');
            editDialog = this.increaseFishCapDialog;
            editDialogTCtor = IncreaseFishingCapacityComponent;
            service = this.service;
            auditButton = {
                id: history.id!,
                getAuditRecordData: this.service.getSimpleAudit.bind(this.service),
                tableName: 'RCap.ShipCapacityRegister'
            };
        }
        else if (history.pageCode === PageCodeEnum.ReduceFishCap) {
            title = this.translate.getValue('fishing-capacity.view-reduce-capacity-application-dialog-title');
            editDialog = this.reduceFishCapDialog;
            editDialogTCtor = ReduceFishingCapacityComponent;
            service = this.service;
            auditButton = {
                id: history.id!,
                getAuditRecordData: this.service.getSimpleAudit.bind(this.service),
                tableName: 'RCap.ShipCapacityRegister'
            };
        }

        const editDialogData: DialogParamsModel = new DialogParamsModel({
            applicationId: history.applicationId,
            isApplication: true,
            isApplicationHistoryMode: false,
            isReadonly: true,
            viewMode: true,
            service: service!,
            applicationsService: this.applicationsService,
            showOnlyRegiXData: false,
            pageCode: history.pageCode
        });

        const dialog = editDialog!.open({
            title: title!,
            TCtor: editDialogTCtor!,
            headerAuditButton: auditButton!,
            headerCancelButton: {
                cancelBtnClicked: this.closeEditDialogBtnClicked.bind(this)
            },
            componentData: editDialogData,
            translteService: this.translate,
            disableDialogClose: true,
            viewMode: true
        }, '1400px');

        dialog.subscribe();
    }

    public gotToApplication(history: ShipFishingCapacityHistoryDTO): void {
        switch (history.pageCode) {
            case PageCodeEnum.RegVessel:
            case PageCodeEnum.DeregShip: {
                if (this.canViewShipApplications) {
                    this.router.navigate(['fishing-vessels-applications'], { state: { applicationId: history.applicationId } });
                }
            } break;
            case PageCodeEnum.IncreaseFishCap:
            case PageCodeEnum.ReduceFishCap: {
                if (this.canViewCapacityApplications) {
                    this.router.navigate(['fishing-capacity-applications'], { state: { applicationId: history.applicationId } });
                }
            } break;
        }
        
    }

    private closeEditDialogBtnClicked(closeFn: HeaderCloseFunction): void {
        closeFn();
    }

    private buildForm(): void {
        this.form = new FormGroup({
            shipCfrControl: new FormControl(),
            shipNameControl: new FormControl(),
            tonnageControl: new FormControl(),
            powerControl: new FormControl()
        });
    }

    private mapFilters(filters: FilterEventArgs): FishingCapacityFilters {
        const result: FishingCapacityFilters = new FishingCapacityFilters({
            freeTextSearch: filters.searchText,
            showInactiveRecords: filters.showInactiveRecords,

            shipCfr: filters.getValue('shipCfrControl'),
            shipName: filters.getValue('shipNameControl')
        });

        const tonnage: RangeInputData | undefined = filters.getValue('tonnageControl');
        if (tonnage !== undefined && tonnage !== null) {
            result.grossTonnageFrom = tonnage.start;
            result.grossTonnageTo = tonnage.end;
        }

        const power: RangeInputData | undefined = filters.getValue('powerControl');
        if (power !== undefined && power !== null) {
            result.powerFrom = power.start;
            result.powerTo = power.end;
        }

        return result;
    }
}