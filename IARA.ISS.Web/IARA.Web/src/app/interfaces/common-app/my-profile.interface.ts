import { Observable } from "rxjs";
import { MyProfileDTO } from "@app/models/generated/dtos/MyProfileDTO";
import { UserPasswordDTO } from "@app/models/generated/dtos/UserPasswordDTO";

export interface IMyProfileService {
    getUserProfile(id: number): Observable<MyProfileDTO>;

    getUserPhoto(id: number): Observable<string>;

    updateUserProfile(profileData: MyProfileDTO): Observable<void>;

    changePassword(userPassword: UserPasswordDTO): Observable<void>;
}