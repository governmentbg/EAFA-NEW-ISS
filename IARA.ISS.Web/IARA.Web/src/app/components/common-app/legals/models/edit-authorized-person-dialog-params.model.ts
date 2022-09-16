import { AuthorizedPersonDTO } from '@app/models/generated/dtos/AuthorizedPersonDTO';
import { AuthorizedPersonRegixDataDTO } from '@app/models/generated/dtos/AuthorizedPersonRegixDataDTO';

export class EditAuthorizedPersonDialogParams {
    public model: AuthorizedPersonDTO | undefined;
    public isEgnLncReadOnly: boolean = false;
    public readOnly: boolean = false;
    public showOnlyRegiXData: boolean = false;
    public expectedResults: AuthorizedPersonRegixDataDTO = new AuthorizedPersonRegixDataDTO();

    public constructor(obj?: Partial<EditAuthorizedPersonDialogParams>) {
        Object.assign(this, obj);
    }
}