namespace IARA.Flux.Models
{
    public class FLUXResponseHeaders
    {
        #region Comments
        //Connection=Keep-Alive
        //Content-Type=text/xml;charset=UTF-8
        //Accept=text/plain, text/xml, application/xml
        //Accept-Charset=utf-8
        //Accept-Encoding=gzip,deflate
        //Host = localhost:62381
        //User-Agent=Apache-HttpClient/4.5.2 (Java/1.8.0_262)
        //Content-Length=124
        //client-cert=127.0.1.1
        //SOAPAction=urn:xeu:bridge-connector:wsdl:v1:post
        //RS = 201
        //CT=fish-fidesinfo @ec.europa.eu
        //RE= Acknowledge Of Receipt
        //NODE_ALIAS= BGR:NAFA
        //BUSINESS_UUID = null
        //BRIDGE_ALIAS= 1
        //FR= XEU
        //ON= BGR16O20211006027844
        //X-Forwarded-For= 212.72.219.145
        //X-Forwarded-Host= i2vms.applications.scrtl.xyz
        //X-Forwarded-Server= i2vms.applications.scrtl.xyz
        #endregion

        /// <summary>
        /// Alive
        /// </summary>
        public string Connection { get; set; }
        /// <summary>
        /// text/xml;charset=UTF-8
        /// </summary>
        public string Content_Type { get; set; }
        /// <summary>
        /// text/plain, text/xml, application/xml
        /// </summary>
        public string Accept { get; set; }
        /// <summary>
        /// utf-8
        /// </summary>
        public string Accept_Charset { get; set; }
        /// <summary>
        /// gzip,deflate
        /// </summary>
        public string Accept_Encoding { get; set; }
        /// <summary>
        /// localhost:62381
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// Apache-HttpClient/4.5.2 (Java/1.8.0_262)
        /// </summary>
        public string User_Agent { get; set; }
        /// <summary>
        /// 124
        /// </summary>
        public string Content_Length { get; set; }
        /// <summary>
        /// 127.0.1.1
        /// </summary>
        public string Client_Cert { get; set; }
        /// <summary>
        /// urn:xeu:bridge-connector:wsdl:v1:post
        /// </summary>
        public string SOAPAction { get; set; }
        /// <summary>
        /// 201
        /// </summary>
        public string RS { get; set; }
        /// <summary>
        /// fish-fidesinfo @ec.europa.eu
        /// </summary>
        public string CT { get; set; }
        /// <summary>
        /// Acknowledge Of Receipt
        /// </summary>
        public string RE { get; set; }
        /// <summary>
        /// BGR:NAFA
        /// </summary>
        public string NODE_ALIAS { get; set; }
        /// <summary>
        /// null
        /// </summary>
        public string? BUSINESS_UUID { get; set; }
        /// <summary>
        /// 1
        /// </summary>
        public string BRIDGE_ALIAS { get; set; }
        /// <summary>
        /// XEU
        /// </summary>
        public string FR { get; set; }
        /// <summary>
        /// BGR16O20211006027844
        /// </summary>
        public string ON { get; set; }
        /// <summary>
        /// 212.72.219.145
        /// </summary>
        public string X_Forwarded_For { get; set; }
        /// <summary>
        /// i2vms.applications.scrtl.xyz
        /// </summary>
        public string X_Forwarded_Host { get; set; }
        /// <summary>
        /// i2vms.applications.scrtl.xyz
        /// </summary>
        public string X_Forwarded_Server { get; set; }
    }
}
