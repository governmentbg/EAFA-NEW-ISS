import { MainNavigation } from '@app/shared/navigation/base/main.navigation';
import { TLCommonModule } from '@app/shared/tl-common.module';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { MaterialModule } from '@app/shared/material.module';
import { CommonApplicationModule } from '../common-app/common-app.module';
import { MyProfilePublicComponent } from './my-profile-public/my-profile-public.component';
import { NewsCardComponent } from './news-card/news-card.component';
import { NewsComponent } from './news/news.component';
import { PaymentRedirectPageComponent } from './payment-redirect/payment-redirect.component';
import { AssociationPickerComponent } from './recreational-fishing/association-picker/association-picker.component';
import { RecreationalFishingAssociationApplicationsComponent } from './recreational-fishing/association/applications/recreational-fishing-association-applications.component';
import { RecreationalFishingAssociationTicketsComponent } from './recreational-fishing/association/tickets/recreational-fishing-association-tickets.component';
import { RecreationalFishingMyTicketsModule } from './recreational-fishing/my-tickets/recreational-fishing-my-tickets.module';
import { RecreationalFishingTicketsComponent } from './recreational-fishing/tickets/recreational-fishing-tickets.component';
import { ScientificFishingComponent } from './scientific-fishing-register/scientific-fishing.component';
import { SubmittedApplicationsComponent } from './submitted-applications/submitted-applications.component';
import { NewsDetailsComponent } from './news-details/news-details.component';
import { CatchesAndSalesPublicComponent } from './catches-and-sales/catches-and-sales.component';
import { StatisticalFormsComponent } from './statistical-forms/statistical-forms.component';
import { ReportViewComponent } from './reports/report-view.component';
import { HomePagePublicComponent } from './home-page-public/home-page-public.component';
import { HomePageCardComponent } from './home-page-public/home-page-card.component';

@NgModule({
    declarations: [
        NewsComponent,
        NewsCardComponent,
        NewsDetailsComponent,
        ScientificFishingComponent,
        RecreationalFishingAssociationApplicationsComponent,
        RecreationalFishingAssociationTicketsComponent,
        RecreationalFishingTicketsComponent,
        AssociationPickerComponent,
        MyProfilePublicComponent,
        SubmittedApplicationsComponent,
        PaymentRedirectPageComponent,
        CatchesAndSalesPublicComponent,
        StatisticalFormsComponent,
        ReportViewComponent,
        HomePagePublicComponent,
        HomePageCardComponent
    ],
    imports: [
        TLCommonModule,
        MaterialModule,
        CommonApplicationModule,
        RouterModule.forChild(MainNavigation.getRoutes()),
        RecreationalFishingMyTicketsModule
    ],
    exports: [
        NewsComponent,
        NewsCardComponent,
        NewsDetailsComponent,
        ScientificFishingComponent,
        RecreationalFishingAssociationApplicationsComponent,
        RecreationalFishingAssociationTicketsComponent,
        RecreationalFishingTicketsComponent,
        AssociationPickerComponent,
        MyProfilePublicComponent,
        SubmittedApplicationsComponent,
        PaymentRedirectPageComponent,
        CatchesAndSalesPublicComponent,
        StatisticalFormsComponent,
        ReportViewComponent,
        HomePagePublicComponent,
        HomePageCardComponent
    ]
})
export class IARAApplicationModule {
}