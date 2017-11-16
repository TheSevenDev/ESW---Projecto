-- MOBILIDADE
CREATE TABLE Notificacao
(
	id_notificacao BIGINT PRIMARY KEY IDENTITY(1,1),
	id_utilizador BIGINT NOT NULL,
	descricao NVARCHAR(255) NOT NULL
)
GO
ALTER TABLE Notificacao
ADD CONSTRAINT fk_N_Utilizador
FOREIGN KEY(id_utilizador)
REFERENCES Utilizador(id_utilizador)