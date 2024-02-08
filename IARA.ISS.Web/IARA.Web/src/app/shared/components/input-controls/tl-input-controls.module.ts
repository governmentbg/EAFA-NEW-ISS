import { NgxMatDateFormats, NgxMatDatetimePickerModule, NgxMatTimepickerModule, NGX_MAT_DATE_FORMATS } from '@angular-material-components/datetime-picker';
import { NgxMatMomentModule } from '@angular-material-components/moment-adapter';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatRadioModule } from '@angular/material/radio';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatTooltipModule } from '@angular/material/tooltip';
import { BrowserModule } from '@angular/platform-browser';
import { TLPipesModule } from '../../pipes/tl-pipes.module';
import { TLHelpModule } from '../tl-help/tl-help.module';
import { TLIconButtonModule } from '../tl-icon-button/tl-icon-button.module';
import { TLErrorComponent } from './components/tl-error/tl-error.component';
import { TLErrorModule } from './components/tl-error/tl-error.module';
import { TLInputAutocompleteComponent } from './tl-autocomplete/tl-autocomplete.component';
import { TLCheckboxTemplateComponent } from './tl-checkbox/components/tl-checkbox-template/tl-checkbox-template.component';
import { TLCheckboxComponent } from './tl-checkbox/tl-checkbox.component';
import { CoordinatesInputComponent } from './tl-coordinates/coordinates-input.component';
import { TLCoordinatesComponents } from './tl-coordinates/tl-coordinates.component';
import { TLDateMonthComponent } from './tl-date-month/tl-date-month.component';
import { TLDateRangeComponent } from './tl-date-range/tl-date-range.component';
import { TLDateTimeComponent } from './tl-date-time/tl-date-time.component';
import { TLDateYearComponent } from './tl-date-year/tl-date-year.component';
import { TLDateComponent } from './tl-date/tl-date.component';
import { TLInputStepperComponent } from './tl-input-stepper/tl-input-stepper.component';
import { TLInputComponent } from './tl-input/tl-input.component';
import { TLPrefixInputComponent } from './tl-prefix-input/tl-prefix-input.component';
import { TLRadioButtonGroupComponent } from './tl-radio-button-group/tl-radio-button-group.component';
import { TLRadioButtonTemplateComponent } from './tl-radio-button-group/tl-radio-button-template/tl-radio-button-template.component';
import { TLRadioButtonComponent } from './tl-radio-button-group/tl-radio-button/tl-radio-button.component';
import { TLRangeInputComponent } from './tl-range-input/tl-range-input.component';
import { TLRangeInputModule } from './tl-range-input/tl-range-input.module';
import { TLSelectComponent } from './tl-select/tl-select.component';
import { TLSlideToggleComponent } from './tl-slide-toggle/tl-slide-toggle.component';
import { TLTextareaComponent } from './tl-textarea/tl-textarea.component';


const CUSTOM_DATE_FORMATS: NgxMatDateFormats = {
    parse: {
        dateInput: 'l, LTS'
    },
    display: {
        dateInput: 'YYYY-MM-DD HH:mm:ss',
        monthYearLabel: 'MMM YYYY',
        dateA11yLabel: 'LL',
        monthYearA11yLabel: 'MMMM YYYY',
    }
};

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        BrowserModule,
        FlexLayoutModule,
        MatButtonModule,
        MatDatepickerModule,
        MatFormFieldModule,
        MatIconModule,
        MatInputModule,
        MatSelectModule,
        MatTooltipModule,
        MatAutocompleteModule,
        MatRadioModule,
        NgxMatDatetimePickerModule,
        NgxMatMomentModule,
        NgxMatTimepickerModule,
        ReactiveFormsModule,
        TLPipesModule,
        TLIconButtonModule,
        MatSlideToggleModule,
        MatCheckboxModule,
        ScrollingModule,
        TLHelpModule,
        TLRangeInputModule,
        TLErrorModule
    ],
    exports: [
        TLDateComponent,
        TLDateMonthComponent,
        TLDateYearComponent,
        TLDateRangeComponent,
        TLDateTimeComponent,
        TLInputComponent,
        TLSelectComponent,
        TLCheckboxComponent,
        TLTextareaComponent,
        TLInputAutocompleteComponent,
        TLSlideToggleComponent,
        CoordinatesInputComponent,
        TLCoordinatesComponents,
        TLInputStepperComponent,
        TLErrorComponent,
        TLRadioButtonGroupComponent,
        TLRadioButtonTemplateComponent,
        TLRadioButtonComponent,
        TLRangeInputComponent,
        TLCheckboxTemplateComponent,
        TLPrefixInputComponent
    ],
    declarations: [
        TLDateComponent,
        TLDateMonthComponent,
        TLDateYearComponent,
        TLDateRangeComponent,
        TLDateTimeComponent,
        TLInputComponent,
        TLSelectComponent,
        TLTextareaComponent,
        TLCheckboxComponent,
        TLInputAutocompleteComponent,
        TLSlideToggleComponent,
        CoordinatesInputComponent,
        TLCoordinatesComponents,
        TLInputStepperComponent,
        TLRadioButtonGroupComponent,
        TLRadioButtonTemplateComponent,
        TLRadioButtonComponent,
        TLCheckboxTemplateComponent,
        TLPrefixInputComponent
    ]
})
export class TLInputControlsModule {
}