import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatTooltipModule } from '@angular/material/tooltip';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { MaterialModule } from '../../material.module';
import { NomenclatureDisplayPipe } from '../../pipes/nomenclature-display.pipe';
import { TLCoordinatesPipe } from '../../pipes/tl-coordinates.pipe';
import { TLPipesModule } from '../../pipes/tl-pipes.module';
import { TLTranslatePipe } from '../../pipes/tl-translate.pipe';
import { TLInputControlsModule } from '../input-controls/tl-input-controls.module';
import { TLIconButtonModule } from '../tl-icon-button/tl-icon-button.module';
import { TLIconModule } from '../tl-icon/tl-icon.module';
import { TLDataColumnInlineEditingComponent } from './data-column-inline-editing/data-column-inline-editing.component';
import { TLDataColumnTemplateComponent } from './data-column-template/data-column-template.component';
import { TLDataColumnComponent } from './data-column/data-column.component';
import { TLGroupHeaderComponent } from './group-header/group-header.component';
import { TLRowDetailComponent } from './row-detail/row-detail.component';
import { TLDataTableComponent } from './tl-data-table.component';

@NgModule({
    imports: [
        CommonModule,
        FlexLayoutModule,
        FormsModule,
        MatButtonModule,
        MatCardModule,
        MaterialModule,
        MatIconModule,
        MatProgressBarModule,
        MatSlideToggleModule,
        MatTooltipModule,
        ReactiveFormsModule,
        TLIconButtonModule,
        TLIconModule,
        TLInputControlsModule,
        TLPipesModule,
        NgxDatatableModule.forRoot({
            messages: {
                emptyMessage: 'Няма данни', // this._translationLoader.getValue('table-search.no-data-found'), //
                totalMessage: 'Общо', // this._translationLoader.getValue('table-search.total'), //
                selectedMessage: 'Избрани' // this._translationLoader.getValue('table-search.selected'), //
            }
        }),
    ],
    exports: [
        TLDataColumnComponent,
        TLDataColumnTemplateComponent,
        TLDataTableComponent,
        TLDataColumnInlineEditingComponent,
        TLRowDetailComponent,
        TLGroupHeaderComponent
    ],
    declarations: [
        TLDataColumnComponent,
        TLDataColumnTemplateComponent,
        TLDataTableComponent,
        TLDataColumnInlineEditingComponent,
        TLRowDetailComponent,
        TLGroupHeaderComponent
    ],
    providers: [TLTranslatePipe, NomenclatureDisplayPipe, TLCoordinatesPipe]
})
export class TLDataTableModule { }