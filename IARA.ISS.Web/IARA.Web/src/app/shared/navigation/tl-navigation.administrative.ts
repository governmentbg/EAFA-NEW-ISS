import { PoundnetsComponent } from '@app/components/administration-app/poundnet-register/poundnets.component';
import { ScientificFishingComponent } from '@app/components/administration-app/scientific-fishing-register/scientific-fishing.component';
import { ExternalUsersComponent } from '@app/components/administration-app/user-management/external-users/external-users.component';
import { InternalUsersComponent } from '@app/components/administration-app/user-management/internal-users/internal-users.component';
import { ApplicationsProcessingComponent } from '@app/components/administration-app/applications-processing/applications-processing.component';
import { BuyersFSCApplicationsComponent } from '@app/components/administration-app/buyers-fsc-register/applications/buyers-applications.component';
import { BuyersComponent } from '@app/components/administration-app/buyers-fsc-register/buyers-register.component';
import { ShipQuotasComponent } from '@app/components/administration-app/catch-quotas-register/ship-quotas-register.component';
import { YearlyQuotasComponent } from '@app/components/administration-app/catch-quotas-register/yearly-quotas-register.component';
import { DashboardComponent } from '@app/components/administration-app/dashboard/dashboard.component';
import { LegalEntitiesApplicationsComponent } from '@app/components/administration-app/legals/applications/legal-entities-applications.component';
import { LegalEntitiesComponent } from '@app/components/administration-app/legals/legal-entities.component';
import { MyProfileAdministrationComponent } from '@app/components/administration-app/my-profile-administration/my-profile-administration.component';
import { NomenclaturesComponent } from '@app/components/administration-app/nomenclatures-register/nomenclatures.component';
import { PermissionsRegisterComponent } from '@app/components/administration-app/permissions-register/permissions-register.component';
import { QualifiedFishersApplicationsComponent } from '@app/components/administration-app/qualified-fishers-register/applications/qualified-fishers-applications.component';
import { QualifiedFishersComponent } from '@app/components/administration-app/qualified-fishers-register/qualified-fishers.component';
import { RecreationalFishingApplicationsComponent } from '@app/components/administration-app/recreational-fishing/applications/recreational-fishing-applications.component';
import { RecreationalFishingAssociationsComponent } from '@app/components/administration-app/recreational-fishing/associations/recreational-fishing-associations.component';
import { RecreationalFishingTicketsComponent } from '@app/components/administration-app/recreational-fishing/tickets/recreational-fishing-tickets.component';
import { LegalEntitiesReportComponent } from '@app/components/administration-app/reports/legal-entities-report/legal-entities-report.component';
import { PersonsReportComponent } from '@app/components/administration-app/reports/persons-report/persons-report.component';
import { RolesRegisterComponent } from '@app/components/administration-app/roles-register/roles-register.component';
import { ScientificFishingApplicationsComponent } from '@app/components/administration-app/scientific-fishing-register/applications/scientific-fishing-applications.component';
import { ShipsRegisterApplicationsComponent } from '@app/components/administration-app/ships-register/applications/ships-register-applications.component';
import { ShipsRegisterComponent } from '@app/components/administration-app/ships-register/ships-register.component';
import { SystemLogComponent } from '@app/components/administration-app/system-log/system-log.component';
import { ErrorLogComponent } from '@app/components/administration-app/error-log/error-log.component';
import { NewsManagementComponent } from '@app/components/administration-app/news-management/news-management.component';
import { TranslationHelpComponent } from '@app/components/administration-app/translation-help/translation-help.component';
import { TranslationLabelsComponent } from '@app/components/administration-app/translation-labels/translation-labels.component';
import { PermissionsEnum } from '../enums/permissions.enum';
import { ITLNavigation } from './base/tl-navigation.interface';
import { EditShipRegisterComponent } from '@app/components/administration-app/ships-register/edit-ship-register/edit-ship-register.component';
import { InspectionsComponent } from '@app/components/administration-app/control-activity/inspections/inspections-register.component';
import { CommercialFishingApplicationsComponent } from '@app/components/administration-app/commercial-fishing-register/applications/commercial-fishing-applications.component';
import { CommercialFishingRegisterComponent } from '@app/components/administration-app/commercial-fishing-register/commercial-fishing-register.component';
import { ReportDefinitionComponent } from '@app/components/administration-app/reports/report-definition/report-definition.component';
import { MaximumFishingCapacityComponent } from '@app/components/administration-app/fishing-capacity/maximum-fishing-capacity/maximum-fishing-capacity.component';
import { InspectorsRegisterComponent } from '@app/components/administration-app/inspectors-register/inspectors-register.component';
import { FishingCapacityApplicationsComponent } from '@app/components/administration-app/fishing-capacity/applications/fishing-capacity-applications.component';
import { FishingCapacityCertificatesRegisterComponent } from '@app/components/administration-app/fishing-capacity/fishing-capacity-certificates-register/fishing-capacity-certificates-register.component';
import { FishingCapacityRegisterComponent } from '@app/components/administration-app/fishing-capacity/fishing-capacity-register/fishing-capacity-register.component';
import { AquacultureFacilitiesApplicationsComponent } from '@app/components/administration-app/aquaculture-facilities/applications/aquaculture-facilities-applications.component';
import { AquacultureFacilitiesComponent } from '@app/components/administration-app/aquaculture-facilities/aquaculture-facilities.component';
import { PatrolVehiclesComponent } from '@app/components/administration-app/patrol-vehicles/patrol-vehicles.component';
import { CatchesAndSalesComponent } from '@app/components/administration-app/catches-and-sales/catches-and-sales.component';
import { FishingCapacityAnalysisComponent } from '@app/components/administration-app/fishing-capacity/analysis/fishing-capacity-analysis.component';
import { ReportParameterDefinitionComponent } from '@app/components/administration-app/reports/report-parameter-definition/report-parameter-definition.component';
import { CrossChecksComponent } from '@app/components/administration-app/cross-checks/cross-checks.component';
import { CrossChecksResultsComponent } from '@app/components/administration-app/cross-checks/cross-checks-results/cross-checks-results.component';
import { AuanRegisterComponent } from '@app/components/administration-app/control-activity/auan-register/auan-register.component';
import { StatisticalFormsComponent } from '@app/components/administration-app/statistical-forms/statistical-forms.component';
import { PenalDecreesComponent } from '@app/components/administration-app/control-activity/penal-decrees/penal-decrees.component';
import { StatisticalFormsApplicationsComponent } from '@app/components/administration-app/statistical-forms/applications/statistical-forms-applications.component';
import { ReportViewComponent } from '@app/components/administration-app/reports/report-view/report-view.component';
import { FluxVmsRequestsComponent } from '@app/components/administration-app/flux-vms-requests/flux-vms-requests.component';
import { ApplicationRegixChecksComponent } from '@app/components/administration-app/applications-regix-checks/application-regix-checks.component';
import { PenalPointsComponent } from '@app/components/administration-app/control-activity/awarded-points/penal-points.component';

