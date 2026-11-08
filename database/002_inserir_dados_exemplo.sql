-- Execute uma vez, após 001_criar_tabelas.sql, em um banco recém-criado.
-- Os preços e estoques são fictícios. As datas de cadastro usam UTC.
USE retrogames;

START TRANSACTION;

INSERT INTO tb_condicao_jogo (co_condicao_jogo, ds_condicao_jogo) VALUES
    (1, 'Novo'),
    (2, 'Usado'),
    (3, 'Recondicionado');

INSERT INTO tb_perfil (co_perfil, ds_perfil) VALUES
    (1, 'Administrador'),
    (2, 'Cliente');

-- IDs explícitos apenas nesta carga para manter as associações previsíveis.
INSERT INTO tb_plataforma (co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento) VALUES
    (1, 'NES / Famicom', 'Nintendo', 1983),
    (2, 'Super Nintendo', 'Nintendo', 1990),
    (3, 'Nintendo 64', 'Nintendo', 1996),
    (4, 'Mega Drive', 'Sega', 1988),
    (5, 'PlayStation', 'Sony', 1994),
    (6, 'Game Boy', 'Nintendo', 1989);

INSERT INTO tb_genero (co_seq_genero, no_genero) VALUES
    (1, 'Plataforma'),
    (2, 'Ação'),
    (3, 'Aventura'),
    (4, 'Corrida'),
    (5, 'RPG'),
    (6, 'Luta'),
    (7, 'Quebra-cabeça'),
    (8, 'Tiro');

INSERT INTO tb_jogo (
    co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco,
    qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo
) VALUES
    (1, 'Super Mario Bros.', 'Mario atravessa o Reino dos Cogumelos para resgatar a princesa.',
        1985, 149.90, 4, TRUE, UTC_TIMESTAMP(), 1, 2),
    (2, 'The Legend of Zelda', 'Link explora Hyrule em busca dos fragmentos da Triforce.',
        1986, 189.90, 2, TRUE, UTC_TIMESTAMP(), 1, 2),
    (3, 'Metroid', 'Samus explora Zebes e encontra equipamentos para alcançar novas áreas.',
        1986, 159.90, 3, TRUE, UTC_TIMESTAMP(), 1, 3),
    (4, 'Super Mario Kart', 'Personagens do Reino dos Cogumelos disputam corridas de kart.',
        1992, 159.90, 5, TRUE, UTC_TIMESTAMP(), 2, 2),
    (5, 'Super Metroid', 'Samus volta a Zebes para recuperar um Metroid capturado.',
        1994, 239.90, 2, TRUE, UTC_TIMESTAMP(), 2, 3),
    (6, 'Donkey Kong Country', 'Donkey Kong e Diddy atravessam a ilha para recuperar suas bananas.',
        1994, 129.90, 6, TRUE, UTC_TIMESTAMP(), 2, 2),
    (7, 'The Legend of Zelda: A Link to the Past', 'Link alterna entre dois mundos para salvar Hyrule.',
        1991, 199.90, 3, TRUE, UTC_TIMESTAMP(), 2, 2),
    (8, 'Mario Kart 64', 'Corridas de kart com pistas em 3D e modo para até quatro jogadores.',
        1996, 169.90, 4, TRUE, UTC_TIMESTAMP(), 3, 2),
    (9, 'Star Fox 64', 'Fox McCloud pilota a Arwing em missões pelo sistema Lylat.',
        1997, 139.90, 3, TRUE, UTC_TIMESTAMP(), 3, 3),
    (10, 'The Legend of Zelda: Ocarina of Time', 'Link viaja no tempo e resolve enigmas para deter Ganondorf.',
        1998, 219.90, 2, TRUE, UTC_TIMESTAMP(), 3, 2),
    (11, 'Sonic the Hedgehog 2', 'Sonic e Tails percorrem fases em alta velocidade para deter o Dr. Robotnik.',
        1992, 119.90, 5, TRUE, UTC_TIMESTAMP(), 4, 2),
    (12, 'Streets of Rage 2', 'Lutadores enfrentam gangues nas ruas para resgatar Adam Hunter.',
        1992, 149.90, 2, FALSE, UTC_TIMESTAMP(), 4, 2),
    (13, 'Gunstar Heroes', 'Dois heróis combinam armas e enfrentam um exército em fases de ação.',
        1993, 179.90, 1, TRUE, UTC_TIMESTAMP(), 4, 3),
    (14, 'Final Fantasy VII', 'Cloud e seus aliados enfrentam a Shinra em uma aventura com batalhas de RPG.',
        1997, 229.90, 3, TRUE, UTC_TIMESTAMP(), 5, 2),
    (15, 'Metal Gear Solid', 'Solid Snake se infiltra na base de Shadow Moses para impedir uma ameaça nuclear.',
        1998, 189.90, 3, TRUE, UTC_TIMESTAMP(), 5, 2),
    (16, 'Tekken 3', 'Lutadores com estilos diferentes disputam um torneio em arenas 3D.',
        1998, 139.90, 4, TRUE, UTC_TIMESTAMP(), 5, 3),
    (17, 'R4: Ridge Racer Type 4', 'Pilotos disputam um campeonato de corridas com equipes e carros desbloqueáveis.',
        1998, 159.90, 2, TRUE, UTC_TIMESTAMP(), 5, 1),
    (18, 'Tetris', 'Encaixe peças para completar linhas antes que a pilha alcance o topo.',
        1989, 99.90, 5, TRUE, UTC_TIMESTAMP(), 6, 2),
    (19, 'Kirby''s Dream Land', 'Kirby atravessa Dream Land para recuperar a comida roubada pelo Rei Dedede.',
        1992, 129.90, 0, TRUE, UTC_TIMESTAMP(), 6, 2),
    (20, 'Super Mario Land 2: 6 Golden Coins', 'Mario reúne seis moedas douradas para retomar seu castelo.',
        1992, 149.90, 2, TRUE, UTC_TIMESTAMP(), 6, 1);

INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES
    (1, 1),
    (2, 2), (2, 3),
    (3, 1), (3, 2), (3, 3),
    (4, 4),
    (5, 1), (5, 2), (5, 3),
    (6, 1),
    (7, 2), (7, 3), (7, 7),
    (8, 4),
    (9, 2), (9, 8),
    (10, 2), (10, 3), (10, 7),
    (11, 1),
    (12, 2), (12, 6),
    (13, 2), (13, 8),
    (14, 5),
    (15, 2), (15, 3),
    (16, 6),
    (17, 4),
    (18, 7),
    (19, 1),
    (20, 1);

COMMIT;
