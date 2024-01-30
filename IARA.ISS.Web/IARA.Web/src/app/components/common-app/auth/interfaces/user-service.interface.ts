import { Observable } from "rxjs";
import { TokenStatus } from '../enums/token-status.enum';
import { UpdatePasswordModel } from "../models/auth/change-password.model";
import { PasswordValidatorModel } from "../models/auth/password-validator.model";
import { ChangeUserPasswordModel } from "../models/auth/update-user-password.model";
import { User } from "../models/auth/user.model";

export interface IGenericUserService<TIdentifier, TUser extends User<TIdentifier>> extends IUserService {
    getUser(): Observable<TUser>;
    User: TUser | undefined;
}

export interface IUserService {
    updateUserPassword(data: UpdatePasswordModel): Observable<void>;
    getPasswordValidators(): Observable<PasswordValidatorModel>;
    changeUserPassword(data: ChangeUserPasswordModel): Observable<void>;
    checkTokenStatus(token: string): Observable<TokenStatus>;
    forgotPassword(email: string): Observable<void>;
}
