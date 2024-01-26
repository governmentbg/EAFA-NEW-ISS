import { CdkScrollableModule, ScrollingModule } from '@angular/cdk/scrolling';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { MatBadgeModule } from '@angular/material/badge';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatMenuModule } from '@angular/material/menu';
import { TLPipesModule } from '../../pipes/tl-pipes.module';
import { TLCardModule } from '../tl-card/tl-card.module';
import { TLIconModule } from '../tl-icon/tl-icon.module';
import { NotificationComponent } from './notification.component';
import { NotificationsMenuComponent } from './notifications-menu.component';

@NgModule({
    imports: [
        CdkScrollableModule,
        CommonModule,
        FlexLayoutModule,
        MatCardModule,
        ScrollingModule,
        TLCardModule,
        TLIconModule,
        MatMenuModule,
        MatButtonModule,
        MatBadgeModule,
        TLPipesModule
    ],
    exports: [
        NotificationComponent,
        NotificationsMenuComponent
    ],
    declarations: [
        NotificationComponent,
        NotificationsMenuComponent
    ]
})
export class NotificationsModule {
}