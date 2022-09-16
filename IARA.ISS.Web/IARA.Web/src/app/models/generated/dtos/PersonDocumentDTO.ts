
import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class PersonDocumentDTO {
    public constructor(obj?: Partial<PersonDocumentDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public documentTypeID?: number;

    @StrictlyTyped(String)
    public documentTypeName?: string;

    @StrictlyTyped(String)
    public documentNumber?: string;

    @StrictlyTyped(Date)
    public documentIssuedOn?: Date;

    @StrictlyTyped(String)
    public documentIssuedBy?: string;
}