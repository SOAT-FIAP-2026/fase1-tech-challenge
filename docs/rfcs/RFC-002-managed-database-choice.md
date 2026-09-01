# RFC-002: Escolha do Banco de Dados Gerenciado (AWS RDS PostgreSQL)

* **Status**: Aprovado
* **Data**: 2026-08-31
* **Autor**: Equipe de Arquitetura (SOAT - Tech Challenge)
* **Domínio**: Persistência & Dados

---

## 1. Resumo Executivo

Este RFC descreve a análise técnica e justifica a adoção do **Amazon RDS para PostgreSQL (versão 16)** como a solução oficial de banco de dados relacional gerenciado para o sistema de gestão da Oficina Mecânica.

---

## 2. Contexto e Requisitos do Domínio

O sistema da oficina lida com fluxos críticos de negócio que exigem estrita consistência transactional:
* **Gestão de Clientes e Veículos**: Relacionamentos 1:N com dados cadastrais e identificadores únicos (CPF, Placa, VIN).
* **Ordens de Serviço (OS)**: Máquina de estados complexa (`Aberta` → `EmDiagnostico` → `DiagnosticoConcluido` → `EmExecucao` → `Finalizado` → `Entregue`).
* **Itens e Serviços da OS**: Cálculo de valores de peças, mão de obra e histórico de alterações em tempo real.

### Requisitos Técnicos Obrigatórios:
1. **Garantia ACID**: Transações atômicas para evitar inconsistências em aprovações financeiras e atualização de status de OS.
2. **Alta Disponibilidade e Backups**: Backup automatizado point-in-time recovery (PITR) e suporte a réplicas de leitura/failover.
3. **Gerenciamento Operacional Zero**: Eliminação da necessidade de administrar SO, patches de segurança ou backups manuais.

---

## 3. Opções Avaliadas

| Critério | AWS RDS PostgreSQL 16 (Escolha) | DynamoDB / NoSQL | Postgres Self-Hosted no K8s |
|---|---|---|---|
| **Modelo de Dados** | Relacional (ACID completo, Foreign Keys, Triggers, Views). | Chave-Valor / Documento (Consistência Eventual por padrão). | Relacional (ACID completo). |
| **Integridade Relacional** | Garantida nativamente no motor do banco de dados. | Requer validação e controle via código da aplicação. | Garantida nativamente. |
| **Operação e Manutenção** | Totalmente gerenciado pela AWS (patches, backups, Multi-AZ). | Totalmente gerenciado pela AWS. | Alta carga operacional (gerenciar backups, PVCs, failover manual). |
| **Escalabilidade** | Vertical (Instance Resize) + Read Replicas. | Horizontal automática e ilimitada. | Limitada pelos nós do Kubernetes. |
| **Compatibilidade com EF Core** | Suporte excelente via `Npgsql.EntityFrameworkCore.PostgreSQL`. | Suporte limitado ou via SDKs customizados. | Suporte excelente. |

---

## 4. Justificativa da Escolha do PostgreSQL no RDS

1. **Adequação ao Modelo Relacional do Domínio**:
   O domínio da oficina mecânica é altamente estruturado. Entidades como `Cliente`, `Veiculo`, `OrdemServico`, `Servico` e `Peca` possuem relacionamentos rígidos com chaves estrangeiras. O suporte do PostgreSQL a restrições de integridade e transações garante a consistência absoluta do sistema.

2. **Benefícios do Serviço Gerenciado (AWS RDS)**:
   * **Automated Backups**: Retenção configurada com backups contínuos e snapshots diários.
   * **Multi-AZ Deployment**: Em produção, fornece replicação síncrona para uma segunda Zona de Disponibilidade com failover automático em menos de 60 segundos em caso de falha de hardware.
   * **Segurança**: Isolamento em Subnet de Banco de Dados privada dentro da VPC, sem IP público, com autenticação via credenciais seguras injetadas via Kubernetes Secrets.

3. **Inovações do PostgreSQL 16**:
   A versão 16 traz otimizações significativas de performance de queries, melhorias na alocação de memória para `JOINs` e suporte aprimorado a métricas de monitoramento que se integram perfeitamente com Datadog / New Relic.

---

## 5. Modelagem e Estrutura Relacional (Resumo)

```
┌──────────────┐       1:N       ┌──────────────┐
│   Clientes   │────────────────<│   Veiculos   │
└──────────────┘                 └──────────────┘
       │                                │
       │ 1:N                            │ 1:N
       ▼                                ▼
┌───────────────────────────────────────────────┐
│                OrdensServico                  │
├───────────────────────────────────────────────┤
│ PK  Id : Guid                                 │
│ FK  ClienteId : Guid                          │
│ FK  VeiculoId : Guid                          │
│     Status : Enum (Aberta, EmDiagnostico...)  │
│     ValorTotal : Decimal                      │
│     DataAbertura : Timestamp                  │
└───────────────────────────────────────────────┘
       │
       │ 1:N
       ▼
┌───────────────────────────────────────────────┐
│              OrdensServicoItens               │
└───────────────────────────────────────────────┘
```

---

## 6. Riscos e Otimizações

* **Pool de Conexões**:
  Para evitar exaustão de conexões causada por múltiplas instâncias da API .NET no EKS e da Lambda de Autenticação, foi implementado o pooling de conexões via Npgsql no backend e otimização de tempo de vida das conexões na Lambda.

---

## 7. Decisão Final

* **Tecnologia**: AWS RDS PostgreSQL 16
* **Status**: Aprovado e implementado via IaC no repositório `tech-challenge-infra-db`.
