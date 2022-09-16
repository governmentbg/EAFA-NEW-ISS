export interface IIdentificationToken {
    aud: string;
    auth_time: number;
    exp: number;
    iat: number;
    idp: string;
    iss: string;
    nbf: number
    nonce: string;
    s_hash: string;
    sid: string;
    sub: string;
}