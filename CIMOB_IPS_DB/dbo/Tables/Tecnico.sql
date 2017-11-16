CREATE TABLE Tecnico
(
	id_tecnico BIGINT PRIMARY KEY IDENTITY(1,1),
	id_utilizador BIGINT NOT NULL,
	nome NVARCHAR(60) NOT NULL,
	telefone BIGINT NOT NULL
)
GO
ALTER TABLE Tecnico
ADD CONSTRAINT fk_T_Utilizador
FOREIGN KEY(id_utilizador)
REFERENCES Utilizador(id_utilizador)