
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { NomenclatureDTO } from './GenericNomenclatureDTO';

export class TicketPeriodPriceDTO extends NomenclatureDTO<number> {
    public constructor(obj?: Partial<TicketPeriodPriceDTO>) {
        if (obj != undefined) {
            super(obj as NomenclatureDTO<number>);
            Object.assign(this, obj);
        }
        else {
            super();
        }
    }

    @StrictlyTyped(String)
    public ticketTypeCode?: string;

    @StrictlyTyped(Number)
    public price?: number;
}