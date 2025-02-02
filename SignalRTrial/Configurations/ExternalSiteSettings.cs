namespace SignalRTrial.Configurations
{
    public class ExternalSiteSettings
    {
        public string CorsPermissionDmains { get; set; } = "";
        public string ReactSite { get; set; } = "";

        public string[] ArrayCorsPermissionDmains
        {
            get { return CorsPermissionDmains.Split(","); }
        }
    }
}
