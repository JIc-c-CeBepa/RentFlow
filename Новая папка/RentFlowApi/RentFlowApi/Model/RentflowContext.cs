using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace RentFlowApi.Model;

public partial class RentflowContext : DbContext
{
    public RentflowContext()
    {
    }

    public RentflowContext(DbContextOptions<RentflowContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Amenity> Amenities { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingStatus> BookingStatuses { get; set; }

    public virtual DbSet<CalendarEntry> CalendarEntries { get; set; }

    public virtual DbSet<ClientBehaviorEvent> ClientBehaviorEvents { get; set; }

    public virtual DbSet<ClientProfile> ClientProfiles { get; set; }

    public virtual DbSet<ClientSession> ClientSessions { get; set; }

    public virtual DbSet<LeadScore> LeadScores { get; set; }

    public virtual DbSet<LeadSource> LeadSources { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Owner> Owners { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Property> Properties { get; set; }

    public virtual DbSet<PropertyPhoto> PropertyPhotos { get; set; }

    public virtual DbSet<Recommendation> Recommendations { get; set; }

    public virtual DbSet<Refund> Refunds { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Rule> Rules { get; set; }

    public virtual DbSet<SeasonalPriceRule> SeasonalPriceRules { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;user=root;password=2314;database=rentflow", Microsoft.EntityFrameworkCore.ServerVersion.Parse("9.3.0-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_unicode_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Amenity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("amenities");

            entity.HasIndex(e => e.Name, "name").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Icon)
                .HasMaxLength(100)
                .HasColumnName("icon");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("bookings");

            entity.HasIndex(e => e.CanceledByUserId, "fk_bookings_canceled_by_user");

            entity.HasIndex(e => e.ArrivalDate, "idx_bookings_arrival_date");

            entity.HasIndex(e => e.DepartureDate, "idx_bookings_departure_date");

            entity.HasIndex(e => e.PropertyId, "idx_bookings_property_id");

            entity.HasIndex(e => e.StatusId, "idx_bookings_status_id");

            entity.HasIndex(e => e.UserId, "idx_bookings_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ArrivalDate).HasColumnName("arrival_date");
            entity.Property(e => e.CanceledAt)
                .HasColumnType("datetime")
                .HasColumnName("canceled_at");
            entity.Property(e => e.CanceledByUserId).HasColumnName("canceled_by_user_id");
            entity.Property(e => e.CancellationReason)
                .HasMaxLength(500)
                .HasColumnName("cancellation_reason");
            entity.Property(e => e.CompletedAt)
                .HasColumnType("datetime")
                .HasColumnName("completed_at");
            entity.Property(e => e.ConfirmedAt)
                .HasColumnType("datetime")
                .HasColumnName("confirmed_at");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DepartureDate).HasColumnName("departure_date");
            entity.Property(e => e.GuestsCount)
                .HasDefaultValueSql("'1'")
                .HasColumnName("guests_count");
            entity.Property(e => e.IsThankYouSent).HasColumnName("is_thank_you_sent");
            entity.Property(e => e.NeedsContactlessCheckin).HasColumnName("needs_contactless_checkin");
            entity.Property(e => e.PrepaymentAmount)
                .HasPrecision(10, 2)
                .HasColumnName("prepayment_amount");
            entity.Property(e => e.PrepaymentPercent)
                .HasPrecision(5, 2)
                .HasColumnName("prepayment_percent");
            entity.Property(e => e.PricePerStay)
                .HasPrecision(10, 2)
                .HasColumnName("price_per_stay");
            entity.Property(e => e.PropertyId).HasColumnName("property_id");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.TotalAmount)
                .HasPrecision(10, 2)
                .HasColumnName("total_amount");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.CanceledByUser).WithMany(p => p.BookingCanceledByUsers)
                .HasForeignKey(d => d.CanceledByUserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_bookings_canceled_by_user");

            entity.HasOne(d => d.Property).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.PropertyId)
                .HasConstraintName("fk_bookings_property");

            entity.HasOne(d => d.Status).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_bookings_status");

            entity.HasOne(d => d.User).WithMany(p => p.BookingUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_bookings_user");
        });

        modelBuilder.Entity<BookingStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("booking_statuses");

            entity.HasIndex(e => e.Name, "name").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<CalendarEntry>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("calendar_entries");

            entity.HasIndex(e => e.BookingId, "idx_calendar_entries_booking_id");

            entity.HasIndex(e => new { e.StartDate, e.EndDate }, "idx_calendar_entries_dates");

            entity.HasIndex(e => e.PropertyId, "idx_calendar_entries_property_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.EntryType)
                .HasColumnType("enum('booking','manual_block')")
                .HasColumnName("entry_type");
            entity.Property(e => e.IsBlocked)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_blocked");
            entity.Property(e => e.PropertyId).HasColumnName("property_id");
            entity.Property(e => e.StartDate).HasColumnName("start_date");

            entity.HasOne(d => d.Booking).WithMany(p => p.CalendarEntries)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_calendar_entries_booking");

            entity.HasOne(d => d.Property).WithMany(p => p.CalendarEntries)
                .HasForeignKey(d => d.PropertyId)
                .HasConstraintName("fk_calendar_entries_property");
        });

        modelBuilder.Entity<ClientBehaviorEvent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("client_behavior_events");

            entity.HasIndex(e => e.EventType, "idx_client_behavior_events_event_type");

            entity.HasIndex(e => e.PropertyId, "idx_client_behavior_events_property_id");

            entity.HasIndex(e => e.SessionId, "idx_client_behavior_events_session_id");

            entity.HasIndex(e => e.UserId, "idx_client_behavior_events_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EventTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("event_time");
            entity.Property(e => e.EventType)
                .HasMaxLength(100)
                .HasColumnName("event_type");
            entity.Property(e => e.PropertyId).HasColumnName("property_id");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Property).WithMany(p => p.ClientBehaviorEvents)
                .HasForeignKey(d => d.PropertyId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_client_behavior_events_property");

            entity.HasOne(d => d.Session).WithMany(p => p.ClientBehaviorEvents)
                .HasForeignKey(d => d.SessionId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_client_behavior_events_session");

            entity.HasOne(d => d.User).WithMany(p => p.ClientBehaviorEvents)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_client_behavior_events_user");
        });

        modelBuilder.Entity<ClientProfile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("client_profiles");

            entity.HasIndex(e => e.LeadSourceId, "idx_client_profiles_lead_source_id");

            entity.HasIndex(e => e.UserId, "user_id").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.FirstVisitAt)
                .HasColumnType("datetime")
                .HasColumnName("first_visit_at");
            entity.Property(e => e.HasLeftContacts).HasColumnName("has_left_contacts");
            entity.Property(e => e.HasSelectedDates).HasColumnName("has_selected_dates");
            entity.Property(e => e.LastVisitAt)
                .HasColumnType("datetime")
                .HasColumnName("last_visit_at");
            entity.Property(e => e.LeadScorePercent)
                .HasPrecision(5, 2)
                .HasColumnName("lead_score_percent");
            entity.Property(e => e.LeadSourceId).HasColumnName("lead_source_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.VisitsCount).HasColumnName("visits_count");

            entity.HasOne(d => d.LeadSource).WithMany(p => p.ClientProfiles)
                .HasForeignKey(d => d.LeadSourceId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_client_profiles_lead_source");

            entity.HasOne(d => d.User).WithOne(p => p.ClientProfile)
                .HasForeignKey<ClientProfile>(d => d.UserId)
                .HasConstraintName("fk_client_profiles_user");
        });