export class Navigation {
    public static getMenu(isPublic: boolean): ITLNavigation[] {
        return Navigation.Menu;
    }

    public static Menu: ITLNavigation[] = [
        {
            id: 'dashboard',
            title: 'Dashboard',
            translate: 'navigation.dashboard',
            type: 'item',
            icon: 'ic-dashboard',
            url: '/dashboard',
            permissions: [],
            component: DashboardComponent,
            isPublic: false
        },
        {
            id: 'application_processing',
            title: 'Application processing',
            translate: 'navigation.application-processing',
            type: 'item',
            icon: 'fa-hourglass-half',
            url: '/application_processing',
            permissions: [PermissionsEnum.ApplicationsRead],
            component: ApplicationsProcessingComponent,
            isPublic: false
        },
        {
            id: 'fishing_vessels',
            title: 'Fishing Vessels',
            translate: 'navigation.fishing-vessels',
            type: 'collapsable',
            icon: 'fa-ship',
            isPublic: false,
            children: [
                {
                    id: 'fishing_vessels_applications',
                    title: 'Applications fishing vessels',
                    translate: 'navigation.fishing-vessels-applications',
                    type: 'item',
                    icon: 'description',
                    url: '/fishing-vessels-applications',
                    permissions: [PermissionsEnum.ShipsRegisterApplicationReadAll, PermissionsEnum.ShipsRegisterApplicationsRead],
                    component: ShipsRegisterApplicationsComponent,
                    isPublic: false
                },
                {
                    id: 'fishing_vessels_register',
                    title: 'Register fishing vessels',
                    translate: 'navigation.fishing-vessels-register',
                    type: 'item',
                    icon: 'fa-book-open',
                    url: '/fishing-vessels',
                    permissions: [PermissionsEnum.ShipsRegisterReadAll, PermissionsEnum.ShipsRegisterRead],
                    component: ShipsRegisterComponent,
                    isPublic: false
                },
                {
                    id: 'fishing_vessels_register_edit',
                    title: 'Edit fishing vessel',
                    translate: 'navigation.fishing-vessels-register',
                    type: 'item',
                    icon: 'fa-book-open',
                    url: '/fishing-vessels/edit',
                    permissions: [PermissionsEnum.ShipsRegisterReadAll, PermissionsEnum.ShipsRegisterRead, PermissionsEnum.ShipsRegisterEditRecords],
                    component: EditShipRegisterComponent,
                    hideInMenu: true,
                    isPublic: false
                }
            ]
        },
        {
            id: 'fishing_capacity',
            title: 'Fishing Capacity',
            translate: 'navigation.fishing-capacity',
            type: 'collapsable',
            icon: 'fa-tachometer-alt',
            isPublic: false,
            children: [
                {
                    id: 'fishing_capacity_applications',
                    title: 'Applications fishing capacity',
                    translate: 'navigation.fishing-capacity-applications',
                    type: 'item',
                    icon: 'description',
                    url: '/fishing-capacity-applications',
                    permissions: [PermissionsEnum.FishingCapacityApplicationsReadAll, PermissionsEnum.FishingCapacityApplicationsRead],
                    component: FishingCapacityApplicationsComponent,
                    isPublic: false
                },
                {
                    id: 'fishing_capacity_register',
                    title: 'Register fishing capacity',
                    translate: 'navigation.fishing-capacity-register',
                    type: 'item',
                    icon: 'storage',
                    url: '/fishing-capacity-register',
                    permissions: [PermissionsEnum.FishingCapacityReadAll, PermissionsEnum.FishingCapacityRead],
                    component: FishingCapacityRegisterComponent,
                    isPublic: false
                },
                {
                    id: 'fishing_capacity_certificates_register',
                    title: 'Register fishing certificates capacity',
                    translate: 'navigation.fishing-capacity-certificates-register',
                    type: 'item',
                    icon: 'fa-stamp',
                    url: '/fishing-capacity-certificates-register',
                    permissions: [PermissionsEnum.FishingCapacityCertificatesRead],
                    component: FishingCapacityCertificatesRegisterComponent,
                    isPublic: false
                },
                {
                    id: 'maximum_fishing_capacity',
                    title: 'Maximum fishing capacity',
                    translate: 'navigation.maximum-fishing-capacity',
                    type: 'item',
                    icon: 'fa-tachometer-alt',
                    url: '/maximum-fishing-capacity',
                    permissions: [PermissionsEnum.MaximumCapacityRead],
                    component: MaximumFishingCapacityComponent,
                    isPublic: false
                },
                {
                    id: 'fishing_capacity_analysis',
                    title: 'Fishing capacity analysis',
                    translate: 'navigation.fishing-capacity-analysis',
                    type: 'item',
                    icon: 'fa-chart-bar',
                    url: '/fishing-capacity-analysis',
                    permissions: [PermissionsEnum.FishingCapacityAnalysis],
                    component: FishingCapacityAnalysisComponent,
                    isPublic: false
                }
            ]
        },
        {
            id: 'poundnets',
            title: 'Poundnets',
            translate: 'navigation.poundnets',
            type: 'item',
            icon: 'fa-hashtag',
            url: '/poundnets',
            permissions: [PermissionsEnum.PoundnetsRead],
            component: PoundnetsComponent,
            isPublic: false
        },
        {
            id: 'aquaculture_farms',
            title: 'Aquaculture farms',
            translate: 'navigation.aqua-culture-farms',
            type: 'collapsable',
            icon: 'ic-fishbowl',
            isPublic: false,
            children: [
                {
                    id: 'aquaculture_farms_applications',
                    title: 'Aquaculture farms applications applications',
                    translate: 'navigation.aquaculture-farms-applications',
                    type: 'item',
                    icon: 'description',
                    url: '/aquaculture-farms-applications',
                    permissions: [PermissionsEnum.AquacultureFacilitiesApplicationsReadAll, PermissionsEnum.AquacultureFacilitiesApplicationsRead],
                    component: AquacultureFacilitiesApplicationsComponent,
                    isPublic: false
                },
                {
                    id: 'aquaculture_farms',
                    title: 'Aquaculture farms register',
                    translate: 'navigation.aquaculture-farms-register',
                    type: 'item',
                    icon: 'storage',
                    url: '/aquaculture-farms',
                    permissions: [PermissionsEnum.AquacultureFacilitiesReadAll, PermissionsEnum.AquacultureFacilitiesRead],
                    component: AquacultureFacilitiesComponent,
                    isPublic: false
                }
            ]
        },
        {
            id: 'buyers_and_sales_centers',
            title: 'Buyers and sales centers',
            translate: 'navigation.buyers-and-sales-centers',
            type: 'collapsable',
            icon: 'fa-handshake',
            isPublic: false,
            children: [
                {
                    id: 'sales_centers_applications',
                    title: 'Buyers and sales centers applications',
                    translate: 'navigation.buyers-and-sales-centers-applications',
                    type: 'item',
                    icon: 'description',
                    url: '/sales-centers-applications',
                    permissions: [PermissionsEnum.BuyersApplicationsReadAll, PermissionsEnum.BuyersApplicationsRead],
                    component: BuyersFSCApplicationsComponent,
                    isPublic: false
                },
                {
                    id: 'sales_centers_register',
                    title: 'Buyers and sales centers register',
                    translate: 'navigation.buyers-and-sales-centers-register',
                    type: 'item',
                    icon: 'storage',
                    url: '/sales-centers-register',
                    permissions: [PermissionsEnum.BuyersReadAll, PermissionsEnum.BuyersRead],
                    component: BuyersComponent,
                    isPublic: false
                }
            ]
        },
        {
            id: 'qualified_fishers',
            title: 'Qualified fishers',
            translate: 'navigation.qualified-fishers',
            type: 'collapsable',
            icon: 'fa-id-badge',
            isPublic: false,
            children: [
                {
                    id: 'qualified_fishers_applications',
                    title: 'Qualified fishers applications',
                    translate: 'navigation.qualified-fishers-applications',
                    type: 'item',
                    icon: 'description',
                    url: '/qualified-fishers-applications',
                    permissions: [PermissionsEnum.QualifiedFishersApplicationsReadAll, PermissionsEnum.QualifiedFishersApplicationsRead],
                    component: QualifiedFishersApplicationsComponent,
                    isPublic: false
                },
                {
                    id: 'qualified_fishers_register',
                    title: 'Qualified fishers register',
                    translate: 'navigation.qualified-fishers-register',
                    type: 'item',
                    icon: 'storage',
                    url: '/qualified-fishers-register',
                    permissions: [PermissionsEnum.QualifiedFishersReadAll, PermissionsEnum.QualifiedFishersRead],
                    component: QualifiedFishersComponent,
                    isPublic: false
                }
            ]
        },
        {
            id: 'fishing_quotas',
            title: 'Fishing quotas',
            translate: 'navigation.fishing-quotas',
            type: 'collapsable',
            icon: 'fa-weight',
            isPublic: false,
            children: [{
                id: 'anual_quotas',
                title: 'Anual quotas',
                translate: 'navigation.anual-quotas',
                type: 'item',
                icon: 'fa-calendar-alt',
                url: '/yearly-quotas-register',
                permissions: [PermissionsEnum.YearlyQuotasRead],
                component: YearlyQuotasComponent,
                isPublic: false
            },
            {
                id: 'quota_distribution',
                title: 'Quota distribution',
                translate: 'navigation.quota-distribution',
                type: 'item',
                icon: 'fa-sync',
                url: '/ship-quotas-register',
                permissions: [PermissionsEnum.ShipQuotasRead],
                component: ShipQuotasComponent,
                isPublic: false
            }]
        },
        {
            id: 'commercial_fishing',
            title: 'Commercial fishing',
            translate: 'navigation.commercial-fishing',
            type: 'collapsable',
            icon: 'fa-fish',
            isPublic: false,
            children: [{
                id: 'commercial_fishing_applications',
                title: 'Commercial fishing applications',
                translate: 'navigation.commercial-fishing-applications',
                type: 'item',
                icon: 'description',
                url: '/commercial-fishing-applications',
                permissions: [
                    PermissionsEnum.CommercialFishingPermitApplicationsReadAll,
                    PermissionsEnum.CommercialFishingPermitApplicationsRead,
                    PermissionsEnum.CommercialFishingPermitLicenseApplicationsReadAll,
                    PermissionsEnum.CommercialFishingPermitLicenseApplicationsRead
                ],
                component: CommercialFishingApplicationsComponent,
                isPublic: false
            }, {
                id: 'commercial_fishing_permits_and_licenses',
                title: 'Commercial fishing permits and licenses',
                translate: 'navigation.commercial-fishing-permits-and-licenses',
                type: 'item',
                icon: 'fa-stamp',
                url: '/commercial-fishing-permits-and-licenses',
                permissions: [
                    PermissionsEnum.CommercialFishingPermitRegisterReadAll,
                    PermissionsEnum.CommercialFishingPermitRegisterRead,
                    PermissionsEnum.CommercialFishingPermitLicenseRegisterReadAll,
                    PermissionsEnum.CommercialFishingPermitLicenseRegisterRead
                ],
                component: CommercialFishingRegisterComponent,
                isPublic: false
            }]
        },
        {
            id: 'catches_and_sales',
            title: 'Catches and sales',
            translate: 'navigation.catches-and-sales',
            type: 'item',
            icon: 'fa-money-bill-alt',
            url: '/log-books-and-declarations',
            isPublic: false,
            component: CatchesAndSalesComponent,
            permissions: [
                PermissionsEnum.FishLogBooksReadAll,
                PermissionsEnum.FishLogBookRead,
                PermissionsEnum.FirstSaleLogBooksReadAll,
                PermissionsEnum.FirstSaleLogBookRead,
                PermissionsEnum.AdmissionLogBooksReadAll,
                PermissionsEnum.AdmissionLogBookRead,
                PermissionsEnum.TransportationLogBooksReadAll,
                PermissionsEnum.TransportationLogBookRead,
                PermissionsEnum.AquacultureLogBooksReadAll,
                PermissionsEnum.AquacultureLogBookRead
            ]
        },
        {
            id: 'recreational_fishing',
            title: 'Recreational fishing',
            translate: 'navigation.recreational-fishing',
            type: 'collapsable',
            icon: 'fa-vest',
            isPublic: false,
            children: [
                {
                    id: 'ticket_issuing',
                    title: 'Ticket issuing',
                    translate: 'navigation.ticket-issuing',
                    type: 'item',
                    icon: 'fa-ticket-alt',
                    url: '/ticket_issuing',
                    permissions: [PermissionsEnum.TicketsAddRecords],
                    component: RecreationalFishingTicketsComponent,
                    isPublic: false
                },
                {
                    id: 'issued_tickets',
                    title: 'Issued tickets',
                    translate: 'navigation.issued-tickets',
                    type: 'item',
                    icon: 'fa-handpoint-up',
                    url: '/ticket_applications',
                    permissions: [PermissionsEnum.TicketsApplicationsReadAll, PermissionsEnum.TicketApplicationsRead],
                    component: RecreationalFishingApplicationsComponent,
                    isPublic: false
                },
                {
                    id: 'associations',
                    title: 'Associations',
                    translate: 'navigation.associations',
                    type: 'item',
                    icon: 'fa-users',
                    url: '/associations',
                    permissions: [PermissionsEnum.AssociationsReadAll, PermissionsEnum.AssociationsRead],
                    component: RecreationalFishingAssociationsComponent,
                    isPublic: false
                }
            ]
        },
        {
            id: 'statistical_forms',
            title: 'Statistical forms',
            translate: 'navigation.statistical-forms',
            type: 'collapsable',
            icon: 'fa-chart-line',
            isPublic: false,
            children: [
                {
                    id: 'statistical_forms_applications',
                    title: 'Statistical forms applications',
                    translate: 'navigation.statistical-forms-applications',
                    type: 'item',
                    icon: 'description',
                    url: '/statistical-forms-applications',
                    permissions: [
                        PermissionsEnum.StatisticalFormsAquaFarmApplicationsReadAll,
                        PermissionsEnum.StatisticalFormsAquaFarmApplicationsRead,
                        PermissionsEnum.StatisticalFormsReworkApplicationsReadAll,
                        PermissionsEnum.StatisticalFormsReworkApplicationsRead,
                        PermissionsEnum.StatisticalFormsFishVesselsApplicationsReadAll,
                        PermissionsEnum.StatisticalFormsFishVesselsApplicationsRead
                    ],
                    component: StatisticalFormsApplicationsComponent,
                    isPublic: false
                },
                {
                    id: 'statistical_forms_register',
                    title: 'Statistical forms register',
                    translate: 'navigation.statistical-forms-register',
                    type: 'item',
                    icon: 'storage',
                    url: '/statistical-forms-register',
                    permissions: [
                        PermissionsEnum.StatisticalFormsAquaFarmReadAll,
                        PermissionsEnum.StatisticalFormsAquaFarmRead,
                        PermissionsEnum.StatisticalFormsReworkReadAll,
                        PermissionsEnum.StatisticalFormsReworkRead,
                        PermissionsEnum.StatisticalFormsFishVesselReadAll,
                        PermissionsEnum.StatisticalFormsFishVesselRead
                    ],
                    component: StatisticalFormsComponent,
                    isPublic: false
                }
            ]
        },
        {
            id: 'control_activity',
            title: 'Control activity',
            translate: 'navigation.control-activity',
            type: 'collapsable',
            icon: 'fa-users-cog',
            isPublic: false,
            children: [
                {
                    id: 'checks_and_inspections',
                    title: 'Checks and inspections',
                    translate: 'navigation.checks-and-inspections',
                    type: 'item',
                    icon: 'fa-bug',
                    url: '/inspections-register',
                    permissions: [PermissionsEnum.InspectionsReadAll, PermissionsEnum.InspectionsRead],
                    component: InspectionsComponent,
                    isPublic: false
                },
                {
                    id: 'administrative_violation_act',
                    title: 'Administrative violation acts',
                    translate: 'navigation.administrative-violation-act',
                    type: 'item',
                    icon: 'fa-gavel',
                    url: '/auan-register',
                    permissions: [PermissionsEnum.AuanRegisterReadAll, PermissionsEnum.AuanRegisterRead],
                    component: AuanRegisterComponent,
                    isPublic: false
                },
                {
                    id: 'penal_decrees',
                    title: 'Penal decrees',
                    translate: 'navigation.penal-decrees',
                    type: 'item',
                    icon: 'fa-file-signature',
                    url: '/penal-decrees',
                    permissions: [PermissionsEnum.PenalDecreesReadAll, PermissionsEnum.PenalDecreesRead],
                    component: PenalDecreesComponent,
                    isPublic: false
                },
                {
                    id: 'awarded_points',
                    title: 'Awarded points',
                    translate: 'navigation.awarded-points',
                    type: 'item',
                    icon: 'fa-award',
                    url: '/awarded-points',
                    permissions: [PermissionsEnum.AwardedPointsReadAll, PermissionsEnum.AwardedPointsRead],
                    component: PenalPointsComponent,
                    isPublic: false
                }
            ]
        },
        {
            id: 'scientific_fishing',
            title: 'Scientific fishing',
            translate: 'navigation.scientific-fishing',
            type: 'collapsable',
            icon: 'fa-flask',
            isPublic: false,
            children: [
                {
                    id: 'scientific_fishing_applications',
                    title: 'Scientific fishing applications',
                    translate: 'navigation.scientific-fishing-applications',
                    type: 'item',
                    icon: 'description',
                    url: '/scientific-fishing-applications',
                    permissions: [PermissionsEnum.ScientificFishingApplicationsReadAll, PermissionsEnum.ScientificFishingApplicationsRead],
                    component: ScientificFishingApplicationsComponent,
                    isPublic: false
                },
                {
                    id: 'scientific_fishing_register',
                    title: 'Scientific fishing register',
                    translate: 'navigation.scientific-fishing-register',
                    type: 'item',
                    icon: 'storage',
                    url: '/scientific-fishing',
                    permissions: [PermissionsEnum.ScientificFishingReadAll, PermissionsEnum.ScientificFishingRead],
                    component: ScientificFishingComponent,
                    isPublic: false
                }
            ]
        },
        {
            id: 'reports',
            title: 'Reports',
            translate: 'navigation.reports',
            type: 'collapsable',
            icon: 'fa-th-list',
            isPublic: false,
            children: [
                {
                    id: 'report_execution',
                    title: 'Reports',
                    translate: 'navigation.report-execution',
                    type: 'item',
                    icon: 'fa-book',
                    url: '/reports',
                    permissions: [PermissionsEnum.ReportRead],
                    component: ReportViewComponent,
                    isPublic: false
                },
                {
                    id: 'report_definition',
                    title: 'Report definition',
                    translate: 'navigation.report-definition',
                    type: 'item',
                    icon: 'fa-th-list',
                    url: '/reports/report_definition',
                    permissions: [PermissionsEnum.ReportEditRecords],
                    component: ReportDefinitionComponent,
                    isPublic: false,
                    hideInMenu: true
                },
                {
                    id: 'report_parameter_definition',
                    title: 'Report parameter definition',
                    translate: 'navigation.report-parameter-definition',
                    type: 'item',
                    icon: 'fa-th-list',
                    url: '/report_parameter_definition',
                    permissions: [PermissionsEnum.ReportParameterRead],
                    component: ReportParameterDefinitionComponent,
                    isPublic: false
                }
            ]
        },
        {
            id: 'cross_checks',
            title: 'Cross checks',
            translate: 'navigation.cross-checks',
            type: 'collapsable',
            icon: 'fa-check-double',
            isPublic: false,
            children: [
                {
                    id: 'cross_checks_checks',
                    title: 'Cross checks',
                    translate: 'navigation.cross-checks-checks',
                    type: 'item',
                    icon: 'fa-people-arrows',
                    url: '/cross-checks',
                    permissions: [PermissionsEnum.CrossChecksRead],
                    component: CrossChecksComponent,
                    isPublic: false
                },
                {
                    id: 'cross_checks_results',
                    title: 'Cross checks results',
                    translate: 'navigation.cross-check-results',
                    type: 'item',
                    icon: 'fa-clipboard-check',
                    url: '/cross-checks-results',
                    permissions: [PermissionsEnum.CrossCheckResultsRead],
                    component: CrossChecksResultsComponent,
                    isPublic: false
                }
            ]
        },
        {
            id: 'personal_data_legal_entities_and_persons_reports',
            title: 'Personal data of Legal entities and Persons reports',
            translate: 'navigation.personal-data-legal-entities-and-persons-reports',
            type: 'collapsable',
            icon: 'group',
            isPublic: false,
            children: [
                {
                    id: 'persons_report',
                    title: 'Persons report',
                    translate: 'navigation.persons-report',
                    type: 'item',
                    icon: 'fa-user',
                    url: '/persons-report',
                    permissions: [PermissionsEnum.PersonsReportRead],
                    component: PersonsReportComponent,
                    isPublic: false
                },
                {
                    id: 'legal_entities_report',
                    title: 'Legal entities report',
                    translate: 'navigation.legal-entities-report',
                    type: 'item',
                    icon: 'fa-user-tie',
                    url: '/legal-entities-report',
                    permissions: [PermissionsEnum.LegalEntitiesReportRead],
                    component: LegalEntitiesReportComponent,
                    isPublic: false
                },
                {
                    id: 'applications_for_access_by_legal-entities',
                    title: 'Applications for access by legal entities',
                    translate: 'navigation.applications-for-access-by-legal-entities',
                    type: 'item',
                    icon: 'description',
                    url: '/legal-entities-applications',
                    permissions: [PermissionsEnum.LegalEntitiesApplicationsReadAll, PermissionsEnum.LegalEntitiesApplicationsRead],
                    component: LegalEntitiesApplicationsComponent,
                    isPublic: false
                },
                {
                    id: 'legal_entities',
                    title: 'Legal entities',
                    translate: 'navigation.legal-entities',
                    type: 'item',
                    icon: 'fa-balance-scale',
                    url: '/legal-entities',
                    permissions: [PermissionsEnum.LegalEntitiesReadAll, PermissionsEnum.LegalEntitiesRead],
                    component: LegalEntitiesComponent,
                    isPublic: false
                }
            ]
        },
        {
            id: 'administration',
            title: 'Administration',
            translate: 'navigation.administration',
            type: 'collapsable',
            icon: 'fa-cogs',
            isPublic: false,
            children: [
                {
                    id: 'system_log',
                    title: 'System log',
                    translate: 'navigation.system-log',
                    type: 'item',
                    icon: 'fa-sd-card',
                    url: '/system-log',
                    permissions: [PermissionsEnum.SystemLogRead],
                    component: SystemLogComponent,
                    isPublic: false
                },
                {
                    id: 'error_log',
                    title: 'Error log',
                    translate: 'navigation.error-log',
                    type: 'item',
                    icon: 'fa-sd-card',
                    url: '/error-log',
                    permissions: [PermissionsEnum.ErrorLogRead],
                    component: ErrorLogComponent,
                    isPublic: false
                },
                {
                    id: 'translation_labels_management',
                    title: 'Label translations management',
                    translate: 'navigation.interface-translations-labels',
                    type: 'item',
                    icon: 'fa-globe',
                    url: '/translation-labels',
                    permissions: [PermissionsEnum.TranslationRead],
                    component: TranslationLabelsComponent,
                    isPublic: false
                },
                {
                    id: 'translation_help_management',
                    title: 'Help translations management',
                    translate: 'navigation.interface-translations-help',
                    type: 'item',
                    icon: 'fa-globe',
                    url: '/translation-help',
                    permissions: [PermissionsEnum.TranslationRead],
                    component: TranslationHelpComponent,
                    isPublic: false
                },
                {
                    id: 'nomenclatures',
                    title: 'Nomenclatures',
                    translate: 'navigation.nomenclatures',
                    type: 'item',
                    icon: 'fa-database',
                    url: '/nomenclatures',
                    permissions: [PermissionsEnum.NomenclaturesRead],
                    component: NomenclaturesComponent,
                    isPublic: false
                },
                {
                    id: 'external_users',
                    title: 'External users',
                    translate: 'navigation.external-users',
                    type: 'item',
                    icon: 'fa-address-book-regular',
                    url: '/external-users',
                    permissions: [PermissionsEnum.ExternalUsersRead],
                    component: ExternalUsersComponent,
                    isPublic: false
                },
                {
                    id: 'internal_users',
                    title: 'Internal users',
                    translate: 'navigation.internal-users',
                    type: 'item',
                    icon: 'fa-address-book',
                    url: '/internal-users',
                    permissions: [PermissionsEnum.InternalUsersRead],
                    component: InternalUsersComponent,
                    isPublic: false
                },
                {
                    id: 'inspectors',
                    title: 'Inspectors',
                    translate: 'navigation.inspectors',
                    type: 'item',
                    icon: 'fa-users-cog',
                    url: '/inspectors',
                    permissions: [PermissionsEnum.InspectorsRead],
                    component: InspectorsRegisterComponent,
                    isPublic: false
                },
                {
                    id: 'patrol_vehicles',
                    title: 'Patrol vehicles',
                    translate: 'navigation.patrol-vehicles',
                    type: 'item',
                    icon: 'fa-ship',
                    url: '/patrol-vehicles',
                    permissions: [PermissionsEnum.PatrolVehiclesRead],
                    component: PatrolVehiclesComponent,
                    isPublic: false
                },
                {
                    id: 'news_management',
                    title: 'News management',
                    translate: 'navigation.news-management',
                    type: 'item',
                    icon: 'fa-newspaper',
                    url: '/news-management',
                    permissions: [PermissionsEnum.NewsManagementRead],
                    component: NewsManagementComponent,
                    isPublic: false
                },
                {
                    id: 'roles',
                    title: 'Roles',
                    translate: 'navigation.roles',
                    type: 'item',
                    icon: 'group',
                    url: '/roles',
                    permissions: [PermissionsEnum.RolesRegisterRead],
                    component: RolesRegisterComponent,
                    isPublic: false
                },
                {
                    id: 'permissions',
                    title: 'Permissions',
                    translate: 'navigation.permissions',
                    type: 'item',
                    icon: 'gavel',
                    url: '/permissions',
                    permissions: [PermissionsEnum.PermissionsRegisterRead],
                    component: PermissionsRegisterComponent,
                    isPublic: false
                },
                {
                    id: 'fluxvmx-requests',
                    title: 'FLUXVMSRequests',
                    translate: 'navigation.flux-vms-requests',
                    type: 'item',
                    icon: 'fa-hourglass-half',
                    url: '/flux-vms-requests',
                    permissions: [PermissionsEnum.FLUXVMSRequestsRead],
                    component: FluxVmsRequestsComponent,
                    isPublic: false
                },
                {
                    id: 'application-regix-checks',
                    title: 'ApplicationRegiXChecks',
                    translate: 'navigation.application-regix-checks',
                    type: 'item',
                    icon: 'fa-hourglass-half',
                    url: '/regix-checks',
                    permissions: [PermissionsEnum.ApplicationRegiXChecksRead],
                    component: ApplicationRegixChecksComponent,
                    isPublic: false
                }
            ]
        },
        {
            id: 'my_profile',
            title: 'My profile',
            translate: '',
            type: 'item',
            hideInMenu: true,
            url: '/my-profile',
            component: MyProfileAdministrationComponent,
            isPublic: false
        }
    ];
}