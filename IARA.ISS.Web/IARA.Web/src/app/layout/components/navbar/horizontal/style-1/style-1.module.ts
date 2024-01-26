import { NgModule } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { NavbarHorizontalStyle1Component } from '@app/layout/components/navbar/horizontal/style-1/style-1.component';
import { FuseNavigationModule } from '@fuse/components';
import { FuseSharedModule } from '@fuse/fuse-shared.module';
import { TranslateModule } from '@ngx-translate/core';



@NgModule({
    declarations: [
        NavbarHorizontalStyle1Component
    ],
    imports: [
        MatButtonModule,
        MatIconModule,
        TranslateModule,
        FuseSharedModule,
        FuseNavigationModule
    ],
    exports: [
        NavbarHorizontalStyle1Component
    ]
})
export class NavbarHorizontalStyle1Module {
}