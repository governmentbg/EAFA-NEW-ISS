import { NgModule } from '@angular/core';
import { NomenclatureDisplayPipe } from './nomenclature-display.pipe';
import { NomenclatureItemPipe } from './nomenclature-item.pipe';
import { TLCoordinatesPipe } from './tl-coordinates.pipe';
import { TLDateDifferencePipe } from './tl-date-difference.pipe';
import { TLJoinPipe } from './tl-join.pipe';
import { TLTranslatePipe } from './tl-translate.pipe';


@NgModule({
    declarations: [
        TLTranslatePipe,
        NomenclatureDisplayPipe,
        NomenclatureItemPipe,
        TLCoordinatesPipe,
        TLDateDifferencePipe,
        TLJoinPipe
    ],
    imports: [],
    exports: [
        TLTranslatePipe,
        NomenclatureDisplayPipe,
        NomenclatureItemPipe,
        TLCoordinatesPipe,
        TLDateDifferencePipe,
        TLJoinPipe
    ]
})
export class TLPipesModule {
}