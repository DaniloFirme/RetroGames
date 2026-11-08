-- Consultas de estudo para executar diretamente no MySQL após as etapas 3 e 4.
USE retrogames;

-- 1. Catálogo: apenas jogos ativos, com plataforma e condição.
SELECT j.co_seq_jogo, j.no_jogo, p.no_plataforma, c.ds_condicao_jogo
FROM tb_jogo AS j
INNER JOIN tb_plataforma AS p ON p.co_seq_plataforma = j.co_plataforma
INNER JOIN tb_condicao_jogo AS c ON c.co_condicao_jogo = j.co_condicao_jogo
WHERE j.st_ativo = TRUE
ORDER BY j.no_jogo, j.co_seq_jogo;

-- 2. Gêneros de um jogo. No exemplo, 3 é o ID de Metroid.
SET @id_jogo = 3;

SELECT g.co_seq_genero, g.no_genero
FROM tb_jogo_genero AS jg
INNER JOIN tb_genero AS g ON g.co_seq_genero = jg.co_genero
WHERE jg.co_jogo = @id_jogo
ORDER BY g.no_genero, g.co_seq_genero;

-- 3. Jogos de um gênero. No exemplo, 3 é o ID de Aventura.
SET @id_genero = 3;

SELECT j.co_seq_jogo, j.no_jogo
FROM tb_jogo_genero AS jg
INNER JOIN tb_jogo AS j ON j.co_seq_jogo = jg.co_jogo
WHERE jg.co_genero = @id_genero
ORDER BY j.no_jogo, j.co_seq_jogo;

-- 4. Quantidade por plataforma. COUNT da chave do jogo retorna 0 sem correspondência.
SELECT p.co_seq_plataforma, p.no_plataforma, COUNT(j.co_seq_jogo) AS qt_jogos
FROM tb_plataforma AS p
LEFT JOIN tb_jogo AS j ON j.co_plataforma = p.co_seq_plataforma
GROUP BY p.co_seq_plataforma, p.no_plataforma
ORDER BY p.no_plataforma, p.co_seq_plataforma;

-- 5. Busca parcial em páginas de cinco. O trecho 'a' tem mais de cinco resultados.
SET @trecho_nome = 'a';

-- Página 1.
SELECT j.co_seq_jogo, j.no_jogo
FROM tb_jogo AS j
WHERE j.no_jogo LIKE CONCAT('%', @trecho_nome, '%')
ORDER BY j.no_jogo, j.co_seq_jogo
LIMIT 5 OFFSET 0;

-- Página 2.
SELECT j.co_seq_jogo, j.no_jogo
FROM tb_jogo AS j
WHERE j.no_jogo LIKE CONCAT('%', @trecho_nome, '%')
ORDER BY j.no_jogo, j.co_seq_jogo
LIMIT 5 OFFSET 5;