        modelBuilder.Entity<ClientSession>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("client_sessions");

            entity.HasIndex(e => e.SourceId, "idx_client_sessions_source_id");

            entity.HasIndex(e => e.UserId, "idx_client_sessions_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DurationSeconds).HasColumnName("duration_seconds");
            entity.Property(e => e.EndedAt)
                .HasColumnType("datetime")
                .HasColumnName("ended_at");
            entity.Property(e => e.SourceId).HasColumnName("source_id");
            entity.Property(e => e.StartedAt)
                .HasColumnType("datetime")
                .HasColumnName("started_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Source).WithMany(p => p.ClientSessions)
                .HasForeignKey(d => d.SourceId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_client_sessions_source");

            entity.HasOne(d => d.User).WithMany(p => p.ClientSessions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_client_sessions_user");
        });

        modelBuilder.Entity<LeadScore>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("lead_scores");

            entity.HasIndex(e => e.CalculatedAt, "idx_lead_scores_calculated_at");

            entity.HasIndex(e => e.UserId, "idx_lead_scores_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CalculatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("calculated_at");
            entity.Property(e => e.ContactsWeight)
                .HasPrecision(5, 2)
                .HasColumnName("contacts_weight");
            entity.Property(e => e.ReturnVisitsWeight)
                .HasPrecision(5, 2)
                .HasColumnName("return_visits_weight");
            entity.Property(e => e.ScorePercent)
                .HasPrecision(5, 2)
                .HasColumnName("score_percent");
            entity.Property(e => e.SelectedDatesWeight)
                .HasPrecision(5, 2)
                .HasColumnName("selected_dates_weight");
            entity.Property(e => e.SessionDurationWeight)
                .HasPrecision(5, 2)
                .HasColumnName("session_duration_weight");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.LeadScores)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_lead_scores_user");
        });

