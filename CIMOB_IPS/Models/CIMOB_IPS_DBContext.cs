﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CIMOB_IPS.Models
{
    public partial class CIMOB_IPS_DBContext : DbContext
    {
        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<Application> Application { get; set; }
        public virtual DbSet<Coordenator> Coordenator { get; set; }
        public virtual DbSet<Course> Course { get; set; }
        public virtual DbSet<Help> Help { get; set; }
        public virtual DbSet<Institution> Institution { get; set; }
        public virtual DbSet<InstitutionProgram> InstitutionProgram { get; set; }
        public virtual DbSet<Mobility> Mobility { get; set; }
        public virtual DbSet<Nationality> Nationality { get; set; }
        public virtual DbSet<Notification> Notification { get; set; }
        public virtual DbSet<Program> Program { get; set; }
        public virtual DbSet<State> State { get; set; }
        public virtual DbSet<Student> Student { get; set; }
        public virtual DbSet<Technician> Technician { get; set; }

        /*
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer(@"Data Source=esw-cimob-db.database.windows.net;Database=CIMOB_IPS_DB;Integrated Security=False;User ID=adminUser; Password=f00n!l06;Connect Timeout=60;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            }
        }*/

        public CIMOB_IPS_DBContext(DbContextOptions<CIMOB_IPS_DBContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => e.IdAccount);

                entity.Property(e => e.IdAccount).HasColumnName("id_account");

                entity.Property(e => e.Active).HasColumnName("active");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(60);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasMaxLength(16);
            });

            modelBuilder.Entity<Application>(entity =>
            {
                entity.HasKey(e => e.IdApplication);

                entity.Property(e => e.IdApplication).HasColumnName("id_application");

                entity.Property(e => e.EmergencyContactName)
                    .IsRequired()
                    .HasColumnName("emergency_contact_name")
                    .HasMaxLength(60);

                entity.Property(e => e.EmergencyContactRelation)
                    .IsRequired()
                    .HasColumnName("emergency_contact_relation")
                    .HasMaxLength(30);

                entity.Property(e => e.EmergencyContactTelephone).HasColumnName("emergency_contact_telephone");

                entity.Property(e => e.FinalEvaluation).HasColumnName("final_evaluation");

                entity.Property(e => e.HasScholarship).HasColumnName("has_scholarship");

                entity.Property(e => e.IdState).HasColumnName("id_state");

                entity.Property(e => e.IdStudent).HasColumnName("id_student");

                entity.Property(e => e.MotivationCard)
                    .IsRequired()
                    .HasColumnName("motivation_card")
                    .HasMaxLength(255);

                entity.HasOne(d => d.IdStateNavigation)
                    .WithMany(p => p.Application)
                    .HasForeignKey(d => d.IdState)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_C_State");

                entity.HasOne(d => d.IdStudentNavigation)
                    .WithMany(p => p.Application)
                    .HasForeignKey(d => d.IdStudent)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_C_Student");
            });

            modelBuilder.Entity<Coordenator>(entity =>
            {
                entity.HasKey(e => e.IdCoordenator);

                entity.Property(e => e.IdCoordenator).HasColumnName("id_coordenator");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(60);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(60);

                entity.Property(e => e.Telephone).HasColumnName("telephone");
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(e => e.IdCourse);

                entity.Property(e => e.IdCourse).HasColumnName("id_course");

                entity.Property(e => e.IdInstitution).HasColumnName("id_institution");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(99);

                entity.HasOne(d => d.IdInstitutionNavigation)
                    .WithMany(p => p.Course)
                    .HasForeignKey(d => d.IdInstitution)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_C_Institution");
            });

            modelBuilder.Entity<Help>(entity =>
            {
                entity.HasKey(e => e.IdHelp);

                entity.Property(e => e.IdHelp).HasColumnName("id_help");

                entity.Property(e => e.ActionName)
                    .IsRequired()
                    .HasColumnName("action_name")
                    .HasMaxLength(50);

                entity.Property(e => e.ControllerName)
                    .IsRequired()
                    .HasColumnName("controller_name")
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasMaxLength(1024);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Institution>(entity =>
            {
                entity.HasKey(e => e.IdInstitution);

                entity.Property(e => e.IdInstitution).HasColumnName("id_institution");

                entity.Property(e => e.IdNationality).HasColumnName("id_nationality");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(45);

                entity.HasOne(d => d.IdNationalityNavigation)
                    .WithMany(p => p.Institution)
                    .HasForeignKey(d => d.IdNationality)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_I_Nationality");
            });

            modelBuilder.Entity<InstitutionProgram>(entity =>
            {
                entity.HasKey(e => new { e.IdProgram, e.IdOutgoingInstitution });

                entity.ToTable("Institution_Program");

                entity.Property(e => e.IdProgram).HasColumnName("id_program");

                entity.Property(e => e.IdOutgoingInstitution).HasColumnName("id_outgoing_institution");

                entity.HasOne(d => d.IdOutgoingInstitutionNavigation)
                    .WithMany(p => p.InstitutionProgram)
                    .HasForeignKey(d => d.IdOutgoingInstitution)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_IP_Institution");

                entity.HasOne(d => d.IdProgramNavigation)
                    .WithMany(p => p.InstitutionProgram)
                    .HasForeignKey(d => d.IdProgram)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_IP_Program");
            });

            modelBuilder.Entity<Mobility>(entity =>
            {
                entity.HasKey(e => e.IdMobility);

                entity.Property(e => e.IdMobility).HasColumnName("id_mobility");

                entity.Property(e => e.BeginDate)
                    .HasColumnName("begin_date")
                    .HasColumnType("date");

                entity.Property(e => e.EndDate)
                    .HasColumnName("end_date")
                    .HasColumnType("date");

                entity.Property(e => e.IdApplication).HasColumnName("id_application");

                entity.Property(e => e.IdOutgoingInstitution).HasColumnName("id_outgoing_institution");

                entity.Property(e => e.IdProgram).HasColumnName("id_program");

                entity.Property(e => e.IdResponsibleTechnician).HasColumnName("id_responsible_technician");

                entity.Property(e => e.IdState).HasColumnName("id_state");

                entity.HasOne(d => d.IdApplicationNavigation)
                    .WithMany(p => p.Mobility)
                    .HasForeignKey(d => d.IdApplication)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_M_Application");

                entity.HasOne(d => d.IdOutgoingInstitutionNavigation)
                    .WithMany(p => p.Mobility)
                    .HasForeignKey(d => d.IdOutgoingInstitution)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_M_Institution");

                entity.HasOne(d => d.IdProgramNavigation)
                    .WithMany(p => p.Mobility)
                    .HasForeignKey(d => d.IdProgram)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_M_Program");

                entity.HasOne(d => d.IdResponsibleTechnicianNavigation)
                    .WithMany(p => p.Mobility)
                    .HasForeignKey(d => d.IdResponsibleTechnician)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_M_Technician");
            });

            modelBuilder.Entity<Nationality>(entity =>
            {
                entity.HasKey(e => e.IdNationality);

                entity.Property(e => e.IdNationality).HasColumnName("id_nationality");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasMaxLength(45);
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.IdNotification);

                entity.Property(e => e.IdNotification).HasColumnName("id_notification");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasMaxLength(255);

                entity.Property(e => e.IdAccount).HasColumnName("id_account");

                entity.HasOne(d => d.IdAccountNavigation)
                    .WithMany(p => p.Notification)
                    .HasForeignKey(d => d.IdAccount)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_N_Account");
            });

            modelBuilder.Entity<Program>(entity =>
            {
                entity.HasKey(e => e.IdProgram);

                entity.Property(e => e.IdProgram).HasColumnName("id_program");

                entity.Property(e => e.ClosingDate)
                    .HasColumnName("closing_date")
                    .HasColumnType("date");

                entity.Property(e => e.CreationDate)
                    .HasColumnName("creation_date")
                    .HasColumnType("date");

                entity.Property(e => e.IdState).HasColumnName("id_state");

                entity.Property(e => e.OpenDate)
                    .HasColumnName("open_date")
                    .HasColumnType("date");

                entity.HasOne(d => d.IdStateNavigation)
                    .WithMany(p => p.Program)
                    .HasForeignKey(d => d.IdState)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_P_State");
            });

            modelBuilder.Entity<State>(entity =>
            {
                entity.HasKey(e => e.IdState);

                entity.Property(e => e.IdState).HasColumnName("id_state");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.IdStudent);

                entity.Property(e => e.IdStudent).HasColumnName("id_student");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasColumnName("address")
                    .HasMaxLength(99);

                entity.Property(e => e.Cc).HasColumnName("cc");

                entity.Property(e => e.Credits).HasColumnName("credits");

                entity.Property(e => e.IdAccount).HasColumnName("id_account");

                entity.Property(e => e.IdCourse).HasColumnName("id_course");

                entity.Property(e => e.IdNationality).HasColumnName("id_nationality");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(60);

                entity.Property(e => e.StudentNum).HasColumnName("student_num");

                entity.Property(e => e.Telephone).HasColumnName("telephone");

                entity.HasOne(d => d.IdAccountNavigation)
                    .WithMany(p => p.Student)
                    .HasForeignKey(d => d.IdAccount)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_E_Account");

                entity.HasOne(d => d.IdCourseNavigation)
                    .WithMany(p => p.Student)
                    .HasForeignKey(d => d.IdCourse)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_E_Course");

                entity.HasOne(d => d.IdNationalityNavigation)
                    .WithMany(p => p.Student)
                    .HasForeignKey(d => d.IdNationality)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_E_Nationality");
            });

            modelBuilder.Entity<Technician>(entity =>
            {
                entity.HasKey(e => e.IdTechnician);

                entity.Property(e => e.IdTechnician).HasColumnName("id_technician");

                entity.Property(e => e.IdAccount).HasColumnName("id_account");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(60);

                entity.Property(e => e.Telephone).HasColumnName("telephone");

                entity.HasOne(d => d.IdAccountNavigation)
                    .WithMany(p => p.Technician)
                    .HasForeignKey(d => d.IdAccount)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_T_Account");
            });
        }
    }
}