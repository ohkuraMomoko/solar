namespace Chailease.SolarEnergy.Web.ViewModel
{
    public class RequestFormViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string BirthdayYear { get; set; }
        public string BirthdayMonth { get; set; }
        public string BirthdayDay { get; set; }
        /// <summary>
        /// {自然人}性別(M/F/)
        /// </summary>
        public string Gender { get; set; }
        /// <summary>
        /// {自然人}戶籍地址
        /// </summary>
        public string RegZipCode { get; set; }
        /// <summary>
        /// {自然人}戶籍地址
        /// </summary>
        public string RegCounty { get; set; }
        /// <summary>
        /// {自然人}戶籍地址
        /// </summary>
        public string RegDist { get; set; }
        /// <summary>
        /// {自然人}戶籍地址
        /// </summary>
        public string RegAddr { get; set; }
        public string ContactZipCode { get; set; }
        public string ContactCounty { get; set; }
        public string ContactDist { get; set; }
        public string ContactAddr { get; set; }

    }
    
}