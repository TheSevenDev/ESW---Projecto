CREATE TABLE Instituicoes_Programa
(
	id_programa BIGINT NOT NULL,
	id_instituicao_outgoing BIGINT NOT NULL
	primary key(id_programa, id_instituicao_outgoing)
)
GO
ALTER TABLE Instituicoes_Programa
ADD CONSTRAINT fk_IP_Programa
FOREIGN KEY(id_programa)
REFERENCES Programa(id_programa)
GO
ALTER TABLE Instituicoes_Programa
ADD CONSTRAINT fk_IP_Instituicao
FOREIGN KEY(id_instituicao_outgoing)
REFERENCES Instituicao(id_instituicao)