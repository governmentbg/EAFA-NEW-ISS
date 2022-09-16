using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using IARA.Common.Enums;

namespace IARA.Common.Utils
{
    public static class EAuthUtils
    {
        public const string UNKNOWN_FORMAT = "^([0-9]+)$";
        public const string LNCH_FORMAT = "^(?:PNF-([0-9]+))$";
        public const string PI_FORMAT = "^(?:PI:([A-Z]{2})-([0-9]+))$";
        public const string PASS_FORMAT = "^(?:PASS([A-Z]{2})-([0-9]+))$";
        public const string IDC_FORMAT = "^(?:IDC([A-Z]{2})-([0-9]+))$";
        public const string BT_FORMAT = "^(?:BT:([A-Z]{2})-([0-9]+))$";
        public const string NTR_FORMAT = "^(?:NTR([A-Z]{2})-([0-9]+))$";
        public const string TIN_FORMAT = "^(?:TIN([A-Z]{2})-([0-9]+))$";
        public const string PNO_FORMAT = "^(?:PNO([A-Z]{2})-([0-9]+))$";

        private static Dictionary<UserIdentifierTypes, string[]> IDENTIFIER_FORMATS = new Dictionary<UserIdentifierTypes, string[]>
        {
            { UserIdentifierTypes.LNCH, new string[]{ LNCH_FORMAT, PI_FORMAT } },
            { UserIdentifierTypes.PASSID, new string[]{ PASS_FORMAT } },
            { UserIdentifierTypes.NPID, new string[]{ IDC_FORMAT } },
            { UserIdentifierTypes.BTRUST_PID, new string[]{ BT_FORMAT } },
            { UserIdentifierTypes.VAT_LEGAL_ID, new string[]{ NTR_FORMAT } },
            { UserIdentifierTypes.VAT_PERSON_ID, new string[]{ TIN_FORMAT } },
            { UserIdentifierTypes.EGN, new string[]{ PNO_FORMAT } }
        };

        public static bool TryParseIdentifier(string identifier, out IdentificationModel identification)
        {
            if (Regex.IsMatch(identifier, UNKNOWN_FORMAT))
            {
                identification = new IdentificationModel
                {
                    Identifier = identifier,
                    IdentifierType = UserIdentifierTypes.UNKNOWN
                };

                return true;
            }
            else
            {
                Match match = Match.Empty;
                int i = 0;

                var identifierFormats = IDENTIFIER_FORMATS.ToList();
                UserIdentifierTypes type;

                do
                {
                    string[] formats = identifierFormats[i].Value;
                    type = identifierFormats[i].Key;
                    i++;

                    for (int j = 0; j < formats.Length; j++)
                    {
                        match = Regex.Match(identifier, formats[j]);
                    }
                } while (!match.Success && i < IDENTIFIER_FORMATS.Count);

                if (match.Success)
                {
                    identification = new IdentificationModel
                    {
                        IdentifierType = type,
                    };

                    if (match.Groups.Count == 1)
                    {
                        identification.Identifier = match.Groups[0].Value;
                    }
                    else if (match.Groups.Count == 2)
                    {
                        identification.Identifier = match.Groups[1].Value;
                    }
                    else
                    {
                        return false;
                    }

                    return true;
                }
                else
                {
                    identification = null;
                    return false;
                }
            }
        }
    }

    public class IdentificationModel
    {
        public string Identifier { get; set; }
        public UserIdentifierTypes IdentifierType { get; set; }
    }
}
