import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FlexLayoutModule } from '@angular/flex-layout';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatTooltipModule } from '@angular/material/tooltip';

import { FusePipesModule } from '@fuse/pipes/pipes.module';

import { FuseMaterialColorPickerComponent } from '@fuse/components/material-color-picker/material-color-picker.component';
import { TranslateModule } from '@ngx-translate/core';

@NgModule({
    declarations: [
        FuseMaterialColorPickerComponent
    ],
    imports: [
        CommonModule,
        TranslateModule,

        FlexLayoutModule,

        MatButtonModule,
        MatIconModule,
        MatMenuModule,
        MatTooltipModule,

        FusePipesModule
    ],
    exports: [
        FuseMaterialColorPickerComponent
    ],
})
export class FuseMaterialColorPickerModule {
}