import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { TLHelpModule } from '../tl-help/tl-help.module';
import { TLIconModule } from '../tl-icon/tl-icon.module';
import { TLPictureUploaderModule } from '../tl-picture-uploader/tl-picture-uploader.module';
import { TLCardContentComponent } from './components/tl-card-content/tl-card-content.component';
import { TLCardSubtitleComponent } from './components/tl-card-subtitle/tl-card-subtitle.component';
import { TLCardTitleComponent } from './components/tl-card-title/tl-card-title.component';
import { TLCardComponent } from './tl-card.component';

@NgModule({
    imports: [
        CommonModule,
        MatCardModule,
        FlexLayoutModule,
        ReactiveFormsModule,
        TLIconModule,
        TLHelpModule,
        TLPictureUploaderModule
    ],
    exports: [
        TLCardComponent,
        TLCardTitleComponent,
        TLCardSubtitleComponent,
        TLCardContentComponent
    ],
    declarations: [
        TLCardComponent,
        TLCardTitleComponent,
        TLCardSubtitleComponent,
        TLCardContentComponent
    ]
})
export class TLCardModule {
}