import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FuseDirectivesModule } from '@fuse/directives/directives';
import { FusePipesModule } from '@fuse/pipes/pipes.module';
import { FuseAlertModule } from './components/alert/alert.module';
import { FuseConfirmDialogModule } from './components/confirm-dialog/confirm-dialog.module';



@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        FuseAlertModule,
        FlexLayoutModule,

        FuseDirectivesModule,
        FusePipesModule,
        FuseConfirmDialogModule
    ],
    exports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,

        FlexLayoutModule,

        FuseDirectivesModule,
        FusePipesModule,
        FuseConfirmDialogModule
    ]
})
export class FuseSharedModule {
}