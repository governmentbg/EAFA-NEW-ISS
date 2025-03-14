import { NgModule } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

import { FuseNavigationModule } from '@fuse/components';
import { FuseSharedModule } from '@fuse/fuse-shared.module';

import { NavbarVerticalStyle2Component } from '@app/layout/components/navbar/vertical/style-2/style-2.component';
import { TranslateModule } from '@ngx-translate/core';

@NgModule({
    declarations: [
        NavbarVerticalStyle2Component
    ],
    imports: [
        MatButtonModule,
        MatIconModule,
        TranslateModule.forChild(),
        FuseSharedModule,
        FuseNavigationModule
    ],
    exports: [
        NavbarVerticalStyle2Component
    ]
})
export class NavbarVerticalStyle2Module {
}