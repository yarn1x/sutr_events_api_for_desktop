namespace college_events_admin_API.Models.Data_transfer_objects
{
    public class ActualAttendanceUpdateDto
    {
        public int EventGroupId { get; set; }

        public int ActualListenersCount { get; set; }

        public int ActualParticipantsCount { get; set; }

        public int ActualSuperParticipantsCount { get; set; }

        public int TotalScore { get; set; }
    }
}
