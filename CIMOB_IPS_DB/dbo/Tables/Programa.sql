CREATE TABLE Programa
(
	id_programa BIGINT PRIMARY KEY IDENTITY(1,1),
	id_estado BIGINT NOT NULL,
	data_criacao DATE NOT NULL,
	data_abertura DATE,
	data_encerramento DATE
)
GO
ALTER TABLE Programa
ADD CONSTRAINT fk_P_Estado
FOREIGN KEY(id_estado)
REFERENCES Estado(id_estado)