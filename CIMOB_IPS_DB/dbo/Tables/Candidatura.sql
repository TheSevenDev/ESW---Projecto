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
GO
ALTER TABLE Candidatura
ADD CONSTRAINT fk_C_Estudante
FOREIGN KEY(id_estudante)
REFERENCES Estudante(id_estudante)
GO
ALTER TABLE Candidatura
ADD CONSTRAINT fk_C_Estado
FOREIGN KEY(id_estado)
REFERENCES Estado(id_estado)