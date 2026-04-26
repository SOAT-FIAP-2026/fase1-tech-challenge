using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fiap.TechChallenge.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cliente",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    cpf_cnpj = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    celular = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    apagado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cliente", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "peca_insumo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    descricao = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    valor_unitario = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    apagado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_peca_insumo", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "permissao",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    descricao = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    apagado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permissao", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "servico",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    descricao = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    valor_unitario = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    apagado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_servico", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "status_ordem_servico",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    descricao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_status_ordem_servico", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "veiculo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    placa = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    marca = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    modelo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ano = table.Column<int>(type: "integer", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    apagado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_veiculo", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "estoque",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    id_peca_insumo = table.Column<Guid>(type: "uuid", nullable: false),
                    quantidade = table.Column<int>(type: "integer", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    apagado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estoque", x => x.id);
                    table.ForeignKey(
                        name: "FK_estoque_peca_insumo_id_peca_insumo",
                        column: x => x.id_peca_insumo,
                        principalTable: "peca_insumo",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "usuario",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    senha_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    id_permissao = table.Column<Guid>(type: "uuid", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    apagado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuario", x => x.id);
                    table.ForeignKey(
                        name: "FK_usuario_permissao_id_permissao",
                        column: x => x.id_permissao,
                        principalTable: "permissao",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ordem_servico",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    id_cliente = table.Column<Guid>(type: "uuid", nullable: false),
                    id_veiculo = table.Column<Guid>(type: "uuid", nullable: false),
                    id_status = table.Column<Guid>(type: "uuid", nullable: false),
                    observacao = table.Column<string>(type: "text", nullable: true),
                    data_abertura = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_conclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    apagado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ordem_servico", x => x.id);
                    table.ForeignKey(
                        name: "FK_ordem_servico_cliente_id_cliente",
                        column: x => x.id_cliente,
                        principalTable: "cliente",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ordem_servico_status_ordem_servico_id_status",
                        column: x => x.id_status,
                        principalTable: "status_ordem_servico",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ordem_servico_veiculo_id_veiculo",
                        column: x => x.id_veiculo,
                        principalTable: "veiculo",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "item_peca_insumo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    id_ordem_servico = table.Column<Guid>(type: "uuid", nullable: false),
                    id_peca_insumo = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item_peca_insumo", x => x.id);
                    table.ForeignKey(
                        name: "FK_item_peca_insumo_ordem_servico_id_ordem_servico",
                        column: x => x.id_ordem_servico,
                        principalTable: "ordem_servico",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_item_peca_insumo_peca_insumo_id_peca_insumo",
                        column: x => x.id_peca_insumo,
                        principalTable: "peca_insumo",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "item_servico",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    id_ordem_servico = table.Column<Guid>(type: "uuid", nullable: false),
                    id_servico = table.Column<Guid>(type: "uuid", nullable: false),
                    data_hora_inicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    data_hora_fim = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item_servico", x => x.id);
                    table.ForeignKey(
                        name: "FK_item_servico_ordem_servico_id_ordem_servico",
                        column: x => x.id_ordem_servico,
                        principalTable: "ordem_servico",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_item_servico_servico_id_servico",
                        column: x => x.id_servico,
                        principalTable: "servico",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "orcamento",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    id_ordem_servico = table.Column<Guid>(type: "uuid", nullable: false),
                    valor_total = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    apagado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orcamento", x => x.id);
                    table.ForeignKey(
                        name: "FK_orcamento_ordem_servico_id_ordem_servico",
                        column: x => x.id_ordem_servico,
                        principalTable: "ordem_servico",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_cliente_cpf_cnpj",
                table: "cliente",
                column: "cpf_cnpj",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_estoque_id_peca_insumo",
                table: "estoque",
                column: "id_peca_insumo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_item_peca_insumo_id_ordem_servico",
                table: "item_peca_insumo",
                column: "id_ordem_servico");

            migrationBuilder.CreateIndex(
                name: "IX_item_peca_insumo_id_ordem_servico_id_peca_insumo",
                table: "item_peca_insumo",
                columns: new[] { "id_ordem_servico", "id_peca_insumo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_item_peca_insumo_id_peca_insumo",
                table: "item_peca_insumo",
                column: "id_peca_insumo");

            migrationBuilder.CreateIndex(
                name: "IX_item_servico_id_ordem_servico",
                table: "item_servico",
                column: "id_ordem_servico");

            migrationBuilder.CreateIndex(
                name: "IX_item_servico_id_ordem_servico_id_servico",
                table: "item_servico",
                columns: new[] { "id_ordem_servico", "id_servico" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_item_servico_id_servico",
                table: "item_servico",
                column: "id_servico");

            migrationBuilder.CreateIndex(
                name: "IX_orcamento_id_ordem_servico",
                table: "orcamento",
                column: "id_ordem_servico",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ordem_servico_id_cliente",
                table: "ordem_servico",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_ordem_servico_id_status",
                table: "ordem_servico",
                column: "id_status");

            migrationBuilder.CreateIndex(
                name: "IX_ordem_servico_id_veiculo",
                table: "ordem_servico",
                column: "id_veiculo");

            migrationBuilder.CreateIndex(
                name: "IX_peca_insumo_descricao",
                table: "peca_insumo",
                column: "descricao",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_permissao_descricao",
                table: "permissao",
                column: "descricao",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_servico_nome",
                table: "servico",
                column: "nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_status_ordem_servico_descricao",
                table: "status_ordem_servico",
                column: "descricao",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuario_email",
                table: "usuario",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuario_id_permissao",
                table: "usuario",
                column: "id_permissao");

            migrationBuilder.CreateIndex(
                name: "IX_veiculo_placa",
                table: "veiculo",
                column: "placa",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "estoque");

            migrationBuilder.DropTable(
                name: "item_peca_insumo");

            migrationBuilder.DropTable(
                name: "item_servico");

            migrationBuilder.DropTable(
                name: "orcamento");

            migrationBuilder.DropTable(
                name: "usuario");

            migrationBuilder.DropTable(
                name: "peca_insumo");

            migrationBuilder.DropTable(
                name: "servico");

            migrationBuilder.DropTable(
                name: "ordem_servico");

            migrationBuilder.DropTable(
                name: "permissao");

            migrationBuilder.DropTable(
                name: "cliente");

            migrationBuilder.DropTable(
                name: "status_ordem_servico");

            migrationBuilder.DropTable(
                name: "veiculo");
        }
    }
}
