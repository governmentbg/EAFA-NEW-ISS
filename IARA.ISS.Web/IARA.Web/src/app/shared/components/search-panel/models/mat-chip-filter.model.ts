import { FormControl } from '@angular/forms';

export class MatChipFilterModel {
    constructor(public value: any,
        public displayValue: string,
        public control: FormControl,
        public isKeywordSearch: boolean = false,
        public partOfMultipleValue = false) { }
}