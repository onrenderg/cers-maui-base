namespace CERS.Models
{
  public  class UserDetails
    {
        public string AUTO_ID { get; set; } = string.Empty;
        public string EPIC_NO { get; set; } = string.Empty;
        public string VOTER_NAME { get; set; } = string.Empty;
        public string RELATION_TYPE { get; set; } = string.Empty;
        public string RELATIVE_NAME { get; set; } = string.Empty;
        public string GENDER { get; set; } = string.Empty;
        public string AGE { get; set; } = string.Empty;
        public string EMAIL_ID { get; set; } = string.Empty;
        public string MOBILE_NUMBER { get; set; } = string.Empty;
        public string AgentName { get; set; } = string.Empty;
        public string AgentMobile { get; set; } = string.Empty;
        public string Panchayat_Name { get; set; } = string.Empty;
        public string LoggedInAs { get; set; } = string.Empty;
        public string OTPID { get; set; } = string.Empty;
        public string NominationForName { get; set; } = string.Empty;
        public string NominationForNameLocal { get; set; } = string.Empty;
        public string PollDate { get; set; } = string.Empty;
        public string NominationDate { get; set; } = string.Empty;
        public string IsLoggedIn { get; set; } = string.Empty;

        public string postcode { get; set; } = string.Empty;
        public string LimitAmt { get; set; } = string.Empty;
        public string ResultDate { get; set; } = string.Empty;
        public string Resultdatethirtydays { get; set; } = string.Empty;
        public string Block_Code { get; set; } = string.Empty;
        public string panwardcouncilname { get; set; } = string.Empty;
        public string panwardcouncilnamelocal { get; set; } = string.Empty;
        public string ExpStatus { get; set; } = string.Empty;
        //added on 25sep24

        public string expStartDate { get; set; } = string.Empty;
        public string expEndDate { get; set; } = string.Empty;
    }
}
