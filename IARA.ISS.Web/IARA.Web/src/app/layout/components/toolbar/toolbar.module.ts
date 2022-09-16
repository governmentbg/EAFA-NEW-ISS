import { ScrollingModule } from '@angular/cdk/scrolling';
import { NgModule } from '@angular/core';
import { MatBadgeModule } from '@angular/material/badge';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatToolbarModule } from '@angular/material/toolbar';
import { RouterModule } from '@angular/router';
import { RecreationalFishingMyTicketsModule } from '@app/components/public-app/recreational-fishing/my-tickets/recreational-fishing-my-tickets.module';
import { ToolbarComponent } from '@app/layout/components/toolbar/toolbar.component';
import { NotificationsMenuComponent } from '@app/shared/components/notifications/notifications-menu.component';
import { NotificationsModule } from '@app/shared/components/notifications/notifications.module';
import { TLIconModule } from '@app/shared/components/tl-icon/tl-icon.module';
import { TLPipesModule } from '@app/shared/pipes/tl-pipes.module';
import { TLCommonModule } from '@app/shared/tl-common.module';
import { FuseSearchBarModule, FuseShortcutsModule } from '@fuse/components';
import { FuseSharedModule } from '@fuse/fuse-shared.module';

@NgModule({
    declarations: [
        ToolbarComponent
    ],
    imports: [
        RouterModule,
        // TranslateModule,
        MatButtonModule,
        MatIconModule,
        MatBadgeModule,
        MatMenuModule,
        MatToolbarModule,

        FuseSharedModule,
        FuseSearchBarModule,
        FuseShortcutsModule,
        TLPipesModule,

        RecreationalFishingMyTicketsModule,
        TLIconModule,
        ScrollingModule,
        TLCommonModule,
        NotificationsModule
    ],
    exports: [
        ToolbarComponent,
        NotificationsMenuComponent
    ]
})
export class ToolbarModule {
}