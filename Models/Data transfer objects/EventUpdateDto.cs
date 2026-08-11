using Microsoft.Identity.Client;

namespace college_events_admin_API.Models.Data_transfer_objects
{
    public class EventUpdateDto
    {
        public EventPostDto Event { get; set; } = null!;
        public List<EventGroupPost> Groups { get; set; } = null!;
    }

    public class EventPostDto
    {
        public int EventId { get; set; }

        public string Title { get; set; } = null!;

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public string FullDescription { get; set; } = null!;

        public string ShortDescription { get; set; } = null!;

        public int OrganizerId { get; set; }

        public string? OrganizerPosition { get; set; }

        public string? OrganizerOrganization { get; set; }

        public int CategoryId { get; set; }

        public int MaxListenersCount { get; set; }

        public int MaxParticipantsCount { get; set; }

        public string? AdditionalInfo { get; set; }

        public List<int> EventLocationsIds { get; set; } = null!;
    }

    public class EventGroupPost
    {
        public int EventGroupId { get; set; }

        public int GroupId { get; set; }

        public int ExpectedListenersCount { get; set; }

        public int ExpectedParticipantsCount { get; set; }

        public int ExpectedSuperParticipantsCount { get; set; }
    }
}
