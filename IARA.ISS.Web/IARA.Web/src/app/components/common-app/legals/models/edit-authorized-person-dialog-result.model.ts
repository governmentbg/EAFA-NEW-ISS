import { AuthorizedPersonDTO } from "@app/models/generated/dtos/AuthorizedPersonDTO";
import { AuthorizedPersonRegixDataDTO } from "@app/models/generated/dtos/AuthorizedPersonRegixDataDTO";

export class EditAuthorizedPersonDialogResult {
    public authorizedPerson: AuthorizedPersonDTO | AuthorizedPersonRegixDataDTO | undefined;
    public isTouched: boolean;

    public constructor(person: AuthorizedPersonDTO | AuthorizedPersonRegixDataDTO | undefined, isTouched: boolean) {
        this.authorizedPerson = person;
        this.isTouched = isTouched;
    }
}