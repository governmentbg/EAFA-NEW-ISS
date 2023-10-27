import { MyProfilePublicComponent } from '@app/components/public-app/my-profile-public/my-profile-public.component';
import { NewsDetailsComponent } from '@app/components/public-app/news-details/news-details.component';
import { NewsComponent } from '@app/components/public-app/news/news.component';
import { PaymentRedirectPageComponent } from '@app/components/public-app/payment-redirect/payment-redirect.component';
import { RecreationalFishingAssociationApplicationsComponent } from '@app/components/public-app/recreational-fishing/association/applications/recreational-fishing-association-applications.component';
import { RecreationalFishingAssociationTicketsComponent } from '@app/components/public-app/recreational-fishing/association/tickets/recreational-fishing-association-tickets.component';
import { RecreationalFishingTicketsComponent } from '@app/components/public-app/recreational-fishing/tickets/recreational-fishing-tickets.component';
import { ReportViewComponent } from '@app/components/public-app/reports/report-view.component';
import { ScientificFishingComponent } from '@app/components/public-app/scientific-fishing-register/scientific-fishing.component';
import { StatisticalFormsComponent } from '@app/components/public-app/statistical-forms/statistical-forms.component';
import { SubmittedApplicationsComponent } from '@app/components/public-app/submitted-applications/submitted-applications.component';
import { PermissionsEnum } from '../enums/permissions.enum';
import { ITLNavigation } from './base/tl-navigation.interface';
import { CatchesAndSalesPublicComponent } from '@app/components/public-app/catches-and-sales/catches-and-sales.component';
import { HomePagePublicComponent } from '@app/components/public-app/home-page-public/home-page-public.component';

export class Navigation {
    public static getMenu(isPublic: boolean): ITLNavigation[] {
        return Navigation.Menu;
    }

