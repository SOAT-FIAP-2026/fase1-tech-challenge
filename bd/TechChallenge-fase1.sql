-- Tabelas de acesso do usuário

CREATE TABLE permissao (
    id UUID PRIMARY KEY,
    descricao VARCHAR(50) NOT NULL,
    criado_em TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    atualizado_em TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    apagado_em TIMESTAMP
);

ALTER TABLE permissao
    ADD CONSTRAINT unq_permissao_descricao UNIQUE (descricao);

CREATE TABLE usuario (
    id UUID PRIMARY KEY,
    nome VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL,
    senha_hash VARCHAR(255) NOT NULL,
    id_permissao UUID NOT NULL,
    criado_em TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    atualizado_em TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    apagado_em TIMESTAMP
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
    celular VARCHAR(20) NOT NULL,
    criado_em TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    atualizado_em TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    apagado_em TIMESTAMP
);

ALTER TABLE cliente
    ADD CONSTRAINT unq_cliente_cpf_cnpj UNIQUE (cpf_cnpj);

CREATE UNIQUE INDEX idx_cliente_cpf_cnpj ON cliente(cpf_cnpj);

CREATE TABLE status_ordem_servico (
    id UUID PRIMARY KEY,
    descricao VARCHAR(100) NOT NULL
);

ALTER TABLE status_ordem_servico
    ADD CONSTRAINT unq_status_ordem_servico_descricao UNIQUE (descricao);

CREATE TABLE veiculo (
    id UUID PRIMARY KEY,
    placa VARCHAR(10) NOT NULL,
    marca VARCHAR(100) NOT NULL,
    modelo VARCHAR(100) NOT NULL,
    ano INT NOT NULL,
    criado_em TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    atualizado_em TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    apagado_em TIMESTAMP
);

ALTER TABLE veiculo
    ADD CONSTRAINT unq_veiculo_placa UNIQUE (placa);

CREATE UNIQUE INDEX idx_veiculo_placa ON veiculo(placa);

CREATE TABLE servico (
    id UUID PRIMARY KEY,
    descricao VARCHAR(255) NOT NULL,
    valor_unitario DECIMAL(10, 2) NOT NULL,
    criado_em TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    atualizado_em TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    apagado_em TIMESTAMP
);

ALTER TABLE servico
    ADD CONSTRAINT unq_servico_descricao UNIQUE (descricao);

CREATE TABLE peca_insumo (
    id UUID PRIMARY KEY,
    descricao VARCHAR(255) NOT NULL,
    valor_unitario DECIMAL(10, 2) NOT NULL,
    criado_em TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    atualizado_em TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    apagado_em TIMESTAMP
);

ALTER TABLE peca_insumo
    ADD CONSTRAINT unq_peca_insumo_descricao UNIQUE (descricao);

-- Tabelas de relacionamento

CREATE TABLE ordem_servico (
    id UUID PRIMARY KEY,
    id_cliente UUID NOT NULL,
    id_veiculo UUID NOT NULL,
    id_status UUID NOT NULL,
    observacao TEXT,
    data_abertura TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    data_conclusao TIMESTAMP,
    criado_em TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    atualizado_em TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    apagado_em TIMESTAMP
);

ALTER TABLE ordem_servico
    ADD CONSTRAINT fk_ordem_servico_cliente FOREIGN KEY (id_cliente) REFERENCES cliente(id),
    ADD CONSTRAINT fk_ordem_servico_veiculo FOREIGN KEY (id_veiculo) REFERENCES veiculo(id),
    ADD CONSTRAINT fk_ordem_servico_status FOREIGN KEY (id_status) REFERENCES status_ordem_servico(id);

CREATE INDEX idx_ordem_servico_cliente ON ordem_servico(id_cliente);
CREATE INDEX idx_ordem_servico_veiculo ON ordem_servico(id_veiculo);
CREATE INDEX idx_ordem_servico_status ON ordem_servico(id_status);

CREATE TABLE estoque (
    id UUID PRIMARY KEY,
    id_peca_insumo UUID NOT NULL,
    quantidade INT NOT NULL,
    criado_em TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    atualizado_em TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    apagado_em TIMESTAMP
);

ALTER TABLE estoque
    ADD CONSTRAINT fk_estoque_peca_insumo FOREIGN KEY (id_peca_insumo) REFERENCES peca_insumo(id),
    ADD CONSTRAINT unq_estoque_peca_insumo UNIQUE (id_peca_insumo);

CREATE TABLE orcamento (
    id UUID PRIMARY KEY,
    id_ordem_servico UUID NOT NULL,
    valor_total DECIMAL(10, 2) NOT NULL,
    criado_em TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    atualizado_em TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    apagado_em TIMESTAMP
);

ALTER TABLE orcamento
    ADD CONSTRAINT fk_orcamento_ordem_servico FOREIGN KEY (id_ordem_servico) REFERENCES ordem_servico(id),
    ADD CONSTRAINT unq_orcamento_ordem_servico UNIQUE (id_ordem_servico);

CREATE INDEX idx_orcamento_ordem_servico ON orcamento(id_ordem_servico);

CREATE TABLE item_servico (
    id UUID PRIMARY KEY,
    id_ordem_servico UUID NOT NULL,
    id_servico UUID NOT NULL,
    data_hora_inicio TIMESTAMP,
    data_hora_fim TIMESTAMP
);

ALTER TABLE item_servico
    ADD CONSTRAINT fk_item_servico_ordem_servico FOREIGN KEY (id_ordem_servico) REFERENCES ordem_servico(id),
    ADD CONSTRAINT fk_item_servico_servico FOREIGN KEY (id_servico) REFERENCES servico(id),
    ADD CONSTRAINT unq_item_servico_ordem_servico_servico UNIQUE (id_ordem_servico, id_servico);

CREATE INDEX idx_item_servico_ordem_servico ON item_servico(id_ordem_servico);
CREATE INDEX idx_item_servico_servico ON item_servico(id_servico);

CREATE TABLE item_peca_insumo (
    id UUID PRIMARY KEY,
    id_ordem_servico UUID NOT NULL,
    id_peca_insumo UUID NOT NULL
);

ALTER TABLE item_peca_insumo
    ADD CONSTRAINT fk_item_peca_insumo_ordem_servico FOREIGN KEY (id_ordem_servico) REFERENCES ordem_servico(id),
    ADD CONSTRAINT fk_item_peca_insumo_peca_insumo FOREIGN KEY (id_peca_insumo) REFERENCES peca_insumo(id),
    ADD CONSTRAINT unq_item_peca_insumo_ordem_servico_peca UNIQUE (id_ordem_servico, id_peca_insumo);

CREATE INDEX idx_item_peca_insumo_ordem_servico ON item_peca_insumo(id_ordem_servico);
CREATE INDEX idx_item_peca_insumo_peca_insumo ON item_peca_insumo(id_peca_insumo);


