import { NgModule } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { NavbarVerticalStyle1Component } from '@app/layout/components/navbar/vertical/style-1/style-1.component';
import { FuseNavigationModule } from '@fuse/components';
import { FuseSharedModule } from '@fuse/fuse-shared.module';
import { TranslateModule } from '@ngx-translate/core';



@NgModule({
    declarations: [
        NavbarVerticalStyle1Component
    ],
    imports: [
        MatButtonModule,
        MatIconModule,
        TranslateModule,
        FuseSharedModule,
        FuseNavigationModule
    ],
    exports: [
        NavbarVerticalStyle1Component
    ]
})
export class NavbarVerticalStyle1Module {
}