CREATE DATABASE CIMOB_IPS_DB;
go

USE CIMOB_IPS_DB
go

CREATE TABLE Ajuda
(
	id_ajuda BIGINT PRIMARY KEY IDENTITY(1,1),
	descricao NVARCHAR(255) NOT NULL
)

-- UTILIZADORES
CREATE TABLE Utilizador
(
	id_utilizador BIGINT PRIMARY KEY IDENTITY(1,1),
	email NVARCHAR(60) NOT NULL,
	password VARBINARY(16) NOT NULL,
	activo bit NOT NULL DEFAULT 0
)

CREATE TABLE Nacionalidade
(
	id_nacionalidade BIGINT PRIMARY KEY IDENTITY(1,1),
	descricao NVARCHAR(45) NOT NULL
)

CREATE TABLE Instituicao
(
	id_instituicao BIGINT PRIMARY KEY IDENTITY(1,1),
	nome NVARCHAR(45) NOT NULL,
	id_nacionalidade BIGINT NOT NULL
)

CREATE TABLE Curso
(
	id_curso BIGINT PRIMARY KEY IDENTITY(1, 1),
	id_instituicao BIGINT NOT NULL,
	nome NVARCHAR(99) NOT NULL
)

CREATE TABLE Estudante
(
	id_estudante BIGINT PRIMARY KEY IDENTITY(1,1),
	id_utilizador BIGINT NOT NULL,
	id_curso BIGINT NOT NULL,
	nome NVARCHAR(60) NOT NULL,
	morada NVARCHAR(99) NOT NULL,
	cc BIGINT NOT NULL,
	telefone BIGINT NOT NULL,
	id_nacionalidade BIGINT NOT NULL,
	ects INT NOT NULL,
	num_aluno BIGINT NOT NULL
)

CREATE TABLE Tecnico
(
	id_tecnico BIGINT PRIMARY KEY IDENTITY(1,1),
	id_utilizador BIGINT NOT NULL,
	nome NVARCHAR(60) NOT NULL,
	telefone BIGINT NOT NULL
)

CREATE TABLE Coordenador
(
	id_coordenador BIGINT PRIMARY KEY IDENTITY(1,1),
	nome NVARCHAR(60) NOT NULL,
	email NVARCHAR(60) NOT NULL,	
	telefone BIGINT
)

-- MOBILIDADE
CREATE TABLE Notificacao
(
	id_notificacao BIGINT PRIMARY KEY IDENTITY(1,1),
	id_utilizador BIGINT NOT NULL,
	descricao NVARCHAR(255) NOT NULL
)

CREATE TABLE Estado
(
	id_estado BIGINT PRIMARY KEY IDENTITY(1,1),
	descricao NVARCHAR(20) NOT NULL
)

CREATE TABLE Candidatura
(
	id_candidatura BIGINT PRIMARY KEY IDENTITY(1,1),
	id_estudante BIGINT NOT NULL,
	id_estado BIGINT NOT NULL,
	bolsa BIT NOT NULL,
	avaliacao_final SMALLINT NOT NULL,
	carta_motivacao NVARCHAR(255) NOT NULL,
	contacto_emergencia_nome NVARCHAR(60) NOT NULL,
	contacto_emergencia_relacao NVARCHAR(30) NOT NULL,
	contacto_emergencia_telefone BIGINT NOT NULL
)

CREATE TABLE Programa
(
	id_programa BIGINT PRIMARY KEY IDENTITY(1,1),
	id_estado BIGINT NOT NULL,
	data_criacao DATE NOT NULL,
	data_abertura DATE,
	data_encerramento DATE
)

CREATE TABLE Instituicoes_Programa
(
	id_programa BIGINT NOT NULL,
	id_instituicao_outgoing BIGINT NOT NULL
	primary key(id_programa, id_instituicao_outgoing)
)

CREATE TABLE Mobilidade
(
	id_mobilidade BIGINT PRIMARY KEY IDENTITY(1,1),
	id_candidatura BIGINT NOT NULL,
	id_programa BIGINT NOT NULL,
	id_estado BIGINT NOT NULL,
	id_tecnico_responsavel BIGINT NOT NULL,
	id_instituicao_outgoing BIGINT NOT NULL,
	data_inicio DATE NOT NULL,
	data_termino DATE
)

ALTER TABLE Instituicao
ADD CONSTRAINT fk_I_Nacionalidade
FOREIGN KEY(id_nacionalidade)
REFERENCES Nacionalidade(id_nacionalidade)

ALTER TABLE Curso
ADD CONSTRAINT fk_C_Instituicao
FOREIGN KEY(id_instituicao)
REFERENCES Instituicao(id_instituicao)

ALTER TABLE Estudante
ADD CONSTRAINT fk_E_Curso
FOREIGN KEY(id_curso)
REFERENCES Curso(id_curso)

ALTER TABLE Estudante
ADD CONSTRAINT fk_E_Utilizador
FOREIGN KEY(id_utilizador)
REFERENCES Utilizador(id_utilizador)

ALTER TABLE Estudante
ADD CONSTRAINT fk_E_Nacionalidade
FOREIGN KEY(id_nacionalidade)
REFERENCES Nacionalidade(id_nacionalidade)

ALTER TABLE Tecnico
ADD CONSTRAINT fk_T_Utilizador
FOREIGN KEY(id_utilizador)
REFERENCES Utilizador(id_utilizador)

ALTER TABLE Notificacao
ADD CONSTRAINT fk_N_Utilizador
FOREIGN KEY(id_utilizador)
REFERENCES Utilizador(id_utilizador)

ALTER TABLE Candidatura
ADD CONSTRAINT fk_C_Estudante
FOREIGN KEY(id_estudante)
REFERENCES Estudante(id_estudante)

ALTER TABLE Candidatura
ADD CONSTRAINT fk_C_Estado
FOREIGN KEY(id_estado)
REFERENCES Estado(id_estado)

ALTER TABLE Programa
ADD CONSTRAINT fk_P_Estado
FOREIGN KEY(id_estado)
REFERENCES Estado(id_estado)

ALTER TABLE Instituicoes_Programa
ADD CONSTRAINT fk_IP_Programa
FOREIGN KEY(id_programa)
REFERENCES Programa(id_programa)

ALTER TABLE Instituicoes_Programa
ADD CONSTRAINT fk_IP_Instituicao
FOREIGN KEY(id_instituicao_outgoing)
REFERENCES Instituicao(id_instituicao)

ALTER TABLE Mobilidade
ADD CONSTRAINT fk_M_Candidatura
FOREIGN KEY(id_candidatura)
REFERENCES Candidatura(id_candidatura)

ALTER TABLE Mobilidade
ADD CONSTRAINT fk_M_Programa
FOREIGN KEY(id_programa)
REFERENCES Programa(id_programa)

ALTER TABLE Mobilidade
ADD CONSTRAINT fk_M_Tecnico
FOREIGN KEY(id_tecnico_responsavel)
REFERENCES Tecnico(id_tecnico)

ALTER TABLE Mobilidade
ADD CONSTRAINT fk_M_Instituicao
FOREIGN KEY(id_instituicao_outgoing)
REFERENCES Instituicao(id_instituicao)
