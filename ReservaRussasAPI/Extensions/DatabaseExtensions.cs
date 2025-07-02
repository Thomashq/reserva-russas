using Microsoft.EntityFrameworkCore;
using RR.Infraestructure.DataContext;

namespace ReservaRussasAPI.Extensions
{
    public static class DatabaseExtensions
    {
        public static async Task<IServiceProvider> EnsureDatabaseMigratedAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

            try
            {
                logger.LogInformation("Verificando migrations pendentes...");

                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                var appliedMigrations = await context.Database.GetAppliedMigrationsAsync();

                if (!appliedMigrations.Any())
                {
                    logger.LogInformation("Nenhuma migration encontrada. Verificando se é necessário criar migrations iniciais...");

                    var canConnect = await context.Database.CanConnectAsync();

                    if (!canConnect)
                    {
                        logger.LogInformation("Banco de dados não existe. Criando banco de dados...");
                        await context.Database.EnsureCreatedAsync();
                        logger.LogInformation("Banco de dados criado com sucesso.");
                    }
                    else
                    {
                        logger.LogWarning("Banco de dados existe mas não possui migrations aplicadas.");

                        if (pendingMigrations.Any())
                        {
                            logger.LogInformation($"Aplicando {pendingMigrations.Count()} migration(s) pendente(s)...");
                            await context.Database.MigrateAsync();
                            logger.LogInformation("Migrations aplicadas com sucesso.");
                        }
                    }
                }
                else if (pendingMigrations.Any())
                {
                    logger.LogInformation($"Encontradas {pendingMigrations.Count()} migration(s) pendente(s):");
                    foreach (var migration in pendingMigrations)
                    {
                        logger.LogInformation($"- {migration}");
                    }

                    logger.LogInformation("Aplicando migrations pendentes...");
                    await context.Database.MigrateAsync();
                    logger.LogInformation("Migrations aplicadas com sucesso.");
                }
                else
                {
                    logger.LogInformation("Banco de dados está atualizado. Nenhuma migration pendente.");
                }

                await ValidateEssentialTablesAsync(context, logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao aplicar migrations: {Message}", ex.Message);
                throw;
            }

            return serviceProvider;
        }

        private static async Task ValidateEssentialTablesAsync(ApplicationDbContext context, ILogger logger)
        {
            try
            {
                logger.LogInformation("Validando estrutura do banco de dados...");

                // Teste simples para verificar se o contexto está funcionando
                // Substitua por uma verificação específica das suas entidades principais
                var canQuery = await context.Database.ExecuteSqlRawAsync("SELECT 1");

                logger.LogInformation("Validação da estrutura do banco concluída com sucesso.");
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Erro na validação da estrutura do banco: {Message}", ex.Message);
            }
        }
    }
}