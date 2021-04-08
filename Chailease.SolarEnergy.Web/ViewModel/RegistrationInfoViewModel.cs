namespace Chailease.SolarEnergy.Web.ViewModel
{
    public class RegistrationInfoViewModel
    {
        public string Source { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }
        public string Recommender { get; set; }
        public string SourceEtc { get; set; }
        /// <summary>
        /// 勾選有相關訊息請通知(等於有勾選所有專案通知信) Y/N
        /// </summary>
        public bool HasNotifyLetter { get; set; }
    }
}