        modelBuilder.Entity<LeadSource>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("lead_sources");

            entity.HasIndex(e => e.Name, "name").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("notifications");

            entity.HasIndex(e => e.BookingId, "idx_notifications_booking_id");

            entity.HasIndex(e => e.IsRead, "idx_notifications_is_read");

            entity.HasIndex(e => e.UserId, "idx_notifications_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsRead).HasColumnName("is_read");
            entity.Property(e => e.Message)
                .HasColumnType("text")
                .HasColumnName("message");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.Type)
                .HasMaxLength(100)
                .HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Booking).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_notifications_booking");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_notifications_user");
        });

        modelBuilder.Entity<Owner>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("owners");

            entity.HasIndex(e => e.PublicSlug, "public_slug").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(255)
                .HasColumnName("company_name");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Phone)
                .HasMaxLength(30)
                .HasColumnName("phone");
            entity.Property(e => e.PublicSlug)
                .HasMaxLength(150)
                .HasColumnName("public_slug");
            entity.Property(e => e.Telegram)
                .HasMaxLength(100)
                .HasColumnName("telegram");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("payments");

            entity.HasIndex(e => e.BookingId, "idx_payments_booking_id");

            entity.HasIndex(e => e.Status, "idx_payments_status");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasPrecision(10, 2)
                .HasColumnName("amount");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.PaidAt)
                .HasColumnType("datetime")
                .HasColumnName("paid_at");
            entity.Property(e => e.PaymentType)
                .HasColumnType("enum('prepayment','fullpayment','refund')")
                .HasColumnName("payment_type");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'pending'")
                .HasColumnType("enum('pending','paid','failed','refunded')")
                .HasColumnName("status");
            entity.Property(e => e.TransactionId)
                .HasMaxLength(255)
                .HasColumnName("transaction_id");

            entity.HasOne(d => d.Booking).WithMany(p => p.Payments)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("fk_payments_booking");
        });

        modelBuilder.Entity<Property>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("properties");

            entity.HasIndex(e => e.IsActive, "idx_properties_is_active");

            entity.HasIndex(e => e.OwnerId, "idx_properties_owner_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(500)
                .HasColumnName("address");
            entity.Property(e => e.BasePrice)
                .HasPrecision(10, 2)
                .HasColumnName("base_price");
            entity.Property(e => e.BookingMode)
                .HasDefaultValueSql("'confirmation'")
                .HasColumnType("enum('instant','confirmation')")
                .HasColumnName("booking_mode");
            entity.Property(e => e.CheckInTime)
                .HasColumnType("time")
                .HasColumnName("check_in_time");
            entity.Property(e => e.CheckOutTime)
                .HasColumnType("time")
                .HasColumnName("check_out_time");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CurrentPrice)
                .HasPrecision(10, 2)
                .HasColumnName("current_price");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_active");
            entity.Property(e => e.IsContactlessCheckinAvailable).HasColumnName("is_contactless_checkin_available");
            entity.Property(e => e.MaxGuests)
                .HasDefaultValueSql("'1'")
                .HasColumnName("max_guests");
            entity.Property(e => e.OwnerId).HasColumnName("owner_id");
            entity.Property(e => e.PrepaymentPercent)
                .HasPrecision(5, 2)
                .HasColumnName("prepayment_percent");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.Owner).WithMany(p => p.Properties)
                .HasForeignKey(d => d.OwnerId)
                .HasConstraintName("fk_properties_owner");

            entity.HasMany(d => d.Amenities).WithMany(p => p.Properties)
                .UsingEntity<Dictionary<string, object>>(
                    "PropertyAmenity",
                    r => r.HasOne<Amenity>().WithMany()
                        .HasForeignKey("AmenityId")
                        .HasConstraintName("fk_property_amenities_amenity"),
                    l => l.HasOne<Property>().WithMany()
                        .HasForeignKey("PropertyId")
                        .HasConstraintName("fk_property_amenities_property"),
                    j =>
                    {
                        j.HasKey("PropertyId", "AmenityId")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("property_amenities");
                        j.HasIndex(new[] { "AmenityId" }, "idx_property_amenities_amenity_id");
                        j.IndexerProperty<int>("PropertyId").HasColumnName("property_id");
                        j.IndexerProperty<int>("AmenityId").HasColumnName("amenity_id");
                    });

            entity.HasMany(d => d.Rules).WithMany(p => p.Properties)
                .UsingEntity<Dictionary<string, object>>(
                    "PropertyRule",
                    r => r.HasOne<Rule>().WithMany()
                        .HasForeignKey("RuleId")
                        .HasConstraintName("fk_property_rules_rule"),
                    l => l.HasOne<Property>().WithMany()
                        .HasForeignKey("PropertyId")
                        .HasConstraintName("fk_property_rules_property"),
                    j =>
                    {
                        j.HasKey("PropertyId", "RuleId")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("property_rules");
                        j.HasIndex(new[] { "RuleId" }, "idx_property_rules_rule_id");
                        j.IndexerProperty<int>("PropertyId").HasColumnName("property_id");
                        j.IndexerProperty<int>("RuleId").HasColumnName("rule_id");
                    });
        });

        modelBuilder.Entity<PropertyPhoto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("property_photos");

            entity.HasIndex(e => e.IsMain, "idx_property_photos_is_main");

            entity.HasIndex(e => e.PropertyId, "idx_property_photos_property_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsMain).HasColumnName("is_main");
            entity.Property(e => e.PropertyId).HasColumnName("property_id");
            entity.Property(e => e.PublicUrl)
                .HasMaxLength(1000)
                .HasColumnName("public_url");
            entity.Property(e => e.SortOrder).HasColumnName("sort_order");

            entity.HasOne(d => d.Property).WithMany(p => p.PropertyPhotos)
                .HasForeignKey(d => d.PropertyId)
                .HasConstraintName("fk_property_photos_property");
        });

        modelBuilder.Entity<Recommendation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("recommendations");

            entity.HasIndex(e => e.OwnerId, "idx_recommendations_owner_id");

            entity.HasIndex(e => e.PropertyId, "idx_recommendations_property_id");

            entity.HasIndex(e => e.Status, "idx_recommendations_status");

            entity.HasIndex(e => e.UserId, "idx_recommendations_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.OwnerId).HasColumnName("owner_id");
            entity.Property(e => e.PropertyId).HasColumnName("property_id");
            entity.Property(e => e.ReasonText)
                .HasColumnType("text")
                .HasColumnName("reason_text");
            entity.Property(e => e.RecommendationText)
                .HasColumnType("text")
                .HasColumnName("recommendation_text");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'new'")
                .HasColumnType("enum('new','viewed','applied','ignored')")
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Owner).WithMany(p => p.Recommendations)
                .HasForeignKey(d => d.OwnerId)
                .HasConstraintName("fk_recommendations_owner");

            entity.HasOne(d => d.Property).WithMany(p => p.Recommendations)
                .HasForeignKey(d => d.PropertyId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_recommendations_property");

            entity.HasOne(d => d.User).WithMany(p => p.Recommendations)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_recommendations_user");
        });

        modelBuilder.Entity<Refund>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("refunds");

            entity.HasIndex(e => e.BookingId, "idx_refunds_booking_id");

            entity.HasIndex(e => e.PaymentId, "idx_refunds_payment_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasPrecision(10, 2)
                .HasColumnName("amount");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.ProcessedAt)
                .HasColumnType("datetime")
                .HasColumnName("processed_at");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'pending'")
                .HasColumnType("enum('pending','processed','failed')")
                .HasColumnName("status");

            entity.HasOne(d => d.Booking).WithMany(p => p.Refunds)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("fk_refunds_booking");

            entity.HasOne(d => d.Payment).WithMany(p => p.Refunds)
                .HasForeignKey(d => d.PaymentId)
                .HasConstraintName("fk_refunds_payment");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("reviews");

            entity.HasIndex(e => e.BookingId, "booking_id").IsUnique();

            entity.HasIndex(e => e.PropertyId, "idx_reviews_property_id");

            entity.HasIndex(e => e.UserId, "idx_reviews_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.Comment)
                .HasColumnType("text")
                .HasColumnName("comment");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsPublished)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_published");
            entity.Property(e => e.PropertyId).HasColumnName("property_id");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Booking).WithOne(p => p.Review)
                .HasForeignKey<Review>(d => d.BookingId)
                .HasConstraintName("fk_reviews_booking");

            entity.HasOne(d => d.Property).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.PropertyId)
                .HasConstraintName("fk_reviews_property");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_reviews_user");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("roles");

            entity.HasIndex(e => e.Name, "name").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Rule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("rules");

            entity.HasIndex(e => e.Name, "name").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .HasColumnName("name");
        });

        modelBuilder.Entity<SeasonalPriceRule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("seasonal_price_rules");

            entity.HasIndex(e => new { e.StartDate, e.EndDate }, "idx_seasonal_price_rules_dates");

            entity.HasIndex(e => e.PropertyId, "idx_seasonal_price_rules_property_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.FixedPrice)
                .HasPrecision(10, 2)
                .HasColumnName("fixed_price");
            entity.Property(e => e.PriceMultiplier)
                .HasPrecision(5, 2)
                .HasColumnName("price_multiplier");
            entity.Property(e => e.PropertyId).HasColumnName("property_id");
            entity.Property(e => e.StartDate).HasColumnName("start_date");

            entity.HasOne(d => d.Property).WithMany(p => p.SeasonalPriceRules)
                .HasForeignKey(d => d.PropertyId)
                .HasConstraintName("fk_seasonal_price_rules_property");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => e.OwnerId, "idx_users_owner_id");

            entity.HasIndex(e => e.RoleId, "idx_users_role_id");

            entity.HasIndex(e => e.Login, "login").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_active");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.Login)
                .HasMaxLength(100)
                .HasColumnName("login");
            entity.Property(e => e.MiddleName)
                .HasMaxLength(100)
                .HasColumnName("middle_name");
            entity.Property(e => e.OwnerId).HasColumnName("owner_id");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(500)
                .HasColumnName("password_hash");
            entity.Property(e => e.Phone)
                .HasMaxLength(30)
                .HasColumnName("phone");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Telegram)
                .HasMaxLength(100)
                .HasColumnName("telegram");

            entity.HasOne(d => d.Owner).WithMany(p => p.Users)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_users_owner");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_users_role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
