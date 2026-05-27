using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LigaDosPets.Domain.Entities;
using LigaDosPets.Infrastructure.Identity;
using LigaDosPets.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;

namespace LigaDosPets.Infrastructure.Persistence.Seed
{
    public static class DataBaseSeeder
    {
        public static async Task SeedAsync(
            AppDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // ── 0. Roles do Identity ──────────────────────────────────────────
            string[] roles = { "Administrador", "Gerente", "Usuário" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // ── 1. Portes ─────────────────────────────────────────────────────
            if (!await context.Portes.AnyAsync())
            {
                context.Portes.AddRange(
                    new Porte { Nome = "Pequeno" },
                    new Porte { Nome = "Médio" },
                    new Porte { Nome = "Grande" }
                );
                await context.SaveChangesAsync();
            }

            // ── 2. Espécies ───────────────────────────────────────────────────
            if (!await context.Especies.AnyAsync())
            {
                context.Especies.AddRange(
                    new Especie { Nome = "Cachorro" },
                    new Especie { Nome = "Gato" },
                    new Especie { Nome = "Ave" },
                    new Especie { Nome = "Réptil" },
                    new Especie { Nome = "Roedor" },
                    new Especie { Nome = "Peixe" }
                );
                await context.SaveChangesAsync();
            }

            // ── 3. Raças ──────────────────────────────────────────────────────
            if (!await context.Racas.AnyAsync())
            {
                var racasPorEspecie = new Dictionary<string, List<string>>
                {
                    { "Cachorro", new List<string> { "Labrador", "Poodle", "Bulldog", "Golden Retriever", "Pastor Alemão", "Pinscher", "Chihuahua", "Rottweiler", "Doberman", "Shih Tzu", "Beagle", "Lulu da Pomerânia", "SRD" } },
                    { "Gato",     new List<string> { "Siamês", "Persa", "Maine Coon", "Angorá", "Bengal", "Sphynx", "Ragdoll", "Himalaio", "SRD" } },
                    { "Ave",      new List<string> { "Calopsita", "Papagaio", "Arara", "Canário", "Periquito", "Agapornis", "Cacatua" } },
                    { "Réptil",   new List<string> { "Iguana", "Camaleão", "Gecko", "Jabuti", "Tartaruga", "Cobra" } },
                    { "Roedor",   new List<string> { "Hamster", "Porquinho-da-Índia", "Chinchila", "Gerbil", "Rato" } },
                    { "Peixe",    new List<string> { "Betta", "Guppy", "Neon", "Tetra", "Disco", "Ciclídeo" } }
                };

                foreach (var item in racasPorEspecie)
                {
                    var especie = await context.Especies.FirstOrDefaultAsync(e => e.Nome == item.Key);
                    if (especie is null) continue;

                    foreach (var nomeRaca in item.Value)
                        context.Racas.Add(new Raca { Nome = nomeRaca, EspecieId = especie.Id });
                }

                await context.SaveChangesAsync();
            }

            // ── 4. Tipos de Usuário ───────────────────────────────────────────
            if (!await context.TiposUsuarios.AnyAsync())
            {
                context.TiposUsuarios.AddRange(
                    new TipoUsuario { Nome = "Usuário" },
                    new TipoUsuario { Nome = "Administrador" },
                    new TipoUsuario { Nome = "Gerente" }
                );
                await context.SaveChangesAsync();
            }

            // ── 5. Status de Solicitação ──────────────────────────────────────
            if (!await context.StatusSolicitacoes.AnyAsync())
            {
                context.StatusSolicitacoes.AddRange(
                    new StatusSolicitacao { Nome = "Pendente" },
                    new StatusSolicitacao { Nome = "Aprovado" },
                    new StatusSolicitacao { Nome = "Rejeitado" }
                );
                await context.SaveChangesAsync();
            }

            // ── 5.1 Características ───────────────────────────────────────────
            if (!await context.Caracteristicas.AnyAsync())
            {
                context.Caracteristicas.AddRange(
                    new Caracteristica { Nome = "Castrado" },
                    new Caracteristica { Nome = "Vacinado" },
                    new Caracteristica { Nome = "Vermifugado" },
                    new Caracteristica { Nome = "Sociável com crianças" },
                    new Caracteristica { Nome = "Sociável com outros animais" },
                    new Caracteristica { Nome = "Aceita coleira/peitoral" },
                    new Caracteristica { Nome = "Adestrado" },
                    new Caracteristica { Nome = "Necessita de cuidados especiais" }
                );
                await context.SaveChangesAsync();
            }
            // ── 5.5 Status de Agendamento ─────────────────────────────────────
            if (!await context.StatusAgendamentos.AnyAsync())
            {
                context.StatusAgendamentos.AddRange(
                    new StatusAgendamento { Nome = "Pendente" },
                    new StatusAgendamento { Nome = "Confirmado" },
                    new StatusAgendamento { Nome = "Realizado" },
                    new StatusAgendamento { Nome = "Cancelado" }
                );
                await context.SaveChangesAsync();
            }

            // ── 6. Usuários ───────────────────────────────────────────────────
            async Task CriarUsuario(string email, string senha, string nome, string tipoNome, string roleName)
            {
                // 1. Busca normalizada para evitar problemas de maiúsculas/minúsculas
                var userEmailUpper = email.ToUpper();
                var existingIdentityUser = await userManager.Users
                    .FirstOrDefaultAsync(u => u.NormalizedEmail == userEmailUpper || u.NormalizedUserName == userEmailUpper);

                if (existingIdentityUser != null) return;

                // 2. Tenta buscar na tabela de domínio para garantir sincronia
                var existingDomainUser = await context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email.ToUpper() == userEmailUpper);

                if (existingDomainUser != null) return;

                var appUser = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(appUser, senha);

                if (!result.Succeeded)
                {
                    // Se o erro for de duplicidade, apenas ignoramos e saímos do método
                    if (result.Errors.Any(e => e.Code == "DuplicateUserName" || e.Code == "DuplicateEmail"))
                        return;

                    throw new InvalidOperationException($"Erro ao criar {email}: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }

                // Adiciona a Role
                if (!await userManager.IsInRoleAsync(appUser, roleName))
                {
                    await userManager.AddToRoleAsync(appUser, roleName);
                }

                var tipo = await context.TiposUsuarios.FirstOrDefaultAsync(t => t.Nome == tipoNome);
                if (tipo is null) return;

                // Adiciona na tabela de perfil do usuário (Domínio)
                context.Usuarios.Add(new Usuario
                {
                    Nome = nome,
                    Email = email,
                    ApplicationUserId = appUser.Id,
                    TipoUsuarioId = tipo.Id,
                    DataCadastro = DateTime.UtcNow
                });

                await context.SaveChangesAsync();
            }

            await CriarUsuario("admin@ligadospets.com", "Admin@123", "Administrador", "Administrador", "Administrador");
            await CriarUsuario("gerente@ligadospets.com", "Gerente@123", "Gerente", "Gerente", "Gerente");
            await CriarUsuario("usuario@ligadospets.com", "Usuario@123", "Usuário Comum", "Usuário", "Usuário");

            // ── 7. Animais e Fotos ────────────────────────────────────────────
            if (await context.Animais.AnyAsync()) return;

            var usuario = await context.Usuarios.FirstOrDefaultAsync();
            if (usuario is null) return;

            // Portes
            var portePequeno = await context.Portes.FirstAsync(p => p.Nome == "Pequeno");
            var porteMedio = await context.Portes.FirstAsync(p => p.Nome == "Médio");
            var porteGrande = await context.Portes.FirstAsync(p => p.Nome == "Grande");

            // Espécies
            var especieCachorro = await context.Especies.FirstAsync(e => e.Nome == "Cachorro");
            var especieGato = await context.Especies.FirstAsync(e => e.Nome == "Gato");
            var especieAve = await context.Especies.FirstAsync(e => e.Nome == "Ave");
            var especiePeixe = await context.Especies.FirstAsync(e => e.Nome == "Peixe");
            var especieReptil = await context.Especies.FirstAsync(e => e.Nome == "Réptil");
            var especieRoedor = await context.Especies.FirstAsync(e => e.Nome == "Roedor");

            // Raças
            var racaShihTzu = await context.Racas.FirstAsync(r => r.Nome == "Shih Tzu" && r.EspecieId == especieCachorro.Id);
            var racaSRDCachorro = await context.Racas.FirstAsync(r => r.Nome == "SRD" && r.EspecieId == especieCachorro.Id);
            var racaPinscher = await context.Racas.FirstAsync(r => r.Nome == "Pinscher" && r.EspecieId == especieCachorro.Id);
            var racaPoodle = await context.Racas.FirstAsync(r => r.Nome == "Poodle" && r.EspecieId == especieCachorro.Id);
            var racaLuludaPomerania = await context.Racas.FirstAsync(r => r.Nome == "Lulu da Pomerânia" && r.EspecieId == especieCachorro.Id);
            var racaSRDGato = await context.Racas.FirstAsync(r => r.Nome == "SRD" && r.EspecieId == especieGato.Id);
            var racaPapagaio = await context.Racas.FirstAsync(r => r.Nome == "Papagaio" && r.EspecieId == especieAve.Id);
            var racaBetta = await context.Racas.FirstAsync(r => r.Nome == "Betta" && r.EspecieId == especiePeixe.Id);
            var racaGuppy = await context.Racas.FirstAsync(r => r.Nome == "Guppy" && r.EspecieId == especiePeixe.Id);
            var racaJabuti = await context.Racas.FirstAsync(r => r.Nome == "Jabuti" && r.EspecieId == especieReptil.Id);
            var racaRato = await context.Racas.FirstAsync(r => r.Nome == "Rato" && r.EspecieId == especieRoedor.Id);

            var animais = new List<Animal>
            {
                new() { Nome = "Lilica",         Idade = 3,  Sexo = "Fêmea", PorteId = portePequeno.Id, EspecieId = especieCachorro.Id, RacaId = racaShihTzu.Id,         UsuarioCadastroId = usuario.Id, Descricao = "Conheça a Lilica, a definição de elegância e fofura. Além de sua beleza marcante, ela se destaca pela personalidade tranquila e dócil. É aquela companheira perfeita que prefere um bom colo e momentos de carinho, tratando todos com uma paciência e uma alegria que são só dela.", DisponivelParaAdocao = true, AguardandoValidacao = false, DataCadastro = DateTime.UtcNow },
                new() { Nome = "Bagheera",       Idade = 4,  Sexo = "Macho", PorteId = porteMedio.Id,   EspecieId = especieCachorro.Id, RacaId = racaSRDCachorro.Id,     UsuarioCadastroId = usuario.Id, Descricao = "Bagheera é o verdadeiro retrato da lealdade. Com sua elegância natural e olhar doce, ele conquista a todos com sua calma e doçura. É um cão extremamente paciente e dedicado, perfeito para quem busca um parceiro tranquilo para dividir momentos de carinho e serenidade.", DisponivelParaAdocao = true, AguardandoValidacao = false, DataCadastro = DateTime.UtcNow },
                new() { Nome = "Estrela",        Idade = 1,  Sexo = "Fêmea", PorteId = portePequeno.Id, EspecieId = especieCachorro.Id, RacaId = racaPinscher.Id,        UsuarioCadastroId = usuario.Id, Descricao = "Prepare-se para conhecer a Estrela, um filhote com energia de sobra e muita vontade de aprender! Com seu porte compacto e olhar atento, ele está pronto para explorar cada canto do mundo (e do seu coração). Inteligente e brincalhão, ele vai adorar descobrir novos brinquedos e te acompanhar em todas as suas aventuras.", AguardandoValidacao = false, DisponivelParaAdocao = true, DataCadastro = DateTime.UtcNow },
                new() { Nome = "Freya",          Idade = 4,  Sexo = "Fêmea", PorteId = portePequeno.Id, EspecieId = especieCachorro.Id, RacaId = racaPinscher.Id,        UsuarioCadastroId = usuario.Id, Descricao = "Conheça a Freya, o verdadeiro especialista em brincar. Com sua pose relaxada e os olhos semicerrados pelo sol, ela sabe exatamente como recarregar as energias. Não se engane pela carinha de sono: ela é uma observadora atenta e uma grande companheira para quem valoriza os pequenos prazeres e uma boa dose de sol.", AguardandoValidacao = false, DisponivelParaAdocao = true, DataCadastro = DateTime.UtcNow },
                new() { Nome = "Kiara",          Idade = 6,  Sexo = "Fêmea", PorteId = porteMedio.Id,   EspecieId = especieCachorro.Id, RacaId = racaPoodle.Id,          UsuarioCadastroId = usuario.Id, Descricao = "A Kiara é a personificação da elegância em miniatura. Com seus cachinhos pretos e um olhar que parece entender cada palavra que dizemos, ela é uma Poodle extremamente inteligente e carinhosa. É aquela companheira fiel que adora estar por perto, sempre pronta para oferecer um carinho e alegrar o ambiente com sua presença gentil.", AguardandoValidacao = false, DisponivelParaAdocao = true, DataCadastro = DateTime.UtcNow },
                new() { Nome = "Kiara",          Idade = 1,  Sexo = "Fêmea", PorteId = porteMedio.Id,   EspecieId = especieCachorro.Id, RacaId = racaLuludaPomerania.Id, UsuarioCadastroId = usuario.Id, Descricao = "Conheça essa pequena nuvem de fofura! Com sua pelagem farta, macia e clara, esse Lulu da Pomerânia é o retrato da delicadeza. Além de sua beleza exuberante, ele possui um olhar inteligente e curioso que cativa qualquer um à primeira vista.", DisponivelParaAdocao = true, AguardandoValidacao = false, DataCadastro = DateTime.UtcNow },
                new() { Nome = "Mike",           Idade = 7,  Sexo = "Macho", PorteId = porteGrande.Id,  EspecieId = especieCachorro.Id, RacaId = racaSRDCachorro.Id,     UsuarioCadastroId = usuario.Id, Descricao = "Conheça o Mike, um verdadeiro mestre na arte de derreter corações com um simples olhar! Com suas sobrancelhas bronze marcantes e olhos castanhos expressivos, ele é dócil, carinhoso e super apegado.", DisponivelParaAdocao = true, AguardandoValidacao = false, DataCadastro = DateTime.UtcNow },
                new() { Nome = "Pandora",        Idade = 5,  Sexo = "Fêmea", PorteId = porteMedio.Id,   EspecieId = especieCachorro.Id, RacaId = racaShihTzu.Id,         UsuarioCadastroId = usuario.Id, Descricao = "Conheça a Pandora, a nossa companheira que brilha por dentro e por fora! Com seus olhos expressivos e carinhosos, ela é dócil, leal e super apegada aos seus humanos.", DisponivelParaAdocao = true, AguardandoValidacao = false, DataCadastro = DateTime.UtcNow },
                new() { Nome = "Chico",          Idade = 2,  Sexo = "Macho", PorteId = porteMedio.Id,   EspecieId = especieAve.Id,      RacaId = racaPapagaio.Id,        UsuarioCadastroId = usuario.Id, Descricao = "Chico é um papagaio muito comunicativo e inteligente. Adora interagir com pessoas, repetir palavras e trazer alegria para o ambiente. Perfeito para quem busca um pet cheio de personalidade.", DisponivelParaAdocao = true, AguardandoValidacao = false, DataCadastro = DateTime.UtcNow },
                new() { Nome = "Felicia",        Idade = 5,  Sexo = "Fêmea", PorteId = portePequeno.Id, EspecieId = especieGato.Id,     RacaId = racaSRDGato.Id,         UsuarioCadastroId = usuario.Id, Descricao = "Felicia é a personificação da elegância felina. Com sua pelagem exuberante e um olhar profundo que parece ler pensamentos, ela traz um ar de serenidade e mistério para qualquer ambiente.", DisponivelParaAdocao = true,AguardandoValidacao = false, DataCadastro = DateTime.UtcNow },
                new() { Nome = "Frajola",        Idade = 1,  Sexo = "Macho", PorteId = portePequeno.Id, EspecieId = especieGato.Id,     RacaId = racaSRDGato.Id,         UsuarioCadastroId = usuario.Id, Descricao = "Se você procura um amigo leal e tranquilo, conheça o Frajola. Sociável e dócil, ele convive muito bem com outros animais e adora encontrar um ponto estratégico para observar o movimento.", DisponivelParaAdocao = true, AguardandoValidacao = false,DataCadastro = DateTime.UtcNow },
                new() { Nome = "Gordinha",       Idade = 4,  Sexo = "Fêmea", PorteId = porteMedio.Id,   EspecieId = especieGato.Id,     RacaId = racaSRDGato.Id,         UsuarioCadastroId = usuario.Id, Descricao = "Se existe alguém que entende de relaxamento, esse alguém é a Gordinha! Com sua pose clássica de barriga para cima, ela mostra que a vida deve ser levada com leveza e muito conforto.", DisponivelParaAdocao = true,AguardandoValidacao = false, DataCadastro = DateTime.UtcNow },
                new() { Nome = "Nala",           Idade = 2,  Sexo = "Fêmea", PorteId = porteMedio.Id,   EspecieId = especieGato.Id,     RacaId = racaSRDGato.Id,         UsuarioCadastroId = usuario.Id, Descricao = "Conheça a Nala, uma gata de beleza clássica e um olhar verde esmeralda que hipnotiza qualquer um. É uma companheira observadora que adora a paz de um ambiente acolhedor.", DisponivelParaAdocao = true, AguardandoValidacao = false, DataCadastro = DateTime.UtcNow },
                new() { Nome = "Ravena",         Idade = 2,  Sexo = "Fêmea", PorteId = portePequeno.Id, EspecieId = especieGato.Id,     RacaId = racaSRDGato.Id,         UsuarioCadastroId = usuario.Id, Descricao = "Como o próprio nome sugere, a Ravena possui um mistério cativante. O contraste marcante entre o branco puro e o preto profundo de suas manchas revela uma gata de beleza única.", DisponivelParaAdocao = true, AguardandoValidacao = false, DataCadastro = DateTime.UtcNow },
                new() { Nome = "Peixe Betta",    Idade = 1,  Sexo = "Macho", PorteId = portePequeno.Id, EspecieId = especiePeixe.Id,    RacaId = racaBetta.Id,           UsuarioCadastroId = usuario.Id, Descricao = "O Peixe Betta é uma verdadeira joia aquática, conhecido por suas caudas exuberantes e cores vibrantes que parecem dançar na água.", DisponivelParaAdocao = true, AguardandoValidacao = false, DataCadastro = DateTime.UtcNow },
                new() { Nome = "Peixe Guppy",    Idade = 2,  Sexo = "Fêmea", PorteId = portePequeno.Id, EspecieId = especiePeixe.Id,    RacaId = racaGuppy.Id,           UsuarioCadastroId = usuario.Id, Descricao = "O Peixe Guppy é um pequeno tesouro aquático, conhecido por suas caudas coloridas e padrões vibrantes que trazem vida a qualquer aquário.", DisponivelParaAdocao = true, AguardandoValidacao = false, DataCadastro = DateTime.UtcNow },
                new() { Nome = "Mestre Oogway",  Idade = 50, Sexo = "Macho", PorteId = porteMedio.Id,   EspecieId = especieReptil.Id,   RacaId = racaJabuti.Id,          UsuarioCadastroId = usuario.Id, Descricao = "Mestre Oogway é um jabuti sábio e sereno, conhecido por sua longevidade e tranquilidade. Sua presença silenciosa é um lembrete constante de que a paciência e a perseverança são virtudes valiosas.", DisponivelParaAdocao = true, AguardandoValidacao = false, DataCadastro = DateTime.UtcNow },
                new() { Nome = "Ziggs",          Idade = 1,  Sexo = "Macho", PorteId = portePequeno.Id, EspecieId = especieRoedor.Id,   RacaId = racaRato.Id,            UsuarioCadastroId = usuario.Id, Descricao = "Ziggs é um Rato cheio de energia e personalidade. Com seu pelo macio e olhos curiosos, ele é o companheiro perfeito para quem busca um pet pequeno, mas cheio de vida.", DisponivelParaAdocao = true, AguardandoValidacao = false, DataCadastro = DateTime.UtcNow },
            };

            await context.Animais.AddRangeAsync(animais);
            await context.SaveChangesAsync();

            // ── 7.1 Associar características aos animais ───────────────────────
            var caracteristicas = await context.Caracteristicas.ToListAsync();
            var castrado = caracteristicas.First(c => c.Nome == "Castrado");
            var vacinado = caracteristicas.First(c => c.Nome == "Vacinado");
            var vermifugado = caracteristicas.First(c => c.Nome == "Vermifugado");
            var sociCrianca = caracteristicas.First(c => c.Nome == "Sociável com crianças");
            var sociAnimais = caracteristicas.First(c => c.Nome == "Sociável com outros animais");
            var coleira = caracteristicas.First(c => c.Nome == "Aceita coleira/peitoral");
            var adestrado = caracteristicas.First(c => c.Nome == "Adestrado");
            var cuidadosEsp = caracteristicas.First(c => c.Nome == "Necessita de cuidados especiais");

            var animalCaracteristicas = new List<AnimalCaracteristica>();

            // Lilica (Shih Tzu)
            var lilica = animais.First(a => a.Nome == "Lilica");
            animalCaracteristicas.Add(new AnimalCaracteristica { AnimalId = lilica.Id, CaracteristicaId = vacinado.Id });
            animalCaracteristicas.Add(new AnimalCaracteristica { AnimalId = lilica.Id, CaracteristicaId = vermifugado.Id });
            animalCaracteristicas.Add(new AnimalCaracteristica { AnimalId = lilica.Id, CaracteristicaId = castrado.Id });
            animalCaracteristicas.Add(new AnimalCaracteristica { AnimalId = lilica.Id, CaracteristicaId = sociCrianca.Id });

            // Bagheera (SRD)
            var bagheera = animais.First(a => a.Nome == "Bagheera");
            animalCaracteristicas.Add(new AnimalCaracteristica { AnimalId = bagheera.Id, CaracteristicaId = vacinado.Id });
            animalCaracteristicas.Add(new AnimalCaracteristica { AnimalId = bagheera.Id, CaracteristicaId = vermifugado.Id });
            animalCaracteristicas.Add(new AnimalCaracteristica { AnimalId = bagheera.Id, CaracteristicaId = castrado.Id });
            animalCaracteristicas.Add(new AnimalCaracteristica { AnimalId = bagheera.Id, CaracteristicaId = sociAnimais.Id });

            // Estrela (Pinscher filhote)
            var estrela = animais.First(a => a.Nome == "Estrela");
            animalCaracteristicas.Add(new AnimalCaracteristica { AnimalId = estrela.Id, CaracteristicaId = vacinado.Id });
            animalCaracteristicas.Add(new AnimalCaracteristica { AnimalId = estrela.Id, CaracteristicaId = vermifugado.Id });

            // Freya (Pinscher adulto)
            var freya = animais.First(a => a.Nome == "Freya");
            animalCaracteristicas.Add(new AnimalCaracteristica { AnimalId = freya.Id, CaracteristicaId = vacinado.Id });
            animalCaracteristicas.Add(new AnimalCaracteristica { AnimalId = freya.Id, CaracteristicaId = castrado.Id });

            // Kiara (Poodle)
            var kiaraPoodle = animais.First(a => a.Nome == "Kiara" && a.RacaId == racaPoodle.Id);
            animalCaracteristicas.Add(new AnimalCaracteristica { AnimalId = kiaraPoodle.Id, CaracteristicaId = vacinado.Id });
            animalCaracteristicas.Add(new AnimalCaracteristica { AnimalId = kiaraPoodle.Id, CaracteristicaId = adestrado.Id });

            // Kiara (Lulu da Pomerânia)
            var kiaraLulu = animais.First(a => a.Nome == "Kiara" && a.RacaId == racaLuludaPomerania.Id);
            animalCaracteristicas.Add(new AnimalCaracteristica { AnimalId = kiaraLulu.Id, CaracteristicaId = vacinado.Id });
            animalCaracteristicas.Add(new AnimalCaracteristica { AnimalId = kiaraLulu.Id, CaracteristicaId = sociCrianca.Id });

            // Mike (SRD grande)
            var mike = animais.First(a => a.Nome == "Mike");
            animalCaracteristicas.Add(new AnimalCaracteristica { AnimalId = mike.Id, CaracteristicaId = vacinado.Id });
            animalCaracteristicas.Add(new AnimalCaracteristica { AnimalId = mike.Id, CaracteristicaId = castrado.Id });
            animalCaracteristicas.Add(new AnimalCaracteristica { AnimalId = mike.Id, CaracteristicaId = coleira.Id });

            // Pandora (Shih Tzu)
            var pandora = animais.First(a => a.Nome == "Pandora");
            animalCaracteristicas.Add(new AnimalCaracteristica { AnimalId = pandora.Id, CaracteristicaId = vacinado.Id });
            animalCaracteristicas.Add(new AnimalCaracteristica { AnimalId = pandora.Id, CaracteristicaId = castrado.Id });
            animalCaracteristicas.Add(new AnimalCaracteristica { AnimalId = pandora.Id, CaracteristicaId = sociCrianca.Id });

            // Chico (Papagaio)
            var chico = animais.First(a => a.Nome == "Chico");
            animalCaracteristicas.Add(new AnimalCaracteristica { AnimalId = chico.Id, CaracteristicaId = sociAnimais.Id });

            // Felicia (Gato SRD)
            var felicia = animais.First(a => a.Nome == "Felicia");
            animalCaracteristicas.Add(new AnimalCaracteristica { AnimalId = felicia.Id, CaracteristicaId = vacinado.Id });
            animalCaracteristicas.Add(new AnimalCaracteristica { AnimalId = felicia.Id, CaracteristicaId = castrado.Id });

            // Frajola (Gato SRD)
            var frajola = animais.First(a => a.Nome == "Frajola");
            animalCaracteristicas.Add(new AnimalCaracteristica { AnimalId = frajola.Id, CaracteristicaId = vacinado.Id });
            animalCaracteristicas.Add(new AnimalCaracteristica { AnimalId = frajola.Id, CaracteristicaId = castrado.Id });
            animalCaracteristicas.Add(new AnimalCaracteristica { AnimalId = frajola.Id, CaracteristicaId = sociAnimais.Id });

            // Gordinha (Gato SRD)
            var gordinha = animais.First(a => a.Nome == "Gordinha");
            animalCaracteristicas.Add(new AnimalCaracteristica { AnimalId = gordinha.Id, CaracteristicaId = castrado.Id });

            // Nala (Gato SRD)
            var nala = animais.First(a => a.Nome == "Nala");
            animalCaracteristicas.Add(new AnimalCaracteristica { AnimalId = nala.Id, CaracteristicaId = vacinado.Id });
            animalCaracteristicas.Add(new AnimalCaracteristica { AnimalId = nala.Id, CaracteristicaId = castrado.Id });

            // Ravena (Gato SRD)
            var ravena = animais.First(a => a.Nome == "Ravena");
            animalCaracteristicas.Add(new AnimalCaracteristica { AnimalId = ravena.Id, CaracteristicaId = vacinado.Id });
            animalCaracteristicas.Add(new AnimalCaracteristica { AnimalId = ravena.Id, CaracteristicaId = castrado.Id });

            // Mestre Oogway (Jabuti)
            var oogway = animais.First(a => a.Nome == "Mestre Oogway");
            animalCaracteristicas.Add(new AnimalCaracteristica { AnimalId = oogway.Id, CaracteristicaId = cuidadosEsp.Id });

            // Peixes e Ziggs não recebem características neste seed

            await context.AnimaisCaracteristicas.AddRangeAsync(animalCaracteristicas);
            await context.SaveChangesAsync();

            // ── 7.2 Fotos ─────────────────────────────────────────────────────
            var fotos = new List<Foto>
            {
                new() { Url = "/imagens/Cachorros/lilica.jpeg",       IsPrincipal = true, AnimalId = lilica.Id },
                new() { Url = "/imagens/Cachorros/Bagheera.jpeg",     IsPrincipal = true, AnimalId = bagheera.Id },
                new() { Url = "/imagens/Cachorros/Estrela.jpeg",      IsPrincipal = true, AnimalId = estrela.Id },
                new() { Url = "/imagens/Cachorros/Freya.jpeg",        IsPrincipal = true, AnimalId = freya.Id },
                new() { Url = "/imagens/Cachorros/Kiara.jpeg",        IsPrincipal = true, AnimalId = kiaraPoodle.Id },
                new() { Url = "/imagens/Cachorros/kiara2.jpeg",       IsPrincipal = true, AnimalId = kiaraLulu.Id },
                new() { Url = "/imagens/Cachorros/Mike.jpeg",         IsPrincipal = true, AnimalId = mike.Id },
                new() { Url = "/imagens/Cachorros/Pandora.jpeg",      IsPrincipal = true, AnimalId = pandora.Id },
                new() { Url = "/imagens/Aves/Chico.jpg",              IsPrincipal = true, AnimalId = chico.Id },
                new() { Url = "/imagens/Gatos/Felicia.jpeg",          IsPrincipal = true, AnimalId = felicia.Id },
                new() { Url = "/imagens/Gatos/Frajola.jpeg",          IsPrincipal = true, AnimalId = frajola.Id },
                new() { Url = "/imagens/Gatos/Gordinha.jpeg",         IsPrincipal = true, AnimalId = gordinha.Id },
                new() { Url = "/imagens/Gatos/Nala.jpeg",             IsPrincipal = true, AnimalId = nala.Id },
                new() { Url = "/imagens/Gatos/Ravena.jpeg",           IsPrincipal = true, AnimalId = ravena.Id },
                new() { Url = "/imagens/Peixes/peixe-betta.jpg",      IsPrincipal = true, AnimalId = animais.First(a => a.Nome == "Peixe Betta").Id },
                new() { Url = "/imagens/Peixes/peixe_guppy.jpg",      IsPrincipal = true, AnimalId = animais.First(a => a.Nome == "Peixe Guppy").Id },
                new() { Url = "/imagens/Repteis/jabuti.jpeg",         IsPrincipal = true, AnimalId = oogway.Id },
                new() { Url = "/imagens/Roedores/Ziggs.jpeg",         IsPrincipal = true, AnimalId = animais.First(a => a.Nome == "Ziggs").Id },
            };

            await context.Fotos.AddRangeAsync(fotos);
            await context.SaveChangesAsync();
        }
    }
}