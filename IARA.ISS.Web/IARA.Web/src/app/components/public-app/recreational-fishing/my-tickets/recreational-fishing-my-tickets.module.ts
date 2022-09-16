import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatBadgeModule } from '@angular/material/badge';
import { TLCardModule } from '@app/shared/components/tl-card/tl-card.module';
import { TLIconModule } from '@app/shared/components/tl-icon/tl-icon.module';
import { TLPipesModule } from '@app/shared/pipes/tl-pipes.module';
import { RecreationalFishingMyTicketsComponent } from './recreational-fishing-my-tickets.component';
import { RecreationalFishingTicketCardComponent } from './ticket-card/recreational-fishing-ticket-card.component';

@NgModule({
    declarations: [
        RecreationalFishingMyTicketsComponent,
        RecreationalFishingTicketCardComponent
    ],
    imports: [
        CommonModule,
        MatMenuModule,
        MatButtonModule,
        FlexLayoutModule,
        MatTooltipModule,
        MatBadgeModule,

        TLIconModule,
        TLCardModule,
        TLPipesModule
    ],
    exports: [
        RecreationalFishingMyTicketsComponent,
        RecreationalFishingTicketCardComponent
    ]
})
export class RecreationalFishingMyTicketsModule {
}