-- Tabelas de acesso do usuário

CREATE TABLE permissao (
    id UUID PRIMARY KEY,
    descricao VARCHAR(50) NOT NULL
);

ALTER table permissao
	ADD CONSTRAINT unq_permissao_descricao UNIQUE (descricao);

CREATE TABLE usuario (
    id UUID PRIMARY KEY,
    nome VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL,
    senha VARCHAR(50) NOT NULL,
    id_permissao UUID NOT NULL
);

ALTER TABLE usuario 
	ADD CONSTRAINT fk_usuario_permissao FOREIGN KEY (id_permissao) REFERENCES permissao(id),
	ADD CONSTRAINT unq_usuario_email UNIQUE (email);

CREATE UNIQUE INDEX idx_usuario_email ON usuario(email);

-- Tabelas de cadastro

CREATE TABLE cliente (
    id UUID PRIMARY KEY,
    nome VARCHAR(255) NOT NULL,
    cpf_cnpj VARCHAR(20) NOT NULL,
    email VARCHAR(255) NOT NULL,
    celular VARCHAR(20) NOT NULL
);

ALTER TABLE cliente 
	ADD CONSTRAINT unq_cliente_cpf_cnpj UNIQUE (cpf_cnpj);

CREATE UNIQUE INDEX idx_cliente_cpf_cnpj ON cliente(cpf_cnpj);

CREATE TABLE os_status (
    id UUID PRIMARY KEY,
    descricao VARCHAR(100) NOT NULL
);

ALTER TABLE os_status 
	ADD CONSTRAINT unq_os_status_descricao UNIQUE (descricao);

CREATE TABLE veiculo (
    id UUID PRIMARY KEY,
    placa VARCHAR(10) NOT NULL,
    marca VARCHAR(100) NOT NULL,
    modelo VARCHAR(100) NOT NULL,
    ano INT NOT NULL
);

ALTER TABLE veiculo 
	ADD CONSTRAINT unq_veiculo_placa UNIQUE (placa);

CREATE UNIQUE INDEX idx_veiculo_placa ON veiculo(placa);

CREATE TABLE servico (
    id UUID PRIMARY KEY,
    descricao VARCHAR(255) NOT NULL,
    preco DECIMAL(10, 2) NOT NULL
);

ALTER TABLE servico 
	ADD CONSTRAINT unq_servico_descricao UNIQUE (descricao);

CREATE TABLE peca_insumo (
    id UUID PRIMARY KEY,
    descricao VARCHAR(255) NOT NULL,
    preco DECIMAL(10, 2) NOT NULL
);

ALTER TABLE peca_insumo 
	ADD CONSTRAINT unq_peca_insumo_descricao UNIQUE (descricao);

-- Tabelas de relacionamento

CREATE TABLE os (
    id UUID PRIMARY KEY,
    id_cliente UUID NOT NULL,
    id_veiculo UUID NOT NULL,
    id_status UUID NOT NULL
);

ALTER TABLE os 
    ADD CONSTRAINT fk_os_cliente FOREIGN KEY (id_cliente) REFERENCES cliente(id),
    ADD CONSTRAINT fk_os_veiculo FOREIGN KEY (id_veiculo) REFERENCES veiculo(id),
    ADD CONSTRAINT fk_os_status FOREIGN KEY (id_status) REFERENCES os_status(id);

CREATE INDEX idx_os_cliente ON os(id_cliente);
CREATE INDEX idx_os_veiculo ON os(id_veiculo);
CREATE INDEX idx_os_status ON os(id_status);

CREATE TABLE estoque (
    id UUID PRIMARY KEY,
    id_peca_insumo UUID,
    quantidade INT NOT NULL
);

ALTER TABLE estoque 
    ADD CONSTRAINT fk_estoque_peca FOREIGN KEY (id_peca_insumo) REFERENCES peca_insumo(id),
    ADD CONSTRAINT unq_estoque_id_peca_insumo UNIQUE (id_peca_insumo);

CREATE TABLE orcamento (
    id UUID PRIMARY KEY,
    id_os UUID NOT NULL,
    preco DECIMAL(10, 2) NOT NULL
);

ALTER TABLE orcamento 
    ADD CONSTRAINT fk_orcamento_os FOREIGN KEY (id_os) REFERENCES os(id),
    ADD CONSTRAINT unq_orcamento_id_os UNIQUE (id_os);

CREATE INDEX idx_orcamento_os ON orcamento(id_os);

CREATE TABLE servico_os (
    id UUID PRIMARY KEY,
    id_os UUID NOT NULL,
    id_servico UUID NOT NULL,
    dth_inicio TIMESTAMP,
    dth_fim TIMESTAMP
);

ALTER TABLE servico_os 
    ADD CONSTRAINT fk_servico_os_os FOREIGN KEY (id_os) REFERENCES os(id),
    ADD CONSTRAINT fk_servico_os_servico FOREIGN KEY (id_servico) REFERENCES servico(id),
    ADD CONSTRAINT unq_servico_os_id_os_id_servico UNIQUE (id_os, id_servico);

CREATE INDEX idx_servico_os_os ON servico_os(id_os);
CREATE INDEX idx_servico_os_servico ON servico_os(id_servico);

CREATE TABLE peca_insumo_os (
    id UUID PRIMARY KEY,
    id_os UUID NOT NULL,
    id_peca_insumo UUID NOT NULL
);

ALTER TABLE peca_insumo_os 
    ADD CONSTRAINT fk_peca_insumo_os_os FOREIGN KEY (id_os) REFERENCES os(id),
    ADD CONSTRAINT fk_peca_insumo_os_peca FOREIGN KEY (id_peca_insumo) REFERENCES peca_insumo(id),
    ADD CONSTRAINT unq_peca_insumo_os_id_os_id_peca_insumo UNIQUE (id_os, id_peca_insumo);

CREATE INDEX idx_peca_insumo_os_os ON peca_insumo_os(id_os);
CREATE INDEX idx_peca_insumo_os_peca ON peca_insumo_os(id_peca_insumo);
