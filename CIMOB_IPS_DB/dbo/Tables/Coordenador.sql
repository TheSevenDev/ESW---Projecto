﻿CREATE TABLE Coordenador
(
	id_coordenador BIGINT PRIMARY KEY IDENTITY(1,1),
	nome NVARCHAR(60) NOT NULL,
	email NVARCHAR(60) NOT NULL,	
	telefone BIGINT
)