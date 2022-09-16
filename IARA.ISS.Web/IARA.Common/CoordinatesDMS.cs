namespace IARA.Common
{
    public struct CoordinatesDMS
    {
        private CoordinatesDMS(string longitude, string latitude)
        {
            this.Longitude = DMSType.Parse(longitude);
            this.Latitude = DMSType.Parse(latitude);
        }

        public CoordinatesDMS(float X, float Y)
        {
            this.Longitude = new DMSType(X);
            this.Latitude = new DMSType(Y);
        }

        public DMSType Longitude { get; }
        public DMSType Latitude { get; }

        public static CoordinatesDMS Parse(string longitude, string latitude)
        {
            return new CoordinatesDMS(longitude, latitude);
        }
    }
}
