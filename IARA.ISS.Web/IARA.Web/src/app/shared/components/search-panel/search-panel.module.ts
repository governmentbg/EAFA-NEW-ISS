import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatTooltipModule } from '@angular/material/tooltip';
import { TLDirectivesModule } from '@app/shared/directives/tl-directives.module';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import filterRemove from '@iconify/icons-mdi/filter-remove';
import groupAdd from '@iconify/icons-mdi/group-add';
import home from '@iconify/icons-mdi/home';
import { TranslateModule } from '@ngx-translate/core';
import { IconModule, IconService } from '@visurel/iconify-angular';
import { TLPipesModule } from '../../pipes/tl-pipes.module';
import { TLIconButtonModule } from '../tl-icon-button/tl-icon-button.module';
import { SearchPanelComponent } from './search-panel.component';

export const appIcons = {
    home,
    'group-add': groupAdd,
    'filter-remove': filterRemove
}

@NgModule({
    imports: [
        CommonModule,
        FlexLayoutModule,
        FontAwesomeModule,
        FormsModule,
        IconModule,
        MatButtonModule,
        MatCardModule,
        MatChipsModule,
        MatExpansionModule,
        MatFormFieldModule,
        MatIconModule,
        MatInputModule,
        MatSelectModule,
        MatTooltipModule,
        ReactiveFormsModule,
        TLIconButtonModule,
        TLPipesModule,
        TranslateModule,
        TLDirectivesModule
    ],
    exports: [
        SearchPanelComponent,
    ],
    declarations: [
        SearchPanelComponent
    ]
})
export class SearchPanelModule {
    constructor(iconService: IconService) {
        iconService.registerAll(appIcons);
    }
}