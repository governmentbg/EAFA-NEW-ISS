
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO';
import { PageCodeEnum } from '@app/enums/page-code.enum';

export class SubmittedByRoleNomenclatureDTO extends NomenclatureDTO<number> {
    public constructor(obj?: Partial<SubmittedByRoleNomenclatureDTO>) {
        if (obj != undefined) {
            super(obj as NomenclatureDTO<number>);
            Object.assign(this, obj);
        }
        else {
            super();
        }
    }

    @StrictlyTyped(Number)
    public applicationPageCode?: PageCodeEnum;

    @StrictlyTyped(Boolean)
    public hasLetterOfAttorney?: boolean;
}