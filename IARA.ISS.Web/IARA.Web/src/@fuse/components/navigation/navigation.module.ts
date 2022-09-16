import { TLIconModule } from '@app/shared/components/tl-icon/tl-icon.module';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { MatRippleModule } from '@angular/material/core';
import { MatIconModule } from '@angular/material/icon';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { FuseNavHorizontalCollapsableComponent } from './horizontal/collapsable/collapsable.component';
import { FuseNavHorizontalItemComponent } from './horizontal/item/item.component';
import { FuseNavigationComponent } from './navigation.component';
import { FuseNavVerticalCollapsableComponent } from './vertical/collapsable/collapsable.component';
import { FuseNavVerticalGroupComponent } from './vertical/group/group.component';
import { FuseNavVerticalItemComponent } from './vertical/item/item.component';



@NgModule({
    imports: [
        CommonModule,
        RouterModule,
        MatIconModule,
        MatRippleModule,
        TLIconModule,
        TranslateModule.forChild()
    ],
    exports: [
        FuseNavigationComponent
    ],
    declarations: [
        FuseNavigationComponent,
        FuseNavVerticalGroupComponent,
        FuseNavVerticalItemComponent,
        FuseNavVerticalCollapsableComponent,
        FuseNavHorizontalItemComponent,
        FuseNavHorizontalCollapsableComponent
    ]
})
export class FuseNavigationModule {
}