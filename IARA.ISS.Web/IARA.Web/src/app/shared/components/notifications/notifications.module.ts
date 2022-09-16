import { CdkScrollableModule, ScrollingModule } from '@angular/cdk/scrolling';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatMenuModule } from '@angular/material/menu';
import { BrowserModule } from '@angular/platform-browser';
import { TLCardModule } from '../tl-card/tl-card.module';
import { TLIconModule } from '../tl-icon/tl-icon.module';
import { NotificationComponent } from './notification.component';
import { NotificationsMenuComponent } from './notifications-menu.component';



@NgModule({
    imports: [
        BrowserModule,
        CdkScrollableModule,
        CommonModule,
        FlexLayoutModule,
        MatCardModule,
        ScrollingModule,
        TLCardModule,
        TLIconModule,
        MatMenuModule,
        MatButtonModule
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