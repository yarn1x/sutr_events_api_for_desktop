using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace college_events_admin_API.Models;

public partial class SutrEventsDbContext : DbContext
{
    public SutrEventsDbContext()
    {
    }

    public SutrEventsDbContext(DbContextOptions<SutrEventsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActualAttendance> ActualAttendances { get; set; }

    public virtual DbSet<AuthorizedUser> AuthorizedUsers { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Criterion> Criteria { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<EventCriterion> EventCriteria { get; set; }

    public virtual DbSet<EventGroup> EventGroups { get; set; }

    public virtual DbSet<EventLocation> EventLocations { get; set; }

    public virtual DbSet<EventPhoto> EventPhotos { get; set; }

    public virtual DbSet<EventResponsible> EventResponsibles { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<News> News { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserPhoto> UserPhotos { get; set; }

    public virtual DbSet<UserType> UserTypes { get; set; }

    public virtual DbSet<UserUsertype> UserUsertypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActualAttendance>(entity =>
        {
            entity.HasKey(e => e.ActualAttendanceId).HasName("PK__actual_a__AB208B5B8916B1C9");

            entity.ToTable("actual_attendances");

            entity.Property(e => e.ActualAttendanceId).HasColumnName("actual_attendance_id");
            entity.Property(e => e.ActualListenersCount).HasColumnName("actual_listeners_count");
            entity.Property(e => e.ActualParticipantsCount).HasColumnName("actual_participants_count");
            entity.Property(e => e.ActualSuperParticipantsCount).HasColumnName("actual_super_participants_count");
            entity.Property(e => e.EventGroupId).HasColumnName("event_group_id");
            entity.Property(e => e.TotalScore)
                .HasDefaultValueSql("('0')")
                .HasColumnName("total_score");

            entity.HasOne(d => d.EventGroup).WithMany(p => p.ActualAttendances)
                .HasForeignKey(d => d.EventGroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__actual_at__event__6EF57B66");
        });

        modelBuilder.Entity<AuthorizedUser>(entity =>
        {
            entity.HasKey(e => e.AuthorizedUserId).HasName("PK__authoriz__9969F0EE4EE0A45A");

            entity.ToTable("authorized_users");

            entity.HasIndex(e => e.Login, "UQ__authoriz__7838F272A7E74F8E").IsUnique();

            entity.Property(e => e.AuthorizedUserId).HasColumnName("authorized_user_id");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Login)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("login");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password_hash");
            entity.Property(e => e.Phone)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.AuthorizedUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__authorize__user___6FE99F9F");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__categori__D54EE9B45199B3AB");

            entity.ToTable("categories");

            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Criterion>(entity =>
        {
            entity.HasKey(e => e.CriterionId).HasName("PK__criteria__6547AEB5803FC43B");

            entity.ToTable("criteria");

            entity.Property(e => e.CriterionId).HasColumnName("criterion_id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Score).HasColumnName("score");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__events__2370F7271FEA85BA");

            entity.ToTable("events");

            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.AdditionalInfo)
                .IsUnicode(false)
                .HasColumnName("additional_info");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Duration)
                .HasComputedColumnSql("(datediff(minute,[start_datetime],[end_datetime]))", false)
                .HasColumnName("duration");
            entity.Property(e => e.EndDatetime)
                .HasColumnType("datetime")
                .HasColumnName("end_datetime");
            entity.Property(e => e.FullDescription)
                .IsUnicode(false)
                .HasColumnName("full_description");
            entity.Property(e => e.MaxListenersCount).HasColumnName("max_listeners_count");
            entity.Property(e => e.MaxParticipantsCount).HasColumnName("max_participants_count");
            entity.Property(e => e.OrganizerId).HasColumnName("organizer_id");
            entity.Property(e => e.OrganizerOrganization)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("organizer_organization");
            entity.Property(e => e.OrganizerPosition)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("organizer_position");
            entity.Property(e => e.ShortDescription)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("short_description");
            entity.Property(e => e.StartDatetime)
                .HasColumnType("datetime")
                .HasColumnName("start_datetime");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("title");

            entity.HasOne(d => d.Category).WithMany(p => p.Events)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__events__category__797309D9");

            entity.HasOne(d => d.Organizer).WithMany(p => p.Events)
                .HasForeignKey(d => d.OrganizerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__events__organize__7A672E12");

            entity.HasOne(d => d.Status).WithMany(p => p.Events)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__events__status_i__7B5B524B");
        });

        modelBuilder.Entity<EventCriterion>(entity =>
        {
            entity.HasKey(e => e.EventCriterionId).HasName("PK__event_cr__CB3871737B2666FB");

            entity.ToTable("event_criteria");

            entity.Property(e => e.EventCriterionId).HasColumnName("event_criterion_id");
            entity.Property(e => e.CriterionId).HasColumnName("criterion_id");
            entity.Property(e => e.EventId).HasColumnName("event_id");

            entity.HasOne(d => d.Criterion).WithMany(p => p.EventCriteria)
                .HasForeignKey(d => d.CriterionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__event_cri__crite__70DDC3D8");

            entity.HasOne(d => d.Event).WithMany(p => p.EventCriteria)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__event_cri__event__71D1E811");
        });

        modelBuilder.Entity<EventGroup>(entity =>
        {
            entity.HasKey(e => e.EventGroupId).HasName("PK__event_gr__FCC87BC74FE90677");

            entity.ToTable("event_groups");

            entity.Property(e => e.EventGroupId).HasColumnName("event_group_id");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.ExpectedListenersCount).HasColumnName("expected_listeners_count");
            entity.Property(e => e.ExpectedParticipantsCount).HasColumnName("expected_participants_count");
            entity.Property(e => e.ExpectedSuperParticipantsCount).HasColumnName("expected_super_participants_count");
            entity.Property(e => e.GroupId).HasColumnName("group_id");

            entity.HasOne(d => d.Event).WithMany(p => p.EventGroups)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__event_gro__event__72C60C4A");

            entity.HasOne(d => d.Group).WithMany(p => p.EventGroups)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__event_gro__group__73BA3083");
        });

        modelBuilder.Entity<EventLocation>(entity =>
        {
            entity.HasKey(e => e.EventLocationId).HasName("PK__event_lo__DC4B50FFD194F2CB");

            entity.ToTable("event_locations");

            entity.Property(e => e.EventLocationId).HasColumnName("event_location_id");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.LocationId).HasColumnName("location_id");

            entity.HasOne(d => d.Event).WithMany(p => p.EventLocations)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__event_loc__event__74AE54BC");

            entity.HasOne(d => d.Location).WithMany(p => p.EventLocations)
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__event_loc__locat__75A278F5");
        });

        modelBuilder.Entity<EventPhoto>(entity =>
        {
            entity.HasKey(e => e.EventPhotoId).HasName("PK__event_ph__2F74DA8FFA63A29B");

            entity.ToTable("event_photos");

            entity.Property(e => e.EventPhotoId).HasColumnName("event_photo_id");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.PhotoUrl)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("photo_url");

            entity.HasOne(d => d.Event).WithMany(p => p.EventPhotos)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__event_pho__event__76969D2E");
        });

        modelBuilder.Entity<EventResponsible>(entity =>
        {
            entity.HasKey(e => e.ResponsibleId).HasName("PK__event_re__0AE83F73E663201A");

            entity.ToTable("event_responsibles");

            entity.Property(e => e.ResponsibleId).HasColumnName("responsible_id");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Event).WithMany(p => p.EventResponsibles)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__event_res__event__778AC167");

            entity.HasOne(d => d.User).WithMany(p => p.EventResponsibles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__event_res__user___787EE5A0");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("PK__groups__D57795A0E3EAF147");

            entity.ToTable("groups");

            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.AuthorizedUserId).HasColumnName("authorized_user_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.StudentsCount).HasColumnName("students_count");

            entity.HasOne(d => d.AuthorizedUser).WithMany(p => p.Groups)
                .HasForeignKey(d => d.AuthorizedUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__groups__authoriz__7C4F7684");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.LocationId).HasName("PK__location__771831EA398309A3");

            entity.ToTable("locations");

            entity.Property(e => e.LocationId).HasColumnName("location_id");
            entity.Property(e => e.InCollege).HasColumnName("in_college");
            entity.Property(e => e.Place)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("place");
        });

        modelBuilder.Entity<News>(entity =>
        {
            entity.HasKey(e => e.NewsId).HasName("PK__news__4C27CCD8EA36072C");

            entity.ToTable("news");

            entity.Property(e => e.NewsId).HasColumnName("news_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Headline)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("headline");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(1024)
                .IsUnicode(false)
                .HasColumnName("image_url");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__statuses__3683B531E150C3B5");

            entity.ToTable("statuses");

            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__users__B9BE370F1606281B");

            entity.ToTable("users");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("last_name");
            entity.Property(e => e.MiddleName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("middle_name");
        });

        modelBuilder.Entity<UserPhoto>(entity =>
        {
            entity.HasKey(e => e.UserPhotoId).HasName("PK__user_pho__DCC201B39AE70248");

            entity.ToTable("user_photos");

            entity.Property(e => e.UserPhotoId).HasColumnName("user_photo_id");
            entity.Property(e => e.AuthorizedUserId).HasColumnName("authorized_user_id");
            entity.Property(e => e.PhotoUrl)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("photo_url");

            entity.HasOne(d => d.AuthorizedUser).WithMany(p => p.UserPhotos)
                .HasForeignKey(d => d.AuthorizedUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__user_phot__autho__7D439ABD");
        });

        modelBuilder.Entity<UserType>(entity =>
        {
            entity.HasKey(e => e.UserTypeId).HasName("PK__user_typ__9424CFA6318E85BA");

            entity.ToTable("user_types");

            entity.Property(e => e.UserTypeId).HasColumnName("user_type_id");
            entity.Property(e => e.TypeName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("type_name");
        });

        modelBuilder.Entity<UserUsertype>(entity =>
        {
            entity.HasKey(e => e.UserUsertypeId).HasName("PK__user_use__C3129A5CF54EF342");

            entity.ToTable("user_usertypes");

            entity.Property(e => e.UserUsertypeId).HasColumnName("user_usertype_id");
            entity.Property(e => e.AuthorizedUserId).HasColumnName("authorized_user_id");
            entity.Property(e => e.UserTypeId).HasColumnName("user_type_id");

            entity.HasOne(d => d.AuthorizedUser).WithMany(p => p.UserUsertypes)
                .HasForeignKey(d => d.AuthorizedUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_user_usertypes_authorized_users");

            entity.HasOne(d => d.UserType).WithMany(p => p.UserUsertypes)
                .HasForeignKey(d => d.UserTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__user_user__user___7F2BE32F");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
