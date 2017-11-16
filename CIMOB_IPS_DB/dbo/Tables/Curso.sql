CREATE TABLE Curso
(
	id_curso BIGINT PRIMARY KEY IDENTITY(1, 1),
	id_instituicao BIGINT NOT NULL,
	nome NVARCHAR(99) NOT NULL
)
GO
ALTER TABLE Curso
ADD CONSTRAINT fk_C_Instituicao
FOREIGN KEY(id_instituicao)
REFERENCES Instituicao(id_instituicao)