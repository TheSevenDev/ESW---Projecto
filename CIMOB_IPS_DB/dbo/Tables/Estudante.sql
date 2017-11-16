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
GO
ALTER TABLE Estudante
ADD CONSTRAINT fk_E_Curso
FOREIGN KEY(id_curso)
REFERENCES Curso(id_curso)
GO
ALTER TABLE Estudante
ADD CONSTRAINT fk_E_Utilizador
FOREIGN KEY(id_utilizador)
REFERENCES Utilizador(id_utilizador)
GO
ALTER TABLE Estudante
ADD CONSTRAINT fk_E_Nacionalidade
FOREIGN KEY(id_nacionalidade)
REFERENCES Nacionalidade(id_nacionalidade)