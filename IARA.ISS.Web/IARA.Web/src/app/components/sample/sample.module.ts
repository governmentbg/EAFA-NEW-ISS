import { TLFileUploadModule } from '@app/shared/components/file-upload/file-upload.module';
import { TLCommonModule } from '@app/shared/tl-common.module';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FuseSharedModule } from '@fuse/fuse-shared.module';
import { SampleDialogComponent } from './sample-dialog/sample-dialog.component';
import { SampleComponent } from './sample.component';

const routes = [
    {
        path: 'sample',
        component: SampleComponent
    }
];

@NgModule({
    declarations: [
        SampleComponent, SampleDialogComponent
    ],
    imports: [
        RouterModule.forChild(routes),
        FuseSharedModule,
        TLFileUploadModule,
        TLCommonModule,
    ],
    exports: [
        SampleComponent
    ]
})

export class SampleModule {
}