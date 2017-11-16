using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CIMOB_IPS.Models
{
    public partial class CIMOB_IPS_DBContext : DbContext
    {
        public virtual DbSet<Ajuda> Ajuda { get; set; }
        public virtual DbSet<Candidatura> Candidatura { get; set; }
        public virtual DbSet<Coordenador> Coordenador { get; set; }
        public virtual DbSet<Curso> Curso { get; set; }
        public virtual DbSet<Estado> Estado { get; set; }
        public virtual DbSet<Estudante> Estudante { get; set; }
        public virtual DbSet<Instituicao> Instituicao { get; set; }
        public virtual DbSet<InstituicoesPrograma> InstituicoesPrograma { get; set; }
        public virtual DbSet<Mobilidade> Mobilidade { get; set; }
        public virtual DbSet<Nacionalidade> Nacionalidade { get; set; }
        public virtual DbSet<Notificacao> Notificacao { get; set; }
        public virtual DbSet<Programa> Programa { get; set; }
        public virtual DbSet<Tecnico> Tecnico { get; set; }
        public virtual DbSet<Utilizador> Utilizador { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=CIMOB_IPS_DB;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ajuda>(entity =>
            {
                entity.HasKey(e => e.IdAjuda);

                entity.Property(e => e.IdAjuda).HasColumnName("id_ajuda");

                entity.Property(e => e.Descricao)
                    .IsRequired()
                    .HasColumnName("descricao")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<Candidatura>(entity =>
            {
                entity.HasKey(e => e.IdCandidatura);

                entity.Property(e => e.IdCandidatura).HasColumnName("id_candidatura");

                entity.Property(e => e.AvaliacaoFinal).HasColumnName("avaliacao_final");

                entity.Property(e => e.Bolsa).HasColumnName("bolsa");

                entity.Property(e => e.CartaMotivacao)
                    .IsRequired()
                    .HasColumnName("carta_motivacao")
                    .HasMaxLength(255);

                entity.Property(e => e.ContactoEmergenciaNome)
                    .IsRequired()
                    .HasColumnName("contacto_emergencia_nome")
                    .HasMaxLength(60);

                entity.Property(e => e.ContactoEmergenciaRelacao)
                    .IsRequired()
                    .HasColumnName("contacto_emergencia_relacao")
                    .HasMaxLength(30);

                entity.Property(e => e.ContactoEmergenciaTelefone).HasColumnName("contacto_emergencia_telefone");

                entity.Property(e => e.IdEstado).HasColumnName("id_estado");

                entity.Property(e => e.IdEstudante).HasColumnName("id_estudante");

                entity.HasOne(d => d.IdEstadoNavigation)
                    .WithMany(p => p.Candidatura)
                    .HasForeignKey(d => d.IdEstado)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_C_Estado");

                entity.HasOne(d => d.IdEstudanteNavigation)
                    .WithMany(p => p.Candidatura)
                    .HasForeignKey(d => d.IdEstudante)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_C_Estudante");
            });

            modelBuilder.Entity<Coordenador>(entity =>
            {
                entity.HasKey(e => e.IdCoordenador);

                entity.Property(e => e.IdCoordenador).HasColumnName("id_coordenador");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(60);

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasColumnName("nome")
                    .HasMaxLength(60);

                entity.Property(e => e.Telefone).HasColumnName("telefone");
            });

            modelBuilder.Entity<Curso>(entity =>
            {
                entity.HasKey(e => e.IdCurso);

                entity.Property(e => e.IdCurso).HasColumnName("id_curso");

                entity.Property(e => e.IdInstituicao).HasColumnName("id_instituicao");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasColumnName("nome")
                    .HasMaxLength(99);

                entity.HasOne(d => d.IdInstituicaoNavigation)
                    .WithMany(p => p.Curso)
                    .HasForeignKey(d => d.IdInstituicao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_C_Instituicao");
            });

            modelBuilder.Entity<Estado>(entity =>
            {
                entity.HasKey(e => e.IdEstado);

                entity.Property(e => e.IdEstado).HasColumnName("id_estado");

                entity.Property(e => e.Descricao)
                    .IsRequired()
                    .HasColumnName("descricao")
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<Estudante>(entity =>
            {
                entity.HasKey(e => e.IdEstudante);

                entity.Property(e => e.IdEstudante).HasColumnName("id_estudante");

                entity.Property(e => e.Cc).HasColumnName("cc");

                entity.Property(e => e.Ects).HasColumnName("ects");

                entity.Property(e => e.IdCurso).HasColumnName("id_curso");

                entity.Property(e => e.IdNacionalidade).HasColumnName("id_nacionalidade");

                entity.Property(e => e.IdUtilizador).HasColumnName("id_utilizador");

                entity.Property(e => e.Morada)
                    .IsRequired()
                    .HasColumnName("morada")
                    .HasMaxLength(99);

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasColumnName("nome")
                    .HasMaxLength(60);

                entity.Property(e => e.NumAluno).HasColumnName("num_aluno");

                entity.Property(e => e.Telefone).HasColumnName("telefone");

                entity.HasOne(d => d.IdCursoNavigation)
                    .WithMany(p => p.Estudante)
                    .HasForeignKey(d => d.IdCurso)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_E_Curso");

                entity.HasOne(d => d.IdNacionalidadeNavigation)
                    .WithMany(p => p.Estudante)
                    .HasForeignKey(d => d.IdNacionalidade)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_E_Nacionalidade");

                entity.HasOne(d => d.IdUtilizadorNavigation)
                    .WithMany(p => p.Estudante)
                    .HasForeignKey(d => d.IdUtilizador)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_E_Utilizador");
            });

            modelBuilder.Entity<Instituicao>(entity =>
            {
                entity.HasKey(e => e.IdInstituicao);

                entity.Property(e => e.IdInstituicao).HasColumnName("id_instituicao");

                entity.Property(e => e.IdNacionalidade).HasColumnName("id_nacionalidade");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasColumnName("nome")
                    .HasMaxLength(45);

                entity.HasOne(d => d.IdNacionalidadeNavigation)
                    .WithMany(p => p.Instituicao)
                    .HasForeignKey(d => d.IdNacionalidade)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_I_Nacionalidade");
            });

            modelBuilder.Entity<InstituicoesPrograma>(entity =>
            {
                entity.HasKey(e => new { e.IdPrograma, e.IdInstituicaoOutgoing });

                entity.ToTable("Instituicoes_Programa");

                entity.Property(e => e.IdPrograma).HasColumnName("id_programa");

                entity.Property(e => e.IdInstituicaoOutgoing).HasColumnName("id_instituicao_outgoing");

                entity.HasOne(d => d.IdInstituicaoOutgoingNavigation)
                    .WithMany(p => p.InstituicoesPrograma)
                    .HasForeignKey(d => d.IdInstituicaoOutgoing)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_IP_Instituicao");

                entity.HasOne(d => d.IdProgramaNavigation)
                    .WithMany(p => p.InstituicoesPrograma)
                    .HasForeignKey(d => d.IdPrograma)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_IP_Programa");
            });

            modelBuilder.Entity<Mobilidade>(entity =>
            {
                entity.HasKey(e => e.IdMobilidade);

                entity.Property(e => e.IdMobilidade).HasColumnName("id_mobilidade");

                entity.Property(e => e.DataInicio)
                    .HasColumnName("data_inicio")
                    .HasColumnType("date");

                entity.Property(e => e.DataTermino)
                    .HasColumnName("data_termino")
                    .HasColumnType("date");

                entity.Property(e => e.IdCandidatura).HasColumnName("id_candidatura");

                entity.Property(e => e.IdEstado).HasColumnName("id_estado");

                entity.Property(e => e.IdInstituicaoOutgoing).HasColumnName("id_instituicao_outgoing");

                entity.Property(e => e.IdPrograma).HasColumnName("id_programa");

                entity.Property(e => e.IdTecnicoResponsavel).HasColumnName("id_tecnico_responsavel");

                entity.HasOne(d => d.IdCandidaturaNavigation)
                    .WithMany(p => p.Mobilidade)
                    .HasForeignKey(d => d.IdCandidatura)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_M_Candidatura");

                entity.HasOne(d => d.IdInstituicaoOutgoingNavigation)
                    .WithMany(p => p.Mobilidade)
                    .HasForeignKey(d => d.IdInstituicaoOutgoing)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_M_Instituicao");

                entity.HasOne(d => d.IdProgramaNavigation)
                    .WithMany(p => p.Mobilidade)
                    .HasForeignKey(d => d.IdPrograma)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_M_Programa");

                entity.HasOne(d => d.IdTecnicoResponsavelNavigation)
                    .WithMany(p => p.Mobilidade)
                    .HasForeignKey(d => d.IdTecnicoResponsavel)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_M_Tecnico");
            });

            modelBuilder.Entity<Nacionalidade>(entity =>
            {
                entity.HasKey(e => e.IdNacionalidade);

                entity.Property(e => e.IdNacionalidade).HasColumnName("id_nacionalidade");

                entity.Property(e => e.Descricao)
                    .IsRequired()
                    .HasColumnName("descricao")
                    .HasMaxLength(45);
            });

            modelBuilder.Entity<Notificacao>(entity =>
            {
                entity.HasKey(e => e.IdNotificacao);

                entity.Property(e => e.IdNotificacao).HasColumnName("id_notificacao");

                entity.Property(e => e.Descricao)
                    .IsRequired()
                    .HasColumnName("descricao")
                    .HasMaxLength(255);

                entity.Property(e => e.IdUtilizador).HasColumnName("id_utilizador");

                entity.HasOne(d => d.IdUtilizadorNavigation)
                    .WithMany(p => p.Notificacao)
                    .HasForeignKey(d => d.IdUtilizador)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_N_Utilizador");
            });

            modelBuilder.Entity<Programa>(entity =>
            {
                entity.HasKey(e => e.IdPrograma);

                entity.Property(e => e.IdPrograma).HasColumnName("id_programa");

                entity.Property(e => e.DataAbertura)
                    .HasColumnName("data_abertura")
                    .HasColumnType("date");

                entity.Property(e => e.DataCriacao)
                    .HasColumnName("data_criacao")
                    .HasColumnType("date");

                entity.Property(e => e.DataEncerramento)
                    .HasColumnName("data_encerramento")
                    .HasColumnType("date");

                entity.Property(e => e.IdEstado).HasColumnName("id_estado");

                entity.HasOne(d => d.IdEstadoNavigation)
                    .WithMany(p => p.Programa)
                    .HasForeignKey(d => d.IdEstado)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_P_Estado");
            });

            modelBuilder.Entity<Tecnico>(entity =>
            {
                entity.HasKey(e => e.IdTecnico);

                entity.Property(e => e.IdTecnico).HasColumnName("id_tecnico");

                entity.Property(e => e.IdUtilizador).HasColumnName("id_utilizador");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasColumnName("nome")
                    .HasMaxLength(60);

                entity.Property(e => e.Telefone).HasColumnName("telefone");

                entity.HasOne(d => d.IdUtilizadorNavigation)
                    .WithMany(p => p.Tecnico)
                    .HasForeignKey(d => d.IdUtilizador)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_T_Utilizador");
            });

            modelBuilder.Entity<Utilizador>(entity =>
            {
                entity.HasKey(e => e.IdUtilizador);

                entity.Property(e => e.IdUtilizador).HasColumnName("id_utilizador");

                entity.Property(e => e.Activo).HasColumnName("activo");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(60);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasMaxLength(16);
            });
        }
    }
}
