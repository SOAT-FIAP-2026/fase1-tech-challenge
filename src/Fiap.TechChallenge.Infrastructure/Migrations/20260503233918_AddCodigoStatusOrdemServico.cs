using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fiap.TechChallenge.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCodigoStatusOrdemServico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1
                        FROM information_schema.columns
                        WHERE table_name = 'status_ordem_servico'
                          AND column_name = 'codigo'
                    ) THEN
                        ALTER TABLE status_ordem_servico
                        ADD COLUMN codigo character varying(50) NOT NULL DEFAULT '';
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                UPDATE status_ordem_servico SET codigo = 'RECEBIDA' WHERE id = 'c3d4e5f6-a7b8-9012-cdef-123456789012';
                UPDATE status_ordem_servico SET codigo = 'EM_DIAGNOSTICO' WHERE id = 'd3e4f5a6-b7c8-9012-defa-234567890123';
                UPDATE status_ordem_servico SET codigo = 'AGUARDANDO_APROVACAO' WHERE id = 'e3f4a5b6-c7d8-9012-efab-345678901234';
                UPDATE status_ordem_servico SET codigo = 'EM_EXECUCAO' WHERE id = 'f4a5b6c7-d8e9-0123-efab-345678901234';
                UPDATE status_ordem_servico SET codigo = 'FINALIZADA' WHERE id = 'd4e5f6a7-b8c9-0123-defa-234567890123';
                UPDATE status_ordem_servico SET codigo = 'ENTREGUE' WHERE id = 'e5f6a7b8-c9d0-1234-efab-345678901234';
                UPDATE status_ordem_servico SET codigo = 'CANCELADA' WHERE id = 'f6a7b8c9-d0e1-2345-fabc-456789012345';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1
                        FROM information_schema.columns
                        WHERE table_name = 'status_ordem_servico'
                          AND column_name = 'codigo'
                    ) THEN
                        ALTER TABLE status_ordem_servico DROP COLUMN codigo;
                    END IF;
                END $$;
            ");
        }
    }
}
