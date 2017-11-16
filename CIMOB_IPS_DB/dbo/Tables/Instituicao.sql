CREATE TABLE Instituicao
(
	id_instituicao BIGINT PRIMARY KEY IDENTITY(1,1),
	nome NVARCHAR(45) NOT NULL,
	id_nacionalidade BIGINT NOT NULL
)
GO
ALTER TABLE Instituicao
ADD CONSTRAINT fk_I_Nacionalidade
FOREIGN KEY(id_nacionalidade)
REFERENCES Nacionalidade(id_nacionalidade)