    public static Menu: ITLNavigation[] = [
        {
            id: 'home',
            title: 'Home',
            translate: 'navigation.home-page-public',
            type: 'item',
            icon: 'ic-home',
            url: '/home',
            permissions: [],
            exceptPermissions: [
                PermissionsEnum.OnlineSubmittedApplicationsRead,
                PermissionsEnum.FishLogBookRead,
                PermissionsEnum.FirstSaleLogBookRead,
                PermissionsEnum.AdmissionLogBookRead,
                PermissionsEnum.TransportationLogBookRead,
                PermissionsEnum.AquacultureLogBookRead,
                PermissionsEnum.TicketsPublicRead,
                PermissionsEnum.TicketsPublicAddRecords,
                PermissionsEnum.ScientificFishingRead,
                PermissionsEnum.StatisticalFormsAquaFarmRead,
                PermissionsEnum.StatisticalFormsReworkRead,
                PermissionsEnum.StatisticalFormsFishVesselRead,
                PermissionsEnum.AssociationsTicketsRead,
                PermissionsEnum.AssociationsTicketsAddRecords,
                PermissionsEnum.ReportRead
            ],
            component: HomePagePublicComponent,
            isPublic: true,
        },
        {
            id: 'news',
            title: 'News',
            translate: 'navigation.news',
            type: 'item',
            icon: 'ic-newspaper-variant-multiple-outline',
            url: '/news',
            permissions: [],
            component: NewsComponent,
            isPublic: true,
        },
        {
            id: 'news-details',
            title: 'News details',
            translate: 'navigation.news-details',
            type: 'item',
            icon: 'ic-newspaper-variant-multiple-outline',
            url: '/news/:id',
            permissions: [],
            component: NewsDetailsComponent,
            isPublic: true,
            hideInMenu: true
        },
        {
            id: 'submitted-applications',
            title: 'Submitted applications',
            translate: 'navigation.submitted-applications',
            type: 'item',
            icon: 'description',
            url: '/submitted-applications',
            permissions: [PermissionsEnum.OnlineSubmittedApplicationsRead],
            component: SubmittedApplicationsComponent,
            isPublic: false
        },
        {
            id: 'catches_and_sales_public',
            title: 'Catches and sales public',
            translate: 'navigation.catches-and-sales-public',
            type: 'item',
            icon: 'fa-money-bill-alt',
            url: '/log-books-and-declarations',
            isPublic: false,
            component: CatchesAndSalesPublicComponent,
            permissions: [
                PermissionsEnum.FishLogBookRead,
                PermissionsEnum.FirstSaleLogBookRead,
                PermissionsEnum.AdmissionLogBookRead,
                PermissionsEnum.TransportationLogBookRead,
                PermissionsEnum.AquacultureLogBookRead
            ]
        },
        {
            id: 'recreational_fishing',
            title: 'Recreational fishing',
            translate: 'navigation.recreational-fishing-public',
            type: 'item',
            icon: 'fa-ticket-alt',
            url: '/recreational-fishing',
            permissions: [PermissionsEnum.TicketsPublicRead],
            component: RecreationalFishingTicketsComponent,
            isPublic: false
        },
        {
            id: 'recreational_fishing_buy_ticket',
            title: 'Recreational fishing buy ticket',
            translate: 'navigation.recreational-fishing-public',
            type: 'item',
            icon: 'fa-ticket-alt',
            url: '/recreational-fishing/purchase-ticket',
            permissions: [PermissionsEnum.TicketsPublicAddRecords],
            component: RecreationalFishingTicketsComponent,
            hideInMenu: true,
            isPublic: false
        },
        {
            id: 'scientific_fishing',
            title: 'Scientific fishing',
            translate: 'navigation.scientific-fishing',
            type: 'item',
            icon: 'fa-flask',
            url: '/scientific-fishing',
            permissions: [PermissionsEnum.ScientificFishingRead],
            component: ScientificFishingComponent,
            isPublic: false
        },
        {
            id: 'statistical_forms',
            title: 'Statistical forms',
            translate: 'navigation.statistical-forms',
            type: 'item',
            icon: 'fa-chart-line',
            url: '/statistical-forms',
            permissions: [
                PermissionsEnum.StatisticalFormsAquaFarmRead,
                PermissionsEnum.StatisticalFormsReworkRead,
                PermissionsEnum.StatisticalFormsFishVesselRead
            ],
            component: StatisticalFormsComponent,
            isPublic: false
        },
        {
            id: 'association',
            title: 'Association',
            translate: 'navigation.association',
            type: 'collapsable',
            icon: 'fa-vest',
            isPublic: false,
            permissions: [
                PermissionsEnum.AssociationsTicketsRead,
                PermissionsEnum.AssociationsTicketsAddRecords
            ],
            children: [
                {
                    id: 'association_ticket_issuing',
                    title: 'Association ticket issuing',
                    translate: 'navigation.association-ticket-issuing',
                    type: 'item',
                    icon: 'fa-ticket-alt',
                    url: '/association_ticket_issuing',
                    permissions: [PermissionsEnum.AssociationsTicketsAddRecords],
                    component: RecreationalFishingAssociationTicketsComponent,
                    isPublic: false
                },
                {
                    id: 'association_issued_tickets',
                    title: 'Association issued tickets',
                    translate: 'navigation.association-issued-tickets',
                    type: 'item',
                    icon: 'fa-handpoint-up',
                    url: '/association_issued_tickets',
                    permissions: [PermissionsEnum.AssociationsTicketsRead],
                    component: RecreationalFishingAssociationApplicationsComponent,
                    isPublic: false
                }
            ]
        },
        {
            id: 'reports',
            title: 'Reports',
            translate: 'navigation.reports',
            type: 'item',
            icon: 'fa-th-list',
            url: '/reports',
            permissions: [PermissionsEnum.ReportRead],
            component: ReportViewComponent,
            isPublic: true,
        },
        {
            id: 'my_profile',
            title: 'My profile',
            translate: '',
            type: 'item',
            hideInMenu: true,
            url: '/my-profile',
            component: MyProfilePublicComponent,
            isPublic: false
        },
        {
            id: 'payment-redirect',
            title: 'Payment redirect',
            translate: '',
            type: 'item',
            hideInMenu: true,
            url: '/payment-redirect',
            component: PaymentRedirectPageComponent,
            isPublic: false
        }
    ];
}