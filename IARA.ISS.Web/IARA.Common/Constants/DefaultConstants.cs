using System;

namespace IARA.Common.Constants
{
    public static class DefaultConstants
    {
        public const string DB_TABLE_NAME = "\"Admin\".\"WorkerQueues\"";
        public const string BASE_REPORTS_PATH = "reports";
        public const int PUBLIC_ROLE_ID = -1;
        public const int SYSTEM_USER_ID = -2;
        public const string SYSTEM_USER = "SYSTEM";
        public const string BACKGROUND_TASK_USER = "BACKGROUND_TASK";
        public const string IP = nameof(IP);
        public const string ENDPOINT = nameof(ENDPOINT);
        public const string BROWSER_INFO = nameof(BROWSER_INFO);
        public const string FORWARDED_FOR = "X-Forwarded-For";
        public const string USER_AGENT = "User-Agent";
        public const string DEFAULT_NUMBER_DECIMAL_SEPARATOR = ".";
        public const string DEFAULT_NUMBER_GROUP_SEPARATOR = " ";

        public static readonly DateTime MIN_VALID_DATE = new DateTime(1900, 1, 1, 0, 0, 0);
        public static readonly DateTime MAX_VALID_DATE = new DateTime(9999, 01, 01, 0, 0, 0);

        public const string HASHER_ALPHABET = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ23456789#%*@&^";
        public const string HASHER_SEPS = "cfhistuCFHISTU";
        public const int HASHER_MIN_ALPHABET_LENGTH = 16;
        public const string HASHER_SALT = "57a554cbc7bd4efc85e6c0cbafcbd3db";
        public const int HASHER_MIN_HASH_LENGTH = 6;

        public const string PASSWORD_COMPLEXITY_PATTERN = @"(?=.*[a-zA-Z])(?=.*[0-9])(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&].{8,}";

        public const long JS_MAX_SAFE_INTEGER = 9007199254740991;

        public const int FLUX_VESSEL_MIN_LENGTH_M = 10;
    }
}
