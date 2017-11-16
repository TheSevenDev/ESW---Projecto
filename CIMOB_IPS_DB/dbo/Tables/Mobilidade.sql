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
GO
ALTER TABLE Mobilidade
ADD CONSTRAINT fk_M_Candidatura
FOREIGN KEY(id_candidatura)
REFERENCES Candidatura(id_candidatura)
GO
ALTER TABLE Mobilidade
ADD CONSTRAINT fk_M_Programa
FOREIGN KEY(id_programa)
REFERENCES Programa(id_programa)
GO
ALTER TABLE Mobilidade
ADD CONSTRAINT fk_M_Tecnico
FOREIGN KEY(id_tecnico_responsavel)
REFERENCES Tecnico(id_tecnico)
GO
ALTER TABLE Mobilidade
ADD CONSTRAINT fk_M_Instituicao
FOREIGN KEY(id_instituicao_outgoing)
REFERENCES Instituicao(id_instituicao)